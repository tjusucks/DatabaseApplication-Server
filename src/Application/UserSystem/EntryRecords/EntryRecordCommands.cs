using MediatR;

namespace DbApp.Application.UserSystem.EntryRecords;

/// <summary>
/// Command to create an entry or exit record based on type.
/// </summary>
public record CreateEntryRecordCommand(
    int VisitorId,
    string Type, // "entry" or "exit"
    string GateName,
    int? TicketId = null
) : IRequest<int>;

/// <summary>
/// Command to update an entry record.
/// </summary>
public record UpdateEntryRecordCommand(
    int EntryRecordId,
    string? EntryGate = null,
    string? ExitGate = null,
    int? TicketId = null
) : IRequest<Unit>;

/// <summary>
/// Command to delete an entry record.
/// </summary>
public record DeleteEntryRecordCommand(int EntryRecordId) : IRequest<Unit>;
