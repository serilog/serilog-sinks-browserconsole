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

using Microsoft.JSInterop;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.BrowserConsole;
using Serilog.Sinks.BrowserConsole.Output;

namespace Serilog;

/// <summary>
/// Adds the WriteTo.BrowserConsole() extension method to <see cref="LoggerConfiguration"/>.
/// </summary>
public static class LoggerConfigurationBrowserConsoleExtensions
{
    const string SerilogToken =
        "<<color:white;background:#8c7574;border-radius:3px;padding:1px 2px;font-weight:600;>>serilog<<_>>";

    const string DefaultConsoleOutputTemplate = SerilogToken + "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

    /// <summary>
    /// Writes log events to the browser console.
    /// </summary>
    /// <param name="sinkConfiguration">Logger sink configuration.</param>
    /// <param name="restrictedToMinimumLevel">The minimum level for
    /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
    /// <param name="outputTemplate">A message template describing the format used to write to the sink.
    /// The default is <code>"(serilog) [{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"</code>.</param>
    /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
    /// <param name="tokenStyles">A dictionary of styles to apply to tokens. See <a href="https://developer.mozilla.org/en-US/docs/Web/API/console#styling_console_output">MDN about console styling</a></param>
    /// <param name="levelSwitch">A switch allowing the pass-through minimum level
    /// to be changed at runtime.</param>
    /// <param name="jsRuntime">An instance of <see cref="IJSRuntime"/> to interact with the browser.</param>
    /// <returns>Configuration object allowing method chaining.</returns>
    public static LoggerConfiguration BrowserConsole(
        this LoggerSinkConfiguration sinkConfiguration,
        LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
        string outputTemplate = DefaultConsoleOutputTemplate,
        IFormatProvider? formatProvider = null,
        IReadOnlyDictionary<string, string>? tokenStyles = null,
        LoggingLevelSwitch? levelSwitch = null,
        IJSRuntime? jsRuntime = null)
    {
        ArgumentNullException.ThrowIfNull(sinkConfiguration);
        var formatter = new OutputTemplateRenderer(outputTemplate, formatProvider, tokenStyles); 
        return sinkConfiguration.Sink(new BrowserConsoleSink(jsRuntime, formatter), restrictedToMinimumLevel, levelSwitch);
    }
}