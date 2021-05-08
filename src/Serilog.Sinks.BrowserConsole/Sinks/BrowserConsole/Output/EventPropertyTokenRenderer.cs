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
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.BrowserConsole.Rendering;

namespace Serilog.Sinks.BrowserConsole.Output
{
    internal class EventPropertyTokenRenderer : OutputTemplateTokenRenderer
    {
        private readonly PropertyToken _token;
        private readonly IFormatProvider _formatProvider;

        public EventPropertyTokenRenderer(PropertyToken token, IFormatProvider formatProvider)
        {
            _token = token;
            _formatProvider = formatProvider;
        }

        public override object[] Render(LogEvent logEvent)
        {
            // If a property is missing, don't render anything (message templates render the raw token here).
            if (!logEvent.Properties.TryGetValue(_token.PropertyName, out var propertyValue))
                return new object[] {Padding.Apply(string.Empty, _token.Alignment)};


            var writer = new StringWriter();

            // If the value is a scalar string, support some additional formats: 'u' for uppercase
            // and 'w' for lowercase.
            if (propertyValue is ScalarValue {Value: string literalString})
            {
                var cased = Casing.Format(literalString, _token.Format);
                writer.Write(cased);
            }
            else
            {
                propertyValue.Render(writer, _token.Format, _formatProvider);
            }

            var str = writer.ToString();
            return new object[]
            {
                _token.Alignment switch
                {
                    null => str,
                    { } => Padding.Apply(str, _token.Alignment)
                }
            };
        }
    }
}