using Serilog.Events;
using Xunit;

namespace Serilog.Sinks.BrowserConsole.Tests
{
    public class ObjectModelInteropTests
    {
        [Fact]
        public void ScalarsInteropAsTheirValue()
        {
            var sv = new ScalarValue(14);
            var iv = ObjectModelInterop.ToInteropValue(sv);
            Assert.Equal(14, iv);
        }

        [Fact]
        public void FormattedScalarsInteropAsStrings()
        {
            var sv = new ScalarValue(14);
            var iv = ObjectModelInterop.ToInteropValue(sv, "000.0");
            Assert.Equal("014.0", iv);
        }
    }
}
