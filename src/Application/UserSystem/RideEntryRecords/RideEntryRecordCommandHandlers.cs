using DbApp.Domain.Interfaces.UserSystem;
using MediatR;
using static DbApp.Domain.Exceptions;

namespace DbApp.Application.UserSystem.RideEntryRecords;

/// <summary>
/// Handlers for ride entry record commands.
/// </summary>
public class RideEntryRecordCommandHandlers(
    IRideEntryRecordService rideEntryRecordService,
    IRideEntryRecordRepository rideEntryRecordRepository) :
    IRequestHandler<CreateRideEntryRecordCommand, int>,
    IRequestHandler<UpdateRideEntryRecordCommand, Unit>,
    IRequestHandler<DeleteRideEntryRecordCommand, Unit>
{
    private readonly IRideEntryRecordService _rideEntryRecordService = rideEntryRecordService;
    private readonly IRideEntryRecordRepository _rideEntryRecordRepository = rideEntryRecordRepository;

    public async Task<int> Handle(CreateRideEntryRecordCommand request, CancellationToken cancellationToken)
    {
        // Handle ride entry or exit based on type
        return request.Type.ToLower() switch
        {
            "entry" => await _rideEntryRecordService.CreateRideEntryAsync(
                request.VisitorId,
                request.RideId,
                request.GateName,
                request.TicketId),
            "exit" => await _rideEntryRecordService.CreateRideExitAsync(
                request.VisitorId,
                request.RideId,
                request.GateName),
            _ => throw new ValidationException($"Invalid type '{request.Type}'. Must be 'entry' or 'exit'.")
        };
    }

    public async Task<Unit> Handle(UpdateRideEntryRecordCommand request, CancellationToken cancellationToken)
    {
        var rideEntryRecord = await _rideEntryRecordRepository.GetByIdAsync(request.RideEntryRecordId)
            ?? throw new NotFoundException($"Ride entry record with ID {request.RideEntryRecordId} not found.");

        // Update only the provided fields
        if (request.EntryGate != null)
            rideEntryRecord.EntryGate = request.EntryGate;

        if (request.ExitGate != null)
            rideEntryRecord.ExitGate = request.ExitGate;

        if (request.ExitTime != null)
            rideEntryRecord.ExitTime = request.ExitTime;

        if (request.TicketId != null)
            rideEntryRecord.TicketId = request.TicketId;

        rideEntryRecord.UpdatedAt = DateTime.UtcNow;

        await _rideEntryRecordRepository.UpdateAsync(rideEntryRecord);
        return Unit.Value;
    }

    public async Task<Unit> Handle(DeleteRideEntryRecordCommand request, CancellationToken cancellationToken)
    {
        var rideEntryRecord = await _rideEntryRecordRepository.GetByIdAsync(request.RideEntryRecordId)
            ?? throw new NotFoundException($"Ride entry record with ID {request.RideEntryRecordId} not found.");
        await _rideEntryRecordRepository.DeleteAsync(rideEntryRecord);
        return Unit.Value;
    }
}
