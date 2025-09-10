using System.Threading.Tasks;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Enums.TicketingSystem;

namespace DbApp.Domain.Interfaces.TicketingSystem
{
    public interface IPaymentService
    {
        Task Pay(int reservationId, string paymentMethod);
        Task Refund(int reservationId, string reason);
    }
}
