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
using Serilog.Crestron.Sinks.CrestronConsole.Rendering;
using Serilog.Crestron.Sinks.CrestronConsole.Themes;
using Serilog.Events;
using Serilog.Parsing;

namespace Serilog.Crestron.Sinks.CrestronConsole.Output
{
    internal class EventPropertyTokenRenderer : OutputTemplateTokenRenderer
    {
        private readonly IFormatProvider? _formatProvider;
        private readonly ConsoleTheme _theme;
        private readonly PropertyToken _token;
        public EventPropertyTokenRenderer(ConsoleTheme theme, PropertyToken token, IFormatProvider? formatProvider)
        {
            _theme = theme;
            _token = token;
            _formatProvider = formatProvider;
        }
        public override void Render(LogEvent logEvent, TextWriter output)
        {
            // If a property is missing, don't render anything (message templates render the raw token here).
            if (!logEvent.Properties.TryGetValue(_token.PropertyName, out var propertyValue))
            {
                Padding.Apply(output, string.Empty, _token.Alignment);
                return;
            }
            var _ = 0;
            using (_theme.Apply(output, ConsoleThemeStyle.SecondaryText, ref _))
            {
                var writer = _token.Alignment.HasValue ? new StringWriter() : output;

                // If the value is a scalar string, support some additional formats: 'u' for uppercase
                // and 'w' for lowercase.
                if (propertyValue is ScalarValue { Value: string literalString } sv)
                {
                    var cased = Casing.Format(literalString, _token.Format);
                    writer.Write(cased);
                }
                else
                {
                    propertyValue.Render(writer, _token.Format, _formatProvider);
                }
                if (!_token.Alignment.HasValue) return;
                var str = writer.ToString()!;
                Padding.Apply(output, str, _token.Alignment);
            }
        }
    }
}