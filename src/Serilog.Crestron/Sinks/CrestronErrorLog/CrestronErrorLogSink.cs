using System;
using System.IO;
using System.Text;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Crestron.Sinks.CrestronErrorLog
{
    /// <summary>A Serilog sink that writes log events to the Error Log of a Crestron 4-series Control Appliances and Crestron Control Servers such as a CP4(N) or VC4 </summary>
    public class CrestronErrorLogSink : ILogEventSink
    {
        private readonly ITextFormatter _formatter;
        private readonly object _syncRoot;
        private readonly StringWriter _buffer = new StringWriter(new StringBuilder(DEFAULT_WRITE_BUFFER_CAPACITY));
        private const int DEFAULT_WRITE_BUFFER_CAPACITY = 256;
        /// <summary>
        /// Serilog Sink Constructor
        /// </summary>
        /// <param name="formatter">ITextFormatter</param>
        /// <param name="syncRoot">An object that will be used to `lock` (sync) access to the console output. If you specify this, you
        /// will have the ability to lock on this object, and guarantee that the console sink will not be about to output anything while
        /// the lock is held.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public CrestronErrorLogSink(ITextFormatter formatter,
            object syncRoot)
        {
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            _syncRoot = syncRoot;
            //Newline Character on an embedded Linux OS on a CP4 is \n
            //To properly display new lines in Crestron Console, or other terminal emulators NewLine needs to be replace to \r\n
            _buffer.NewLine = "\r\n";
        }
        /// <summary>
        /// Emit the log
        /// </summary>
        /// <param name="logEvent">Log Level Event</param>
        public void Emit(LogEvent logEvent)
        {
            lock (_syncRoot)
            {
                _ = _buffer.GetStringBuilder().Clear();
                _formatter.Format(logEvent, _buffer);
                if (logEvent.Exception != null)
                {
                    global::Crestron.SimplSharp.ErrorLog.Exception(_buffer.ToString(), logEvent.Exception);
                    return;
                }
                switch (logEvent.Level)
                {
                    case LogEventLevel.Verbose:
                        global::Crestron.SimplSharp.ErrorLog.Ok(_buffer.ToString());
                        break;
                    case LogEventLevel.Debug:
                        global::Crestron.SimplSharp.ErrorLog.Info(_buffer.ToString());
                        break;
                    case LogEventLevel.Information:
                        global::Crestron.SimplSharp.ErrorLog.Notice(_buffer.ToString());
                        break;
                    case LogEventLevel.Warning:
                        global::Crestron.SimplSharp.ErrorLog.Warn(_buffer.ToString());
                        break;
                    case LogEventLevel.Error:
                        global::Crestron.SimplSharp.ErrorLog.Error(_buffer.ToString());
                        break;
                    case LogEventLevel.Fatal:
                        global::Crestron.SimplSharp.ErrorLog.Error(_buffer.ToString());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}