using DbApp.Domain.Enums.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.MaintenanceRecords;

public record CreateMaintenanceRecordCommand(
    int RideId,
    int TeamId,
    int? ManagerId,
    MaintenanceType MaintenanceType,
    DateTime StartTime,
    decimal Cost,
    string? PartsReplaced,
    string? MaintenanceDetails
) : IRequest<int>;

public record UpdateMaintenanceRecordCommand(
    int MaintenanceId,
    int RideId,
    int TeamId,
    int? ManagerId,
    MaintenanceType MaintenanceType,
    DateTime StartTime,
    DateTime? EndTime,
    decimal Cost,
    string? PartsReplaced,
    string? MaintenanceDetails,
    bool IsCompleted,
    bool? IsAccepted,
    DateTime? AcceptanceDate,
    string? AcceptanceComments
) : IRequest<Unit>;

public record CompleteMaintenanceCommand(
    int MaintenanceId,
    DateTime EndTime,
    string? MaintenanceDetails
) : IRequest<Unit>;

public record AcceptMaintenanceCommand(
    int MaintenanceId,
    bool IsAccepted,
    string? AcceptanceComments
) : IRequest<Unit>;

public record DeleteMaintenanceRecordCommand(int MaintenanceId) : IRequest<Unit>;
