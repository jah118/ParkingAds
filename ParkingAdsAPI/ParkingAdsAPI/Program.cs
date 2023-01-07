using ParkingAdsAPI.RabbitMQs;
using ParkingAdsAPI.Util;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .Build();

var configuration = configurationBuilder.Build();

// Logger
using var log = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

Log.Logger = log;
Log.Logger.Information("Application Starting");

// Add services to the container.

builder.Services.AddOptions();
builder.Host.UseSerilog();


//var appSettings = configuration.GetSection("ApiSettings").Get<AppSettings>();
//builder.Services.AddSingleton(appSettings);


builder.Services.Configure<AppSettings>(configuration.GetSection("ApiSettings"));
builder.Services.AddScoped<IMessageProducer, RabbitMQProducer>();
builder.Services.AddScoped<IMessageConsume, RabbitMQConsume>();
builder.Services.AddHttpContextAccessor();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
