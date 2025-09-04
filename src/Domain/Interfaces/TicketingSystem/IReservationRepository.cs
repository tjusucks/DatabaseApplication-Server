using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Specifications.TicketingSystem;
using DbApp.Domain.Statistics.TicketingSystem;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface IReservationRepository
{
    /// <summary>
    /// Search reservations by visitor ID with various filters and pagination.
    /// </summary>
    Task<List<Reservation>> SearchByVisitorAsync(ReservationSearchByVisitorSpec spec);

    /// <summary>
    /// Count reservations by visitor ID with various filters.
    /// </summary>
    Task<int> CountByVisitorAsync(ReservationCountByVisitorSpec spec);

    /// <summary>
    /// Search reservations by multiple criteria (admin use).
    /// </summary>
    Task<List<Reservation>> SearchAsync(ReservationSearchSpec spec);

    /// <summary>
    /// Count reservations by multiple criteria (admin use).
    /// </summary>
    Task<int> CountAsync(ReservationCountSpec spec);

    /// <summary>
    /// Get reservation statistics for a visitor.
    /// </summary>
    Task<ReservationStats> GetStatsByVisitorAsync(ReservationStatsByVisitorSpec spec);
}
