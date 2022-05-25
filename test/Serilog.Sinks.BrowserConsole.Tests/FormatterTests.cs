using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Parsing;
using Serilog.Sinks.BrowserConsole.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Serilog.Sinks.BrowserConsole.Tests
{
    public class StyleFormatterTests
    {
        const string STYLE1 = "s1";
        const string STYLE2 = "s2";
        const string STYLE3 = "s3";
        [Fact]
        public void SupportsStylingSimple()
        {
            var formatter = new OutputTemplateRenderer($"<<{STYLE1}>>Hello<<_>> <<{STYLE2}>>World<<_>>", default);
            var args = formatter.Format(new LogEvent(DateTimeOffset.Now, LogEventLevel.Verbose, null, MessageTemplate.Empty, Array.Empty<LogEventProperty>()));
            Assert.Equal(new object[] { "%cHello%c %cWorld%c", STYLE1, "", STYLE2, "" }, args);
        }

        [Fact]
        public void SupportsStylingWithTimestamp()
        {
            var NOW = DateTimeOffset.Now;
            var formatter = new OutputTemplateRenderer($"<<{STYLE1}>>Hello<<_>> <<{STYLE2}>>{{{OutputProperties.TimestampPropertyName}:HH:mm}}<<_>>", default);
            var args = formatter.Format(new LogEvent(NOW, LogEventLevel.Verbose, null, MessageTemplate.Empty, Array.Empty<LogEventProperty>()));
            Assert.Equal(new object[] { $"%cHello%c %c%s%c", STYLE1, "", STYLE2, NOW.ToString("HH:mm"), "" }, args);
        }

        [Fact]
        public void SupportsStylingWithNewLine()
        {
            var formatter = new OutputTemplateRenderer($"<<{STYLE1}>>Hello<<_>> <<{STYLE2}>>{{{OutputProperties.NewLinePropertyName}}}<<_>>", default);
            var args = formatter.Format(new LogEvent(DateTimeOffset.Now, LogEventLevel.Verbose, null, MessageTemplate.Empty, Array.Empty<LogEventProperty>()));
            Assert.Equal(new object[] { $"%cHello%c %c{Environment.NewLine}%c", STYLE1, "", STYLE2, "" }, args);
        }

        [Fact]
        public void SupportsStylingWithLevel()
        {
            var LEVEL = LogEventLevel.Verbose;
            var formatter = new OutputTemplateRenderer($"<<{STYLE1}>>Hello<<_>> <<{STYLE2}>>{{{OutputProperties.LevelPropertyName}}}<<_>>", default);
            var args = formatter.Format(new LogEvent(DateTimeOffset.Now, LEVEL, null, MessageTemplate.Empty, Array.Empty<LogEventProperty>()));
            Assert.Equal(new object[] { $"%cHello%c %c%s%c", STYLE1, "", STYLE2, LEVEL.ToString(), "" }, args);
        }

        [Fact]
        public void SupportsStylingWithException()
        {
            var EXCEPTION = new Exception("Foo did bar");
            var formatter = new OutputTemplateRenderer($"<<{STYLE1}>>Hello<<_>> <<{STYLE2}>>{{{OutputProperties.ExceptionPropertyName}}}<<_>>", default);
            var args = formatter.Format(new LogEvent(DateTimeOffset.Now, LogEventLevel.Verbose, EXCEPTION, MessageTemplate.Empty, Array.Empty<LogEventProperty>()));
            Assert.Equal(new object[] { $"%cHello%c %c%s%c", STYLE1, "", STYLE2, EXCEPTION.ToString(), "" }, args);
        }

        [Fact]
        public void SupportsStylingWithProperties()
        {
            var PROPERTIES = new[] {
                new LogEventProperty("Foo", new ScalarValue(42)),
            };
            var formatter = new OutputTemplateRenderer($"<<{STYLE1}>>Hello<<_>> <<{STYLE2}>>{{{OutputProperties.PropertiesPropertyName}}}<<_>>", default);
            var args = formatter.Format(new LogEvent(DateTimeOffset.Now, LogEventLevel.Verbose, null, MessageTemplate.Empty, PROPERTIES));
            Assert.Equal(new object[] { $"%cHello%c %c%o%c", STYLE1, "", STYLE2, ((ScalarValue)PROPERTIES[0].Value).Value, "" }, args);
        }

        [Fact]
        public void SupportsStylingWithSimpleMessage()
        {
            var MESSAGE = "and welcome";
            var formatter = new OutputTemplateRenderer($"<<{STYLE1}>>Hello<<_>> <<{STYLE2}>>{{{OutputProperties.MessagePropertyName}}}<<_>>", default);
            var args = formatter.Format(new LogEvent(DateTimeOffset.Now, LogEventLevel.Verbose, null, new MessageTemplateParser().Parse(MESSAGE), Array.Empty<LogEventProperty>()));
            Assert.Equal(new object[] { $"%cHello%c %c{MESSAGE}%c", STYLE1, "", STYLE2, "" }, args);
        }

        [Fact]
        public void SupportsStylingWithMessageContainingInterpolation()
        {
            var PLACE = "my tests";
            var MESSAGE = "and welcome to {Place}";
            var formatter = new OutputTemplateRenderer($"<<{STYLE1}>>Hello<<_>> <<{STYLE2}>>{{{OutputProperties.MessagePropertyName}}}<<_>>", default);
            var args = formatter.Format(new LogEvent(DateTimeOffset.Now, LogEventLevel.Verbose, null, new MessageTemplateParser().Parse(MESSAGE), new[]{
                new LogEventProperty("Place", new ScalarValue(PLACE)),
            }));
            Assert.Equal(new object[] { $"%cHello%c %c{MESSAGE.Replace("{Place}", "%o")}%c", STYLE1, "", STYLE2, PLACE, "" }, args);
        }

        [Fact]
        public void SupportsStylingWithinSimpleMessage()
        {
            var MESSAGE = $"and <<{STYLE3}>>welcome";
            var formatter = new OutputTemplateRenderer($"<<{STYLE1}>>Hello<<_>> <<{STYLE2}>>{{{OutputProperties.MessagePropertyName}}}<<_>>", default);
            var args = formatter.Format(new LogEvent(DateTimeOffset.Now, LogEventLevel.Verbose, null, new MessageTemplateParser().Parse(MESSAGE), Array.Empty<LogEventProperty>()));
            Assert.Equal(new object[] { $"%cHello%c %c{MESSAGE.Replace($"<<{STYLE3}>>", "%c")}%c", STYLE1, "", STYLE2, STYLE2 + ";" + STYLE3, "" }, args);
        }

        [Fact]
        public void EscapePercentFromMessages()
        {
            var MESSAGE = $"and another % sign";
            var formatter = new OutputTemplateRenderer($"<<{STYLE1}>>A first<<_>> % sign <<{STYLE2}>>{{{OutputProperties.MessagePropertyName}}}<<_>>", default);
            var args = formatter.Format(new LogEvent(DateTimeOffset.Now, LogEventLevel.Verbose, null, new MessageTemplateParser().Parse(MESSAGE), Array.Empty<LogEventProperty>()));
            Assert.Equal(new object[] { $"%cA first%c %% sign %c{MESSAGE.Replace($"%", "%%")}%c", STYLE1, "", STYLE2, "" }, args);
        }

        [Fact]
        public void SupportsTokenStyling()
        {
            var MESSAGE = $"Test";
            var LEVEL = LogEventLevel.Verbose;
            var NOW = DateTimeOffset.Now;
            var formatter = new OutputTemplateRenderer($"{{{OutputProperties.LevelPropertyName}}}@{{{OutputProperties.TimestampPropertyName}:HH:mm}}: {{{OutputProperties.MessagePropertyName}}}", default, new Dictionary<string, string>
            {
                {OutputProperties.LevelPropertyName, STYLE1 },
                {OutputProperties.TimestampPropertyName, STYLE2 },
                {OutputProperties.MessagePropertyName, STYLE3 }
            });
            var args = formatter.Format(new LogEvent(NOW, LEVEL, null, new MessageTemplateParser().Parse(MESSAGE), Array.Empty<LogEventProperty>()));
            Assert.Equal(new object[] { $"%c%s%c@%c%s%c: %c{MESSAGE}%c", STYLE1, LEVEL.ToString(), "", STYLE2, NOW.ToString("HH:mm"), "", STYLE3, "" }, args);
        }

        [Fact]
        public void SupportsStylingInheritance1()
        {
            var MESSAGE = $"2 <<{STYLE3}>>{{Prop}}<<_>> 3";
            var PROP_VALUE = "INJECTED PROP";
            var formatter = new OutputTemplateRenderer($"<<{STYLE1}>>1<<_>> <<{STYLE2}>>{{{OutputProperties.MessagePropertyName}}}<<_>>", default);
            var args = formatter.Format(new LogEvent(DateTimeOffset.Now, LogEventLevel.Verbose, null, new MessageTemplateParser().Parse(MESSAGE), new[] {
                new LogEventProperty("Prop", new ScalarValue(PROP_VALUE))
            }));
            Assert.Equal(new object[] { $"%c1%c %c2 %c%o%c 3%c", STYLE1, "", STYLE2, STYLE2 + ";" + STYLE3, PROP_VALUE, STYLE2, "" }, args);
        }

        [Fact]
        public void SupportsStylingInheritance2()
        {
            var MESSAGE = $"1 <<{STYLE2}>>{{Prop}}<<_;{STYLE3}>> 3 <<_>> 4";
            var PROP_VALUE = "INJECTED PROP";
            var formatter = new OutputTemplateRenderer($"<<{STYLE1}>>{{{OutputProperties.MessagePropertyName}}}<<_>>", default);
            var args = formatter.Format(new LogEvent(DateTimeOffset.Now, LogEventLevel.Verbose, null, new MessageTemplateParser().Parse(MESSAGE), new[] {
                new LogEventProperty("Prop", new ScalarValue(PROP_VALUE))
            }));
            Assert.Equal(new object[] { $"%c1 %c%o%c 3 %c 4%c", STYLE1, STYLE1 + ";" + STYLE2, PROP_VALUE, STYLE1 + ";" + STYLE3, STYLE1, "" }, args);
        }

        [Fact]
        public void SupportsStylingInheritance3()
        {
            var MESSAGE = $"1 <<{STYLE2}>>{{Prop}}<<_;{STYLE3}>> 3 <<_>> 4";
            var PROP_VALUE = "INJECTED PROP";
            var formatter = new OutputTemplateRenderer($"{{{OutputProperties.MessagePropertyName}}}", default, new Dictionary<string, string>
            {
                {OutputProperties.MessagePropertyName, STYLE1 },
            });
            var args = formatter.Format(new LogEvent(DateTimeOffset.Now, LogEventLevel.Verbose, null, new MessageTemplateParser().Parse(MESSAGE), new[] {
                new LogEventProperty("Prop", new ScalarValue(PROP_VALUE))
            }));
            Assert.Equal(new object[] { $"%c1 %c%o%c 3 %c 4%c", STYLE1, STYLE1 + ";" + STYLE2, PROP_VALUE, STYLE1 + ";" + STYLE3, STYLE1, "" }, args);
        }

        [Fact]
        public void SupportsStylingInheritance4()
        {
            var MESSAGE = $"<<{STYLE1}>>1 {{Prop}}<<_>>";
            var PROP_VALUE = "INJECTED PROP";
            var formatter = new OutputTemplateRenderer($"{{{OutputProperties.MessagePropertyName}}}", default);
            var args = formatter.Format(new LogEvent(DateTimeOffset.Now, LogEventLevel.Verbose, null, new MessageTemplateParser().Parse(MESSAGE), new[] {
                new LogEventProperty("Prop", new ScalarValue(PROP_VALUE))
            }));
            Assert.Equal(new object[] { $"%c1 %o%c", STYLE1, PROP_VALUE, "" }, args);
        }
    }
}
