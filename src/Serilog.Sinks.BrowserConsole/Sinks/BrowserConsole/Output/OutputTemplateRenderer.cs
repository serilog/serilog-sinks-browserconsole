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
                    PropertyToken pt => WrapTokenStyle(
                        tokenStyles.GetValueOrDefault(pt.PropertyName),
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
                        new StyleTokenRenderer(style),
                        renderer,
                        new StyleTokenRenderer("")
                } :
                new[] { renderer };

        public object[] Format(LogEvent logEvent)
        {
            if (logEvent is null) throw new ArgumentNullException(nameof(logEvent));

            var buffer = new List<SConsoleToken>(_renderers.Length * 2);
            foreach (var renderer in _renderers)
            {
                renderer.Render(logEvent, buffer.Add);
            }

            var templateBuilder = new StringBuilder();
            var argsList = new List<object>(buffer.Count);
            foreach (var token in buffer)
            {
                templateBuilder.Append(token.TemplateStr);
                if (token.Arg is not null)
                {
                    argsList.Add(token.Arg.Value);
                }
            }
            return new object[] { templateBuilder.ToString() }.Concat(argsList).ToArray();
        }
    }
}