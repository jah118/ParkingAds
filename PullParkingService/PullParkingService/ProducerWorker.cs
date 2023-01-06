using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PullParkingService.RabbitMQs;
using PullParkingService.util;
using RabbitMQ.Client;
using Serilog;
using Timer = System.Threading.Timer;

namespace PullParkingService;

public class ProducerWorker : BackgroundService
{
    private readonly IMessageProducer _messagePublisher;
    private readonly IParkingPullService _parkingPullService;


    private AppSettings _appSettings;



    public ProducerWorker(IMessageProducer messagePublisher, IOptionsMonitor<AppSettings> optionsMonitor,
        IParkingPullService parkingPullService
    )
    {
        _appSettings = optionsMonitor.CurrentValue;
        _messagePublisher = messagePublisher;
        _parkingPullService = parkingPullService;
        
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Information("BackgroundService Started");
        DoWork();
        // Option 2 (.NET 6)
        var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_appSettings.TimerInterval1));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            Log.Information("Started DoWork");
            DoWork();
        }
    }


    /// <summary>
    ///     Gets Ad data, format it and send it to msg queue
    /// </summary>
    private async Task DoWork()
    {
        try
        {

            // TODO this needs to be check on startup maybe use fluint validation later on settings class
            if (string.IsNullOrEmpty(_appSettings.URL))
                Log.Error("Configuration ERROR, No URI TO GET ads FROM got: {Uri}",
                    _appSettings.URL);

            var parkingDataItem = await _parkingPullService!.GetData(_appSettings.URL);
            Log.Information("AD data: {@Data}", parkingDataItem);

            if (parkingDataItem.Success is false )
            {
                //TODO do some smart error handling like change the Timer interval, alert after 5 retries, that service is dead  or no data is received
                Log.Error("ERROR, No good data received");
            }
            else
            {
                Console.WriteLine("data formating ");
                Console.WriteLine(_appSettings.URL);
                // TODO do formatining and paring 
                // TODO check format by regex like "<[a-zA-Z]+ [a-zA-Z]+='[^']*'>[a-zA-Z]+ [a-zA-Z]+</[a-zA-Z]+>"
                var formattedData = parkingDataItem;

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
}