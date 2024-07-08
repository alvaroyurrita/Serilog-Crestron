using System.Linq;
using Crestron.SimplSharp;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace CrestronLoggingDemo
{
    public partial class ControlSystem
    {
        private readonly (LogEventLevel Level, string Description)[] _levels = 
        {
            (LogEventLevel.Verbose, "Level 1: Verbose"),
            (LogEventLevel.Debug, "Level 2: Debug"),
            (LogEventLevel.Information, "Level 3: Information"),
            (LogEventLevel.Warning, "Level 4: Warning"),
            (LogEventLevel.Error, "Level 5: Error"),
            (LogEventLevel.Fatal, "Level 6: Fatal")
        };
        private void SetConsoleLogLevelSwitch()
        {
            _loggingLevelSwitch = new LoggingLevelSwitch
            {
                MinimumLevel = _levels.First().Level
            };
            _ = CrestronConsole.AddNewConsoleCommand(SetErrorLevel,
                "SetErrorLevel",
                "Sets Level for Logging [1-6]",
                ConsoleAccessLevelEnum.AccessOperator);
        }
        private void SetErrorLevel(string cmdParameters)
        {
            if (string.IsNullOrEmpty(cmdParameters))
            {
                CrestronConsole.PrintLine("Current Log Level is {0}",
                    _levels.First(l => l.Level == _loggingLevelSwitch.MinimumLevel).Description);
                return;
            }
            if (int.TryParse(cmdParameters, out var level) && !(level >= 1 && level <= _levels.Length))
            {
                CrestronConsole.PrintLine("Level should be a number between 1 to 6");
                return;
            }
            --level;
            _loggingLevelSwitch.MinimumLevel = _levels[level].Level;
            CrestronConsole.PrintLine("---New Level: {0} ---", _levels[level].Description);
            RunExamples(Log.Logger);
        }
    }
}