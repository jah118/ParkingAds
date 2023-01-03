using Microsoft.Extensions.Configuration;
using pullADs.Facade;
using Serilog;


//DI, Seri
namespace pullADs.util
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