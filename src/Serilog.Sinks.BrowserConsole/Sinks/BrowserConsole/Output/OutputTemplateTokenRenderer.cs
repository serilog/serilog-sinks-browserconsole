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
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Serilog.Sinks.BrowserConsole.Output;

class TokenEmitter
{
    private StringBuilder _template = new();
    private List<object?> _args = [];

    internal void Literal(string template)
    {
        _template.Append(template.Replace("%", "%%"));
    }

    internal void Text(object @string)
    {
        _template.Append("%s");
        _args.Add(@string);
    }
    internal void Text(string @string) => Text((object)@string);

    internal void Object(object? @object)
    {
        _template.Append("%o");
        _args.Add(@object);
    }

    internal void Integer(object @int)
    {
        _template.Append("%d");
        _args.Add(@int);
    }

    internal void Float(object @float) {
        _template.Append("%f");
        _args.Add(@float);
    }

    internal object?[] YieldArgs() => [_template.ToString(), .. _args];

    internal void Style(string styleContent)
    {
        _template.Append("%c");
        _args.Add(styleContent);
    }
} 
    
abstract class OutputTemplateTokenRenderer
{
    public abstract void Render(LogEvent logEvent, TokenEmitter emitToken);
}