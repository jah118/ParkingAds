using Microsoft.Extensions.Configuration;
using Serilog;


//DI, Seri
namespace pullADs.util
{
    public class AppSettingsHandler : IAppSettingsHandler
    {
        public AppSettingsHandler(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, true);

            IConfiguration config = builder.Build();

            ApiSettings = config.GetSection("ApiSettings").Get<ApiSettings>();
        }

        public ApiSettings ApiSettings { get; set; }
    }
}