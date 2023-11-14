using System;

namespace RaysGitOpsDemo.Chassis.Caching;

/// <summary>
/// Contains the settings from the RaysGitOpsDemo:Caching config section from appsettings.json.
/// </summary>
internal class CacheConfiguration
{
    /// <summary>
    /// The default time-to-live put on entries placed in the local in-memory cache.
    /// </summary>
    public TimeSpan DefaultMemoryCacheTTL { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// The connection string ot the Redis node to use for the second, distributed level of the cache.
    /// </summary>
    public string Redis { get; set; } = "localhost:6379";


    /// <summary>
    /// The default time-to-live put on entries placed in the Redis cache.
    /// </summary>
    public TimeSpan DefaultDistributedCacheTTL { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// The default amount of time to wait when removing items from the cache.
    /// </summary>
    public TimeSpan DefaultRemoveCommandTimeout { get; set; } = TimeSpan.FromSeconds(5);
}
