namespace DbApp.Domain.Statistics.ResourceSystem;

public class MaintenanceRecordStats
{
    public int TotalMaintenances { get; set; }
    public int CompletedMaintenances { get; set; }
    public int AcceptedMaintenances { get; set; }
    public decimal TotalCost { get; set; }
    public decimal AverageCost { get; set; }
    public Dictionary<string, int> MaintenanceTypeBreakdown { get; set; } = new();
}
