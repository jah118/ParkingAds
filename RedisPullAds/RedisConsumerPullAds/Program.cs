using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RedisConsumerPullAds.util;
using Serilog;
using StackExchange.Redis;



namespace RedisConsumerPullAds
{

    static class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                //.ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            Log.Logger.Information("Application Starting");

            IHost host = Host.CreateDefaultBuilder()
                .ConfigureHostConfiguration(hostConfig =>
                {
                    hostConfig.SetBasePath(Directory.GetCurrentDirectory());
                    hostConfig.AddJsonFile("appsettings.json", optional: true);
                    hostConfig.AddEnvironmentVariables();
                    hostConfig.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    services.Configure<AppSettings>(configuration.GetSection("ApiSettings"));
                    services.AddApplicationInsightsTelemetryWorkerService();

                    services.AddHostedService<ConsumerWorkerHandelAds>();
                })
                .Build();

            Log.Information("Application build");

            // // Run the microservice
            host.Run();
        }
    }
}