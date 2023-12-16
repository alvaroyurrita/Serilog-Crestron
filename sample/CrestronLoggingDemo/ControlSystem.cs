using Crestron.SimplSharp; // For Basic SIMPL# Classes
using Crestron.SimplSharpPro; // For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog.Crestron.Enrichers;
using Serilog.Crestron.Global_Extensions;
using Serilog.Crestron.Sinks;
using Serilog.Crestron.Sinks.CrestronConsole.Themes;

namespace CrestronLoggingDemo
{
    public class ControlSystem : CrestronControlSystem
    {
        private Logger _consoleLog;
        private Logger _errorLogFromXml;
        private (LogEventLevel Level, string Description)[] _levels;
        private LoggingLevelSwitch _loggingLevelSwitch;
        private Logger _errorLogFromJson;
        private Logger _fileLogFromXml;
        /// <summary>
        /// ControlSystem Constructor. Starting point for the SIMPL#Pro program.
        /// Use the constructor to:
        /// * Initialize the maximum number of threads (max = 400)
        /// * Register devices
        /// * Register event handlers
        /// * Add Console Commands
        /// 
        /// Please be aware that the constructor needs to exit quickly; if it doesn't
        /// exit in time, the SIMPL#Pro program will exit.
        /// 
        /// You cannot send / receive data in the constructor
        /// </summary>
        public ControlSystem()
            : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;

