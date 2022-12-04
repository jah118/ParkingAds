using Microsoft.Extensions.Configuration;
using RedisConsumer.Facade;


namespace RedisConsumer.util
{
    public class AppSettingsHandler : IAppSettingsHandler
    {
        public AppSettingsHandler(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, true);

            IConfiguration config = builder.Build();

            AppSettings = config.GetSection("ApiSettings").Get<AppSettings>();
        }

        public AppSettings? AppSettings { get; set; }
    }
}