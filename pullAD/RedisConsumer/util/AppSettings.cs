using RedisConsumer.Facade;

namespace RedisConsumer.util
{
    public class AppSettings : IAppSettings
    {
        public string? RedisConn { get; set; }
        public string? RabbitConn { get; set; }
    }
}