                //Subscribe to the controller events (System, Program, and Ethernet)
                CrestronEnvironment.SystemEventHandler += ControllerSystemEventHandler;
                CrestronEnvironment.ProgramStatusEventHandler += ControllerProgramEventHandler;
                CrestronEnvironment.EthernetEventHandler += ControllerEthernetEventHandler;
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }
        /// <summary>
        /// InitializeSystem - this method gets called after the constructor 
        /// has finished. 
        /// 
        /// Use InitializeSystem to:
        /// * Start threads
        /// * Configure ports, such as serial and verisports
        /// * Start and initialize socket connections
        /// Send initial device configurations
        /// 
        /// Please be aware that InitializeSystem needs to exit quickly also; 
        /// if it doesn't exit in time, the SIMPL#Pro program will exit.
        /// </summary>
        public override void InitializeSystem()
        {
            try
            {
                Serilog.Debugging.SelfLog.Enable(CrestronConsole.PrintLine);

                _levels = new (LogEventLevel Level, string Description)[]
                {
                    (LogEventLevel.Verbose, "Level 1: Verbose"),
                    (LogEventLevel.Debug, "Level 2: Debug"),
                    (LogEventLevel.Information, "Level 3: Information"),
                    (LogEventLevel.Warning, "Level 4: Warning"),
                    (LogEventLevel.Error, "Level 5: Error"),
                    (LogEventLevel.Fatal, "Level 6: Fatal"),
                };
                _loggingLevelSwitch = new LoggingLevelSwitch
                {
                    MinimumLevel = _levels.First().Level
                };
 
                //Setting up logger instances 
                //In a normal application most likely a global logger will be all that is needed.
                _consoleLog = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .Enrich.WithSlotNo()
                    .Enrich.WithProgramName()
                    .MinimumLevel.ControlledBy(_loggingLevelSwitch)
                    .WriteTo.CrestronConsole(
                        outputTemplate:
                        "[{Level:u3} {SlotNo}-{ProgramName}-{FileName}-{MemberName}-{LineNumber}-{Timestamp:HH:mm:ss}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();
                
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
                    _errorLogFromXml.Dispose();
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
                
                //Setting Up Error Log with JSON Configuration.
                //Execute RunErrorLogExamples in the console to generate a new set of errors
                //No need to watch for file changes, since Microsoft.Extensions.Configuration.Json supports Hot Reload
                //https://swimburger.net/blog/dotnet/changing-serilog-minimum-level-without-application-restart-on-dotnet-framework-and-core
                var errorLogJsonConfig = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("ErrorLog.json",false,true)
                    .Build();
                _errorLogFromJson = new LoggerConfiguration()
                    .ReadFrom.Configuration(errorLogJsonConfig)
                    .CreateLogger();
                
                //Setting Up File Log with JSON Configuration and Configuration Hot Reload. File is saved to the User directory.
                //Note that Hot Reload will work on 4-Series Servers without the full path to the configuration file
                //Ideal when working on VC4
                //Use Tail LogFile.txt -F to watch the added logs every 30 seconds.
                var fileLogJsonConfig = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("FileLog.json", optional:false, reloadOnChange:true)
                    .Build();
                var logFilePath =  Path.Combine(Crestron.SimplSharp.CrestronIO.Directory.GetApplicationRootDirectory() + Path.DirectorySeparatorChar, "user","LogFile.txt");
                _fileLogFromXml = new LoggerConfiguration()
                    .Enrich.WithSlotNo()
                    .Enrich.WithProgramName()
                    .ReadFrom.Configuration(fileLogJsonConfig)
                    .WriteTo.File(
                        path:logFilePath,
                        outputTemplate:"[{Level:u3} {SlotNo}-{ProgramName}-{FileName}-{MemberName}-{LineNumber}-{Timestamp:HH:mm:ss}] {Message:lj}{NewLine}{Exception}",
                        rollingInterval:RollingInterval.Day)
                    .CreateLogger();
                //creates a timer that logs to file every 30s 100 times
                var countdown = 0;
                var logToFile = new System.Timers.Timer(30000);
                logToFile.Elapsed += (o, e) =>
                {
                    RunExamples(_fileLogFromXml);
                    ++countdown;
                    if (countdown > 100) logToFile.AutoReset = false;
                };
                logToFile.AutoReset = true;
                logToFile.Enabled = true;

               
               //setting up console commands (not applicable when running on VC4
                _ = CrestronConsole.AddNewConsoleCommand(args => RunExamples(_consoleLog),
                    "RunConsoleExamples",
                    "Sends different log levels to the Console",
                    ConsoleAccessLevelEnum.AccessOperator);
                _ = CrestronConsole.AddNewConsoleCommand(args => RunExamples(_errorLogFromJson),
                    "RunErrorLogExamples",
                    "Sends different log levels to the ErrorLog. Run ERRLOG PLOGALL to see all",
                    ConsoleAccessLevelEnum.AccessOperator);
                _ = CrestronConsole.AddNewConsoleCommand(SetErrorLevel,
                    "SetErrorLevel",
                    "Sets Level for Logging [1-6]",
                    ConsoleAccessLevelEnum.AccessOperator);
                _ = CrestronConsole.AddNewConsoleCommand(BrowseTheme,
                    "BrowseThemes",
                    "Shows all possible possible themes. Run in a SSH terminal best results.",
                    ConsoleAccessLevelEnum.AccessOperator);
                var t = new Thread(o =>
                {
                    Thread.Sleep(1000);
                    CrestronConsole.PrintLine("");
                    CrestronConsole.PrintLine("---Generating Some Log Entries....");
                    RunExamples(_consoleLog);
                    RunExamples(_errorLogFromXml);
                    RunExamples(_errorLogFromJson);
                    RunExamples(_fileLogFromXml);
                    return null;
                }, new object());
                t.Start();
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }
        private void SetErrorLevel(string cmdparameters)
        {
            if (int.TryParse(cmdparameters, out var level) && !(level >= 1 && level <= _levels.Length))
            {
                CrestronConsole.PrintLine("Level should be a number between 1 to 6");
                return;
            }
            --level;
            _loggingLevelSwitch.MinimumLevel = _levels[level].Level;
            CrestronConsole.PrintLine("---New Level: {0} ---", _levels[level].Description);
            RunExamples(_consoleLog);
        }
        private void BrowseTheme(string cmdparameters)
        {
            //setting up themes reference 
            var themes = new (ConsoleTheme Theme, string Description)[]
            {
                (ConsoleTheme.None, "ConsoleTheme.None"),
                (AnsiConsoleTheme.Code, "AnsiConsoleTheme.Code"),
                (AnsiConsoleTheme.Grayscale, "AnsiConsoleTheme.Grayscale"),
                (AnsiConsoleTheme.Literate, "AnsiConsoleTheme.Literate"),
                (AnsiConsoleTheme.Sixteen, "AnsiConsoleTheme.Sixteen"),
            };
            foreach (var (theme, description) in themes)
            {
                using (var themedConsoleLog = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .Enrich.WithSlotNo()
                    .Enrich.WithProgramName()
                    .WriteTo.CrestronConsole(
                        theme: theme)
                    .CreateLogger())
                {
                    CrestronConsole.PrintLine("---New Theme: {0} ---", description);
                    RunExamples(themedConsoleLog);
                    CrestronConsole.PrintLine("");
                }
            }
        }

        private void RunExamples(ILogger log)
        {
            //Levels. From Minimum to Maximum
            //Verbose: Verbose is the noisiest level, rarely (if ever) enabled for a production app.
            //Debug: Debug is used for internal system events that are not necessarily observable from the outside, but useful when determining how something happened.
            //Information: Information events describe things happening in the system that correspond to its responsibilities and functions. Generally these are the observable actions the system can perform.
            //Warning: When service is degraded, endangered, or may be behaving outside of its expected parameters, Warning level events are used.
            //Error: When functionality is unavailable or expectations broken, an Error event is used.
            //Fatal: The most critical level, Fatal events demand immediate attention.
            try
            {
                log.FromHere()
                    .Verbose("Verbose Level 1. Example: Running Program: A Verbose");
                log.FromHere().Debug("Debug Level 2. Example: This should not be displayed in Crestron ErrorLog");
                log.FromHere().Information("Information Level 3. Example: I am running on Thread: {ThreadId}",
                    Thread.CurrentThread.ManagedThreadId);
                //sample of object serializing
                var position = new Position
                {
                    Lat = 25,
                    Long = 134,
                    Coins = 0,
                };
                log.FromHere().Warning("Warning Level 4. Example: No coins remain at position {@Position:j}",
                    position);
                throw new DivideByZeroException();
            }
            catch (DivideByZeroException e)
            {
                log.FromHere().Error(e, "Error Level 5. Example: Exception Happened");
            }
            try
            {
                var badClass = new BadClass();
                badClass.NullList.Add("hello");
            }
            catch (Exception e)
            {
                log.FromHere().Fatal(e, "Fatal Level 6. Example: Generic Exception");
            }
        }
        private class Position
        {
            public int Lat { get; set; }
            public int Long { get; set; }
            public int Coins { get; set; }
            public string[] Sides { get; set; } = { "Heads", "Tails" };
        }
        public class BadClass
        {
            //This class should have initialized nullList. This will trigger a Null Reference Exception to classes that try to add to this list
            public List<string> NullList { get; set; }
        }
        /// <summary>
        /// Event Handler for Ethernet events: Link Up and Link Down. 
        /// Use these events to close / re-open sockets, etc. 
        /// </summary>
        /// <param name="ethernetEventArgs">This parameter holds the values 
        /// such as whether it's a Link Up or Link Down event. It will also indicate 
        /// which Ethernet adapter this event belongs to.
        /// </param>
        private void ControllerEthernetEventHandler(EthernetEventArgs ethernetEventArgs)
        {
            switch (ethernetEventArgs.EthernetEventType)
            {
                //Determine the event type Link Up or Link Down
                case eEthernetEventType.LinkDown:
                    //Next need to determine which adapter the event is for. 
                    //LAN is the adapter is the port connected to external networks.
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {
                        //
                    }
                    break;
                case eEthernetEventType.LinkUp:
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        /// <summary>
        /// Event Handler for Programmatic events: Stop, Pause, Resume.
        /// Use this event to clean up when a program is stopping, pausing, and resuming.
        /// This event only applies to this SIMPL#Pro program, it doesn't receive events
        /// for other programs stopping
        /// </summary>
        /// <param name="programStatusEventType"></param>
        private void ControllerProgramEventHandler(eProgramStatusEventType programStatusEventType)
        {
            switch (programStatusEventType)
            {
                case eProgramStatusEventType.Paused:
                    //The program has been paused.  Pause all user threads/timers as needed.
                    break;
                case eProgramStatusEventType.Resumed:
                    //The program has been resumed. Resume all the user threads/timers as needed.
                    break;
                case eProgramStatusEventType.Stopping:
                    //The program has been stopped.
                    //Close all threads. 
                    //Shutdown all Client/Servers in the system.
                    //General cleanup.
                    //Unsubscribe to all System Monitor events
                    _errorLogFromXml.Dispose();
                    _consoleLog.Dispose();
                    _fileLogFromXml.Dispose();
                    _errorLogFromJson.Dispose();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(programStatusEventType), programStatusEventType, null);
            }
        }
        /// <summary>
        /// Event Handler for system events, Disk Inserted/Ejected, and Reboot
        /// Use this event to clean up when someone types in reboot, or when your SD /USB
        /// removable media is ejected / re-inserted.
        /// </summary>
        /// <param name="systemEventType"></param>
        private void ControllerSystemEventHandler(eSystemEventType systemEventType)
        {
            switch (systemEventType)
            {
                case eSystemEventType.DiskInserted:
                    //Removable media was detected on the system
                    break;
                case eSystemEventType.DiskRemoved:
                    //Removable media was detached from the system
                    break;
                case eSystemEventType.Rebooting:
                    //The system is rebooting. 
                    //Very limited time to preform clean up and save any settings to disk.
                    _errorLogFromXml.Dispose();
                    _consoleLog.Dispose();
                    _fileLogFromXml.Dispose();
                    _errorLogFromJson.Dispose();
                    break;
                case eSystemEventType.TimeChange:
                    break;
                case eSystemEventType.AuthenticationStateChange:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(systemEventType), systemEventType, null);
            }
        }
    }
}