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
using System.IO;
using Serilog.Crestron.Sinks.CrestronConsole.Themes;
using Serilog.Data;
using Serilog.Events;

namespace Serilog.Crestron.Sinks.CrestronConsole.Formatting
{
    internal abstract class ThemedValueFormatter : LogEventPropertyValueVisitor<ThemedValueFormatterState, int>
    {
        private readonly ConsoleTheme _theme;

        protected ThemedValueFormatter(ConsoleTheme theme) =>
            _theme = theme ?? throw new ArgumentNullException(nameof(theme));

        protected StyleReset ApplyStyle(TextWriter output, ConsoleThemeStyle style, ref int invisibleCharacterCount) =>
            _theme.Apply(output, style, ref invisibleCharacterCount);

        public int Format(LogEventPropertyValue value, TextWriter output, string? format, bool literalTopLevel = false) =>
            Visit(new ThemedValueFormatterState { Output = output, Format = format, IsTopLevel = literalTopLevel }, value);

        public abstract ThemedValueFormatter SwitchTheme(ConsoleTheme theme);
    }
}
