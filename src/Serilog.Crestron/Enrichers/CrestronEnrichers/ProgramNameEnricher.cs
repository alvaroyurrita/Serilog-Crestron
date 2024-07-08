using Crestron.SimplSharp;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Crestron.Enrichers.CrestronEnrichers
{
    /// <summary>
    ///     Class that Enriches the log with a Crestron Program ID Tag for Crestron Control Appliances or Room Name for
    ///     Crestron Control Servers.
    /// </summary>
    public class ProgramNameEnricher : CachedPropertyEnricher
    {
        /// <summary>
        ///     The property name added to enriched log events.
        /// </summary>
        public const string PROGRAM_NAME_PROPERTY_NAME_NAME = "ProgramName";
        /// <summary>
        ///     Creates Property using a propertyFactory.
        /// </summary>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        protected override LogEventProperty CreateProperty(ILogEventPropertyFactory propertyFactory)
        {
            var roomName = InitialParametersClass.RoomName == ""
                ? InitialParametersClass.ProgramIDTag
                : InitialParametersClass.RoomName;
            return propertyFactory.CreateProperty(PROGRAM_NAME_PROPERTY_NAME_NAME, roomName);
        }
    }
}