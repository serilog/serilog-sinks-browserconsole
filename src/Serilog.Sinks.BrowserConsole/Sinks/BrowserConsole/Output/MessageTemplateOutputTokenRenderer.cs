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
using System.IO;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.BrowserConsole.Formatting;
using Serilog.Sinks.BrowserConsole.Rendering;

namespace Serilog.Sinks.BrowserConsole.Output
{
    class MessageTemplateOutputTokenRenderer : OutputTemplateTokenRenderer
    {
        readonly PropertyToken _token;
        readonly MessageTemplateRenderer _renderer;

        public MessageTemplateOutputTokenRenderer(PropertyToken token, IFormatProvider formatProvider)
        {
            _token = token ?? throw new ArgumentNullException(nameof(token));

            var isLiteral = token.Format?.Contains('l', StringComparison.Ordinal) == true;
            var isJson = token.Format?.Contains('j', StringComparison.Ordinal) == true;
            
            var valueFormatter = isJson
                ? (ValueFormatter)new JsonValueFormatter(formatProvider)
                : new DisplayValueFormatter(formatProvider);

            _renderer = new MessageTemplateRenderer(valueFormatter, isLiteral);
        }

        public override void Render(LogEvent logEvent, TextWriter output)
        {
            if (_token.Alignment is null)
            {
                _renderer.Render(logEvent.MessageTemplate, logEvent.Properties, output);
                return;
            }

            var buffer = new StringWriter();
            var invisible = _renderer.Render(logEvent.MessageTemplate, logEvent.Properties, buffer);
            var value = buffer.ToString();
            Padding.Apply(output, value, _token.Alignment.Value.Widen(invisible));
        }
    }
}