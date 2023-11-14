using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RaysGitOpsDemo.Chassis.Caching;
using RaysGitOpsDemo.Chassis.Logging;
using RaysGitOpsDemo.Chassis.Swagger;

namespace RaysGitOpsDemo.Chassis;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to add the chassis.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Add the chassis to dependency injection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to inject into.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> object created from appsettings.</param>
    /// <returns>The same <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="ArgumentNullException">If any parameter is null.</exception>
    public static IServiceCollection AddChassis(this IServiceCollection services, IConfiguration configuration)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        services
            .AddChassisLogging(configuration)
            .AddChassisCaching(configuration)
            .AddChassisSwagger(configuration);

        return services;
    }
}
