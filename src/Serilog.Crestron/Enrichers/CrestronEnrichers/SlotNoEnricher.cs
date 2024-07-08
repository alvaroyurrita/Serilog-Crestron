using Crestron.SimplSharp;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Crestron.Enrichers.CrestronEnrichers
{
    /// <summary>
    ///     Class that Enriches the log with Crestron SlotNo on Crestron Control Appliances or RoomId on Crestron Control
    ///     Servers.
    /// </summary>
    public class SlotNoEnricher : CachedPropertyEnricher
    {
        /// <summary>
        ///     The property name added to enriched log events.
        /// </summary>
        public const string SLOT_NO_PROPERTY_NAME_NAME = "SlotNo";
        /// <summary>
        ///     Creates Property using a propertyFactory.
        /// </summary>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        protected override LogEventProperty CreateProperty(ILogEventPropertyFactory propertyFactory)
        {
            var slotNo = int.TryParse(InitialParametersClass.RoomId, out var roomId)
                ? $"{roomId:00}"
                : InitialParametersClass.RoomId;
            return propertyFactory.CreateProperty(SLOT_NO_PROPERTY_NAME_NAME, slotNo);
        }
    }
}