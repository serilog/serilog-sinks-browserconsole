using System;
using System.Collections.Generic;
using System.IO;
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

            var renderers = new List<OutputTemplateTokenRenderer>();
            foreach (var token in template.Tokens)
            {
                if (token is TextToken tt)
                {
                    renderers.Add(new TextTokenRenderer(tt.Text));
                    continue;
                }

                var pt = (PropertyToken) token;
                if (pt.PropertyName == OutputProperties.LevelPropertyName)
                {
                    renderers.Add(new LevelTokenRenderer(pt));
                }
                else if (pt.PropertyName == OutputProperties.NewLinePropertyName)
                {
                    renderers.Add(new NewLineTokenRenderer(pt.Alignment));
                }
                else if (pt.PropertyName == OutputProperties.ExceptionPropertyName)
                {
                    renderers.Add(new ExceptionTokenRenderer(pt));
                }
                else if (pt.PropertyName == OutputProperties.MessagePropertyName)
                {
                    renderers.Add(new MessageTemplateOutputTokenRenderer(pt, formatProvider));
                }
                else if (pt.PropertyName == OutputProperties.TimestampPropertyName)
                {
                    renderers.Add(new TimestampTokenRenderer(pt, formatProvider));
                }
                else if (pt.PropertyName == "Properties")
                {
                    renderers.Add(new PropertiesTokenRenderer(pt, template, formatProvider));
                }
                else
                {
                    renderers.Add(new EventPropertyTokenRenderer(pt, formatProvider));
                }
            }

            _renderers = renderers.ToArray();
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