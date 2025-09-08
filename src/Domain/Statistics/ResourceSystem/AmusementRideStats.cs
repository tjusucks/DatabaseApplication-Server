namespace DbApp.Domain.Statistics.ResourceSystem;  
  
public class AmusementRideStats  
{  
    public int TotalRides { get; set; }  
    public int OperationalRides { get; set; }  
    public int MaintenanceRides { get; set; }  
    public int ClosedRides { get; set; }  
    public int TotalCapacity { get; set; }  
    public double AverageCapacity { get; set; }  
    public double AverageDuration { get; set; }  
    public DateTime? FirstOpenDate { get; set; }  
    public DateTime? LastOpenDate { get; set; }  
}