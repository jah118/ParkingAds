using Microsoft.Extensions.Configuration;
using RedisConsumerPullAds.Facade;

namespace RedisConsumerPullAds.util;

public class AppSettingsHandler : IAppSettingsHandler
{
    public AppSettingsHandler(IConfigurationBuilder builder)
    {
        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true);

        IConfiguration config = builder.Build();

        AppSettings = config.GetSection("ApiSettings").Get<AppSettings>();
    }

    public AppSettings? AppSettings { get; set; }
}