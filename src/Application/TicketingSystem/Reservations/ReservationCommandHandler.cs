using AutoMapper;
using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.Reservations;

/// <summary>
/// Handler for reservation command operations.
/// </summary>
public class ReservationCommandHandler(
    IReservationService reservationService,
    IPaymentService paymentService,
    IMapper mapper) :
    IRequestHandler<CreateReservationCommand, CreateReservationResponseDto>,
    IRequestHandler<UpdateReservationStatusCommand, ReservationDto>,
    IRequestHandler<ProcessPaymentCommand, ReservationDto>,
    IRequestHandler<CancelReservationCommand, bool>
{
    private readonly IReservationService _reservationService = reservationService;
    private readonly IPaymentService _paymentService = paymentService;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Handle creating a new reservation.
    /// </summary>
    public async Task<CreateReservationResponseDto> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var reservationItems = _mapper.Map<List<ReservationItem>>(request.Items);

        var savedReservation = await _reservationService.CreateReservationAsync(
            request.VisitorId,
            request.VisitDate,
            reservationItems,
            request.PromotionId,
            request.SpecialRequests
        );

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
        var updatedReservation = await _reservationService.UpdateReservationStatusAsync(request.ReservationId, request.Status, null);
        return _mapper.Map<ReservationDto>(updatedReservation);
    }

    /// <summary>
    /// Handle processing a payment for a reservation.
    /// </summary>
    public async Task<ReservationDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        await _paymentService.Pay(request.ReservationId, request.PaymentMethod);
        var reservation = await _reservationService.GetReservationByIdAsync(request.ReservationId);
        return _mapper.Map<ReservationDto>(reservation);
    }

    /// <summary>
    /// Handle cancelling a reservation.
    /// </summary>
    public async Task<bool> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        // 假设取消操作由用户自己发起，传入 VisitorId 以进行权限验证
        // 在实际应用中，你可能需要从当前用户上下文中获取此 ID
        return await _reservationService.CancelReservationAsync(request.ReservationId, "Cancelled by user", null);
    }
}
