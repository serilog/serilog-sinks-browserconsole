using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Parsing;

namespace Serilog.Sinks.BrowserConsole.Output
{
    class OutputTemplateRenderer
    {
        readonly OutputTemplateTokenRenderer[] _renderers;

        public OutputTemplateRenderer(string outputTemplate, IFormatProvider formatProvider, IReadOnlyDictionary<string, string> tokenStyles = default)
        {
            if (outputTemplate is null) throw new ArgumentNullException(nameof(outputTemplate));
            var template = new MessageTemplateParser().Parse(outputTemplate);

            _renderers = template.Tokens
                .SelectMany(token => token switch
                {
                    TextToken tt => new[] { new TextTokenRenderer(tt.Text) }, // TextTokenRenderer is in charge of parsing `<<` flags for styles
                    PropertyToken pt => WrapTokenStyle(
                        tokenStyles?.GetValueOrDefault(pt.PropertyName),
                        pt.PropertyName switch
                        {
                            OutputProperties.LevelPropertyName => new LevelTokenRenderer(pt),
                            OutputProperties.NewLinePropertyName => new NewLineTokenRenderer(pt.Alignment),
                            OutputProperties.ExceptionPropertyName => new ExceptionTokenRenderer(),
                            OutputProperties.MessagePropertyName => new MessageTemplateOutputTokenRenderer(),
                            OutputProperties.TimestampPropertyName => new TimestampTokenRenderer(pt, formatProvider),
                            OutputProperties.PropertiesPropertyName => new PropertiesTokenRenderer(pt, template),
                            _ => new EventPropertyTokenRenderer(pt, formatProvider)
                        }),
                    _ => throw new InvalidOperationException()
                })
                .ToArray();
        }

        private static IEnumerable<OutputTemplateTokenRenderer> WrapTokenStyle(string style, OutputTemplateTokenRenderer renderer) =>
            style != null ?
                new[]
                {
                        new StyleTokenRenderer(style), // Define the desired styles
                        renderer,
                        StyleTokenRenderer.Reset // Reset the style after the actual internal renderer has been injected.
                } :
                new[] { renderer };

        public object[] Format(LogEvent logEvent)
        {
            if (logEvent is null) throw new ArgumentNullException(nameof(logEvent));

            // Collect tokens. Tokens contains the string to append to the `console.log` 1st argument (like a plain string, or `%c` for styles), and the argument to push after.
            var tokensBuffer = new List<SConsoleToken>(_renderers.Length * 2);
            foreach (var renderer in _renderers)
            {
                renderer.Render(logEvent, tokensBuffer.Add);
            }

            // Now that we have all tokens, build the full `console.log("TEMPLATE", ...args)` template & args list.
            var templateBuilder = new StringBuilder();
            var argsList = new List<object>(tokensBuffer.Count);
            foreach (var token in tokensBuffer)
            {
                templateBuilder.Append(token.TemplateStr);
                // Some tokens might not have an argument to push to `console.log`.
                if (token.Arg is not null)
                {
                    argsList.Add(token.Arg.Value);
                }
            }
            // Return the full arguments list.
            return new object[] { templateBuilder.ToString() }.Concat(argsList).ToArray();
        }
    }
}