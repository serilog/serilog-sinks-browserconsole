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

using Serilog.Events;
using System.Collections.Generic;
using System.Linq;

namespace Serilog.Sinks.BrowserConsole.Output
{
    interface IInheritStyle
    {
        public void Render(LogEvent logEvent, TokenEmitter emitToken, List<string> styleContext);
    }
    class StyleTokenRenderer : OutputTemplateTokenRenderer, IInheritStyle
    {
        public static readonly StyleTokenRenderer Reset = new(null);

        public readonly string style;

        public StyleTokenRenderer(string style)
        {
            this.style = style?.Trim();
        }

        public override void Render(LogEvent logEvent, TokenEmitter emitToken) => Render(logEvent, emitToken, new List<string>());
        public void Render(LogEvent logEvent, TokenEmitter emitToken, List<string> styleContext)
        {
            emitToken(SConsoleToken.Style(string.Join(';', ApplyOnContext(styleContext))));
        }

        /// <summary>
        /// Modify the style context to add or remove this renderer's style.
        /// </summary>
        /// <param name="styleContext">The style context to alter.</param>
        /// <returns>the modified style context. Note that it is the same instance as <paramref name="styleContext"/></returns>
        public List<string> ApplyOnContext(List<string> styleContext)
        {
            if (string.IsNullOrEmpty(style))
                styleContext.RemoveAt(styleContext.Count - 1);
            else
                styleContext.Add(style);
            return styleContext;
        }
    }
}
