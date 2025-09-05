using AutoMapper;
using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Enums.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.Reservations;

/// <summary>
/// Handler for reservation command operations.
/// </summary>
public class ReservationCommandHandler(
    IReservationRepository reservationRepository,
    ITicketTypeRepository ticketTypeRepository,
    IPromotionRepository promotionRepository,
    IMapper mapper) :
    IRequestHandler<CreateReservationCommand, CreateReservationResponseDto>,
    IRequestHandler<UpdateReservationStatusCommand, ReservationDto>,
    IRequestHandler<ProcessPaymentCommand, ReservationDto>,
    IRequestHandler<CancelReservationCommand, bool>
{
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly ITicketTypeRepository _ticketTypeRepository = ticketTypeRepository;
    private readonly IPromotionRepository _promotionRepository = promotionRepository;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Handle creating a new reservation.
    /// </summary>
    public async Task<CreateReservationResponseDto> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        // 1. 验证票型存在性和库存
        var ticketTypes = new Dictionary<int, TicketType>();
        foreach (var item in request.Items)
        {
            var ticketType = await _ticketTypeRepository.GetByIdAsync(item.TicketTypeId);
            if (ticketType == null)
                throw new ArgumentException($"Ticket type {item.TicketTypeId} not found");
                
            // 检查库存限制
            if (ticketType.MaxSaleLimit.HasValue)
            {
                var soldCount = await _ticketTypeRepository.GetSoldCountAsync(item.TicketTypeId, request.VisitDate);
                if (soldCount + item.Quantity > ticketType.MaxSaleLimit.Value)
                    throw new InvalidOperationException($"Insufficient stock for ticket type {ticketType.TypeName}");
            }
            
            ticketTypes[item.TicketTypeId] = ticketType;
        }

        // 2. 验证优惠活动（如果有）
        Promotion? promotion = null;
        if (request.PromotionId.HasValue)
        {
            promotion = await _promotionRepository.GetByIdAsync(request.PromotionId.Value);
            if (promotion == null || !promotion.IsActive || promotion.StartDatetime > DateTime.UtcNow || promotion.EndDatetime < DateTime.UtcNow)
                throw new ArgumentException($"Promotion {request.PromotionId.Value} is not valid or active");
        }

        // 3. 创建预订
        var reservation = new Reservation
        {
            VisitorId = request.VisitorId,
            ReservationTime = DateTime.UtcNow,
            VisitDate = request.VisitDate,
            PromotionId = request.PromotionId,
            SpecialRequests = request.SpecialRequests,
            PaymentStatus = PaymentStatus.Pending,
            Status = ReservationStatus.Pending
        };

        // 4. 创建预订项目并计算价格
        decimal totalAmount = 0;
        decimal totalDiscount = 0;

        foreach (var item in request.Items)
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
                DiscountAmount = 0, // 暂时设为0，后续可以添加折扣逻辑
                TotalAmount = itemTotal
            };

            reservation.ReservationItems.Add(reservationItem);
        }

        // 5. 应用优惠折扣
        if (promotion != null)
        {
            // 优惠计算逻辑待实现
        }

        reservation.DiscountAmount = totalDiscount;
        reservation.TotalAmount = totalAmount - totalDiscount;

        // 6. 保存预订
        var savedReservation = await _reservationRepository.AddAsync(reservation);

        // 7. 返回DTO
        return new CreateReservationResponseDto
        {
            ReservationId = savedReservation.ReservationId,
            TotalAmount = savedReservation.TotalAmount,
            PaymentStatus = savedReservation.PaymentStatus,
            Status = savedReservation.Status,
            Message = "Reservation created successfully"
        };
    }

    /// <summary>
    /// Handle updating reservation status.
    /// </summary>
    public async Task<ReservationDto> Handle(UpdateReservationStatusCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId);
        if (reservation == null)
            throw new ArgumentException($"Reservation {request.ReservationId} not found");

        reservation.Status = request.Status;
        var updatedReservation = await _reservationRepository.UpdateAsync(reservation);

        return _mapper.Map<ReservationDto>(updatedReservation);
    }

    /// <summary>
    /// Handle payment processing.
    /// </summary>
    public async Task<ReservationDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId);
        if (reservation == null)
            throw new ArgumentException($"Reservation {request.ReservationId} not found");

        reservation.PaymentStatus = request.PaymentStatus;
        reservation.PaymentMethod = request.PaymentMethod;

        // 如果支付成功，更新预订状态为已确认
        if (request.PaymentStatus == PaymentStatus.Paid)
        {
            reservation.Status = ReservationStatus.Confirmed;
            
            // 生成电子票
            await GenerateTicketsAsync(reservation);
        }

        var updatedReservation = await _reservationRepository.UpdateAsync(reservation);
        return _mapper.Map<ReservationDto>(updatedReservation);
    }

    /// <summary>
    /// Handle reservation cancellation.
    /// </summary>
    public async Task<bool> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId);
        if (reservation == null)
            return false;

        // 检查是否可以取消
        if (reservation.Status == ReservationStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed reservation");

        reservation.Status = ReservationStatus.Cancelled;
        await _reservationRepository.UpdateAsync(reservation);

        return true;
    }

    /// <summary>
    /// Generate tickets for confirmed reservation.
    /// </summary>
    private Task GenerateTicketsAsync(Reservation reservation)
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

    /// <summary>
    /// Generate unique ticket serial number.
    /// </summary>
    private static string GenerateTicketSerialNumber()
    {
        return $"TKT{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}
