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
using Serilog.Formatting.Json;
using JsonValueFormatter = Serilog.Formatting.Json.JsonValueFormatter;

namespace Serilog.Sinks.BrowserConsole.Formatting
{
    internal class DisplayValueFormatter : ValueFormatter
    {
        readonly IFormatProvider _formatProvider;

        public DisplayValueFormatter(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
        }
        
        protected override int VisitScalarValue(ValueFormatterState state, ScalarValue scalar)
        {
            if (scalar is null)
                throw new ArgumentNullException(nameof(scalar));
            return FormatLiteralValue(scalar, state.Output, state.Format);
        }

        protected override int VisitSequenceValue(ValueFormatterState state, SequenceValue sequence)
        {
            if (sequence is null)
                throw new ArgumentNullException(nameof(sequence));

            state.Output.Write('[');

            var delim = string.Empty;
            foreach (var item in sequence.Elements)
            {
                if (delim.Length != 0)
                    state.Output.Write(delim);

                delim = Delimiter;
                Visit(state, item);
            }
            
            state.Output.Write(']');

            return 0;
        }

        protected override int VisitStructureValue(ValueFormatterState state, StructureValue structure)
        {
            var count = 0;

            if (structure.TypeTag != null)
            {
                state.Output.Write(structure.TypeTag);
                state.Output.Write(' ');
            }
            
            state.Output.Write('{');

            var delim = string.Empty;
            foreach (var item in structure.Properties)
            {
                if (delim.Length != 0)
                {
                    state.Output.Write(delim);
                }

                delim = Delimiter;

                var property = item;
                
                state.Output.Write(property.Name);
                state.Output.Write('=');

                count += Visit(state.Nest(), property.Value);
            }
            
            state.Output.Write('}');
            return count;
        }

        protected override int VisitDictionaryValue(ValueFormatterState state, DictionaryValue dictionary)
        {
            var count = 0;
            
            state.Output.Write('{');

            var delim = string.Empty;
            foreach (var element in dictionary.Elements)
            {
                if (delim.Length != 0)
                    state.Output.Write(delim);

                delim = Delimiter;
                    
                state.Output.Write('[');
                count += Visit(state.Nest(), element.Key);
                state.Output.Write("]=");
                count += Visit(state.Nest(), element.Value);
            }
            
            state.Output.Write('}');

            return count;
        }

        public int FormatLiteralValue(ScalarValue scalar, TextWriter output, string format)
        {
            var value = scalar.Value;

            switch (value)
            {
                case null:
                    output.Write("null");
                    break;
                case string str:
                {
                    if (format != "l")
                        Serilog.Formatting.Json.JsonValueFormatter.WriteQuotedJsonString(str, output);
                    else
                        output.Write(str);
            
                    break;
                }
                case ValueType when value is byte or sbyte or short or ushort or int or uint or long or ulong 
                    or float or double or decimal:
                    scalar.Render(output, format, _formatProvider);
                    break;
                case bool b:
                    output.Write(b);
                    break;
                case char ch:
                    output.Write('\'');
                    output.Write(ch);
                    output.Write('\'');
                    break;
                default:
                    scalar.Render(output, format, _formatProvider);
                    break;
            }

            return 0;
        }
    }
}