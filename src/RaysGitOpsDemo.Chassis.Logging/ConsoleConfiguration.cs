using System;
using Serilog;

namespace RaysGitOpsDemo.Chassis.Logging;

/// <summary>
/// Configuration for the Console log sink.
/// </summary>
internal class ConsoleConfiguration : AbstractSinkConfiguration
{
    /// <summary>
    /// If true, write logs entries to the console.  Defaults to true.
    /// </summary>
    public bool Enabled { get; set; } = true;
    
    /// <summary>
    /// The output template to use when writing log entries to the console.
    /// </summary>
    public string OutputTemplate { get; set; } = "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}]: {Message}{NewLine}{Exception}";

    /// <summary>
    /// If true, always color output, even on redirect.  Defaults to false.
    /// </summary>
    public bool AlwaysColor { get; set; }

    internal override bool IsEnabled() => Enabled;

    internal override LoggerConfiguration ConfigureSink(LoggerConfiguration loggerConfiguration, IServiceProvider services) => loggerConfiguration
        .WriteTo.Console(outputTemplate: OutputTemplate, applyThemeToRedirectedOutput: AlwaysColor);
}
