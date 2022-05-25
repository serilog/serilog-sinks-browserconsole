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
    class PropertyTokenRenderer : OutputTemplateTokenRenderer
    {
        readonly PropertyToken _token;
        readonly LogEventPropertyValue _propertyValue;
        public PropertyTokenRenderer(PropertyToken token, LogEventPropertyValue propertyValue)
        {
            _token = token;
            _propertyValue = propertyValue;
        }

        public override void Render(LogEvent logEvent, TokenEmitter emitToken)
        {
            if (_propertyValue is ScalarValue sv)
            {
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
                        emitToken(SConsoleToken.Integer(sv.Value));
                        break;
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Single:
                        emitToken(SConsoleToken.Float(sv.Value));
                        break;
                    case TypeCode.String:
                    case TypeCode.Char:
                        emitToken(SConsoleToken.String(sv.Value));
                        break;
                    default:
                        emitToken(SConsoleToken.Object(sv, _token.Format));
                        break;
                }
            }
            else
                emitToken(SConsoleToken.Object(_propertyValue, _token.Format));
        }
    }
}