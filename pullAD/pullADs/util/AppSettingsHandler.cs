using Microsoft.Extensions.Configuration;

namespace pullADs.util
{
    public class AppSettingsHandler
    {
        public AppSettingsHandler()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            ApiSettings = config.GetSection("ApiSettings").Get<ApiSettings>();
        }

        public ApiSettings ApiSettings { get; private set; }
    }
}