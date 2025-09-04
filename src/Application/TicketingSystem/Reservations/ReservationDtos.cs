using DbApp.Domain.Enums.TicketingSystem;

namespace DbApp.Application.TicketingSystem.Reservations;

/// <summary>
/// Summary DTO for reservation search results.
/// </summary>
public class ReservationSummaryDto
{
    public int ReservationId { get; set; }
    public DateTime ReservationTime { get; set; }
    public DateTime VisitDate { get; set; }
    public string VisitorName { get; set; } = string.Empty;
    public string? VisitorEmail { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public ReservationStatus Status { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PromotionName { get; set; }
    public int TotalTickets { get; set; }
}

/// <summary>
/// Visitor reservation statistics.
/// </summary>
public class ReservationStatsDto
{
    public int TotalReservations { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal TotalRefunded { get; set; }
    public decimal NetSpent { get; set; }
    public int TotalTickets { get; set; }
    public decimal AverageOrderValue { get; set; }
    public DateTime? FirstReservation { get; set; }
    public DateTime? LastReservation { get; set; }
}

/// <summary>
/// Search result containing reservation and pagination info.
/// </summary>
public class ReservationSearchResult
{
    public List<ReservationSummaryDto> Reservations { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

/// <summary>
/// Detailed reservation item DTO.
/// </summary>
public class ReservationItemDto
{
    public int ItemId { get; set; }
    public int TicketTypeId { get; set; }
    public string TicketTypeName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
}

/// <summary>
/// Detailed reservation DTO with all related information.
/// </summary>
public class ReservationDto
{
    public int ReservationId { get; set; }
    public int VisitorId { get; set; }
    public string VisitorName { get; set; } = string.Empty;
    public string? VisitorEmail { get; set; }
    public DateTime ReservationTime { get; set; }
    public DateTime VisitDate { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public ReservationStatus Status { get; set; }
    public string? PaymentMethod { get; set; }
    public int? PromotionId { get; set; }
    public string? PromotionName { get; set; }
    public string? SpecialRequests { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ReservationItemDto> Items { get; set; } = [];
}
