using Crestron.SimplSharp;
using Serilog;
using Serilog.Crestron.Enrichers;
using Serilog.Crestron.Sinks;
using Serilog.Crestron.Sinks.CrestronConsole.Themes;

namespace CrestronLoggingDemo;
public partial class ControlSystem
{
    private void SetBrowseThemeConsoleCommand()
    {
        _ = CrestronConsole.AddNewConsoleCommand(BrowseTheme,
            "BrowseThemes",
            "Shows all possible possible themes. Run in a SSH terminal best results.",
            ConsoleAccessLevelEnum.AccessOperator);
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
            (AnsiConsoleTheme.Sixteen, "AnsiConsoleTheme.Sixteen")
        };
        foreach (var (theme, description) in themes)
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