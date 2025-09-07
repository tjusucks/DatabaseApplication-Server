using DbApp.Domain.Interfaces.UserSystem;
using MediatR;
using static DbApp.Domain.Exceptions;

namespace DbApp.Application.UserSystem.EntryRecords;

/// <summary>
/// Centralized handler for all entry record commands.
/// </summary>
public class EntryRecordCommandHandlers(
    IEntryRecordRepository entryRecordRepo,
    IEntryRecordService entryRecordService) :
    IRequestHandler<CreateEntryRecordCommand, int>,
    IRequestHandler<UpdateEntryRecordCommand, Unit>,
    IRequestHandler<DeleteEntryRecordCommand, Unit>
{
    private readonly IEntryRecordRepository _entryRecordRepo = entryRecordRepo;
    private readonly IEntryRecordService _entryRecordService = entryRecordService;

    public async Task<int> Handle(CreateEntryRecordCommand request, CancellationToken cancellationToken)
    {
        if (request.Type.Equals("entry", StringComparison.InvariantCultureIgnoreCase))
        {
            return await _entryRecordService.CreateEntryAsync(request.VisitorId, request.GateName, request.TicketId);
        }
        else if (request.Type.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
        {
            return await _entryRecordService.CreateExitAsync(request.VisitorId, request.GateName);
        }
        else
        {
            throw new ValidationException($"Invalid type '{request.Type}'. Must be 'entry' or 'exit'.");
        }
    }

    public async Task<Unit> Handle(UpdateEntryRecordCommand request, CancellationToken cancellationToken)
    {
        var entryRecord = await _entryRecordRepo.GetByIdAsync(request.EntryRecordId)
            ?? throw new NotFoundException($"Entry record with ID {request.EntryRecordId} not found.");

        if (request.EntryGate != null)
        {
            entryRecord.EntryGate = request.EntryGate;
        }
        if (request.ExitGate != null)
        {
            entryRecord.ExitGate = request.ExitGate;
        }
        if (request.TicketId.HasValue)
        {
            entryRecord.TicketId = request.TicketId;
        }

        await _entryRecordRepo.UpdateAsync(entryRecord);
        return Unit.Value;
    }

    public async Task<Unit> Handle(DeleteEntryRecordCommand request, CancellationToken cancellationToken)
    {
        var entryRecord = await _entryRecordRepo.GetByIdAsync(request.EntryRecordId)
            ?? throw new NotFoundException($"Entry record with ID {request.EntryRecordId} not found.");

        await _entryRecordRepo.DeleteAsync(entryRecord);
        return Unit.Value;
    }
}
