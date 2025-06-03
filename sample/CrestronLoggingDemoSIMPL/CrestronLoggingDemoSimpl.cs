using Crestron.SimplSharp;
using Serilog;

namespace CrestronLoggingDemo_SIMPL
{
    public partial class CrestronLoggingDemoSimpl
    {
        private readonly ILogger _logger;
        public CrestronLoggingDemoSimpl()
        {
            //Inject Logger to supporting drivers as needed
            _logger = SetupLogger().ForContext<CrestronLoggingDemoSimpl>();;
            //Crestron Events. Be sure to add appropriate processes to handle things like Rebooting, or Stopping
            CrestronEnvironment.EthernetEventHandler += CrestronEnvironmentOnEthernetEventHandler;
            CrestronEnvironment.SystemEventHandler += CrestronEnvironmentOnSystemEventHandler;
            CrestronEnvironment.ProgramStatusEventHandler += CrestronEnvironmentOnProgramStatusEventHandler;
        }
        public void PrintLogSamples()
        {
            _logger.Verbose("This is a Verbose log message");
            _logger.Debug("This is a Debug log message");
            _logger.Information("This is an Information log message");
            _logger.Warning("This is a Warning log message");
            _logger.Error("This is an Error log message");
            _logger.Fatal("This is an Fatal log message");
        }
    }
}

