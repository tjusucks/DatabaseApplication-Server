using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Enums.TicketingSystem;
using DbApp.Domain.Statistics.TicketingSystem;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface ITicketRepository
{
    /// <summary>
    /// Search ticket sale records with filtering and pagination.
    /// </summary>
    Task<List<Ticket>> SearchAsync(
        string? keyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? ticketTypeId = null,
        int? promotionId = null,
        PaymentStatus? paymentStatus = null,
        string? sortBy = "SalesDate",
        bool descending = true,
        int page = 1,
        int pageSize = 20);

    /// <summary>
    /// Get total count of ticket sale records with filtering.
    /// </summary>
    Task<int> CountAsync(
        string? keyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? ticketTypeId = null,
        int? promotionId = null,
        PaymentStatus? paymentStatus = null);

    /// <summary>
    /// Get overall ticket sale statistics with filtering.
    /// </summary>
    Task<TicketSaleStats> GetStatsAsync(
        string? keyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? ticketTypeId = null,
        int? promotionId = null,
        PaymentStatus? paymentStatus = null);

    /// <summary>
    /// Get grouped ticket sale statistics by specified dimension with filtering.
    /// </summary>
    Task<List<GroupedTicketSaleStats>> GetGroupedStatsAsync(
        string? keyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? ticketTypeId = null,
        int? promotionId = null,
        PaymentStatus? paymentStatus = null,
        string groupBy = "TicketType",
        string? sortBy = "Revenue",
        bool descending = true);
}
