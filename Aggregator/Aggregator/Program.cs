using Aggregator;
using Aggregator.RabbitMQs;
using Aggregator.util;
using Serilog;
using StackExchange.Redis;

///////////////////////////////////////////////////////////////////////////////////////////////////////
/*
 * This service is a stub for all the parking part (ParkingService) of Diagram, from page 49 Figur 9.9 
 */
///////////////////////////////////////////////////////////////////////////////////////////////////////


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
Log.Logger.Information("Application Starting");


IHost host = Host.CreateDefaultBuilder()
    .ConfigureHostConfiguration(hostConfig =>
    {
        hostConfig.SetBasePath(Directory.GetCurrentDirectory());
        hostConfig.AddJsonFile("appsettings.json", optional: true, true);
        hostConfig.AddEnvironmentVariables();
        hostConfig.AddCommandLine(args);
    })
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        services.Configure<AppSettings>(configuration.GetSection("ApiSettings"));
        // var redisConn  = configuration.GetSection("ApiSettings:redisConn").Value;
        var multiplexer = ConnectionMultiplexer.Connect("localhost");
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);


        services.AddSingleton<IMessageProducer, RabbitMqProducer>();
        services.AddHostedService<Worker>();
        services.AddApplicationInsightsTelemetryWorkerService();
    })
    .Build();

Log.Information("Application build");
host.Run();