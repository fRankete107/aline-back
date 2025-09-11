using Microsoft.Extensions.Diagnostics.HealthChecks;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.HealthChecks;

public class ApiHealthCheck : IHealthCheck
{
    private readonly IServiceProvider _serviceProvider;

    public ApiHealthCheck(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var data = new Dictionary<string, object>();
        var issues = new List<string>();

        try
        {
            // Check critical services
            await CheckService<IStudentService>("StudentService", data, issues);
            await CheckService<IAnalyticsService>("AnalyticsService", data, issues);
            await CheckService<IPaymentService>("PaymentService", data, issues);
            await CheckService<IReservationService>("ReservationService", data, issues);
            await CheckService<IAuthService>("AuthService", data, issues);

            data["services_checked"] = 5;
            data["timestamp"] = DateTime.UtcNow;
            data["environment"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";

            // Add system information
            data["machine_name"] = Environment.MachineName;
            data["process_id"] = Environment.ProcessId;
            data["working_set"] = GC.GetTotalMemory(false);

            if (issues.Count > 0)
            {
                data["issues"] = issues;
                return HealthCheckResult.Degraded($"Some services have issues: {string.Join(", ", issues)}", null, data);
            }

            return HealthCheckResult.Healthy("All critical services are available and responding", data);
        }
        catch (Exception ex)
        {
            data["error"] = ex.Message;
            return HealthCheckResult.Unhealthy("API health check failed", ex, data);
        }
    }

    private async Task CheckService<T>(string serviceName, Dictionary<string, object> data, List<string> issues)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetService<T>();
            
            if (service == null)
            {
                issues.Add($"{serviceName} not registered");
                data[$"{serviceName.ToLower()}_status"] = "Not Registered";
            }
            else
            {
                data[$"{serviceName.ToLower()}_status"] = "Available";
            }
        }
        catch (Exception ex)
        {
            issues.Add($"{serviceName}: {ex.Message}");
            data[$"{serviceName.ToLower()}_status"] = "Error";
            data[$"{serviceName.ToLower()}_error"] = ex.Message;
        }

        await Task.CompletedTask; // Placeholder for potential async service checks
    }
}