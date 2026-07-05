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
using Serilog.Sinks.BrowserConsole.Rendering;

namespace Serilog.Sinks.BrowserConsole.Output;

class TimestampTokenRenderer : OutputTemplateTokenRenderer
{
    readonly PropertyToken _token;
    readonly IFormatProvider? _formatProvider;

    public TimestampTokenRenderer(PropertyToken token, IFormatProvider? formatProvider)
    {
        _token = token;
        _formatProvider = formatProvider;
    }

    public override void Render(LogEvent logEvent, TokenEmitter emitToken)
    {
        // We need access to ScalarValue.Render() to avoid this alloc; just ensures
        // that custom format providers are supported properly.
        var sv = new ScalarValue(logEvent.Timestamp);
        var buffer = new StringWriter();
        sv.Render(buffer, _token.Format, _formatProvider);
        var str = buffer.ToString();

        if (_token.Alignment is not null)
            emitToken.Text(Padding.Apply(str, _token.Alignment));
        else
            emitToken.Text(str);
    }
}