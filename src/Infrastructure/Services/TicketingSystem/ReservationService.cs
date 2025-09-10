using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Enums.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using static DbApp.Domain.Exceptions;

namespace DbApp.Infrastructure.Services.TicketingSystem;

public class ReservationService(
    IReservationRepository reservationRepository,
    ITicketTypeRepository ticketTypeRepository,
    IPaymentService paymentService,
    ApplicationDbContext context) : IReservationService
{
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly ITicketTypeRepository _ticketTypeRepository = ticketTypeRepository;
    private readonly IPaymentService _paymentService = paymentService;
    private readonly ApplicationDbContext _context = context;

    public async Task<Reservation> GetReservationByIdAsync(int reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId)
            ?? throw new NotFoundException($"Reservation with ID {reservationId} not found.");
        return reservation;
    }

    public async Task<Reservation> CreateReservationAsync(int visitorId, DateTime visitDate, List<ReservationItem> items, int? promotionId, string? specialRequests)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 验证票型存在性和库存
            var ticketTypes = new Dictionary<int, TicketType>();
            foreach (var item in items)
            {
                var ticketType = await _ticketTypeRepository.GetByIdAsync(item.TicketTypeId) ?? throw new ArgumentException($"Ticket type {item.TicketTypeId} not found");

                // 检查库存限制
                if (ticketType.MaxSaleLimit.HasValue)
                {
                    var soldCount = await _ticketTypeRepository.GetSoldCountAsync(item.TicketTypeId, visitDate);
                    if (soldCount + item.Quantity > ticketType.MaxSaleLimit.Value)
                        throw new InvalidOperationException($"Insufficient stock for ticket type {ticketType.TypeName}");
                }

                ticketTypes[item.TicketTypeId] = ticketType;
            }

            // 创建预订
            var reservation = new Reservation
            {
                VisitorId = visitorId,
                ReservationTime = DateTime.UtcNow,
                VisitDate = visitDate,
                PromotionId = promotionId,
                SpecialRequests = specialRequests,
                PaymentStatus = PaymentStatus.Pending,
                Status = ReservationStatus.Pending
            };

            // 创建预订项目并计算基础价格
            decimal totalAmount = 0;

            foreach (var item in items)
            {
                var ticketType = ticketTypes[item.TicketTypeId];
                var unitPrice = ticketType.BasePrice;

                var itemTotal = unitPrice * item.Quantity;
                totalAmount += itemTotal;

                var reservationItem = new ReservationItem
                {
                    TicketTypeId = item.TicketTypeId,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    DiscountAmount = 0,
                    TotalAmount = itemTotal
                };

                reservation.ReservationItems.Add(reservationItem);
            }

            reservation.DiscountAmount = 0;
            reservation.TotalAmount = totalAmount;

            // 保存预订
            var savedReservation = await _reservationRepository.AddAsync(reservation);

            await transaction.CommitAsync();

            return savedReservation;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Reservation> UpdateReservationStatusAsync(int reservationId, ReservationStatus status, string? reason)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId) ?? throw new ArgumentException($"Reservation {reservationId} not found");
        reservation.Status = status;

        return await _reservationRepository.UpdateAsync(reservation);
    }

    public async Task<bool> CancelReservationAsync(int reservationId, string reason, int? requestingVisitorId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            if (reservation == null)
                return false;

            // 权限检查：只有预订者自己或管理员可以取消
            if (requestingVisitorId.HasValue && reservation.VisitorId != requestingVisitorId.Value)
            {
                throw new UnauthorizedAccessException("You are not authorized to cancel this reservation.");
            }

            // 业务规则检查
            if (reservation.Status == ReservationStatus.Completed || reservation.Status == ReservationStatus.Cancelled)
                throw new InvalidOperationException($"Cannot cancel a reservation with status '{reservation.Status}'");

            // 如果已支付，则触发退款流程
            if (reservation.PaymentStatus == PaymentStatus.Paid)
            {
                await _paymentService.Refund(reservationId, reason);
            }

            reservation.Status = ReservationStatus.Cancelled;
            await _reservationRepository.UpdateAsync(reservation);

            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public Task GenerateTicketsAsync(Reservation reservation)
    {
        foreach (var item in reservation.ReservationItems)
        {
            for (int i = 0; i < item.Quantity; i++)
            {
                var ticket = new Ticket
                {
                    ReservationItemId = item.ItemId,
                    TicketTypeId = item.TicketTypeId,
                    VisitorId = reservation.VisitorId,
                    SerialNumber = GenerateTicketSerialNumber(),
                    ValidFrom = reservation.VisitDate.Date,
                    ValidTo = reservation.VisitDate.Date.AddDays(1),
                    Status = TicketStatus.Issued
                };

                item.Tickets.Add(ticket);
            }
        }

        return Task.CompletedTask;
    }

    private static string GenerateTicketSerialNumber()
    {
        return $"TKT{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}
