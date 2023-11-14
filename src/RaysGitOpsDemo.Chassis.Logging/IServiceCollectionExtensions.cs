using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RaysGitOpsDemo.Chassis.Common;
using Serilog;
using Serilog.Events;

namespace RaysGitOpsDemo.Chassis.Logging;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to add logging.
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
    public static IServiceCollection AddChassisLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        services.Configure<LoggingConfiguration>(configuration.GetSection($"{Constants.ConfigurationSection}:Logging"));

        services.AddLogging(builder =>
        {
            builder
                .ClearProviders()
                .SetMinimumLevel(LogLevel.Trace)
                .AddSerilog();
        });

        var provider = services.BuildServiceProvider();
        var config = provider.GetRequiredService<IOptions<LoggingConfiguration>>().Value;
        var hostEnvironment = provider.GetRequiredService<IHostEnvironment>();

        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Is(config.MinimumLevel)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.With(new EnvironmentEnricher(hostEnvironment));

        foreach (var sink in config.SinkConfigurations)
        {
            if (sink.IsEnabled())
            {
                loggerConfig = sink.ConfigureSink(loggerConfig, provider);
            }
        }

        Serilog.Log.Logger = loggerConfig.CreateLogger();

        return services;
    }
}
