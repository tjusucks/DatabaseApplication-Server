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
        string? keyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        PaymentStatus? paymentStatus = null,
        ReservationStatus? status = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        int? promotionId = null,
        string? sortBy = "ReservationTime",
        bool descending = true,
        int page = 1,
        int pageSize = 20);

    /// <summary>
    /// Count reservations by visitor ID with various filters.
    /// </summary>
    Task<int> CountByVisitorAsync(
        int visitorId,
        string? keyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        PaymentStatus? paymentStatus = null,
        ReservationStatus? status = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        int? promotionId = null);

    /// <summary>
    /// Search reservations by multiple criteria (admin use).
    /// </summary>
    Task<List<Reservation>> SearchAsync(
        int? visitorId = null,
        string? keyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        PaymentStatus? paymentStatus = null,
        ReservationStatus? status = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        int? promotionId = null,
        string? sortBy = "ReservationTime",
        bool descending = true,
        int page = 1,
        int pageSize = 20);

    /// <summary>
    /// Count reservations by multiple criteria (admin use).
    /// </summary>
    Task<int> CountAsync(
        int? visitorId = null,
        string? keyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        PaymentStatus? paymentStatus = null,
        ReservationStatus? status = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        int? promotionId = null);

    /// <summary>
    /// Get reservation statistics for a visitor.
    /// </summary>
    Task<ReservationRecordStats> GetStatsByVisitorAsync(
        int visitorId,
        DateTime? startDate = null,
        DateTime? endDate = null);
}
