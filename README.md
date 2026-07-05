# Serilog.Sinks.BrowserConsole [![Build status](https://ci.appveyor.com/api/projects/status/s458q719m2pfwnyk?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-browserconsole) [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Serilog.Sinks.BrowserConsole.svg)](https://nuget.org/packages/Serilog.Sinks.BrowserConsole)

A Serilog sink that takes advantage of the unique features of the browser console in the Blazor/WASM applications.

**Versioning:** This package tracks the versioning and target framework support of its _Microsoft.AspNetCore.Components.WebAssembly_ dependency. Most users should choose the version of _Serilog.Sinks.BrowserConsole_ that matches their application's target framework. I.e. if you're targeting .NET 7.x, choose a 7.x version of _Serilog.Sinks.BrowserConsole_. If you're targeting .NET 8.x, choose an 8.x _Serilog.Sinks.BrowserConsole_ version, and so on.

### What's it do?

The sink writes log events to the browser console. Unlike the normal Serilog console sink, which writes out formatted text, this sink takes advantage of the unique capabilities of the browser console to print interactive, fully-structured data.

![Serilog.Sinks.BrowserConsole](https://raw.githubusercontent.com/serilog/serilog-sinks-browserconsole/dev/assets/Screenshot.png)

### Getting started

Configure the logging pipeline in `Program.Main()`:

```csharp
// dotnet add package Serilog.Sinks.BrowserConsole

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
