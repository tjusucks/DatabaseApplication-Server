using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Specifications.TicketingSystem;
using DbApp.Domain.Statistics.TicketingSystem;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface IReservationRepository
{
    // 基础CRUD操作
    /// <summary>
    /// Add a new reservation to the repository.
    /// </summary>
    Task<Reservation> AddAsync(Reservation reservation);

    /// <summary>
    /// Update an existing reservation.
    /// </summary>
    Task<Reservation> UpdateAsync(Reservation reservation);

    /// <summary>
    /// Get reservation by ID.
    /// </summary>
    Task<Reservation?> GetByIdAsync(int reservationId);

    /// <summary>
    /// Delete a reservation by ID.
    /// </summary>
    Task<bool> DeleteAsync(int reservationId);

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
