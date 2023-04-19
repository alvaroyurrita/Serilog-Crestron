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
using System.Linq;
using System.Text;
using Serilog.Crestron.Sinks.CrestronConsole.Formatting;
using Serilog.Crestron.Sinks.CrestronConsole.Rendering;
using Serilog.Crestron.Sinks.CrestronConsole.Themes;
using Serilog.Events;
using Serilog.Parsing;

namespace Serilog.Crestron.Sinks.CrestronConsole.Output
{
    internal class PropertiesTokenRenderer : OutputTemplateTokenRenderer
    {
        private readonly MessageTemplate _outputTemplate;
        private readonly ConsoleTheme _theme;
        private readonly PropertyToken _token;
        private readonly ThemedValueFormatter _valueFormatter;

        public PropertiesTokenRenderer(ConsoleTheme theme, PropertyToken token, MessageTemplate outputTemplate, IFormatProvider? formatProvider)
        {
            _outputTemplate = outputTemplate;
            _theme = theme ?? throw new ArgumentNullException(nameof(theme));
            _token = token ?? throw new ArgumentNullException(nameof(token));

            var isJson = false;

            if (token.Format != null)
            {
                foreach (var t in token.Format.Where(t => t == 'j'))
                {
                    isJson = true;
                }
            }

            _valueFormatter = isJson
                ? (ThemedValueFormatter)new ThemedJsonValueFormatter(theme, formatProvider)
                : new ThemedDisplayValueFormatter(theme, formatProvider);
        }

        public override void Render(LogEvent logEvent, TextWriter output)
        {
            var included = logEvent.Properties
                .Where(p => !TemplateContainsPropertyName(logEvent.MessageTemplate, p.Key) &&
                            !TemplateContainsPropertyName(_outputTemplate, p.Key))
                .Select(p => new LogEventProperty(p.Key, p.Value));

            var value = new StructureValue(included);

            if (_token.Alignment is null || !_theme.CanBuffer)
            {
                _ = _valueFormatter.Format(value, output, null);
                return;
            }

            var buffer = new StringWriter(new StringBuilder(value.Properties.Count * 16));
            var invisible = _valueFormatter.Format(value, buffer, null);
            var str = buffer.ToString();
            Padding.Apply(output, str, _token.Alignment.Value.Widen(invisible));
        }
        private static bool TemplateContainsPropertyName(MessageTemplate template, string propertyName)
        {
            foreach (var token in template.Tokens)
            {
                if (token is PropertyToken namedProperty &&
                    namedProperty.PropertyName == propertyName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}