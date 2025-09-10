using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Enums.TicketingSystem;
using System.Threading.Tasks;

namespace DbApp.Domain.Interfaces.TicketingSystem
{
    public interface IReservationService
    {
        Task<Reservation> GetReservationByIdAsync(int reservationId);
        Task<Reservation> CreateReservationAsync(int visitorId, DateTime visitDate, List<ReservationItem> items, int? promotionId, string? specialRequests);
        Task<Reservation> UpdateReservationStatusAsync(int reservationId, ReservationStatus status, string? reason);
        Task<bool> CancelReservationAsync(int reservationId, string reason, int? requestingVisitorId);
        Task GenerateTicketsAsync(Reservation reservation);
    }
}
