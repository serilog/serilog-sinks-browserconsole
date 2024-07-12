// Copyright 2020 Serilog Contributors
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

using Serilog.Events;

namespace Serilog.Sinks.BrowserConsole;

static class ObjectModelInterop
{
    /// <summary>
    /// Convert a Serilog <see cref="LogEventPropertyValue"/> for JavaScript interop.
    /// </summary>
    public static object? ToInteropValue(LogEventPropertyValue value, string? format = null)
    {
        switch (value)
        {
            case ScalarValue sv when format == null:
                return sv.Value;

            case ScalarValue sv:
                var sw = new StringWriter();
                sv.Render(sw, format);
                return sw.ToString();
            
            case SequenceValue sqv:
                return sqv.Elements
                    .Select(e => ToInteropValue(e))
                    .ToArray();
            
            case StructureValue st:
                return st.Properties
                    .ToDictionary(kv => kv.Name, kv => ToInteropValue(kv.Value));
            
            case DictionaryValue dv:
                return dv.Elements
                    // May generate a runtime exception if the key is null, but this is very unusual in .NET because
                    // the original dictionary that was serialized most likely was of a type without null keys. We
                    // might still do better than this in the future.
                    .ToDictionary(kv => ToInteropValue(kv.Key)!, kv => ToInteropValue(kv.Value));
            
            default:
                return value;
        }
    }
}