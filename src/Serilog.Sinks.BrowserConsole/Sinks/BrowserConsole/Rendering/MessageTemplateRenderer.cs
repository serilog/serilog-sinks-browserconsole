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
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.BrowserConsole.Formatting;

namespace Serilog.Sinks.BrowserConsole.Rendering
{
    internal class MessageTemplateRenderer
    {
        readonly ValueFormatter _valueFormatter;
        readonly bool _isLiteral;

        public MessageTemplateRenderer(ValueFormatter valueFormatter, bool isLiteral)
        {
            _valueFormatter = valueFormatter;
            _isLiteral = isLiteral;
        }

        public string Render(MessageTemplate template, IReadOnlyDictionary<string, LogEventPropertyValue> properties)
        {
            var output = new StringWriter();
            foreach (var token in template.Tokens)
            {
                switch (token)
                {
                    case TextToken tt:
                        RenderTextToken(tt, output);
                        break;
                    case PropertyToken pt:
                        RenderPropertyToken(pt, properties, output);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            return output.ToString();
        }

        int RenderTextToken(TextToken tt, TextWriter output)
        {
            output.Write(tt.Text);
            return 0;
        }

        int RenderPropertyToken(PropertyToken pt, IReadOnlyDictionary<string, LogEventPropertyValue> properties,
            TextWriter output)
        {
            if (!properties.TryGetValue(pt.PropertyName, out var propertyValue))
            {
                output.Write(pt.ToString());
                return 0;
            }

            if (!pt.Alignment.HasValue)
            {
                return RenderValue(_valueFormatter, propertyValue, output, pt.Format);
            }

            return RenderAlignedPropertyTokenUnbuffered(pt, output, propertyValue);
        }

        int RenderAlignedPropertyTokenUnbuffered(PropertyToken pt, TextWriter output,
            LogEventPropertyValue propertyValue)
        {
            var valueOutput = new StringWriter();
            RenderValue(_valueFormatter, propertyValue, valueOutput, pt.Format);

            var valueLength = valueOutput.ToString().Length;
            // ReSharper disable once PossibleInvalidOperationException
            if (valueLength >= pt.Alignment.Value.Width)
                return RenderValue(_valueFormatter, propertyValue, output, pt.Format);


            if (pt.Alignment.Value.Direction == AlignmentDirection.Left)
            {
                var invisible = RenderValue(_valueFormatter, propertyValue, output, pt.Format);
                output.Write(Padding.Apply(string.Empty, pt.Alignment.Value.Widen(-valueLength)));
                return invisible;
            }

            output.Write(Padding.Apply(string.Empty, pt.Alignment.Value.Widen(-valueLength)));
            return RenderValue(_valueFormatter, propertyValue, output, pt.Format);
        }

        int RenderValue(ValueFormatter valueFormatter, LogEventPropertyValue propertyValue, TextWriter output,
            string format)
        {
            if (_isLiteral && propertyValue is ScalarValue {Value: string} sv)
            {
                output.Write(sv.Value);
                return 0;
            }

            return valueFormatter.Format(propertyValue, output, format, _isLiteral);
        }
    }
}