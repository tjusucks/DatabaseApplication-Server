using DbApp.Application.UserSystem.Visitors.Services;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Visitors;

/// <summary>
/// Handler for registering a new visitor.
/// </summary>
public class RegisterVisitorCommandHandler(IVisitorRepository visitorRepository, IUserRepository userRepository) 
    : IRequestHandler<RegisterVisitorCommand, int>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<int> Handle(RegisterVisitorCommand request, CancellationToken cancellationToken)
    {
        // Verify user exists
        _ = await _userRepository.GetByIdAsync(request.UserId)
            ?? throw new InvalidOperationException($"User with ID {request.UserId} not found");

        // Check if visitor already exists
        var existingVisitor = await _visitorRepository.GetByUserIdAsync(request.UserId);
        if (existingVisitor != null)
        {
            throw new InvalidOperationException($"Visitor already exists for user ID {request.UserId}");
        }

        var visitor = new Visitor
        {
            VisitorId = request.UserId, // Foreign key to User
            VisitorType = VisitorType.Regular,
            Points = 0,
            Height = request.Height,
            IsBlacklisted = false,
            CreatedAt = DateTime.UtcNow
        };

        return await _visitorRepository.CreateAsync(visitor);
    }
}

/// <summary>
/// Handler for upgrading a visitor to member status.
/// </summary>
public class UpgradeToMemberCommandHandler(IVisitorRepository visitorRepository) 
    : IRequestHandler<UpgradeToMemberCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<Unit> Handle(UpgradeToMemberCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepository.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException($"Visitor with ID {request.VisitorId} not found");

        if (visitor.IsBlacklisted)
        {
            throw new InvalidOperationException("Cannot upgrade blacklisted visitor to member");
        }

        MembershipService.UpgradeToMember(visitor);
        await _visitorRepository.UpdateAsync(visitor);

        return Unit.Value;
    }
}

/// <summary>
/// Handler for adding points to a visitor's account.
/// </summary>
public class AddPointsCommandHandler(IVisitorRepository visitorRepository) 
    : IRequestHandler<AddPointsCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<Unit> Handle(AddPointsCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepository.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException($"Visitor with ID {request.VisitorId} not found");

        if (request.Points <= 0)
        {
            throw new ArgumentException("Points to add must be positive", nameof(request));
        }

        visitor.Points += request.Points;
        
        // Update member level if necessary
        MembershipService.UpdateMemberLevel(visitor);
        
        await _visitorRepository.UpdateAsync(visitor);

        return Unit.Value;
    }
}

/// <summary>
/// Handler for deducting points from a visitor's account.
/// </summary>
public class DeductPointsCommandHandler(IVisitorRepository visitorRepository) 
    : IRequestHandler<DeductPointsCommand, bool>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<bool> Handle(DeductPointsCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepository.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException($"Visitor with ID {request.VisitorId} not found");

        if (request.Points <= 0)
        {
            throw new ArgumentException("Points to deduct must be positive", nameof(request));
        }

        if (visitor.Points < request.Points)
        {
            return false; // Insufficient points
        }

        visitor.Points -= request.Points;
        
        // Update member level if necessary
        MembershipService.UpdateMemberLevel(visitor);
        
        await _visitorRepository.UpdateAsync(visitor);

        return true;
    }
}

/// <summary>
/// Handler for updating visitor information.
/// </summary>
public class UpdateVisitorCommandHandler(IVisitorRepository visitorRepository)
    : IRequestHandler<UpdateVisitorCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<Unit> Handle(UpdateVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepository.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException($"Visitor with ID {request.VisitorId} not found");

        if (request.Height < 50 || request.Height > 300)
        {
            throw new ArgumentException("Height must be between 50 and 300 cm", nameof(request));
        }

        visitor.Height = request.Height;
        visitor.UpdatedAt = DateTime.UtcNow;

        await _visitorRepository.UpdateAsync(visitor);

        return Unit.Value;
    }
}

/// <summary>
/// Handler for blacklisting a visitor.
/// </summary>
public class BlacklistVisitorCommandHandler(IVisitorRepository visitorRepository)
    : IRequestHandler<BlacklistVisitorCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<Unit> Handle(BlacklistVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepository.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException($"Visitor with ID {request.VisitorId} not found");

        visitor.IsBlacklisted = true;
        visitor.UpdatedAt = DateTime.UtcNow;

        await _visitorRepository.UpdateAsync(visitor);

        return Unit.Value;
    }
}

/// <summary>
/// Handler for removing a visitor from blacklist.
/// </summary>
public class RemoveFromBlacklistCommandHandler(IVisitorRepository visitorRepository)
    : IRequestHandler<RemoveFromBlacklistCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<Unit> Handle(RemoveFromBlacklistCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepository.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException($"Visitor with ID {request.VisitorId} not found");

        visitor.IsBlacklisted = false;
        visitor.UpdatedAt = DateTime.UtcNow;

        await _visitorRepository.UpdateAsync(visitor);

        return Unit.Value;
    }
}

/// <summary>
/// Handler for deleting a visitor.
/// </summary>
public class DeleteVisitorCommandHandler(IVisitorRepository visitorRepository)
    : IRequestHandler<DeleteVisitorCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<Unit> Handle(DeleteVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepository.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException($"Visitor with ID {request.VisitorId} not found");

        await _visitorRepository.DeleteAsync(visitor);

        return Unit.Value;
    }
}
