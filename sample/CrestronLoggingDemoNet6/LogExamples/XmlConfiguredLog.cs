using Crestron.SimplSharp;
using Serilog;
using Serilog.Core;
using Serilog.Crestron.Enrichers;

namespace CrestronLoggingDemo;
public partial class ControlSystem
{
    private ILogger _errorLogFromXml = Logger.None;
    private void SetXmlConfiguredLog()
    {
        //Setting Up Error Log with XML Configuration.
        //Notice that it is necessary to run a FileSystemWatcher to detect changes in the configuration
        _errorLogFromXml = new LoggerConfiguration()
            .Enrich.WithSlotNo()
            .Enrich.WithProgramName()
            .ReadFrom.AppSettings(filePath: "App.config")
            .CreateLogger();
        var errorLogXmlConfigWatcher = new FileSystemWatcher(InitialParametersClass.ProgramDirectory.ToString())
        {
            NotifyFilter = NotifyFilters.LastWrite,
            Filter = "App.config",
            IncludeSubdirectories = false,
            EnableRaisingEvents = true
        };
        errorLogXmlConfigWatcher.Changed += (o, e) =>
        {
            _errorLogFromXml = new LoggerConfiguration()
                .Enrich.WithSlotNo()
                .Enrich.WithProgramName()
                .ReadFrom.AppSettings(filePath: "App.config")
                .CreateLogger();
            ErrorLog.Notice("------------------------------------------------------");
            ErrorLog.Notice("---- App.Config Changed.  Generating some logs -------");
            ErrorLog.Notice("------------------------------------------------------");
            RunExamples(_errorLogFromXml);
        };
    }
}