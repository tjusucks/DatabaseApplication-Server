using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Visitors;

/// <summary>
/// Centralized handler for all visitor commands.
/// </summary>
public class VisitorCommandHandlers(IVisitorRepository visitorRepo) :
    IRequestHandler<CreateVisitorCommand, int>,
    IRequestHandler<UpdateVisitorCommand, Unit>,
    IRequestHandler<DeleteVisitorCommand, Unit>,
    IRequestHandler<BlacklistVisitorCommand, Unit>,
    IRequestHandler<UnblacklistVisitorCommand, Unit>,
    IRequestHandler<UpgradeToMemberCommand, Unit>,
    IRequestHandler<RemoveMembershipCommand, Unit>,
    IRequestHandler<AddPointsCommand, Unit>,
    IRequestHandler<DeductPointsCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepo = visitorRepo;

    public async Task<int> Handle(CreateVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = new Visitor
        {
            User = new User
            {
                Username = request.Username,
                PasswordHash = request.PasswordHash,
                Email = request.Email,
                DisplayName = request.DisplayName,
                PhoneNumber = request.PhoneNumber,
                BirthDate = request.BirthDate,
                Gender = request.Gender,
                RegisterTime = DateTime.UtcNow,
                PermissionLevel = 1,
                RoleId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            VisitorType = request.VisitorType,
            Points = 0,
            IsBlacklisted = false,
            Height = request.Height,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        return await _visitorRepo.CreateAsync(visitor);
    }

    public async Task<Unit> Handle(UpdateVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepo.GetByIdAsync(request.VisitorId)
            ?? throw new KeyNotFoundException($"Visitor {request.VisitorId} not found.");

        if (request.DisplayName != null)
        {
            visitor.User.DisplayName = request.DisplayName;
        }
        if (request.PhoneNumber != null)
        {
            visitor.User.PhoneNumber = request.PhoneNumber;
        }
        if (request.BirthDate.HasValue)
        {
            visitor.User.BirthDate = request.BirthDate;
        }
        if (request.Gender.HasValue)
        {
            visitor.User.Gender = request.Gender;
        }
        if (request.VisitorType.HasValue)
        {
            visitor.VisitorType = request.VisitorType.Value;
        }
        if (request.Height.HasValue)
        {
            visitor.Height = request.Height.Value;
        }

        await _visitorRepo.UpdateAsync(visitor);
        return Unit.Value;
    }

    public async Task<Unit> Handle(DeleteVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepo.GetByIdAsync(request.VisitorId)
            ?? throw new KeyNotFoundException($"Visitor {request.VisitorId} not found.");

        await _visitorRepo.DeleteAsync(visitor);
        return Unit.Value;
    }

    public async Task<Unit> Handle(BlacklistVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepo.GetByIdAsync(request.VisitorId)
            ?? throw new KeyNotFoundException($"Visitor {request.VisitorId} not found.");

        visitor.IsBlacklisted = true;
        await _visitorRepo.UpdateAsync(visitor);
        return Unit.Value;
    }

    public async Task<Unit> Handle(UnblacklistVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepo.GetByIdAsync(request.VisitorId)
            ?? throw new KeyNotFoundException($"Visitor {request.VisitorId} not found.");

        visitor.IsBlacklisted = false;
        await _visitorRepo.UpdateAsync(visitor);
        return Unit.Value;
    }

    public async Task<Unit> Handle(UpgradeToMemberCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepo.GetByIdAsync(request.VisitorId)
            ?? throw new KeyNotFoundException($"Visitor {request.VisitorId} not found.");

        visitor.MemberLevel = "Member";
        visitor.MemberSince = DateTime.UtcNow;
        await _visitorRepo.UpdateAsync(visitor);
        return Unit.Value;
    }

    public async Task<Unit> Handle(RemoveMembershipCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepo.GetByIdAsync(request.VisitorId)
            ?? throw new KeyNotFoundException($"Visitor {request.VisitorId} not found.");

        visitor.MemberLevel = null;
        visitor.MemberSince = null;
        visitor.Points = 0;
        await _visitorRepo.UpdateAsync(visitor);
        return Unit.Value;
    }

    public async Task<Unit> Handle(AddPointsCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepo.GetByIdAsync(request.VisitorId)
            ?? throw new KeyNotFoundException($"Visitor {request.VisitorId} not found.");

        visitor.Points += request.Points;
        await _visitorRepo.UpdateAsync(visitor);
        return Unit.Value;
    }

    public async Task<Unit> Handle(DeductPointsCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepo.GetByIdAsync(request.VisitorId)
            ?? throw new KeyNotFoundException($"Visitor {request.VisitorId} not found.");

        visitor.Points = Math.Max(0, visitor.Points - request.Points);
        await _visitorRepo.UpdateAsync(visitor);
        return Unit.Value;
    }
}
