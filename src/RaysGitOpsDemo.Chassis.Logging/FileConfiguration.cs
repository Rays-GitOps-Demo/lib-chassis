using System;
using Serilog;

namespace RaysGitOpsDemo.Chassis.Logging;

/// <summary>
/// Configuration for the File log sink.
/// </summary>
internal class FileConfiguration : AbstractSinkConfiguration
{
    /// <summary>
    /// The output template to use when writing log entries to a file.
    /// </summary>
    public string OutputTemplate { get; set; } = "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}]: {Message}{NewLine}{Exception}";

    /// <summary>
    /// The path to the file where logs should be written.
    /// </summary>
    public string LogFile { get; set; } = "./";

    /// <summary>
    /// If true, the log file may be shared by multiple processes.
    /// </summary>
    public bool Shared { get; set; }

    /// <summary>
    /// Indicates if flushing to the output file can be buffered or not.
    /// </summary>
    public bool Buffered { get; set; }

    /// <summary>
    /// If provided, a full disk flush will be performed periodically at the specified interval.
    /// </summary>
    public TimeSpan FlushToDiskInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// If true, a new file will be created when the file size limit is reached. Filenames will
    /// have a number appended in the format _NNN, with the first filename given no number.
    /// </summary>
    public bool RollOnFileSizeLimit { get; set; } = true;

    /// <summary>
    /// The maximum size of the log file.  The default is 1 GB.
    /// </summary>
    public long FileSizeLimitBytes { get; set; } = 1073741824; // 1 GB

    /// <summary>
    /// The interval at which logging will roll over to a new file.
    /// </summary>
    public RollingInterval RollingInterval { get; set; } = RollingInterval.Infinite;

    /// <summary>
    /// The maximum number of log files that will be retained, including the current log file.
    /// The default is 31.
    /// </summary>
    public int RetainedFileCountLimit { get; set; } = 31;

    internal override bool IsEnabled() => !string.IsNullOrWhiteSpace(LogFile);

    internal override LoggerConfiguration ConfigureSink(LoggerConfiguration loggerConfiguration, IServiceProvider services) => loggerConfiguration
        .WriteTo.File(
            LogFile,
            outputTemplate: OutputTemplate,
            fileSizeLimitBytes: FileSizeLimitBytes,
            buffered: Buffered,
            flushToDiskInterval: FlushToDiskInterval,
            rollOnFileSizeLimit: RollOnFileSizeLimit,
            rollingInterval: RollingInterval,
            retainedFileCountLimit: RetainedFileCountLimit);
}
