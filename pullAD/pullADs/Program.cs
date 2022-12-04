using System;
using System.IO;
using pullADs.util;
using System.Timers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using pullADs.Facade;
using Serilog;

namespace pullADs
{
    public static class Program
    {
        private static System.Timers.Timer _aTimer = null!;

        private static IApiSettings _apiSettings;
        private static IAdPullService _adPullService;
        private static IAppSettingsHandler _settingsHandler;
        private static readonly IMessageProducer _messagePublisher;

        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //setups
            var builder = new ConfigurationBuilder();
            _settingsHandler = new AppSettingsHandler(builder);
            _apiSettings = _settingsHandler.ApiSettings;

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger.Information("Application Starting");

            _adPullService = new AdPullService();

            // Gets called here so TimerInterval does not block or delay a quick start by waiting
            DataHandling();

            // The code runs when a event from OnTimedEvent in SetTimer gets raised.
            // which is delegate that runs from an event from Timer.Elapsed
            // SetTimer(_apiSettings.TimerInterval1);    // <-------- use this one 
            SetTimer(500);

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
            GC.KeepAlive(_aTimer);
            _aTimer.AutoReset = true;
            _aTimer.Enabled = true;
        }

        // private static async Task DataHandling(Startup startupVars, AdPullService adPullService)
        private static async Task DataHandling()
        {
            try
            {
                Log.Information("Started DataHandling");

                // TODO: this needs to be check on startup maybe use fleunt validation later on settings class
                if (string.IsNullOrEmpty(_apiSettings.AdUrl))
                {
                    Log.Error("Configuration ERROR, No URI TO GET ads FROM got: {Uri}",
                        _apiSettings.AdUrl);
                }

                var data = await _adPullService!.GetAd(_apiSettings.AdUrl);
                Log.Logger.Information("AD data: {@Data}", data);

                if (data.Success is false || string.IsNullOrEmpty(data.Content))
                {
                    //TODO do some smart error handling like change the Timer interval, alert after 5 retries, that service is dead  or no data is received
                    Log.Error("ERROR, No good data received");
                }
                else
                {
                    Console.WriteLine("data formating ");
                    Console.WriteLine(_apiSettings.AdUrl.ToString());
                    // TODO do formatining and paring 
                    var formattedData = data;

                    Console.WriteLine("rabbit send ");
                    
                    _messagePublisher.SendMessage(formattedData);
                    
                }
            }
            catch (Exception e)
            {
                //TODO do some smart error handling like change the Timer interval, alert after 5 retries, that service is dead  
                Console.WriteLine(e);
                Log.Error("Exception Hit------------ ");
            }
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Log.Logger.Information("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                e.SignalTime);

            //TODO: add support for getting data on the envent. 
            DataHandling();
        }
    }
}