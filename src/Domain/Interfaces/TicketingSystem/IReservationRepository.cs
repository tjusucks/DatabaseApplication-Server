using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Enums.TicketingSystem;
using DbApp.Domain.Statistics.TicketingSystem;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface IReservationRepository
{
    /// <summary>
    /// Search reservations by visitor ID with various filters and pagination.
    /// </summary>
    Task<List<Reservation>> SearchByVisitorAsync(
        int visitorId,
        DateTime? startDate,
        DateTime? endDate,
        PaymentStatus? paymentStatus,
        ReservationStatus? status,
        string? sortBy,
        bool descending,
        int page,
        int pageSize);

    /// <summary>
    /// Count reservations by visitor ID with various filters.
    /// </summary>
    Task<int> CountByVisitorAsync(
        int visitorId,
        DateTime? startDate,
        DateTime? endDate,
        PaymentStatus? paymentStatus,
        ReservationStatus? status);

    /// <summary>
    /// Search reservations by multiple criteria (admin use).
    /// </summary>
    Task<List<Reservation>> SearchAsync(
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        PaymentStatus? paymentStatus,
        ReservationStatus? status,
        decimal? minAmount,
        decimal? maxAmount,
        int? promotionId,
        string? sortBy,
        bool descending,
        int page,
        int pageSize);

    /// <summary>
    /// Count reservations by multiple criteria (admin use).
    /// </summary>
    Task<int> CountAsync(
        string? keyword,
        DateTime? startDate,
        DateTime? endDate,
        PaymentStatus? paymentStatus,
        ReservationStatus? status,
        decimal? minAmount,
        decimal? maxAmount,
        int? promotionId);

    /// <summary>
    /// Get reservation statistics for a visitor.
    /// </summary>
    Task<ReservationStats> GetStatsByVisitorAsync(
        int visitorId,
        DateTime? startDate,
        DateTime? endDate);
}
