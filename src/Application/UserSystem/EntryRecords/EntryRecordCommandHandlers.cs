using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.EntryRecords;

/// <summary>
/// Exception thrown when an entry record is not found.
/// </summary>
public class EntryRecordNotFoundException : Exception
{
    public EntryRecordNotFoundException(int entryRecordId)
        : base($"Entry record with ID {entryRecordId} not found.") { }
}

/// <summary>
/// Centralized handler for all entry record commands.
/// </summary>
public class EntryRecordCommandHandlers(IEntryRecordRepository entryRecordRepo) :
    IRequestHandler<CreateEntryRecordCommand, int>,
    IRequestHandler<UpdateEntryRecordCommand, Unit>,
    IRequestHandler<DeleteEntryRecordCommand, Unit>
{
    private readonly IEntryRecordRepository _entryRecordRepo = entryRecordRepo;

    public async Task<int> Handle(CreateEntryRecordCommand request, CancellationToken cancellationToken)
    {
        if (request.Type.Equals("entry", StringComparison.InvariantCultureIgnoreCase))
        {
            // Create new entry record
            var entryRecord = new EntryRecord
            {
                VisitorId = request.VisitorId,
                EntryTime = DateTime.UtcNow,
                EntryGate = request.GateName,
                TicketId = request.TicketId
            };
            return await _entryRecordRepo.CreateAsync(entryRecord);
        }
        else if (request.Type.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
        {
            // Find active entry record and update with exit information
            var activeEntry = await _entryRecordRepo.GetActiveEntryByVisitorIdAsync(request.VisitorId)
                ?? throw new InvalidOperationException($"No active entry found for visitor {request.VisitorId}.");
            activeEntry.ExitTime = DateTime.UtcNow;
            activeEntry.ExitGate = request.GateName;
            await _entryRecordRepo.UpdateAsync(activeEntry);
            return activeEntry.EntryRecordId;
        }
        else
        {
            throw new ArgumentException($"Invalid type '{request.Type}'. Must be 'entry' or 'exit'.");
        }
    }

    public async Task<Unit> Handle(UpdateEntryRecordCommand request, CancellationToken cancellationToken)
    {
        var entryRecord = await _entryRecordRepo.GetByIdAsync(request.EntryRecordId)
            ?? throw new EntryRecordNotFoundException(request.EntryRecordId);

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
            ?? throw new EntryRecordNotFoundException(request.EntryRecordId);

        await _entryRecordRepo.DeleteAsync(entryRecord);
        return Unit.Value;
    }
}
