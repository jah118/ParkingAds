namespace ParkingAdsAPI.Data;

public class JSONModel
{
    
    public string searchedLocation  { get; set; }
    public string topicKey { get; set; }
    public Sesssion sesssion { get; set; }
    public string adData { get; set; }
    public ParkingData parkingData { get; set; }
}

public class Sesssion
{
    public string time_sent { get; set; }
    public string  messageId { get; set; }
    public string aggregatorTarget { get; set; }
    public int splitCounter { get; set; }
}

public class ParkingData
{
    public string data { get; set; }
    public List<string>? dataList { get; set; }

    
}