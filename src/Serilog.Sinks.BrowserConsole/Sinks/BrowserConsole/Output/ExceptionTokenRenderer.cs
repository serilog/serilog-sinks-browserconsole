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
using Serilog.Events;

namespace Serilog.Sinks.BrowserConsole.Output
{
    internal class ExceptionTokenRenderer : OutputTemplateTokenRenderer
    {
        /// <summary>
        /// "Renders" <see cref="Exception"/> to array of single exception element, if exception is not <see langword="null"/>.
        /// If logevent has no exception set the result will be empty array of objects
        /// </summary>
        /// <param name="logEvent">Logging event that should be rendered</param>
        /// <returns>Array of objects to pass to browser console</returns>
        public override object[] Render(LogEvent logEvent) =>
            logEvent.Exception is null ? 
                Array.Empty<object>() : new object[] {logEvent.Exception.ToString()};
    }
}