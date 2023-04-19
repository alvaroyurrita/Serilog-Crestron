using Serilog.Core;
using Serilog.Events;

namespace Serilog.Crestron.Enrichers.CrestronEnrichers
{
    /// <summary>
    /// Abstract Factory Class to create Cached Log Event Property. <br/>
    /// Use this on properties than never changes.  It reads it once through the duration of the program 
    /// </summary>
    public abstract class CachedPropertyEnricher : ILogEventEnricher
    {
        private LogEventProperty? CachedProperty { get; set; }

        /// <summary>
        /// Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) => logEvent.AddPropertyIfAbsent(GetLogEventProperty(propertyFactory));

        // Don't care about thread-safety, in the worst case the field gets overwritten and one
        // property will be Garbage Collected
        private LogEventProperty GetLogEventProperty(ILogEventPropertyFactory propertyFactory) =>
            CachedProperty ??= CreateProperty(propertyFactory);

        /// <summary>
        /// Creates Property using a propertyFactory.
        /// </summary>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        protected abstract LogEventProperty CreateProperty(ILogEventPropertyFactory propertyFactory);
    }
}