using Newtonsoft.Json;

namespace ParkingAdsAPI.Data
{
    public class DataModels
    {
        public class AdData
        {
            public string? description { get; set; }
            public string? type { get; set; }
        }

        public class AggregatorTarget
        {
            public string? description { get; set; }
            public string? type { get; set; }
        }

        public class Coord
        {
            public string? type { get; set; }
        }

        public class Current
        {
            public string? type { get; set; }
        }

        public class Date
        {
            public string? type { get; set; }
        }

        public class Items
        {
            public string? type { get; set; }
            public Properties? properties { get; set; }
            public List<string>? required { get; set; }
        }

        public class Max
        {
            public string? type { get; set; }
        }

        public class MessageId
        {
            public string? description { get; set; }
            public string? type { get; set; }
            public string? optional { get; set; }
        }

        public class Name
        {
            public string? type { get; set; }
        }

        public class ParkingData
        {
            public string? type { get; set; }
            public string? description { get; set; }
            public int? minItems { get; set; }
            public bool? uniqueItems { get; set; }
            public Items? items { get; set; }
        }

        public class Properties
        {
            public TopicKey topicKey { get; set; }
            public Sesssion sesssion { get; set; }
            public AdData adData { get; set; }
            public ParkingData parkingData { get; set; }
            public TimeSent time_sent { get; set; }
            public MessageId messageId { get; set; }
            public AggregatorTarget aggregatorTarget { get; set; }
            public SplitCounter splitCounter { get; set; }
            public Date date { get; set; }
            public Name name { get; set; }
            public Coord coord { get; set; }
            public Max max { get; set; }
            public Current current { get; set; }
        }

        public class Sesssion
        {
            public string description { get; set; }
            public string type { get; set; }
            public Properties properties { get; set; }
        }

        public class SplitCounter
        {
            public string description { get; set; }
            public string type { get; set; }
        }

        public class TimeSent
        {
            public string type { get; set; }
            public string format { get; set; }
        }

        public class TopicKey
        {
            public string description { get; set; }
            public string type { get; set; }
        }
    }
}
