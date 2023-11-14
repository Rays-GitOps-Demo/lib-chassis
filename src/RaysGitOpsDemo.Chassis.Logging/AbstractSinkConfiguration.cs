using System;
using Serilog;

namespace RaysGitOpsDemo.Chassis.Logging;

/// <summary>
/// An abstract base class for all concrete sink configuration to extend.
/// </summary>
internal abstract class AbstractSinkConfiguration
{
    /// <summary>
    ///     Determines if the sink is enabled or not.
    /// </summary>
    /// <returns>Boolean indicating if the sink is enabled.</returns>
    internal abstract bool IsEnabled();

    /// <summary>
    ///     Configures the sink with an appropriate configuration.
    /// </summary>
    /// <param name="loggerConfiguration">The configuration to build off of.</param>
    /// <param name="services">The <see cref="IServiceProvider"/> to retrieve dependant services from.</param>
    /// <returns>An updated configuration set up to use the sink.</returns>
    internal abstract LoggerConfiguration ConfigureSink(LoggerConfiguration loggerConfiguration, IServiceProvider services);
}