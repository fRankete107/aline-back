using Microsoft.Extensions.Caching.Memory;
using System.Text;
using System.Text.Json;

namespace PilatesStudioAPI.Middleware;

public class ResponseCachingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ResponseCachingMiddleware> _logger;

    private static readonly HashSet<string> CacheableEndpoints = new()
    {
        "/api/analytics/dashboard",
        "/api/analytics/students",
        "/api/analytics/classes",
        "/api/analytics/financial",
        "/api/analytics/instructors",
        "/api/analytics/reservations",
        "/api/analytics/subscriptions",
        "/api/plans",
        "/api/zones",
        "/api/instructors"
    };

    private static readonly Dictionary<string, TimeSpan> EndpointCacheDurations = new()
    {
        { "/api/analytics/dashboard", TimeSpan.FromMinutes(5) },
        { "/api/analytics/students", TimeSpan.FromMinutes(15) },
        { "/api/analytics/classes", TimeSpan.FromMinutes(15) },
        { "/api/analytics/financial", TimeSpan.FromMinutes(15) },
        { "/api/analytics/instructors", TimeSpan.FromMinutes(30) },
        { "/api/analytics/reservations", TimeSpan.FromMinutes(10) },
        { "/api/analytics/subscriptions", TimeSpan.FromMinutes(15) },
        { "/api/plans", TimeSpan.FromHours(1) },
        { "/api/zones", TimeSpan.FromHours(2) },
        { "/api/instructors", TimeSpan.FromMinutes(30) }
    };

    public ResponseCachingMiddleware(RequestDelegate next, IMemoryCache cache, ILogger<ResponseCachingMiddleware> logger)
    {
        _next = next;
        _cache = cache;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only cache GET requests
        if (context.Request.Method != HttpMethods.Get)
        {
            await _next(context);
            return;
        }

        var path = context.Request.Path.Value?.ToLowerInvariant();
        
        // Check if endpoint is cacheable
        if (!IsCacheableEndpoint(path))
        {
            await _next(context);
            return;
        }

        // Generate cache key
        var cacheKey = GenerateCacheKey(context);

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out CachedResponse? cachedResponse))
        {
            _logger.LogInformation("Serving cached response for {Path}", path);
            await WriteCachedResponseAsync(context, cachedResponse!);
            return;
        }

        // Intercept response stream
        var originalStream = context.Response.Body;
        using var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        try
        {
            await _next(context);

            // Only cache successful responses
            if (context.Response.StatusCode == 200)
            {
                var responseContent = responseStream.ToArray();
                var contentType = context.Response.ContentType;

                // Cache the response
                var cacheDuration = GetCacheDuration(path);
                var cacheEntry = new CachedResponse
                {
                    Content = responseContent,
                    ContentType = contentType,
                    StatusCode = context.Response.StatusCode,
                    Headers = context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
                };

                _cache.Set(cacheKey, cacheEntry, cacheDuration);
                _logger.LogInformation("Cached response for {Path} with duration {Duration}", path, cacheDuration);
            }

            // Write response to original stream
            responseStream.Seek(0, SeekOrigin.Begin);
            await responseStream.CopyToAsync(originalStream);
        }
        finally
        {
            context.Response.Body = originalStream;
        }
    }

    private static bool IsCacheableEndpoint(string? path)
    {
        if (string.IsNullOrEmpty(path)) return false;

        return CacheableEndpoints.Any(endpoint => path.StartsWith(endpoint, StringComparison.OrdinalIgnoreCase));
    }

    private static string GenerateCacheKey(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        var queryString = context.Request.QueryString.Value ?? "";
        var userRole = context.User.FindFirst("role")?.Value ?? "anonymous";
        
        // Include user role in cache key for role-based caching
        return $"response_{path}_{queryString}_{userRole}".Replace('/', '_').Replace('?', '_').Replace('&', '_').Replace('=', '_');
    }

    private static TimeSpan GetCacheDuration(string? path)
    {
        if (string.IsNullOrEmpty(path)) return TimeSpan.FromMinutes(5);

        var matchingEndpoint = EndpointCacheDurations.Keys
            .FirstOrDefault(endpoint => path.StartsWith(endpoint, StringComparison.OrdinalIgnoreCase));

        return matchingEndpoint != null ? EndpointCacheDurations[matchingEndpoint] : TimeSpan.FromMinutes(5);
    }

    private static async Task WriteCachedResponseAsync(HttpContext context, CachedResponse cachedResponse)
    {
        context.Response.StatusCode = cachedResponse.StatusCode;
        context.Response.ContentType = cachedResponse.ContentType;

        // Set cache headers
        foreach (var header in cachedResponse.Headers)
        {
            if (!context.Response.Headers.ContainsKey(header.Key))
            {
                context.Response.Headers.Add(header.Key, header.Value);
            }
        }

        // Add cache hit header for debugging
        context.Response.Headers.Add("X-Cache-Status", "HIT");

        await context.Response.Body.WriteAsync(cachedResponse.Content);
    }

    private class CachedResponse
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string? ContentType { get; set; }
        public int StatusCode { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new();
    }
}

// Extension method for easy registration
public static class ResponseCachingMiddlewareExtensions
{
    public static IApplicationBuilder UseResponseCaching(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ResponseCachingMiddleware>();
    }
}