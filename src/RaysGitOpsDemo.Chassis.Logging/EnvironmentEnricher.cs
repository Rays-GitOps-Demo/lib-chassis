using Microsoft.Extensions.Hosting;

namespace RaysGitOpsDemo.Chassis.Logging;

/// <summary>
/// Enriches log entries with the hosting Environment.
/// </summary>
internal class EnvironmentEnricher : Enricher
{
    private readonly string _environmentName;

    /// <summary>
    /// Construct a new EnvironmentEnricher with the specified IHostingEnvironment, which will be used to
    /// determine which hosting environment we're operating in.
    /// </summary>
    /// <param name="hostEnvironment">The IHostingEnvironment to pull environment information from.</param>
    public EnvironmentEnricher(IHostEnvironment hostEnvironment)
    {
        _environmentName = hostEnvironment?.EnvironmentName 
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? "Unknown";
    }

    /// <inheritdoc />
    protected override IDictionary<string, object> GetEnrichmentProperties() => new Dictionary<string, object>
    {
        ["Environment"] = _environmentName
    };
}
