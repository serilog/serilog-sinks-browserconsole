﻿using Serilog.Events;

namespace Serilog.Sinks.BrowserConsole.Output
{
    /// <summary>
    /// Used to generate `console.log` arguments.
    /// </summary>
    internal struct SConsoleToken
    {
        public SConsoleToken(string templateStr, ScalarValue arg = null)
        {
            TemplateStr = templateStr;
            Arg = arg;
        }
        public string TemplateStr { get; init; }
        public ScalarValue Arg { get; init; }

        public static SConsoleToken Style(string style) => new("%c", new ScalarValue(style));
        public static SConsoleToken Template(string rawText) => new(rawText.Replace("%", "%%"));
        public static SConsoleToken String(object @string) => new("%s", new ScalarValue(@string));
        public static SConsoleToken Integer(object @int) => new("%d", new ScalarValue(@int));
        public static SConsoleToken Float(object @float) => new("%f", new ScalarValue(@float));
        public static SConsoleToken Object(LogEventPropertyValue value, string format = default) => new("%o", new ScalarValue(ObjectModelInterop.ToInteropValue(value, format)));
    }
}
