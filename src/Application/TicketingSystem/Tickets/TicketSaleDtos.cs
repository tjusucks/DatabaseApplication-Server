using DbApp.Domain.Enums.TicketingSystem;

namespace DbApp.Application.TicketingSystem.Tickets;

/// <summary>
/// Summary information for ticket sale record.
/// </summary>
public class TicketSaleSummaryDto
{
    public int TicketId { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string TicketTypeName { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public string VisitorName { get; set; } = string.Empty;
    public string VisitorEmail { get; set; } = string.Empty;
    public string? PromotionName { get; set; }
    public DateTime SalesDate { get; set; }
    public DateTime? VisitDate { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public TicketStatus Status { get; set; }
}

/// <summary>
/// Paginated search results for ticket sales.
/// </summary>
public class TicketSaleResult
{
    public List<TicketSaleSummaryDto> TicketSales { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

/// <summary>
/// Overall ticket sale statistics.
/// </summary>
public class TicketSaleStatsDto
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
/// Grouped ticket sale statistics by different dimensions.
/// </summary>
public class GroupedTicketSaleStatsDto
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
