// Important!

using System;
using Crestron.SimplSharp;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Crestron.Sinks.CrestronErrorLog;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace Serilog.Crestron.Sinks
{
    /// <summary>
    ///     Adds the WriteTo.CrestronErrorLog() extension method to <see cref="LoggerConfiguration" />.
    /// </summary>
    public static class CrestronErrorLogLoggerConfigurationExtensions
    {
        private const string DEFAULT_CONSOLE_OUTPUT_TEMPLATE = "[{Level:u3}] {Message:lj}";
        private static readonly object DefaultSyncRoot = new object();
        /// <summary>
        ///     Writes log events to <see cref="ErrorLog" />.C
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="formatProvider">
        ///     Controls the rendering of log events into text, for example to log JSON. To
        ///     control plain text formatting, use the overload that accepts an output template.
        /// </param>
        /// <param name="syncRoot">
        ///     An object that will be used to `lock` (sync) access to the console output. If you specify this, you
        ///     will have the ability to lock on this object, and guarantee that the console sink will not be about to output
        ///     anything while
        ///     the lock is held.
        /// </param>
        /// <param name="restrictedToMinimumLevel">
        ///     The minimum level for
        ///     events passed through the sink. Ignored when <paramref name="levelSwitch" /> is specified.
        /// </param>
        /// <param name="outputTemplate">
        ///     A message template describing the format used to write to the sink.
        ///     The default is <code>"[{Level:u3}] {Message:lj}"</code>.
        /// </param>
        /// <param name="levelSwitch">
        ///     A switch allowing the pass-through minimum level
        ///     to be changed at runtime.
        /// </param>
        /// <returns>Configuration object allowing method chaining.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="sinkConfiguration" /> is <code>null</code></exception>
        /// <exception cref="ArgumentNullException">When <paramref name="outputTemplate" /> is <code>null</code></exception>
        public static LoggerConfiguration CrestronErrorLog(
            this LoggerSinkConfiguration sinkConfiguration,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = DEFAULT_CONSOLE_OUTPUT_TEMPLATE,
            IFormatProvider? formatProvider = null,
            LoggingLevelSwitch? levelSwitch = null,
            object? syncRoot = null)
        {
            if (sinkConfiguration is null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (outputTemplate is null) throw new ArgumentNullException(nameof(outputTemplate));
            syncRoot ??= DefaultSyncRoot;
            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return sinkConfiguration.Sink(new CrestronErrorLogSink(formatter, syncRoot), restrictedToMinimumLevel,
                levelSwitch);
        }
    }
}