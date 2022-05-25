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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Serilog.Sinks.BrowserConsole.Output
{
    class TextTokenRenderer : OutputTemplateTokenRenderer, IInheritStyle
    {
        private readonly string _text;

        public TextTokenRenderer(string text)
        {
            _text = text;
        }

        public override void Render(LogEvent logEvent, TokenEmitter emitToken) => Render(logEvent, emitToken, new List<string>());
        /// <summary>
        /// Generate console tokens for the given string. Style arguments are passed under the format <code>Base text &lt;&lt;style: css;&gt;&gt;Formatted text&lt;&lt;_&gt;&gt; Unformatted text</code>
        /// </summary>
        /// <param name="logEvent">The log event to format</param>
        /// <param name="emitToken">A function yielding console arguments</param>
        /// <param name="styleContext">TODO</param>
        /// <exception cref="FormatException">Thrown if an open style tag is found, but no closing</exception>
        public void Render(LogEvent logEvent, TokenEmitter emitToken, List<string> styleContext)
        {
            var textIter = _text;
            var selfStyleContext = styleContext.ToList();
            while (!string.IsNullOrEmpty(textIter))
            {
                var openTagIndex = textIter.IndexOf("<<");
                if (openTagIndex == -1) // If no open tag, add full text & exit loop
                {
                    emitToken(SConsoleToken.Template(textIter));
                    break;
                }
                var displayText = textIter[..openTagIndex];
                emitToken(SConsoleToken.Template(displayText));

                var closeTagIndex = textIter.IndexOf(">>", openTagIndex);
                if (closeTagIndex == -1)
                {
                    throw new FormatException("Open tag found without close tag");
                }
                var styleContentComponents = textIter[(openTagIndex + 2)..closeTagIndex].Trim().Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                var styleContentOut = new List<string>(styleContentComponents.Count());
                foreach (var styleContent in styleContentComponents)
                {
                    if (styleContent == "__")
                        selfStyleContext = styleContext.ToList();
                    else if (styleContent == "_")
                        selfStyleContext.RemoveAt(selfStyleContext.Count - 1);
                    else
                        styleContentOut.Add(styleContent);
                }

                if (styleContentOut.Any())
                    selfStyleContext.Add(string.Join(';', styleContentOut));
                emitToken(SConsoleToken.Style(string.Join(';', selfStyleContext)));

                textIter = textIter[(closeTagIndex + 2)..];
            }
            styleContext.RemoveRange(0, styleContext.Count);
            styleContext.AddRange(selfStyleContext);
        }
    }
}
