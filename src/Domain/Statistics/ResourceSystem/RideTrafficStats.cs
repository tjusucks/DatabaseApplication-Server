namespace DbApp.Domain.Statistics.ResourceSystem;  
  
public class RideTrafficStats  
{  
    public int TotalRecords { get; set; }  
    public int CrowdedRecords { get; set; }  
    public double AverageVisitorCount { get; set; }  
    public double AverageQueueLength { get; set; }  
    public double AverageWaitingTime { get; set; }  
    public int MaxVisitorCount { get; set; }  
    public int MaxQueueLength { get; set; }  
    public int MaxWaitingTime { get; set; }  
    public double CrowdedPercentage { get; set; }  
}