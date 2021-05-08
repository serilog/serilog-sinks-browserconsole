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

using System.IO;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.BrowserConsole.Rendering;

namespace Serilog.Sinks.BrowserConsole.Output
{
    internal class LevelTokenRenderer : OutputTemplateTokenRenderer
    {
        private readonly PropertyToken _levelToken;

        public LevelTokenRenderer(PropertyToken levelToken)
        {
            _levelToken = levelToken;
        }

        public override object[] Render(LogEvent logEvent)
        {
            var moniker = LevelOutputFormat.GetLevelMoniker(logEvent.Level, _levelToken.Format);
            var allignedOutput = Padding.Apply(moniker, _levelToken.Alignment);
            return new object[] {allignedOutput};
        }
    }
}