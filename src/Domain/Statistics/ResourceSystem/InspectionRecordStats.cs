namespace DbApp.Domain.Statistics.ResourceSystem;

public class InspectionRecordStats
{
    public int TotalInspections { get; set; }
    public int PassedInspections { get; set; }
    public int FailedInspections { get; set; }
    public double PassRate { get; set; }
    public Dictionary<string, int> CheckTypeBreakdown { get; set; } = [];
}
