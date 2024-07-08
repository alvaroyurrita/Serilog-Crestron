using System;
using System.IO;
using System.Text;
using Serilog.Core;
using Serilog.Crestron.Sinks.CrestronConsole.Platform;
using Serilog.Crestron.Sinks.CrestronConsole.Themes;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Crestron.Sinks.CrestronConsole
{
    /// <summary>
    ///     A Serilog sink that writes log events to the Text Console of a Crestron 4-series Control Appliances and
    ///     Control Servers.
    /// </summary>
    public class CrestronConsoleSink : ILogEventSink
    {
        private const int DEFAULT_WRITE_BUFFER_CAPACITY = 256;
        private readonly StringWriter _buffer = new StringWriter(new StringBuilder(DEFAULT_WRITE_BUFFER_CAPACITY));
        private readonly ITextFormatter _formatter;
        private readonly object _syncRoot;
        private readonly ConsoleTheme _theme;
        /// <summary>
        ///     Serilog Sink Constructor
        /// </summary>
        /// <param name="theme">Theme to use</param>
        /// <param name="formatter">ITextFormatter</param>
        /// <param name="syncRoot">
        ///     An object that will be used to `lock` (sync) access to the console output. If you specify this, you
        ///     will have the ability to lock on this object, and guarantee that the console sink will not be about to output
        ///     anything while
        ///     the lock is held.
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public CrestronConsoleSink(
            ConsoleTheme theme,
            ITextFormatter formatter,
            object syncRoot)
        {
            _theme = theme ?? throw new ArgumentNullException(nameof(theme));
            _formatter = formatter;
            _syncRoot = syncRoot ?? throw new ArgumentNullException(nameof(syncRoot));
            //Newline Character on an embedded Linux OS on a CP4 is \n
            //To properly display new lines in Crestron Console, or other terminal emulators NewLine needs to be replace to \r\n
            _buffer.NewLine = "\r\n";
        }
        /// <summary>
        ///     Emit the log
        /// </summary>
        /// <param name="logEvent">Log Level Event</param>
        public void Emit(LogEvent logEvent)
        {
            var output = new CrestronConsoleTextWriter();

            // ANSI escape codes can be pre-rendered into a buffer; however, if we're on Windows and
            // using its console coloring APIs, the color switches would happen during the off-screen
            // buffered write here and have no effect when the line is actually written out.
            if (_theme.CanBuffer)
                lock (_syncRoot)
                {
                    _ = _buffer.GetStringBuilder().Clear();
                    _formatter.Format(logEvent, _buffer);
                    var formattedLogEventText = _buffer.ToString();
                    output.Write(formattedLogEventText);
                    output.Flush();
                }
            else
                lock (_syncRoot)
                {
                    _formatter.Format(logEvent, output);
                    output.Flush();
                }
        }
    }
}