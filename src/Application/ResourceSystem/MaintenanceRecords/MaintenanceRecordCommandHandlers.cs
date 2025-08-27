using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.MaintenanceRecords;

public class CreateMaintenanceRecordCommandHandler(IMaintenanceRecordRepository repository)
    : IRequestHandler<CreateMaintenanceRecordCommand, int>
{
    public async Task<int> Handle(CreateMaintenanceRecordCommand request, CancellationToken cancellationToken)
    {
        var record = new MaintenanceRecord
        {
            RideId = request.RideId,
            TeamId = request.TeamId,
            ManagerId = request.ManagerId,
            MaintenanceType = request.MaintenanceType,
            StartTime = request.StartTime,
            Cost = request.Cost,
            PartsReplaced = request.PartsReplaced,
            MaintenanceDetails = request.MaintenanceDetails,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await repository.CreateAsync(record);
    }
}

public class UpdateMaintenanceRecordCommandHandler(IMaintenanceRecordRepository repository)
    : IRequestHandler<UpdateMaintenanceRecordCommand, Unit>
{
    public async Task<Unit> Handle(UpdateMaintenanceRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await repository.GetByIdAsync(request.MaintenanceId)
            ?? throw new InvalidOperationException("Maintenance record not found");

        record.RideId = request.RideId;
        record.TeamId = request.TeamId;
        record.ManagerId = request.ManagerId;
        record.MaintenanceType = request.MaintenanceType;
        record.StartTime = request.StartTime;
        record.EndTime = request.EndTime;
        record.Cost = request.Cost;
        record.PartsReplaced = request.PartsReplaced;
        record.MaintenanceDetails = request.MaintenanceDetails;
        record.IsCompleted = request.IsCompleted;
        record.IsAccepted = request.IsAccepted;
        record.AcceptanceDate = request.AcceptanceDate;
        record.AcceptanceComments = request.AcceptanceComments;
        record.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateAsync(record);
        return Unit.Value;
    }
}

public class CompleteMaintenanceCommandHandler(IMaintenanceRecordRepository repository)
    : IRequestHandler<CompleteMaintenanceCommand, Unit>
{
    public async Task<Unit> Handle(CompleteMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var record = await repository.GetByIdAsync(request.MaintenanceId)
            ?? throw new InvalidOperationException("Maintenance record not found");

        record.EndTime = request.EndTime;
        record.IsCompleted = true;
        record.MaintenanceDetails = request.MaintenanceDetails ?? record.MaintenanceDetails;
        record.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateAsync(record);
        return Unit.Value;
    }
}

public class AcceptMaintenanceCommandHandler(IMaintenanceRecordRepository repository)
    : IRequestHandler<AcceptMaintenanceCommand, Unit>
{
    public async Task<Unit> Handle(AcceptMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var record = await repository.GetByIdAsync(request.MaintenanceId)
            ?? throw new InvalidOperationException("Maintenance record not found");

        record.IsAccepted = request.IsAccepted;
        record.AcceptanceDate = DateTime.UtcNow;
        record.AcceptanceComments = request.AcceptanceComments;
        record.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateAsync(record);
        return Unit.Value;
    }
}

public class DeleteMaintenanceRecordCommandHandler(IMaintenanceRecordRepository repository)
    : IRequestHandler<DeleteMaintenanceRecordCommand, Unit>
{
    public async Task<Unit> Handle(DeleteMaintenanceRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await repository.GetByIdAsync(request.MaintenanceId)
            ?? throw new InvalidOperationException("Maintenance record not found");

        await repository.DeleteAsync(record);
        return Unit.Value;
    }
}
