using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.InspectionRecords;

public class CreateInspectionRecordCommandHandler(IInspectionRecordRepository repository)
    : IRequestHandler<CreateInspectionRecordCommand, int>
{
    public async Task<int> Handle(CreateInspectionRecordCommand request, CancellationToken cancellationToken)
    {
        var record = new InspectionRecord
        {
            RideId = request.RideId,
            TeamId = request.TeamId,
            CheckDate = request.CheckDate,
            CheckType = request.CheckType,
            IsPassed = request.IsPassed,
            IssuesFound = request.IssuesFound,
            Recommendations = request.Recommendations,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await repository.CreateAsync(record);
    }
}

public class UpdateInspectionRecordCommandHandler(IInspectionRecordRepository repository)
    : IRequestHandler<UpdateInspectionRecordCommand, Unit>
{
    public async Task<Unit> Handle(UpdateInspectionRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await repository.GetByIdAsync(request.InspectionId)
            ?? throw new InvalidOperationException("Inspection record not found");

        record.RideId = request.RideId;
        record.TeamId = request.TeamId;
        record.CheckDate = request.CheckDate;
        record.CheckType = request.CheckType;
        record.IsPassed = request.IsPassed;
        record.IssuesFound = request.IssuesFound;
        record.Recommendations = request.Recommendations;
        record.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateAsync(record);
        return Unit.Value;
    }
}

public class CompleteInspectionCommandHandler(IInspectionRecordRepository repository)
    : IRequestHandler<CompleteInspectionCommand, Unit>
{
    public async Task<Unit> Handle(CompleteInspectionCommand request, CancellationToken cancellationToken)
    {
        var record = await repository.GetByIdAsync(request.InspectionId)
            ?? throw new InvalidOperationException("Inspection record not found");

        record.IsPassed = request.IsPassed;
        record.IssuesFound = request.IssuesFound;
        record.Recommendations = request.Recommendations;
        record.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateAsync(record);
        return Unit.Value;
    }
}

public class DeleteInspectionRecordCommandHandler(IInspectionRecordRepository repository)
    : IRequestHandler<DeleteInspectionRecordCommand, Unit>
{
    public async Task<Unit> Handle(DeleteInspectionRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await repository.GetByIdAsync(request.InspectionId)
            ?? throw new InvalidOperationException("Inspection record not found");

        await repository.DeleteAsync(record);
        return Unit.Value;
    }
}
