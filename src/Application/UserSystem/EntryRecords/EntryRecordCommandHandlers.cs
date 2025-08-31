using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.EntryRecords;

/// <summary>
/// Handler for registering visitor entry into the park.
/// </summary>
public class RegisterEntryCommandHandler(
    IEntryRecordRepository entryRecordRepository,
    IVisitorRepository visitorRepository) : IRequestHandler<RegisterEntryCommand, int>
{
    private readonly IEntryRecordRepository _entryRecordRepository = entryRecordRepository;
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<int> Handle(RegisterEntryCommand request, CancellationToken cancellationToken)
    {
        // Verify visitor exists
        var visitor = await _visitorRepository.GetByIdAsync(request.VisitorId);
        if (visitor == null)
        {
            throw new InvalidOperationException($"Visitor with ID {request.VisitorId} not found.");
        }

        // Check if visitor is blacklisted
        if (visitor.IsBlacklisted)
        {
            throw new InvalidOperationException($"Visitor {request.VisitorId} is blacklisted and cannot enter the park.");
        }

        // Check if visitor is already in the park
        var activeEntry = await _entryRecordRepository.GetActiveEntryForVisitorAsync(request.VisitorId);
        if (activeEntry != null)
        {
            throw new InvalidOperationException($"Visitor {request.VisitorId} is already in the park (entry at {activeEntry.EntryTime}).");
        }

        var entryRecord = new EntryRecord
        {
            VisitorId = request.VisitorId,
            EntryTime = DateTime.UtcNow,
            EntryGate = request.EntryGate,
            TicketId = request.TicketId,
            CreatedAt = DateTime.UtcNow
        };

        return await _entryRecordRepository.CreateAsync(entryRecord);
    }
}

/// <summary>
/// Handler for registering visitor exit from the park.
/// </summary>
public class RegisterExitCommandHandler(IEntryRecordRepository entryRecordRepository) : IRequestHandler<RegisterExitCommand, Unit>
{
    private readonly IEntryRecordRepository _entryRecordRepository = entryRecordRepository;

    public async Task<Unit> Handle(RegisterExitCommand request, CancellationToken cancellationToken)
    {
        // Find the active entry record for the visitor
        var activeEntry = await _entryRecordRepository.GetActiveEntryForVisitorAsync(request.VisitorId);
        if (activeEntry == null)
        {
            throw new InvalidOperationException($"No active entry found for visitor {request.VisitorId}. Visitor is not currently in the park.");
        }

        // Update the entry record with exit information
        activeEntry.ExitTime = DateTime.UtcNow;
        activeEntry.ExitGate = request.ExitGate;
        activeEntry.UpdatedAt = DateTime.UtcNow;

        await _entryRecordRepository.UpdateAsync(activeEntry);
        return Unit.Value;
    }
}

/// <summary>
/// Handler for updating an entry record.
/// </summary>
public class UpdateEntryRecordCommandHandler(IEntryRecordRepository entryRecordRepository) : IRequestHandler<UpdateEntryRecordCommand, Unit>
{
    private readonly IEntryRecordRepository _entryRecordRepository = entryRecordRepository;

    public async Task<Unit> Handle(UpdateEntryRecordCommand request, CancellationToken cancellationToken)
    {
        var entryRecord = await _entryRecordRepository.GetByIdAsync(request.EntryRecordId);
        if (entryRecord == null)
        {
            throw new InvalidOperationException($"Entry record with ID {request.EntryRecordId} not found.");
        }

        // Update only provided fields
        if (!string.IsNullOrEmpty(request.EntryGate))
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

        entryRecord.UpdatedAt = DateTime.UtcNow;

        await _entryRecordRepository.UpdateAsync(entryRecord);
        return Unit.Value;
    }
}

/// <summary>
/// Handler for deleting an entry record.
/// </summary>
public class DeleteEntryRecordCommandHandler(IEntryRecordRepository entryRecordRepository) : IRequestHandler<DeleteEntryRecordCommand, Unit>
{
    private readonly IEntryRecordRepository _entryRecordRepository = entryRecordRepository;

    public async Task<Unit> Handle(DeleteEntryRecordCommand request, CancellationToken cancellationToken)
    {
        var entryRecord = await _entryRecordRepository.GetByIdAsync(request.EntryRecordId);
        if (entryRecord == null)
        {
            throw new InvalidOperationException($"Entry record with ID {request.EntryRecordId} not found.");
        }

        await _entryRecordRepository.DeleteAsync(entryRecord);
        return Unit.Value;
    }
}
