using System.Timers;
using Microsoft.Extensions.Configuration;
using pullADs.Facade;
using pullADs.util;
using Serilog;
using Timer = System.Timers.Timer;

namespace pullADs;

public static class Program
{
    private static Timer _aTimer = null!;

    private static IAppSettings _appSettings;
    private static IAdPullService _adPullService;
    private static IMessageProducer _messagePublisher;

    private static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");

        //setups
        var builder = new ConfigurationBuilder();
        IAppSettingsHandler settingsHandler = new AppSettingsHandler(builder);
        _appSettings = settingsHandler.AppSettings;
        _messagePublisher = new RabbitMQProducer(settingsHandler.AppSettings);

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
        SetTimer(_appSettings.TimerInterval1); // <-------- use this one 
        // SetTimer(1000);

        Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
        Console.WriteLine("Press 'S' to stop");

        //TODO Do something better like wtf is this.....
        while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.S))
        {
            // do something
        }

        _aTimer.Stop();
        _aTimer.Dispose();

        Console.WriteLine("Terminating the application...");
    }

    /// <summary>
    ///     Set a TIMER THAT FIRES AN INTERVAL AT A GIVEN INTERVAL
    /// </summary>
    /// <param name="interval"></param>
    private static void SetTimer(int interval)
    {
        // Create a timer with a two second interval.
        _aTimer = new Timer(interval);
        // Hook up the Elapsed event for the timer. 
        _aTimer.Elapsed += OnTimedEvent;
        GC.KeepAlive(_aTimer);
        _aTimer.AutoReset = true;
        _aTimer.Enabled = true;
    }

    /// <summary>
    ///     Gets Ad data, format it and send it to msg queue
    /// </summary>
    private static async Task DataHandling()
    {
        try
        {
            Log.Information("Started DataHandling");

            // TODO this needs to be check on startup maybe use fleunt validation later on settings class
            if (string.IsNullOrEmpty(_appSettings.AdUrl))
                Log.Error("Configuration ERROR, No URI TO GET ads FROM got: {Uri}",
                    _appSettings.AdUrl);

            var data = await _adPullService!.GetAd(_appSettings.AdUrl);
            Log.Logger.Information("AD data: {@Data}", data);

            if (data.Success is false || string.IsNullOrEmpty(data.Content))
            {
                //TODO do some smart error handling like change the Timer interval, alert after 5 retries, that service is dead  or no data is received
                Log.Error("ERROR, No good data received");
            }
            else
            {
                Console.WriteLine("data formating ");
                Console.WriteLine(_appSettings.AdUrl);
                // TODO do formatining and paring 
                // TODO check format by regex like "<[a-zA-Z]+ [a-zA-Z]+='[^']*'>[a-zA-Z]+ [a-zA-Z]+</[a-zA-Z]+>"
                var formattedData = data;

                Console.WriteLine("rabbit send ");

                _messagePublisher.SendMessage(formattedData);
            }
        }
        catch (Exception e)
        {
            //TODO: Maybe add two try catch to separate error handling logic for pull method and msg producer 
            //TODO: do some smart error handling like change the Timer interval, alert after 5 retries, that service is dead  
            Console.WriteLine(e);
            Log.Error("Exception Hit------------ ");
        }
    }


    private static void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        Log.Information("The Elapsed event was raised at {0:HH:mm:ss.fff}",
            e.SignalTime);
        DataHandling();
    }
}