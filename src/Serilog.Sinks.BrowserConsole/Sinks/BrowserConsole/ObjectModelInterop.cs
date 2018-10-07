// Copyright 2018 Serilog Contributors
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

using System.IO;
using System.Linq;
using Serilog.Events;

namespace Serilog.Sinks.BrowserConsole
{
    static class ObjectModelInterop
    {
        /// <summary>
        /// Convert a Serilog <see cref="LogEventPropertyValue"/> for JavaScript interop.
        /// </summary>
        public static object ToInteropValue(LogEventPropertyValue value, string format = null)
        {
            if (value is ScalarValue sv)
            {
                if (format == null)
                    return sv.Value;
                var sw = new StringWriter();
                sv.Render(sw, format);
                return sw.ToString();
            }

            if (value is SequenceValue sqv)
            {
                return sqv.Elements
                    .Select(e => ToInteropValue(e))
                    .ToArray();
            }

            if (value is StructureValue st)
            {
                return st.Properties
                    .ToDictionary(kv => kv.Name, kv => ToInteropValue(kv.Value));
            }

            var dv = (DictionaryValue)value;
            return dv.Elements
                .ToDictionary(kv => ToInteropValue(kv.Key), kv => ToInteropValue(kv.Value));
        }
    }
}