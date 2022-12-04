using System;
using System.IO;
using pullADs.util;
using System.Timers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace pullADs
{
    public static class Program
    {
        private static System.Timers.Timer _aTimer = null!;
        
        private static ApiSettings ApiSettings { get; set; } = null!;

        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //setups

            var builder = new ConfigurationBuilder();
            AppSettingsHandler appSetup = new AppSettingsHandler(builder);
            ApiSettings = appSetup.ApiSettings;
            
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger.Information("Application Starting");

            
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<IAdPullService, AdPullService>();
                })
                .UseSerilog()
                .Build();

            var svc = ActivatorUtilities.CreateInstance<AdPullService>(host.Services);
            //svc.Run();
            
            
            AdPullService adPullService = new AdPullService();

            DataHandling(ApiSettings, adPullService);

            // The code runs when a event from OnTimedEvent in SetTimer gets raised.
            // which is delegate that runs from an event from Timer.Elapsed
            SetTimer(ApiSettings.TimerInterval1);

            Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
            Console.WriteLine("Press 'S' to stop");

            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.S))
            {
                // do something
            }

            _aTimer.Stop();
            _aTimer.Dispose();

            Console.WriteLine("Terminating the application...");
        }

        private static void SetTimer(int interval)
        {
            // Create a timer with a two second interval.
            _aTimer = new System.Timers.Timer(interval);
            // Hook up the Elapsed event for the timer. 
            _aTimer.Elapsed += OnTimedEvent;
            _aTimer.AutoReset = true;
            _aTimer.Enabled = true;
        }

        // private static async Task DataHandling(Startup startupVars, AdPullService adPullService)
        private static async Task DataHandling(ApiSettings startupVars, AdPullService adPullService)
        {
            Console.WriteLine("pull");

            if (startupVars.AdUrl != null)
            {
                var temp = await adPullService!.GetAd(startupVars.AdUrl);
            }

            Console.WriteLine("data formating ");

            Console.WriteLine("rabbit send ");
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                e.SignalTime);
            //TODO: add support for getting data on the envent. 
           // DataHandling(ApiSettings, adPullService);

        }
    }
}