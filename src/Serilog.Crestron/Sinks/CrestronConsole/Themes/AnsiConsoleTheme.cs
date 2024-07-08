// Copyright 2017 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Serilog.Crestron.Sinks.CrestronConsole.Themes
{
    /// <summary>
    ///     A console theme using the ANSI terminal escape sequences. Recommended
    ///     for Linux and Windows 10+.
    /// </summary>
    public class AnsiConsoleTheme : ConsoleTheme
    {
        private const string ANSI_STYLE_RESET = "\x1b[0m";
        private readonly IReadOnlyDictionary<ConsoleThemeStyle, string> _styles;
        /// <summary>
        ///     Construct a theme given a set of styles.
        /// </summary>
        /// <param name="styles">Styles to apply within the theme.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="styles" /> is <code>null</code></exception>
        public AnsiConsoleTheme(IReadOnlyDictionary<ConsoleThemeStyle, string> styles)
        {
            if (styles is null) throw new ArgumentNullException(nameof(styles));
            _styles = styles.ToDictionary(kv => kv.Key, kv => kv.Value);
        }
        /// <summary>
        ///     A 256-color theme along the lines of Visual Studio Code.
        /// </summary>
        public static AnsiConsoleTheme Code => AnsiConsoleThemes.Code;
        /// <summary>
        ///     A theme using only gray, black and white.
        /// </summary>
        public static AnsiConsoleTheme Grayscale => AnsiConsoleThemes.Grayscale;
        /// <summary>
        ///     A theme in the style of the original <i>Serilog.Sinks.Literate</i>.
        /// </summary>
        public static AnsiConsoleTheme Literate => AnsiConsoleThemes.Literate;
        /// <summary>
        ///     A theme in the style of the original <i>Serilog.Sinks.Literate</i> using only standard 16 terminal colors that will
        ///     work on light backgrounds.
        /// </summary>
        public static AnsiConsoleTheme Sixteen => AnsiConsoleThemes.Sixteen;
        /// <inheritdoc />
        public override bool CanBuffer => true;
        /// <inheritdoc />
        protected override int ResetCharCount { get; } = ANSI_STYLE_RESET.Length;
        /// <inheritdoc />
        protected override int Set(TextWriter output, ConsoleThemeStyle style)
        {
            if (!_styles.TryGetValue(style, out var ansiStyle)) return 0;
            output.Write(ansiStyle);
            return ansiStyle.Length;
        }
        /// <inheritdoc />
        public override void Reset(TextWriter output)
        {
            output.Write(ANSI_STYLE_RESET);
        }
    }
}