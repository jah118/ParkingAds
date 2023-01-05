namespace PullParkingService.model
{
    public interface IParkingDataItem
    {
        bool Success { get; set; }
        string Content { get; set; }
    }
    
    public class ParkingDataItem : IParkingDataItem
    {
        public ParkingDataItem()
        {
        }

        public ParkingDataItem(string content) : this()
        {
            Content = content;
        }

        public bool Success { get; set; }
        public string Content { get; set; } = null!;
    }
}