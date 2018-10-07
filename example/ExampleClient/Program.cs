using System;
using Microsoft.AspNetCore.Blazor.Hosting;
using Serilog;
using Serilog.Debugging;

namespace ExampleClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SelfLog.Enable(m => Console.Error.WriteLine(m));

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.BrowserConsole()
                .CreateLogger();

            Log.Debug("Hello, browser!");
            Log.Warning("Received strange response {@Response} from server", new { Username = "example", Cats = 7 });

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
