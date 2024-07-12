﻿using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Debugging;
using System.Threading.Tasks;

namespace ExampleClient;

public class Program
{
    public static async Task Main(string[] args)
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
				var builder = WebAssemblyHostBuilder.CreateDefault(args);

				builder.RootComponents.Add<App>("app");

				builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

				await builder.Build().RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An exception occurred while creating the WASM host");
                throw;
            }
        }
}