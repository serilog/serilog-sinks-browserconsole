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

using System;
using System.Collections.Generic;
using Microsoft.JSInterop;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;

namespace Serilog.Sinks.BrowserConsole
{
    class BrowserConsoleSink : ILogEventSink
    {
        private readonly IJSRuntime _runtime;

        public BrowserConsoleSink(IJSRuntime runtime)
        {
            _runtime = runtime ?? DefaultWebAssemblyJSRuntime.Instance;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            var outputStream = SelectConsoleMethod(logEvent.Level);
            var args = Format(logEvent);
            _runtime.InvokeAsync<string>(outputStream, args);
        }

        static string SelectConsoleMethod(LogEventLevel logLevel)
        {
            if (logLevel >= LogEventLevel.Error)
                return "console.error";
            if (logLevel == LogEventLevel.Warning)
                return "console.warn";
            if (logLevel == LogEventLevel.Information)
                return "console.info";
            return "console.log";
        }

        static object[] Format(LogEvent logEvent)
        {
            var result = new List<object>();

            foreach (var token in logEvent.MessageTemplate.Tokens)
            {
                if (token is TextToken tt)
                {
                    result.Add(tt.Text);
                }
                else
                {
                    var pt = (PropertyToken) token;
                    if (logEvent.Properties.TryGetValue(pt.PropertyName, out var pv))
                    {
                        result.Add(ObjectModelInterop.ToInteropValue(pv, pt.Format));
                    }
                    else
                    {
                        result.Add(null);
                    }
                }
            }

            if (logEvent.Exception != null)
            {
                result.Add(Environment.NewLine);
                result.Add(logEvent.Exception.ToString());
            }

            return result.ToArray();
        }
    }
}
