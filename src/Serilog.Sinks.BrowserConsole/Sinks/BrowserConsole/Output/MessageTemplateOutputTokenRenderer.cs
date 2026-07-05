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
using Serilog.Parsing;

namespace Serilog.Sinks.BrowserConsole.Output;

class MessageTemplateOutputTokenRenderer : OutputTemplateTokenRenderer
{   
    public override void Render(LogEvent logEvent, TokenEmitter emitToken)
    {
        foreach (var token in logEvent.MessageTemplate.Tokens)
        {
            switch (token)
            {
                case TextToken tt:
                    new TextTokenRenderer(tt.Text).Render(logEvent, emitToken);
                    break;
                case PropertyToken pt:
                    if (logEvent.Properties.TryGetValue(pt.PropertyName, out var propertyValue))
                    {
                        new PropertyTokenRenderer(pt, propertyValue).Render(logEvent, emitToken);
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
