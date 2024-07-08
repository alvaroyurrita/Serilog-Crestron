using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Crestron.Enrichers;
using Timer = System.Timers.Timer;

namespace CrestronLoggingDemo;
public partial class ControlSystem
{
    private ILogger _fileLogFromXml = Logger.None;
    private void SetJsonConfiguredLogToFile()
    {
        //Setting Up File Log with JSON Configuration and Configuration Hot Reload. File is saved to the User directory.
        //Note that Hot Reload will work on 4-Series Servers without the full path to the configuration file
        //Ideal when working on VC4
        //Use Tail LogFile.txt -F to watch the added logs every 30 seconds.
        var fileLogJsonConfig = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("FileLog.json", false, true)
            .Build();
        var logFilePath =
            Path.Combine(
                Crestron.SimplSharp.CrestronIO.Directory.GetApplicationRootDirectory() +
                Path.DirectorySeparatorChar, "user", "LogFile.txt");
        _fileLogFromXml = new LoggerConfiguration()
            .Enrich.WithSlotNo()
            .Enrich.WithProgramName()
            .ReadFrom.Configuration(fileLogJsonConfig)
            .WriteTo.File(
                logFilePath,
                outputTemplate:
                "[{Level:u3} {SlotNo}-{ProgramName}-{FileName}-{MemberName}-{LineNumber}-{Timestamp:HH:mm:ss}] {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();
        //creates a timer that logs to file every 30s 100 times
        var countdown = 0;
        var logToFile = new Timer(30000);
        logToFile.Elapsed += (o, e) =>
        {
            RunExamples(_fileLogFromXml);
            ++countdown;
            if (countdown > 100) logToFile.AutoReset = false;
        };
        logToFile.AutoReset = true;
        logToFile.Enabled = true;
    }
}