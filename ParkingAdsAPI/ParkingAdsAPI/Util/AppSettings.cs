namespace ParkingAdsAPI.Util
{
    public class AppSettings : IAppSettings
    {

        public string? RedisConn { get; set; }
        public string? RabbitConn { get; set; }
        public string? RabbitQueueName { get; set; }
    }

    public interface IAppSettings
    {
        public string? RedisConn { get; set; }
        public string? RabbitConn { get; set; }
        public string? RabbitQueueName { get; set; }
    }
}
