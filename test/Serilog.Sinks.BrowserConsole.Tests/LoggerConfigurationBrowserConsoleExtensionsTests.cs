#nullable enable

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Xunit;

namespace Serilog.Sinks.BrowserConsole.Tests;

public class LoggerConfigurationBrowserConsoleExtensionsTests
{
    [Fact]
    public void SuppliedJsRuntimeIsUsed()
    {
        var jsRuntime = new RecordingJsRuntime();

        using var logger = new LoggerConfiguration()
            .WriteTo.BrowserConsole(jsRuntime)
            .CreateLogger();

        logger.Information("Hello");

        Assert.Contains(
            jsRuntime.Invocations,
            invocation => invocation.Identifier == "console.info");
    }

    class RecordingJsRuntime : IJSRuntime
    {
        public List<Invocation> Invocations { get; } = [];

        public ValueTask<TValue> InvokeAsync<TValue>(
            string identifier,
            object?[]? args)
        {
            Invocations.Add(new Invocation(identifier, args));

            return new ValueTask<TValue>(default(TValue)!);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(
            string identifier,
            CancellationToken cancellationToken,
            object?[]? args)
        {
            Invocations.Add(new Invocation(identifier, args));

            return new ValueTask<TValue>(default(TValue)!);
        }
    }

    record Invocation(string Identifier, object?[]? Arguments);
}
