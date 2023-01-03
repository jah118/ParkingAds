using Microsoft.Extensions.Configuration;
using RedisConsumerPullAds.Facade;

namespace RedisConsumerPullAds.util;

public class AppSettingsHandler : IAppSettingsHandler
{
    public AppSettingsHandler(IConfigurationBuilder builder)
    {
        builder.SetBasePath(Directory.GetCurrentDirectory())
        // builder.SetBasePath("C:\\Users\\jonas\\Documents\\GitHub\\pullAdService\\pullAD\\RedisConsumerPullAds")
            .AddJsonFile("appsettings.json", false, true);

        IConfiguration config = builder.Build();

        AppSettings = config.GetSection("ApiSettings").Get<AppSettings>();
    }

    public AppSettings? AppSettings { get; set; }
}

internal class ConfigurationMissingException : Exception
{
    public ConfigurationMissingException()
    {
    }

    public ConfigurationMissingException(string name)
        : base(string.Format("Invalid Student Name: {0}", name))
    {
    }
}