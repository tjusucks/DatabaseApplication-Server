using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.RideTrafficStats;

public class CreateRideTrafficStatCommandHandler(IRideTrafficStatRepository repository)
    : IRequestHandler<CreateRideTrafficStatCommand, Unit>
{
    public async Task<Unit> Handle(CreateRideTrafficStatCommand request, CancellationToken cancellationToken)
    {
        var stat = new RideTrafficStat
        {
            RideId = request.RideId,
            RecordTime = request.RecordTime,
            VisitorCount = request.VisitorCount,
            QueueLength = request.QueueLength,
            WaitingTime = request.WaitingTime,
            IsCrowded = request.IsCrowded,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.CreateAsync(stat);
        return Unit.Value;
    }
}

public class UpdateRideTrafficStatCommandHandler(IRideTrafficStatRepository repository)
    : IRequestHandler<UpdateRideTrafficStatCommand, Unit>
{
    public async Task<Unit> Handle(UpdateRideTrafficStatCommand request, CancellationToken cancellationToken)
    {
        var stat = await repository.GetByIdAsync(request.RideId, request.RecordTime)
            ?? throw new InvalidOperationException("Ride traffic stat not found");

        stat.VisitorCount = request.VisitorCount;
        stat.QueueLength = request.QueueLength;
        stat.WaitingTime = request.WaitingTime;
        stat.IsCrowded = request.IsCrowded;
        stat.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateAsync(stat);
        return Unit.Value;
    }
}

public class DeleteRideTrafficStatCommandHandler(IRideTrafficStatRepository repository)
    : IRequestHandler<DeleteRideTrafficStatCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRideTrafficStatCommand request, CancellationToken cancellationToken)
    {
        var stat = await repository.GetByIdAsync(request.RideId, request.RecordTime)
            ?? throw new InvalidOperationException("Ride traffic stat not found");

        await repository.DeleteAsync(stat);
        return Unit.Value;
    }
}
