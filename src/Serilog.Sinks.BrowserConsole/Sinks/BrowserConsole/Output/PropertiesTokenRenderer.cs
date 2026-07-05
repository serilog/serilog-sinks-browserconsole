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

class PropertiesTokenRenderer : OutputTemplateTokenRenderer
{
    readonly MessageTemplate _outputTemplate;
    readonly PropertyToken _token;
    public PropertiesTokenRenderer(PropertyToken token, MessageTemplate outputTemplate)
    {
        _outputTemplate = outputTemplate;
        _token = token;
    }

    public override void Render(LogEvent logEvent, TokenEmitter emitToken)
    {
        var included = logEvent.Properties
            .Where(p => !TemplateContainsPropertyName(logEvent.MessageTemplate, p.Key) &&
                        !TemplateContainsPropertyName(_outputTemplate, p.Key))
            .Select(p => new LogEventProperty(p.Key, p.Value));

        foreach (var property in included)
        {
            new PropertyTokenRenderer(_token, property.Value).Render(logEvent, emitToken);
        }
    }
    private void HandleProperty(LogEventProperty property, TokenEmitter emitToken)
    {
        if (property.Value is ScalarValue sv)
        {
            if(sv.Value is null)
            {
                emitToken.Object(ObjectModelInterop.ToInteropValue(property.Value, _token.Format));
                return;
            }
            switch (Type.GetTypeCode(sv.Value.GetType()))
            {
                // See https://stackoverflow.com/a/1750024
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    emitToken.Integer(sv.Value);
                    break;
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    emitToken.Float(sv.Value);
                    break;
                case TypeCode.String:
                case TypeCode.Char:
                    emitToken.Text(sv.Value);
                    break;
                default:
                    emitToken.Object(ObjectModelInterop.ToInteropValue(property.Value, _token.Format));
                    break;
            }
        }
        else
            emitToken.Object(ObjectModelInterop.ToInteropValue(property.Value, _token.Format));
    }

    static bool TemplateContainsPropertyName(MessageTemplate template, string propertyName)
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