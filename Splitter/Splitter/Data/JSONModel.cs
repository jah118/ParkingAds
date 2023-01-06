namespace Splitter.Data;

/*
 * Convert this to class lib to be shared insted of this copy paste stuff 
 */
public class JsonModel
{


    public string? SearchedLocation  { get; set; }
    public string? TopicKey { get; set; }
    public Sesssion? Sesssion { get; set; }
    public string? AdData { get; set; }
    public ParkingData? ParkingData { get; set; }
    public TrafficData? TrafficData  { get; set; }  

}

public class Sesssion
{
    public string? TimeSent { get; set; }
    public string?  MessageId { get; set; }
    public string? AggregatorTarget { get; set; }
    public int? SplitCounter { get; set; }
}

public class ParkingData
{
    public bool Success { get; set; } = false;
    public string? Data { get; set; }
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

public class TrafficData
{
    public string? Data { get; set; }
}