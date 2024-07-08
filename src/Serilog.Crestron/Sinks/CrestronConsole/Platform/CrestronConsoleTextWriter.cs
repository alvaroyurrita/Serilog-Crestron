using System;
using System.IO;
using System.Text;

namespace Serilog.Crestron.Sinks.CrestronConsole.Platform
{
    /// <summary>
    /// </summary>
    public class CrestronConsoleTextWriter : TextWriter
    {
        /// <summary>
        /// </summary>
        public CrestronConsoleTextWriter()
        {
        }
        /// <summary>
        /// </summary>
        /// <param name="formatProvider"></param>
        public CrestronConsoleTextWriter(IFormatProvider formatProvider) : base(formatProvider)
        {
        }
        /// <summary>
        ///     Crestron Console is capable of displaying characters above 127. Encoding is set to ISO-8859-1 so they don't get
        ///     confused with unicode.
        /// </summary>
        public override Encoding Encoding => Encoding.GetEncoding("iso-8859-1");
        /// <summary>
        ///     Writes to Crestron Console
        /// </summary>
        /// <param name="value">Value To write</param>
        public override void Write(char value)
        {
            global::Crestron.SimplSharp.CrestronConsole.Print(FormatProvider != null
                ? value.ToString(FormatProvider)
                : value.ToString());
        }
        /// <summary>
        ///     Writes to Crestron Console
        /// </summary>
        /// <param name="value">Value To write</param>
        public override void Write(string? value)
        {
            global::Crestron.SimplSharp.CrestronConsole.Print(value ?? "");
        }
        /// <summary>
        ///     Writes to Crestron Console
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public override void Write(string message, params object?[] args)
        {
            global::Crestron.SimplSharp.CrestronConsole.Print(message, args!);
        }
        /// <summary>
        ///     Writes to Crestron Console a single new line
        /// </summary>
        public override void WriteLine()
        {
            global::Crestron.SimplSharp.CrestronConsole.Print("\r\n");
        }
        /// <summary>
        ///     Writes to Crestron Console and generates a new line after
        /// </summary>
        /// <param name="value">Value To write</param>
        public override void WriteLine(string? value)
        {
            global::Crestron.SimplSharp.CrestronConsole.Print(value ?? "");
            WriteLine();
        }
        /// <summary>
        ///     Writes to Crestron Console and generates a new line after
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public override void WriteLine(string message, params object?[] args)
        {
            global::Crestron.SimplSharp.CrestronConsole.Print(message, args!);
            WriteLine();
        }
        /// <summary>
        ///     Does nothing.
        /// </summary>
        public override void Flush()
        {
            // Do nothing since we don't flush Crestron Console
        }
    }
}