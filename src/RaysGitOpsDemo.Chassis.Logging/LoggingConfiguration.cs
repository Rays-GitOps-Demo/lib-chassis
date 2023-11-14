using Serilog.Events;

namespace RaysGitOpsDemo.Chassis.Logging;

internal class LoggingConfiguration
{
    public LoggingConfiguration()
    {
        SinkConfigurations = new List<AbstractSinkConfiguration>
        {
            Console,
            File
        };
    }

    /// <summary>
    /// The minimum log level to write to sinks.  Log entries for lower levels will be discarded.
    /// </summary>
    public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Information;

    /// <summary>
    /// Get a collection containing all sink configurations.
    /// </summary>
    public List<AbstractSinkConfiguration> SinkConfigurations { get; }

    /// <summary>
    /// Configuration for the Console sink.
    /// </summary>
    public ConsoleConfiguration Console { get; set; } = new ConsoleConfiguration();

    /// <summary>
    /// Configuration for the File sink.
    /// </summary>
    public FileConfiguration File { get; set; } = new FileConfiguration();
}