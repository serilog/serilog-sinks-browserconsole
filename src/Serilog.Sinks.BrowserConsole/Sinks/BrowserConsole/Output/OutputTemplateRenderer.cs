using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Parsing;

namespace Serilog.Sinks.BrowserConsole.Output;

class OutputTemplateRenderer
{
    readonly OutputTemplateTokenRenderer[] _renderers;

    public OutputTemplateRenderer(string outputTemplate, IFormatProvider? formatProvider)
    {
        ArgumentNullException.ThrowIfNull(outputTemplate);
        var template = new MessageTemplateParser().Parse(outputTemplate);
            
        _renderers = template.Tokens
            .Select(token => token switch
            {
                TextToken tt => new TextTokenRenderer(tt.Text),
                PropertyToken pt => pt.PropertyName switch
                {
                    OutputProperties.LevelPropertyName => new LevelTokenRenderer(pt) as OutputTemplateTokenRenderer,
                    OutputProperties.NewLinePropertyName => new NewLineTokenRenderer(pt.Alignment),
                    OutputProperties.ExceptionPropertyName => new ExceptionTokenRenderer(),
                    OutputProperties.MessagePropertyName => new MessageTemplateOutputTokenRenderer(),
                    OutputProperties.TimestampPropertyName => new TimestampTokenRenderer(pt, formatProvider),
                    OutputProperties.PropertiesPropertyName => new PropertiesTokenRenderer(pt, template),
                    _ => new EventPropertyTokenRenderer(pt, formatProvider)
                },
                _ => throw new InvalidOperationException()
            })
            .ToArray();
    }

    public object?[] Format(LogEvent logEvent)
    {
        ArgumentNullException.ThrowIfNull(logEvent);

        var buffer = new List<object?>(_renderers.Length * 2);
        foreach (var renderer in _renderers)
        {
            renderer.Render(logEvent, buffer.Add);
        }
        return buffer.ToArray();
    }
}