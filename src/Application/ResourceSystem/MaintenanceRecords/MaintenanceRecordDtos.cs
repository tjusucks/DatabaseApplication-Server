using DbApp.Domain.Enums.ResourceSystem;  
  
namespace DbApp.Application.ResourceSystem.MaintenanceRecords;  
  
/// <summary>  
/// Summary DTO for maintenance record search results.  
/// </summary>  
public class MaintenanceRecordSummaryDto  
{  
    public int MaintenanceId { get; set; }  
    public int RideId { get; set; }  
    public string RideName { get; set; } = string.Empty;  
    public int TeamId { get; set; }  
    public string TeamName { get; set; } = string.Empty;  
    public int? ManagerId { get; set; }  
    public string? ManagerName { get; set; }  
    public MaintenanceType MaintenanceType { get; set; }  
    public DateTime StartTime { get; set; }  
    public DateTime? EndTime { get; set; }  
    public decimal Cost { get; set; }  
    public string? PartsReplaced { get; set; }  
    public string? MaintenanceDetails { get; set; }  
    public bool IsCompleted { get; set; }  
    public bool? IsAccepted { get; set; }  
    public DateTime? AcceptanceDate { get; set; }  
    public string? AcceptanceComments { get; set; }  
    public DateTime CreatedAt { get; set; }  
    public DateTime UpdatedAt { get; set; }  
}  
  
/// <summary>  
/// Maintenance record statistics DTO.  
/// </summary>  
public class MaintenanceRecordStatsDto  
{  
    public int TotalMaintenances { get; set; }  
    public int CompletedMaintenances { get; set; }  
    public int AcceptedMaintenances { get; set; }  
    public decimal TotalCost { get; set; }  
    public decimal AverageCost { get; set; }  
    public Dictionary<MaintenanceType, int> MaintenanceTypeDistribution { get; set; } = new();  
    public DateTime? FirstMaintenance { get; set; }  
    public DateTime? LastMaintenance { get; set; }  
}  
  
/// <summary>  
/// Search result containing maintenance records and pagination info.  
/// </summary>  
public class MaintenanceRecordResult  
{  
    public List<MaintenanceRecordSummaryDto> MaintenanceRecords { get; set; } = [];  
    public int TotalCount { get; set; }  
    public int Page { get; set; }  
    public int PageSize { get; set; }  
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);  
}