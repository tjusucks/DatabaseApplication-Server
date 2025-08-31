using DbApp.Domain.Enums.TicketingSystem;

namespace DbApp.Domain.Specifications.TicketingSystem;

/// <summary>
/// Base specification for ticket sale filtering.
/// </summary>
public abstract class TicketSaleBaseSpec
{
    /// <summary>
    /// Keyword for searching ticket records.
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// Start date for filtering ticket sales.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date for filtering ticket sales.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Ticket type ID for filtering.
    /// </summary>
    public int? TicketTypeId { get; set; }

    /// <summary>
    /// Promotion ID for filtering.
    /// </summary>
    public int? PromotionId { get; set; }

    /// <summary>
    /// Payment status for filtering.
    /// </summary>
    public PaymentStatus? PaymentStatus { get; set; }
}

/// <summary>
/// Specification for searching ticket sales with pagination and sorting.
/// </summary>
public class TicketSaleSearchSpec : TicketSaleBaseSpec
{
    /// <summary>
    /// Sort field.
    /// </summary>
    public string? SortBy { get; set; } = "SalesDate";

    /// <summary>
    /// Sort direction.
    /// </summary>
    public bool Descending { get; set; } = true;

    /// <summary>
    /// Page number for pagination.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Page size for pagination.
    /// </summary>
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// Specification for counting ticket sales.
/// </summary>
public class TicketSaleCountSpec : TicketSaleBaseSpec
{
}

/// <summary>
/// Specification for ticket sale statistics.
/// </summary>
public class TicketSaleStatsSpec : TicketSaleBaseSpec
{
}

/// <summary>
/// Specification for grouped ticket sale statistics.
/// </summary>
public class TicketSaleGroupedStatsSpec : TicketSaleBaseSpec
{
    /// <summary>
    /// Group by dimension (TicketType, Promotion, PaymentStatus, Date).
    /// </summary>
    public string GroupBy { get; set; } = "TicketType";

    /// <summary>
    /// Sort field for grouped results.
    /// </summary>
    public string? SortBy { get; set; } = "Revenue";

    /// <summary>
    /// Sort direction for grouped results.
    /// </summary>
    public bool Descending { get; set; } = true;
}
