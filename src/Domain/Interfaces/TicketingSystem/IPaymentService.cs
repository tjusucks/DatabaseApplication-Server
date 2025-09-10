namespace DbApp.Domain.Interfaces.TicketingSystem
{
    public interface IPaymentService
    {
        Task Pay(int reservationId, string paymentMethod);
        Task Refund(int reservationId, string reason);
    }
}
