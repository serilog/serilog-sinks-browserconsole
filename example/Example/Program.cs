using System;
using Microsoft.AspNetCore.Blazor.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Debugging;

namespace Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SelfLog.Enable(m => Console.Error.WriteLine(m));

            Log.Logger = new LoggerConfiguration()
                .WriteTo.BrowserConsole()
                .CreateLogger();

            Log.Debug("Hello, browser!");
            Log.Warning("Received strange {@Response} from server", new { Username = "example", Cats = 7 });

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An exception occurred while creating the WASM host");
                throw;
            }
        }

        public static IWebAssemblyHostBuilder CreateHostBuilder(string[] args) =>
            BlazorWebAssemblyHost.CreateDefaultBuilder()
                .UseBlazorStartup<Startup>();
    }
}
