using System.Linq;
using System.Runtime.CompilerServices;

namespace Serilog.Crestron.Global_Extensions
{
    /// <summary>
    ///     Provides Useful Extensions used on all sinks.
    /// </summary>
    public static class CrestronLoggerExtensions
    {
        /// <summary>
        ///     Extension to ILogger that injects three properties <br />
        ///     <list type="bullet">
        ///         <item>
        ///             <term>FileName:</term>
        ///             <description> Name of the File where the log originated </description>
        ///         </item>
        ///         <item>
        ///             <term>MemberName:</term>
        ///             <description> Name of the containing Method where the log originated </description>
        ///         </item>
        ///         <item>
        ///             <term>LineNumber:</term>
        ///             <description> Line Number where the log originated </description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <example>
        ///     How to consume this extension
        ///     <code>
        /// Log.Logger.Here().Verbose("{FileName}-{MemberName}-{LineNumber} Something to log");
        /// </code>
        /// </example>
        public static ILogger FromHere(this ILogger logger,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var progName = sourceFilePath.Split('\\').Last();
            return logger.ForContext("MemberName", memberName)
                .ForContext("FileName", progName)
                .ForContext("LineNumber", sourceLineNumber);
        }
    }
}