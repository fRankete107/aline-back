using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace PilatesStudioAPI.HealthChecks;

public class MemoryCacheHealthCheck : IHealthCheck
{
    private readonly IMemoryCache _cache;

    public MemoryCacheHealthCheck(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Test cache operations
            var testKey = "health_check_test";
            var testValue = DateTime.UtcNow.ToString();
            
            // Test write
            _cache.Set(testKey, testValue, TimeSpan.FromSeconds(1));
            
            // Test read
            var cachedValue = _cache.Get(testKey);
            
            if (cachedValue?.ToString() != testValue)
            {
                return Task.FromResult(HealthCheckResult.Degraded("Cache read/write operations are not working correctly"));
            }

            // Get cache statistics if available
            var data = new Dictionary<string, object>();
            
            try
            {
                // Use reflection to get internal cache statistics
                var cacheField = typeof(MemoryCache).GetField("_cache", BindingFlags.NonPublic | BindingFlags.Instance);
                if (cacheField?.GetValue(_cache) is IDictionary<object, object> cache)
                {
                    data["cached_items_count"] = cache.Count;
                }
            }
            catch
            {
                // Ignore reflection errors
            }

            data["cache_type"] = _cache.GetType().Name;
            data["test_successful"] = true;

            // Clean up test data
            _cache.Remove(testKey);

            return Task.FromResult(HealthCheckResult.Healthy("Memory cache is working correctly", data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Memory cache is not working", ex));
        }
    }
}