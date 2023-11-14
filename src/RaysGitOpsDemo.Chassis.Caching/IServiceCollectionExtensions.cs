using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RaysGitOpsDemo.Chassis.Common;

namespace RaysGitOpsDemo.Chassis.Caching;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to add caching.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Add logging to dependency injection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to inject into.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> object created from appsettings.</param>
    /// <returns>The same <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="ArgumentNullException">If any parameter is null.</exception>
    public static IServiceCollection AddChassisCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        services.Configure<CacheConfiguration>(configuration.GetSection($"{Constants.ConfigurationSection}:Caching"));
        var provider = services.BuildServiceProvider();
        var cachingConfig = provider.GetRequiredService<IOptions<CacheConfiguration>>().Value;

        services
            .AddMemoryCache()
            .AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cachingConfig.Redis;
            })
            .TryAddSingleton<ICache, TwoLevelCache>();
        
        return services;
    }
}
