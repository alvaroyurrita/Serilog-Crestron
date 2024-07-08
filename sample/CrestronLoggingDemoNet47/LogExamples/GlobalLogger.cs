using Serilog;
using Serilog.Core;
using Serilog.Crestron.Enrichers;
using Serilog.Crestron.Sinks;
using Serilog.Events;

namespace CrestronLoggingDemo
{
    public partial class ControlSystem
    {
        private LoggingLevelSwitch _loggingLevelSwitch = new LoggingLevelSwitch();
        private void SetUpGlobalLog()
        {
            _loggingLevelSwitch.MinimumLevel = LogEventLevel.Verbose;
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithSlotNo()
                .Enrich.WithProgramName()
                .MinimumLevel.ControlledBy(_loggingLevelSwitch)
                .WriteTo.CrestronConsole(
                    outputTemplate:
                    "[{Level:u3} {SlotNo}-{ProgramName}-{FileName}-{MemberName}-{LineNumber}-{Timestamp:HH:mm:ss}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }
    }
}