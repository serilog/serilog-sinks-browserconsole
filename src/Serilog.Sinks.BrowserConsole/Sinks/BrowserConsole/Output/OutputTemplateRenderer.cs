using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Parsing;
using System;

namespace Serilog.Sinks.BrowserConsole.Output;

class OutputTemplateRenderer
{
    readonly OutputTemplateTokenRenderer[] _renderers;
    private readonly IFormatProvider? _formatProvider;
    private readonly IReadOnlyDictionary<string, string>? _tokenStyles;
    private readonly MessageTemplate _template;

    public OutputTemplateRenderer(string outputTemplate, IFormatProvider? formatProvider, IReadOnlyDictionary<string, string>? tokenStyles = default)
    {
        ArgumentNullException.ThrowIfNull(outputTemplate);
        _formatProvider = formatProvider;
        _tokenStyles = tokenStyles;
        _template = new MessageTemplateParser().Parse(outputTemplate);

        _renderers = _template.Tokens
            .SelectMany(token => token switch
            {
                TextToken tt => [new TextTokenRenderer(tt.Text)],
                PropertyToken pt => WrapStyle(pt),
                _ => throw new InvalidOperationException()
            })
            .ToArray();
    }

    private OutputTemplateTokenRenderer[] WrapStyle(PropertyToken token)
    {
        OutputTemplateTokenRenderer renderer = token.PropertyName switch
        {
            OutputProperties.LevelPropertyName => new LevelTokenRenderer(token),
            OutputProperties.NewLinePropertyName => new NewLineTokenRenderer(token.Alignment),
            OutputProperties.ExceptionPropertyName => new ExceptionTokenRenderer(),
            OutputProperties.MessagePropertyName => new MessageTemplateOutputTokenRenderer(),
            OutputProperties.TimestampPropertyName => new TimestampTokenRenderer(token, _formatProvider),
            OutputProperties.PropertiesPropertyName => new PropertiesTokenRenderer(token, _template),
            _ => new EventPropertyTokenRenderer(token, _formatProvider)
        };
        if (_tokenStyles?.TryGetValue(token.PropertyName, out var style) ?? false)
        {
            return [new StyleTokenRenderer(style), renderer, StyleTokenRenderer.Reset];
        }
        else
        {
            return [renderer];
        }
    }

    public object?[] Format(LogEvent logEvent)
    {
        ArgumentNullException.ThrowIfNull(logEvent);

        var tokenEmitter = new TokenEmitter();
        foreach (var renderer in _renderers)
        {
            renderer.Render(logEvent, tokenEmitter);
        }
        return tokenEmitter.YieldArgs();
    }
}