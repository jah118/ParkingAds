namespace PullParkingService.model
{
    public interface IParkingDataItem
    {
        bool Success { get; set; }
        public List<PInerData> Content { get; set; }
    }

    public class ParkingDataItem : IParkingDataItem
    {
        public bool Success { get; set; } = false;
        public List<PInerData> Content { get; set; }
    }

    public class PInerData
    {
        public string date { get; set; }
        public string name { get; set; }
        public string coord { get; set; }
        public string max { get; set; }
        public string current { get; set; }
    }
}