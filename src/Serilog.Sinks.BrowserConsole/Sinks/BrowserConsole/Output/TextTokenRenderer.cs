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

namespace Serilog.Sinks.BrowserConsole.Output
{
    class TextTokenRenderer : OutputTemplateTokenRenderer
    {
        private readonly string _text;

        public TextTokenRenderer(string text)
        {
            _text = text;
        }

        public override IEnumerable<ConsoleArgBuilder> ConsoleArgs(LogEvent logEvent)
        {
            var textIter = _text;
            while (!string.IsNullOrEmpty(textIter))
            {
                var openTagIndex = textIter.IndexOf("<<");
                if (openTagIndex == -1) // If no open tag, add full text & exit loop
                {
                    yield return ConsoleArgBuilder.Template(textIter);
                    yield break;
                }
                var displayText = textIter[..openTagIndex];
                yield return ConsoleArgBuilder.Template(displayText);

                var closeTagIndex = textIter.IndexOf(">>", openTagIndex);
                if (closeTagIndex == -1)
                {
                    throw new FormatException("Open tag found without close tag");
                }
                var styleContent = textIter[(openTagIndex + 2)..closeTagIndex];
                if (styleContent.Trim() == "_")
                {
                    styleContent = "";
                }
                yield return ConsoleArgBuilder.Style(styleContent);

                textIter = textIter[(closeTagIndex + 2)..];
            }
        }
    }
}
