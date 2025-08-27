using DbApp.Domain.Entities.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.MaintenanceRecords;

public record GetMaintenanceRecordByIdQuery(int MaintenanceId) : IRequest<MaintenanceRecord?>;

public record GetAllMaintenanceRecordsQuery() : IRequest<List<MaintenanceRecord>>;

public record GetMaintenanceRecordsByRideQuery(int RideId) : IRequest<List<MaintenanceRecord>>;

public record GetPendingMaintenanceRecordsQuery() : IRequest<List<MaintenanceRecord>>;

public record GetMaintenanceRecordsByTeamQuery(int TeamId) : IRequest<List<MaintenanceRecord>>;

public record GetMaintenanceRecordsByDateRangeQuery(DateTime StartDate, DateTime EndDate) : IRequest<List<MaintenanceRecord>>;
