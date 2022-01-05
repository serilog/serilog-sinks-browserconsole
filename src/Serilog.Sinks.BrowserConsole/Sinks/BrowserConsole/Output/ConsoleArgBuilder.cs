using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.BrowserConsole.Output
{
    /// <summary>
    /// Used to generate `console.log` arguments.
    /// </summary>
    internal abstract class ConsoleArgBuilder
    {
        /// <summary>
        /// Adds text to the template
        /// </summary>
        private class TemplateConsoleArgBuilder : ConsoleArgBuilder
        {
            private readonly string _text;
            public TemplateConsoleArgBuilder(string text)
            {
                _text = text;
            }

            public override void Render(StringBuilder stringBuilder, TokenEmitter addArgument)
            {
                stringBuilder.Append(_text.Replace("%", "%%"));
            }
        }

        /// <summary>
        /// Adds a style placeholder (%c) in the template and the style to the arguments list.
        /// </summary>
        private class StyleConsoleArgBuilder : ConsoleArgBuilder
        {
            private readonly string style;

            public StyleConsoleArgBuilder(string style)
            {
                this.style = style;
            }

            public override void Render(StringBuilder stringBuilder, TokenEmitter addArgument)
            {
                stringBuilder.Append("%c");
                addArgument(style);
            }
        }

        /// <summary>
        /// Adds a string placeholder (%s) in the template and the string to the arguments list.
        /// </summary>
        private class StringConsoleArgBuilder : ConsoleArgBuilder
        {
            private readonly string _text;
            public StringConsoleArgBuilder(string text)
            {
                _text = text;
            }

            public override void Render(StringBuilder stringBuilder, TokenEmitter addArgument)
            {
                stringBuilder.Append("%s");
                addArgument(_text);
            }
        }

        /// <summary>
        /// Adds an object placeholder (%o) in the template and the object to the arguments list.
        /// </summary>
        private class ObjectConsoleArgBuilder : ConsoleArgBuilder
        {
            private readonly LogEventPropertyValue _value;
            private readonly string _format;

            public ObjectConsoleArgBuilder(LogEventPropertyValue value, string format)
            {
                _value = value;
                _format = format;
            }
            public override void Render(StringBuilder stringBuilder, TokenEmitter addArgument)
            {
                stringBuilder.Append("%o");
                addArgument(ObjectModelInterop.ToInteropValue(_value, _format));
            }
        }

        /// <summary>
        /// Add a style token.
        /// </summary>
        /// <param name="style">The style list</param>
        /// <returns>The token</returns>
        public static ConsoleArgBuilder Style(string style) => new StyleConsoleArgBuilder(style);
        public static ConsoleArgBuilder Template(string rawText) => new TemplateConsoleArgBuilder(rawText);
        public static ConsoleArgBuilder String(string @string) => new StringConsoleArgBuilder(@string);
        public static ConsoleArgBuilder Object(LogEventPropertyValue value, string format = default) => new ObjectConsoleArgBuilder(value, format);

        public abstract void Render(StringBuilder stringBuilder, TokenEmitter addArgument);
    }
}
