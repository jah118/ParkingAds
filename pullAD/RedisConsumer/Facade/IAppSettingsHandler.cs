using RedisConsumer.util;

namespace RedisConsumer.Facade;

public interface IAppSettingsHandler
{
    AppSettings? AppSettings { get; set; }
}