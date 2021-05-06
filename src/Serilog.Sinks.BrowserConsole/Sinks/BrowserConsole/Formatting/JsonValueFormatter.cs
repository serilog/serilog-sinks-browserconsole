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
using System.Globalization;
using System.IO;
using Serilog.Events;

namespace Serilog.Sinks.BrowserConsole.Formatting
{
    internal class JsonValueFormatter : ValueFormatter
    {
        readonly DisplayValueFormatter _displayFormatter;
        readonly IFormatProvider _formatProvider;

        public JsonValueFormatter(IFormatProvider formatProvider)
        {
            _displayFormatter = new DisplayValueFormatter(formatProvider);
            _formatProvider = formatProvider;
        }

        protected override int VisitScalarValue(ValueFormatterState state, ScalarValue scalar)
        {
            if (scalar is null)
                throw new ArgumentNullException(nameof(scalar));

            // At the top level, for scalar values, use "display" rendering.
            return state.IsTopLevel
                ? _displayFormatter.FormatLiteralValue(scalar, state.Output, state.Format)
                : FormatLiteralValue(scalar, state.Output);
        }

        protected override int VisitSequenceValue(ValueFormatterState state, SequenceValue sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            state.Output.Write('[');

            var delim = string.Empty;
            foreach (var item in sequence.Elements)
            {
                if (delim.Length != 0)
                    state.Output.Write(delim);

                delim = Delimiter;
                Visit(state.Nest(), item);
            }

            state.Output.Write(']');
            return 0;
        }

        protected override int VisitStructureValue(ValueFormatterState state, StructureValue structure)
        {
            var count = 0;


            state.Output.Write('{');

            var delim = string.Empty;
            foreach (var item in structure.Properties)
            {
                if (delim.Length != 0)
                    state.Output.Write(delim);

                delim = Delimiter;

                Serilog.Formatting.Json.JsonValueFormatter.WriteQuotedJsonString(item.Name, state.Output);
                state.Output.Write(": ");
                count += Visit(state.Nest(), item.Value);
            }

            if (structure.TypeTag != null)
            {
                state.Output.Write(delim);
                Serilog.Formatting.Json.JsonValueFormatter.WriteQuotedJsonString("$type", state.Output);
                state.Output.Write(": ");
                Serilog.Formatting.Json.JsonValueFormatter.WriteQuotedJsonString(structure.TypeTag, state.Output);
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

                Serilog.Formatting.Json.JsonValueFormatter.WriteQuotedJsonString((element.Key.Value ?? "null").ToString(), state.Output);
                state.Output.Write(": ");
                count += Visit(state.Nest(), element.Value);
            }

            state.Output.Write('}');
            return count;
        }

        int FormatLiteralValue(ScalarValue scalar, TextWriter output)
        {
            var value = scalar.Value;
            
            switch (value)
            {
                case null:
                    output.Write("null");
                    break;
                case string str:
                    Serilog.Formatting.Json.JsonValueFormatter.WriteQuotedJsonString(str, output);
                    break;
                case ValueType and (byte or sbyte
                    or short or ushort 
                    or int or uint 
                    or long or ulong
                    or decimal):
                    output.Write(((IFormattable) value).ToString(null, CultureInfo.InvariantCulture));
                    break;
                case double d:
                {
                    if (double.IsNaN(d) || double.IsInfinity(d))
                        Serilog.Formatting.Json.JsonValueFormatter.WriteQuotedJsonString(d.ToString(CultureInfo.InvariantCulture), output);
                    else
                        output.Write(d.ToString("R", CultureInfo.InvariantCulture));
                    break;
                }
                case float f:
                {
                    if (double.IsNaN(f) || double.IsInfinity(f))
                        Serilog.Formatting.Json.JsonValueFormatter.WriteQuotedJsonString(f.ToString(CultureInfo.InvariantCulture), output);
                    else
                        output.Write(f.ToString("R", CultureInfo.InvariantCulture));
                    break;
                }
                case bool b:
                    output.Write(b ? "true" : "false");
                    break;
                case char ch:
                    Serilog.Formatting.Json.JsonValueFormatter.WriteQuotedJsonString(ch.ToString(), output);
                    break;
                case ValueType when value is DateTime or DateTimeOffset:
                    output.Write('"');
                    output.Write(((IFormattable) value).ToString("O", CultureInfo.InvariantCulture));
                    output.Write('"');
                    break;
                default:
                    Serilog.Formatting.Json.JsonValueFormatter.WriteQuotedJsonString(value.ToString(), output);
                    break;
            }

            return 0;
        }
    }
}