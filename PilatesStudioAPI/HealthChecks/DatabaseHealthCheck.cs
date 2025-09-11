using Microsoft.Extensions.Diagnostics.HealthChecks;
using PilatesStudioAPI.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace PilatesStudioAPI.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly PilatesStudioDbContext _context;

    public DatabaseHealthCheck(PilatesStudioDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if database can be connected to and queried
            await _context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
            
            // Additional checks
            var userCount = await _context.Users.CountAsync(cancellationToken);
            var studentCount = await _context.Students.CountAsync(cancellationToken);
            var classCount = await _context.Classes.CountAsync(cancellationToken);

            var data = new Dictionary<string, object>
            {
                ["users"] = userCount,
                ["students"] = studentCount,
                ["classes"] = classCount,
                ["database_provider"] = _context.Database.ProviderName ?? "Unknown"
            };

            return HealthCheckResult.Healthy("Database is accessible and responsive", data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database is not accessible", ex);
        }
    }
}