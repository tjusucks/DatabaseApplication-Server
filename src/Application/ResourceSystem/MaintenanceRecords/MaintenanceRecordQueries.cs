using DbApp.Domain.Enums.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.MaintenanceRecords;

/// <summary>  
/// Query to get maintenance record by ID.  
/// </summary>  
public record GetMaintenanceRecordByIdQuery(int MaintenanceId) : IRequest<MaintenanceRecordSummaryDto?>;

/// <summary>  
/// Unified query to search maintenance records with comprehensive filtering options.  
/// </summary>  
public record SearchMaintenanceRecordsQuery(
    string? SearchTerm = null,
    int? RideId = null,
    int? TeamId = null,
    int? ManagerId = null,
    MaintenanceType? MaintenanceType = null,
    bool? IsCompleted = null,
    bool? IsAccepted = null,
    DateTime? StartTimeFrom = null,
    DateTime? StartTimeTo = null,
    DateTime? EndTimeFrom = null,
    DateTime? EndTimeTo = null,
    decimal? MinCost = null,
    decimal? MaxCost = null,
    int Page = 1,
    int PageSize = 10
) : IRequest<MaintenanceRecordResult>;

/// <summary>  
/// Query to get maintenance record statistics.  
/// </summary>  
public record GetMaintenanceRecordStatsQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int? RideId = null
) : IRequest<MaintenanceRecordStatsDto>;

/// <summary>  
/// Command to create a new maintenance record.  
/// </summary>  
public record CreateMaintenanceRecordCommand(
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
) : IRequest<int>;

/// <summary>  
/// Command to update an existing maintenance record.  
/// </summary>  
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
) : IRequest;

/// <summary>  
/// Command to delete a maintenance record.  
/// </summary>  
public record DeleteMaintenanceRecordCommand(int MaintenanceId) : IRequest<bool>;
