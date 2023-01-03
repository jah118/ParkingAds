using RedisConsumerPullAds.util;

namespace RedisConsumerPullAds.Facade;

public interface IAppSettingsHandler
{
    AppSettings? AppSettings { get; set; }
}