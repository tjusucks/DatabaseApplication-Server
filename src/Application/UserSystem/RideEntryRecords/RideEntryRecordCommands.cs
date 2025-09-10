using MediatR;

namespace DbApp.Application.UserSystem.RideEntryRecords;

/// <summary>
/// Command to create a ride entry or exit record.
/// </summary>
public record CreateRideEntryRecordCommand(
    int VisitorId,
    int RideId,
    string Type, // "entry" or "exit"
    string GateName,
    int? TicketId = null
) : IRequest<int>;

/// <summary>
/// Command to update a ride entry record (e.g., mark exit).
/// </summary>
public record UpdateRideEntryRecordCommand(
    int EntryRecordId,
    string? EntryGate = null,
    string? ExitGate = null,
    DateTime? ExitTime = null,
    int? TicketId = null
) : IRequest<Unit>;

/// <summary>
/// Command to delete a ride entry record.
/// </summary>
public record DeleteRideEntryRecordCommand(int EntryRecordId) : IRequest<Unit>;
