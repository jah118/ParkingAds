using Microsoft.Extensions.Configuration;
using PullADsToMQ.Facade;
using Serilog;


//DI, Seri
namespace PullADsToMQ.util
{
    public class AppSettingsHandler : IAppSettingsHandler
    {
        public AppSettingsHandler(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, true)
                .AddEnvironmentVariables();

            IConfiguration config = builder.Build();

            AppSettings = config.GetSection("ApiSettings").Get<AppSettings>();
        }

        public AppSettings AppSettings { get; set; }
    }
}