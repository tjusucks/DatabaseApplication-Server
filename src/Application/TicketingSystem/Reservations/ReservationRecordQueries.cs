using DbApp.Domain.Enums.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.Reservations;

/// <summary>
/// Search reservation records by visitor with filtering options.
/// </summary>
public record SearchReservationRecordByVisitorQuery(
    int VisitorId,
    string? Keyword = null, // Search in visitor name, email, phone.
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
) : IRequest<ReservationRecordResult>;

/// <summary>
/// Search reservation records by multiple criteria (admin use).
/// </summary>
public record SearchReservationRecordQuery(
    int? VisitorId = null,
    string? Keyword = null, // Search in visitor name, email, phone.
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
) : IRequest<ReservationRecordResult>;

/// <summary>
/// Get reservation record statistics for a visitor.
/// </summary>
public record GetVisitorReservationRecordStatsQuery(
    int VisitorId,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IRequest<ReservationRecordStatsDto>;
