# Serilog.Sinks.BrowserConsole [![Build status](https://ci.appveyor.com/api/projects/status/s458q719m2pfwnyk?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-browserconsole)

A console sink for the Blazor/Wasm environment.

**Usage:**

```csharp
// dotnet add package Serilog.Sinks.BrowserConsole -v ...

Log.Logger = new LoggerConfiguration()
    .WriteTo.BrowserConsole()
    .CreateLogger();

Log.Information("Hello, browser!");
```
