// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RouteSlip;
using RouteSlip.RabbitMQs;
using RouteSlip.util;
using Serilog;

Console.WriteLine("Hello, World!");


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
        services.AddScoped<IMessageProducer, RabbitMqProducer>();
        services.AddHostedService<ConsumerWorker>();
        services.AddApplicationInsightsTelemetryWorkerService();

    })
    .Build();
Log.Information("Application build");

// // Run the microservice
host.Run();