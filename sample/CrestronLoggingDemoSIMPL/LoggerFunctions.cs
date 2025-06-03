using Crestron.SimplSharp;
using Serilog;
using Serilog.Core;
using Serilog.Crestron.Sinks;
using Serilog.Events;

namespace CrestronLoggingDemo_SIMPL
{
    public partial class CrestronLoggingDemoSimpl
    {
        //Logger
        private readonly LoggingLevelSwitch _loggingLevelSwitch = new LoggingLevelSwitch { MinimumLevel = LogEventLevel.Error };
        public void SetLogLevel(ushort level)
        {
            if (level > 5) return;
            _loggingLevelSwitch.MinimumLevel = (LogEventLevel)level;
        }
        private ILogger SetupLogger()
        {
            var loginToConsole = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(_loggingLevelSwitch)
                .WriteTo.CrestronConsole(
                    outputTemplate:
                    "[{SourceContext} - {Timestamp:HH:mm:ss:fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            var loginToErrorLog = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(_loggingLevelSwitch)
                .WriteTo.CrestronErrorLog(
                    outputTemplate: "[{SourceContext} - {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            return CrestronEnvironment.DevicePlatform == eDevicePlatform.Server ? loginToErrorLog : loginToConsole;
        }
    }
}
