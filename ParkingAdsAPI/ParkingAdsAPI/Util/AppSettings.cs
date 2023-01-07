namespace ParkingAdsAPI.Util
{
    public class AppSettings : IAppSettings
    {
        public string? RedisConn { get; set; }
        public string? RabbitConn { get; set; }
        public string? RabbitQueueNameConsume { get; set; }
        public string? RabbitQueueNameProduce { get; set; }
        public string? RedisKey { get; set; }
    }

    public interface IAppSettings
    {
        public string? RedisConn { get; set; }
        public string? RabbitConn { get; set; }
        public string? RabbitQueueNameProduce { get; set; }
        public string? RabbitQueueNameConsume { get; set; }
        public string? RedisKey { get; set; }


    }
}