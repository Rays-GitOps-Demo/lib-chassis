using Serilog.Core;
using Serilog.Events;

namespace RaysGitOpsDemo.Chassis.Logging;

/// <summary>
/// An abstract base class for all concrete log enrichers to extend.
/// </summary>
internal abstract class Enricher : ILogEventEnricher
{
    /// <summary>
    /// Enrich the current log event with additional properties.
    /// </summary>
    /// <param name="logEvent">The <see cref="LogEvent" /> being enriched.</param>
    /// <param name="propertyFactory">Ignored</param>
    public virtual void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent = logEvent ?? throw new ArgumentNullException(nameof(logEvent));
        var properties = GetEnrichmentProperties();

        foreach(var property in properties)
        {
            logEvent.AddOrUpdateProperty(new LogEventProperty(property.Key, new ScalarValue(property.Value)));
        }
    }

    /// <summary>
    /// Return the additional properties to add to the current log event.
    /// </summary>
    protected abstract IDictionary<string, object> GetEnrichmentProperties();
}
