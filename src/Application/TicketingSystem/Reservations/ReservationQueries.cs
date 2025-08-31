using DbApp.Domain.Enums.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.Reservations;

/// <summary>
/// Search reservations by visitor with filtering options.
/// </summary>
public record SearchReservationByVisitorQuery(
    int VisitorId,
    string? Keyword = null, // Search in visitor name, email, phone, promotion name, ticket type name.
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    PaymentStatus? PaymentStatus = null,
    ReservationStatus? Status = null,
    decimal? MinAmount = null,
    decimal? MaxAmount = null,
    int? PromotionId = null,
    string? SortBy = "ReservationTime", // ReservationTime, TotalAmount, VisitDate.
    bool Descending = true,
    int Page = 1,
    int PageSize = 20
) : IRequest<ReservationSearchResult>;

/// <summary>
/// Search reservations by multiple criteria (admin use).
/// </summary>
public record SearchReservationQuery(
    int? VisitorId = null,
    string? Keyword = null, // Search in visitor name, email, phone, promotion name, ticket type name.
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    PaymentStatus? PaymentStatus = null,
    ReservationStatus? Status = null,
    decimal? MinAmount = null,
    decimal? MaxAmount = null,
    int? PromotionId = null,
    string? SortBy = "ReservationTime", // ReservationTime, TotalAmount, VisitDate.
    bool Descending = true,
    int Page = 1,
    int PageSize = 20
) : IRequest<ReservationSearchResult>;

/// <summary>
/// Get reservation statistics for a visitor.
/// </summary>
public record GetVisitorReservationStatsQuery(
    int VisitorId,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IRequest<ReservationStatsDto>;
