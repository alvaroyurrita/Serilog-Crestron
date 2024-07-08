using System;
using System.Threading.Tasks;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronThread;
using Serilog;
using Serilog.Debugging;
// For Basic SIMPL# Classes
// For Basic SIMPL#Pro classes

namespace CrestronLoggingDemo
{
    public partial class ControlSystem : CrestronControlSystem
    {
        public ControlSystem()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;
                //**** Example Logs: ****
                //Creates a global log that will
                SetUpGlobalLog();
                CrestronEnvironment.SystemEventHandler += ControllerSystemEventHandler;
                CrestronEnvironment.ProgramStatusEventHandler += ControllerProgramEventHandler;
                CrestronEnvironment.EthernetEventHandler += ControllerEthernetEventHandler;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error in the constructor");
            }
        }
        public override void InitializeSystem()
        {
            try
            {
                SelfLog.Enable(CrestronConsole.PrintLine);

                //Setting Up Error Log with XML Configuration.
                SetXmlConfiguredLog();
                //Setting Up Error Log with JSON Configuration.
                SetJsonConfiguredLog();
                //Setting Up Error Log with JSON Configuration that writes to a file
                SetJsonConfiguredLogToFile();

                //**** Console Commands ****
                //Creating console command to switch between error log levels
                SetConsoleLogLevelSwitch();
                //Creating console commands to run logs manually either to console or to error log
                SetRunExamplesCommands();
                //Creating console command to write logs with different themes to console
                SetBrowseThemeConsoleCommand();

                //Running Initial sample logs
                Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    CrestronConsole.PrintLine("---Serilog Testing Environment");
                    CrestronConsole.PrintLine("---Try the following console commands");
                    CrestronConsole.PrintLine(
                        "---SETERRORLEVEL [1-6]: Set level for logging. 1: Verbose, 2: Debug, 3: Information, 4: Warning, 5: Error, 6: Fatal");
                    CrestronConsole.PrintLine("---RUNCONSOLEEXAMPLES: To run a sample of logs to the console");
                    CrestronConsole.PrintLine("---RUNERRORLOGEXAMPLES: To run a sample of logs to the Error Log");
                    CrestronConsole.PrintLine(
                        "---BROWSETHEMES: To run a sample of console log themes. Only good on a PC");
                    CrestronConsole.PrintLine("");
                    CrestronConsole.PrintLine("");
                    CrestronConsole.PrintLine("---Generating Some Log Entries....");
                    RunExamples(Log.Logger);
                    RunExamples(_errorLogFromXml);
                    RunExamples(_errorLogFromJson);
                    RunExamples(_fileLogFromXml);
                });
            }
            catch (Exception e)
            {
                Log.Error(e, "Error in InitializeSystem");
            }
        }
    }
}