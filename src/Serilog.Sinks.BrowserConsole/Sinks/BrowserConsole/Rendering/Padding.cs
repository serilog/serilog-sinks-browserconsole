// Copyright 2013-2017 Serilog Contributors
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
using System.Linq;
using System.Text;
using Serilog.Parsing;

namespace Serilog.Sinks.BrowserConsole.Rendering
{
    internal static class Padding
    {
        static readonly char[] PaddingChars = Enumerable.Repeat(' ', 80).ToArray();

        /// <summary>
        /// Constructs the output string containing the provided value and
        /// applying direction-based padding when <paramref name="alignment"/> is provided.
        /// </summary>
        /// <param name="value">Provided value.</param>
        /// <param name="alignment">The alignment settings to apply when rendering <paramref name="value"/>.</param>
        /// <returns>string with value and applied padding</returns>
        public static string Apply(string value, Alignment? alignment)
        {
            if (alignment is null || value.Length >= alignment.Value.Width)
            {
                return value;
            }

            var sb = new StringBuilder();
            var pad = alignment.Value.Width - value.Length;

            if (alignment.Value.Direction == AlignmentDirection.Left)
                sb.Append(value);

            sb.Append(PaddingChars, 0, pad);
            
            if (alignment.Value.Direction == AlignmentDirection.Right)
                sb.Append(value);
            return sb.ToString();
        }
    }
}