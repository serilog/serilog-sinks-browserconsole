# Serilog.Sinks.BrowserConsole [![Build status](https://ci.appveyor.com/api/projects/status/s458q719m2pfwnyk?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-browserconsole) [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Serilog.Sinks.BrowserConsole.svg)](https://nuget.org/packages/Serilog.Sinks.BrowserConsole)

A console sink for the Blazor/Wasm environment.

**Usage:**

```csharp
// dotnet add package Serilog.Sinks.BrowserConsole -v ...

Log.Logger = new LoggerConfiguration()
    .WriteTo.BrowserConsole()
    .CreateLogger();

Log.Information("Hello, browser!");
```
