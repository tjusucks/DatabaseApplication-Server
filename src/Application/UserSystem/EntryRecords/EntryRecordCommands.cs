using MediatR;

namespace DbApp.Application.UserSystem.EntryRecords;

/// <summary>
/// Command to register a visitor's entry into the park.
/// </summary>
/// <param name="VisitorId">The ID of the visitor entering the park.</param>
/// <param name="EntryGate">The name of the entrance gate used.</param>
/// <param name="TicketId">Optional ticket ID used for entry.</param>
public record RegisterEntryCommand(
    int VisitorId,
    string EntryGate,
    int? TicketId = null
) : IRequest<int>;

/// <summary>
/// Command to register a visitor's exit from the park.
/// </summary>
/// <param name="VisitorId">The ID of the visitor exiting the park.</param>
/// <param name="ExitGate">The name of the exit gate used.</param>
public record RegisterExitCommand(
    int VisitorId,
    string ExitGate
) : IRequest<Unit>;

/// <summary>
/// Command to update an entry record.
/// </summary>
/// <param name="EntryRecordId">The ID of the entry record to update.</param>
/// <param name="EntryGate">Updated entry gate name.</param>
/// <param name="ExitGate">Updated exit gate name.</param>
/// <param name="TicketId">Updated ticket ID.</param>
public record UpdateEntryRecordCommand(
    int EntryRecordId,
    string? EntryGate = null,
    string? ExitGate = null,
    int? TicketId = null
) : IRequest<Unit>;

/// <summary>
/// Command to delete an entry record.
/// </summary>
/// <param name="EntryRecordId">The ID of the entry record to delete.</param>
public record DeleteEntryRecordCommand(int EntryRecordId) : IRequest<Unit>;
