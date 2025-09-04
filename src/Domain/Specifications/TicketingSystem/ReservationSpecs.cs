using DbApp.Domain.Enums.TicketingSystem;

namespace DbApp.Domain.Specifications.TicketingSystem;

/// <summary>
/// Base specification for reservations filtering.
/// </summary>
public abstract class ReservationBaseSpec
{
    /// <summary>
    /// Keyword for searching reservation records.
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// Start date for filtering reservation records.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date for filtering reservation records.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Payment status for filtering.
    /// </summary>
    public PaymentStatus? PaymentStatus { get; set; }

    /// <summary>
    /// Reservation status for filtering.
    /// </summary>
    public ReservationStatus? Status { get; set; }

    /// <summary>
    /// Minimum total amount for filtering.
    /// </summary>
    public decimal? MinAmount { get; set; }

    /// <summary>
    /// Maximum total amount for filtering.
    /// </summary>
    public decimal? MaxAmount { get; set; }

    /// <summary>
    /// Promotion ID for filtering.
    /// </summary>
    public int? PromotionId { get; set; }
}

/// <summary>
/// Specification for searching reservations by visitor ID.
/// </summary>
public class ReservationSearchByVisitorSpec : ReservationBaseSpec
{
    /// <summary>
    /// Visitor ID for filtering.
    /// </summary>
    public int VisitorId { get; set; }

    /// <summary>
    /// Sort field.
    /// </summary>
    public string? SortBy { get; set; } = "ReservationTime";

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
/// Specification for counting reservations by visitor ID.
/// </summary>
public class ReservationCountByVisitorSpec : ReservationBaseSpec
{
    /// <summary>
    /// Visitor ID for filtering.
    /// </summary>
    public int VisitorId { get; set; }
}

/// <summary>
/// Specification for searching reservations (admin use).
/// </summary>
public class ReservationSearchSpec : ReservationBaseSpec
{
    /// <summary>
    /// Visitor ID for filtering (optional).
    /// </summary>
    public int? VisitorId { get; set; }

    /// <summary>
    /// Sort field.
    /// </summary>
    public string? SortBy { get; set; } = "ReservationTime";

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
/// Specification for counting reservations (admin use).
/// </summary>
public class ReservationCountSpec : ReservationBaseSpec
{
    /// <summary>
    /// Visitor ID for filtering (optional).
    /// </summary>
    public int? VisitorId { get; set; }
}

/// <summary>
/// Specification for getting reservation statistics by visitor.
/// </summary>
public class ReservationStatsByVisitorSpec
{
    /// <summary>
    /// Visitor ID for statistics.
    /// </summary>
    public int VisitorId { get; set; }

    /// <summary>
    /// Start date for filtering statistics.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date for filtering statistics.
    /// </summary>
    public DateTime? EndDate { get; set; }
}
