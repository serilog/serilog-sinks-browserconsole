using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Parsing;

namespace Serilog.Sinks.BrowserConsole.Output
{
    class OutputTemplateRenderer
    {
        readonly OutputTemplateTokenRenderer[] _renderers;

        public OutputTemplateRenderer(string outputTemplate, IFormatProvider formatProvider)
        {
            if (outputTemplate is null) throw new ArgumentNullException(nameof(outputTemplate));
            var template = new MessageTemplateParser().Parse(outputTemplate);
            
            _renderers = template.Tokens
                .Select(token => token switch
                {
                    TextToken tt => new TextTokenRenderer(tt.Text),
                    PropertyToken pt => pt.PropertyName switch
                    {
                        OutputProperties.LevelPropertyName => new LevelTokenRenderer(pt) as OutputTemplateTokenRenderer,
                        OutputProperties.NewLinePropertyName => new NewLineTokenRenderer(pt.Alignment),
                        OutputProperties.ExceptionPropertyName => new ExceptionTokenRenderer(pt),
                        OutputProperties.MessagePropertyName => new MessageTemplateOutputTokenRenderer(pt, formatProvider),
                        OutputProperties.TimestampPropertyName => new TimestampTokenRenderer(pt, formatProvider),
                        "Properties" => new PropertiesTokenRenderer(pt, template, formatProvider),
                        _ => new EventPropertyTokenRenderer(pt, formatProvider)
                    },
                    _ => throw new InvalidOperationException()
                })
                .ToArray();
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            if (logEvent is null) throw new ArgumentNullException(nameof(logEvent));
            if (output is null) throw new ArgumentNullException(nameof(output));

            foreach (var renderer in _renderers)
                renderer.Render(logEvent, output);
        }
    }
}