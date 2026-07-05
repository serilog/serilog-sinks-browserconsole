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

namespace Serilog.Sinks.BrowserConsole.Output;

class TextTokenRenderer : OutputTemplateTokenRenderer
{
    readonly string _text;

    public TextTokenRenderer(string text)
    {
        _text = text;
    }

    public override void Render(LogEvent logEvent, TokenEmitter emitToken)
    {
        var textIter = _text;
        while (!string.IsNullOrEmpty(textIter))
        {
            var openTagIndex = textIter.IndexOf("<<");
            if (openTagIndex == -1) // If no open tag, add full text & exit loop
            {
                emitToken.Literal(textIter);
                return;
            }
            var displayText = textIter[..openTagIndex];
            emitToken.Literal(displayText);

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
            emitToken.Style(styleContent);

            textIter = textIter[(closeTagIndex + 2)..];
        }
    }
}