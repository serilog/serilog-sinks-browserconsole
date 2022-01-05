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
                    TextToken tt => new[] { new TextTokenRenderer(tt.Text) },
                    PropertyToken pt => WrapStyle(pt, tokenStyles, pt.PropertyName switch
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

        private IEnumerable<OutputTemplateTokenRenderer> WrapStyle(PropertyToken token, IReadOnlyDictionary<string, string> tokenStyles, OutputTemplateTokenRenderer renderer)
        {
            if (tokenStyles?.TryGetValue(token.PropertyName, out var style) ?? false)
            {
                return new[]
                {
                    new TextTokenRenderer($"<<{style}>>"),
                    renderer,
                    new TextTokenRenderer("<<_>>")
                };
            }
            else
            {
                return new[] { renderer };
            }
        }

        public object[] Format(LogEvent logEvent)
        {
            if (logEvent is null) throw new ArgumentNullException(nameof(logEvent));

            var templateBuilder = new StringBuilder();
            var buffer = new List<object>(_renderers.Length * 2);
            foreach (var renderer in _renderers)
            {
                foreach (var consoleToken in renderer.ConsoleArgs(logEvent))
                {
                    consoleToken.Render(templateBuilder, buffer.Add);
                }
            }
            return buffer.Prepend(templateBuilder.ToString()).ToArray();
        }
    }
}