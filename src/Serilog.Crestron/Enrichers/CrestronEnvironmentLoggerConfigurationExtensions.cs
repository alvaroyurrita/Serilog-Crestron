using System;
using Serilog.Configuration;
using Serilog.Crestron.Enrichers.CrestronEnrichers;

namespace Serilog.Crestron.Enrichers
{
    /// <summary>
    /// Extends <see cref="LoggerConfiguration"/> to add enrichers for <see cref="System.Environment"/>.
    /// capabilities.
    /// </summary>
    public static class CrestronEnvironmentLoggerConfigurationExtensions
    {
        /// <summary>
        /// Enrich log events with a SlotNo property containing the value of the Running Program Slot No on Crestron Control Appliances, or the Room ID on Crestron Control Servers.
        /// </summary>
        /// <param name="enrichmentConfiguration">Logger enrichment configuration.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        public static LoggerConfiguration WithSlotNo(
            this LoggerEnrichmentConfiguration enrichmentConfiguration) => enrichmentConfiguration == null
                ? throw new ArgumentNullException(nameof(enrichmentConfiguration))
                : enrichmentConfiguration.With<SlotNoEnricher>();
        /// <summary>
        /// Enrich log events with a ProgramName containing the value of the Crestron Program ID Tag on Crestron Control Appliances, or Room Name on Crestron Control Servers.
        /// </summary>
        /// <param name="enrichmentConfiguration">Logger enrichment configuration.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        public static LoggerConfiguration WithProgramName(
            this LoggerEnrichmentConfiguration enrichmentConfiguration) => enrichmentConfiguration == null
                ? throw new ArgumentNullException(nameof(enrichmentConfiguration))
                : enrichmentConfiguration.With<ProgramNameEnricher>();
    }
}