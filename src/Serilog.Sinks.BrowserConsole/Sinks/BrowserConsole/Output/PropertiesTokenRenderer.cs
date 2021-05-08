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
using System.Linq;
using Serilog.Events;
using Serilog.Parsing;

namespace Serilog.Sinks.BrowserConsole.Output
{
    class PropertiesTokenRenderer : OutputTemplateTokenRenderer
    {
        readonly MessageTemplate _outputTemplate;
        private readonly PropertyToken _token;
        public PropertiesTokenRenderer(PropertyToken token, MessageTemplate outputTemplate, IFormatProvider formatProvider)
        {
            _outputTemplate = outputTemplate;
            _token = token;
        }

        public override object[] Render(LogEvent logEvent)
        {
            var included = logEvent.Properties
                .Where(p => !TemplateContainsPropertyName(logEvent.MessageTemplate, p.Key) &&
                            !TemplateContainsPropertyName(_outputTemplate, p.Key))
                .Select(p => new LogEventProperty(p.Key, p.Value));

            return included.Select(p => ObjectModelInterop.ToInteropValue(p.Value, _token.Format)).ToArray();
        }

        private static bool TemplateContainsPropertyName(MessageTemplate template, string propertyName)
        {
            foreach (var token in template.Tokens)
            {
                if (token is PropertyToken namedProperty &&
                    namedProperty.PropertyName == propertyName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}