using Crestron.SimplSharp;
using Serilog;
using Serilog.Crestron.Global_Extensions;
using Thread = Crestron.SimplSharpPro.CrestronThread.Thread;

namespace CrestronLoggingDemo;
public partial class ControlSystem
{
    private void SetRunExamplesCommands()
    {
        //setting up console commands (not applicable when running on VC4
        _ = CrestronConsole.AddNewConsoleCommand(args => RunExamples(Log.Logger),
            "RunConsoleExamples",
            "Sends different log levels to the Console",
            ConsoleAccessLevelEnum.AccessOperator);
        _ = CrestronConsole.AddNewConsoleCommand(args => RunExamples(_errorLogFromJson),
            "RunErrorLogExamples",
            "Sends different log levels to the ErrorLog. Run ERRLOG PLOGALL to see all",
            ConsoleAccessLevelEnum.AccessOperator);
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
                Coins = 0
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
}