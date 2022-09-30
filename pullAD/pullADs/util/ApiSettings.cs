namespace pullADs.util
{
    public class ApiSettings : IApiSettings
    {
        public string? AdUrl { get; set; }
        public string? RedisConn { get; set; }

        public int TimerInterval1 { get; private set; } = 60000;
        public int TimerInterval2 { get; private set; } = 120000;
    }
}