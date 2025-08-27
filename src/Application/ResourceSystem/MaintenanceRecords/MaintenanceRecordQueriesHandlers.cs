using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.MaintenanceRecords;

public class GetMaintenanceRecordByIdQueryHandler(IMaintenanceRecordRepository repository)
    : IRequestHandler<GetMaintenanceRecordByIdQuery, MaintenanceRecord?>
{
    public async Task<MaintenanceRecord?> Handle(GetMaintenanceRecordByIdQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(request.MaintenanceId);
    }
}

public class GetAllMaintenanceRecordsQueryHandler(IMaintenanceRecordRepository repository)
    : IRequestHandler<GetAllMaintenanceRecordsQuery, List<MaintenanceRecord>>
{
    public async Task<List<MaintenanceRecord>> Handle(GetAllMaintenanceRecordsQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetAllAsync();
    }
}

public class GetMaintenanceRecordsByRideQueryHandler(IMaintenanceRecordRepository repository)
    : IRequestHandler<GetMaintenanceRecordsByRideQuery, List<MaintenanceRecord>>
{
    public async Task<List<MaintenanceRecord>> Handle(GetMaintenanceRecordsByRideQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetByRideIdAsync(request.RideId);
    }
}

public class GetPendingMaintenanceRecordsQueryHandler(IMaintenanceRecordRepository repository)
    : IRequestHandler<GetPendingMaintenanceRecordsQuery, List<MaintenanceRecord>>
{
    public async Task<List<MaintenanceRecord>> Handle(GetPendingMaintenanceRecordsQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetPendingAsync();
    }
}
