namespace DbApp.Domain.Statistics.TicketingSystem;

/// <summary>
/// Overall ticket sale statistics domain object.
/// </summary>
public class TicketSaleStats
{
    public int TotalTicketsSold { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalRefunded { get; set; }
    public decimal NetRevenue { get; set; }
    public decimal AverageTicketPrice { get; set; }
    public int TotalRefundedTickets { get; set; }
    public decimal RefundRate { get; set; }
    public DateTime? FirstSale { get; set; }
    public DateTime? LastSale { get; set; }
}

/// <summary>
/// Grouped ticket sale statistics domain object.
/// </summary>
public class GroupedTicketSaleStats
{
    public string GroupKey { get; set; } = string.Empty; // The value used for grouping.
    public string GroupName { get; set; } = string.Empty; // Display name for the group.
    public int TicketsSold { get; set; }
    public decimal Revenue { get; set; }
    public decimal AveragePrice { get; set; }
    public int RefundedTickets { get; set; }
    public decimal RefundAmount { get; set; }
    public decimal RefundRate { get; set; }
    public DateTime? FirstSale { get; set; }
    public DateTime? LastSale { get; set; }
}
