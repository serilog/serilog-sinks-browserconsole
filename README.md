# Serilog.Sinks.BrowserConsole [![Build status](https://ci.appveyor.com/api/projects/status/s458q719m2pfwnyk?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-browserconsole) [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Serilog.Sinks.BrowserConsole.svg)](https://nuget.org/packages/Serilog.Sinks.BrowserConsole)

A console sink for the Blazor/Wasm environment.

### What's it do?

The sink writes log events to the browser console. Unlike the normal Serilog console sink, which writes out formatted text, this sink takes advantage of the unique capabilities of the browser console to print interactive, fully-structured data.

![Serilog.Sinks.BrowserConsole](https://raw.githubusercontent.com/serilog/serilog-sinks-browserconsole/dev/assets/Screenshot.png)

### Getting started

Configure the logging pipeline in `Program.Main()`:

```csharp
// dotnet add package Serilog.Sinks.BrowserConsole -v ...

Log.Logger = new LoggerConfiguration()
    .WriteTo.BrowserConsole()
    .CreateLogger();

Log.Information("Hello, browser!");
```

A more detailed example is available [in this repository](https://github.com/serilog/serilog-sinks-browserconsole/tree/dev/example/ExampleClient).

### Styling your stuff

In your sink's `outputTemplate` parameter, you can leverage [`console.log` styling capabilities](https://developer.mozilla.org/en-US/docs/Web/API/console#styling_console_output) by using the `<<...>>` placeholder. Use `<<_>>` to reset styles to defaults.

Example:

```
<<color: red;>>{Level}<<_>>: {Message}
```

You can also define styles for tokens via the `tokenStyles` dictionary.