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
using Microsoft.JSInterop;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.BrowserConsole.Output;

namespace Serilog.Sinks.BrowserConsole
{
    internal class BrowserConsoleSink : ILogEventSink
    {
        private readonly IJSRuntime _runtime;
        private readonly OutputTemplateRenderer _formatter;

        public BrowserConsoleSink(IJSRuntime runtime, OutputTemplateRenderer formatter)
        {
            _runtime = runtime ?? DefaultWebAssemblyJSRuntime.Instance;
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        }

        public async void Emit(LogEvent logEvent)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            var outputStream = SelectConsoleMethod(logEvent.Level);
            var args = _formatter.Format(logEvent);
            await _runtime.InvokeAsync<string>(outputStream, args);
        }

        private static string SelectConsoleMethod(LogEventLevel logLevel) =>
            logLevel switch
            {
                >= LogEventLevel.Error => "console.error",
                LogEventLevel.Warning => "console.warn",
                LogEventLevel.Information => "console.info",
                _ => "console.log"
            };
    }
}
