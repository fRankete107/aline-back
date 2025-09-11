using Serilog;
using Serilog.Context;
using System.Diagnostics;

namespace PilatesStudioAPI.Middleware;

public class StructuredLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<StructuredLoggingMiddleware> _logger;

    public StructuredLoggingMiddleware(RequestDelegate next, ILogger<StructuredLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = Guid.NewGuid().ToString("N")[..8];
        var requestId = context.TraceIdentifier;
        
        // Add correlation context for all logs during this request
        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("RequestId", requestId))
        using (LogContext.PushProperty("RequestMethod", context.Request.Method))
        using (LogContext.PushProperty("RequestPath", context.Request.Path))
        using (LogContext.PushProperty("UserAgent", context.Request.Headers.UserAgent.ToString()))
        using (LogContext.PushProperty("RemoteIP", GetClientIP(context)))
        using (LogContext.PushProperty("UserId", GetUserId(context)))
        using (LogContext.PushProperty("UserRole", GetUserRole(context)))
        {
            // Add correlation ID to response headers for client tracking
            context.Response.Headers.Add("X-Correlation-ID", correlationId);

            try
            {
                _logger.LogInformation("Request started for {Method} {Path}",
                    context.Request.Method, context.Request.Path);

                await _next(context);

                stopwatch.Stop();

                // Log successful request completion
                _logger.LogInformation("Request completed successfully in {Duration}ms with status {StatusCode}",
                    stopwatch.ElapsedMilliseconds, context.Response.StatusCode);

                // Log performance warnings for slow requests
                if (stopwatch.ElapsedMilliseconds > 3000)
                {
                    _logger.LogWarning("Slow request detected: {Method} {Path} took {Duration}ms",
                        context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex, "Request failed after {Duration}ms with error: {ErrorMessage}",
                    stopwatch.ElapsedMilliseconds, ex.Message);

                // Log additional context for specific error types
                LogErrorContext(ex);

                throw;
            }
            finally
            {
                // Log request metrics for monitoring
                LogRequestMetrics(context, stopwatch.ElapsedMilliseconds);
            }
        }
    }

    private static string GetClientIP(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            var ip = forwardedFor.Split(',').FirstOrDefault()?.Trim();
            if (!string.IsNullOrEmpty(ip))
                return ip;
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
            return realIp;

        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    private static string? GetUserId(HttpContext context)
    {
        return context.User.FindFirst("sub")?.Value ??
               context.User.FindFirst("id")?.Value ??
               context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    }

    private static string? GetUserRole(HttpContext context)
    {
        return context.User.FindFirst("role")?.Value ??
               context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
    }

    private void LogErrorContext(Exception ex)
    {
        switch (ex)
        {
            case UnauthorizedAccessException:
                _logger.LogWarning("Unauthorized access attempt detected");
                break;
            case ArgumentException argEx:
                _logger.LogWarning("Invalid argument provided: {ArgumentName} - {Message}",
                    argEx.ParamName, argEx.Message);
                break;
            case InvalidOperationException:
                _logger.LogWarning("Invalid operation attempted: {Message}", ex.Message);
                break;
            case TimeoutException:
                _logger.LogError("Request timeout occurred: {Message}", ex.Message);
                break;
        }
    }

    private void LogRequestMetrics(HttpContext context, long duration)
    {
        var method = context.Request.Method;
        var path = context.Request.Path.Value;
        var statusCode = context.Response.StatusCode;
        var contentLength = context.Response.ContentLength ?? 0;

        // Log metrics for monitoring systems
        using (LogContext.PushProperty("MetricType", "RequestDuration"))
        using (LogContext.PushProperty("HttpMethod", method))
        using (LogContext.PushProperty("Endpoint", path))
        using (LogContext.PushProperty("StatusCode", statusCode))
        using (LogContext.PushProperty("ResponseSize", contentLength))
        using (LogContext.PushProperty("Duration", duration))
        {
            var logLevel = GetLogLevelForStatusCode(statusCode);
            _logger.Log(logLevel, "Request metrics: {Method} {Path} - {StatusCode} ({Duration}ms, {Size} bytes)",
                method, path, statusCode, duration, contentLength);
        }

        // Log business metrics for specific endpoints
        LogBusinessMetrics(context, path, method, statusCode);
    }

    private static LogLevel GetLogLevelForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            >= 200 and < 300 => LogLevel.Information,
            >= 300 and < 400 => LogLevel.Information,
            >= 400 and < 500 => LogLevel.Warning,
            >= 500 => LogLevel.Error,
            _ => LogLevel.Information
        };
    }

    private void LogBusinessMetrics(HttpContext context, string? path, string method, int statusCode)
    {
        if (string.IsNullOrEmpty(path)) return;

        // Log specific business events
        using (LogContext.PushProperty("BusinessEvent", true))
        {
            switch (path.ToLowerInvariant())
            {
                case var p when p.Contains("/auth/login") && method == "POST":
                    var loginResult = statusCode == 200 ? "Success" : "Failed";
                    _logger.LogInformation("Login attempt: {Result}", loginResult);
                    break;

                case var p when p.Contains("/reservations") && method == "POST":
                    if (statusCode == 201)
                        _logger.LogInformation("New reservation created successfully");
                    else if (statusCode >= 400)
                        _logger.LogWarning("Reservation creation failed with status {StatusCode}", statusCode);
                    break;

                case var p when p.Contains("/payments") && method == "POST":
                    if (statusCode == 201)
                        _logger.LogInformation("New payment processed successfully");
                    else if (statusCode >= 400)
                        _logger.LogWarning("Payment processing failed with status {StatusCode}", statusCode);
                    break;

                case var p when p.Contains("/analytics/") && method == "GET":
                    _logger.LogInformation("Analytics data requested from endpoint {Endpoint}", path);
                    break;
            }
        }
    }
}

// Extension method for easy registration
public static class StructuredLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseStructuredLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<StructuredLoggingMiddleware>();
    }
}