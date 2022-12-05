using pullADs.Facade;

namespace pullADs.util
{
    public class AppSettings : IAppSettings
    {
        public string? AdUrl { get; set; }
        public string? RedisConn { get; set; }
        public string? RabbitConn { get; set; }
        public string? RabbitChannelAds { get; set; }

        public int TimerInterval1 { get; private set; } = 60000;
        public int TimerInterval2 { get; private set; } = 120000;
    }
}