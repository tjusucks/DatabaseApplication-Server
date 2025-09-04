using DbApp.Application.UserSystem.Visitors.Services;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Visitors;

/// <summary>
/// Handler for creating a new visitor with complete user information.
/// Uses single transaction to ensure data consistency.
/// </summary>
public class CreateVisitorCommandHandler(
    IVisitorRepository visitorRepository) : IRequestHandler<CreateVisitorCommand, int>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<int> Handle(CreateVisitorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Create user entity with navigation property
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                DisplayName = request.DisplayName,
                PhoneNumber = request.PhoneNumber,
                BirthDate = request.BirthDate,
                Gender = request.Gender,
                PasswordHash = request.PasswordHash,
                RegisterTime = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                RoleId = 1 // Default role for visitors (seeded data)
            };

            // Create visitor with user navigation property - single transaction
            var visitor = new Visitor
            {
                User = user, // Navigation property - EF will handle the relationship
                VisitorType = request.VisitorType,
                Height = request.Height,
                Points = 0,
                MemberLevel = "Bronze",
                IsBlacklisted = false,
                CreatedAt = DateTime.UtcNow
            };

            // Single SaveChangesAsync call = single transaction
            await _visitorRepository.CreateAsync(visitor);

            return visitor.VisitorId;
        }
        catch (Exception ex)
        {
            // Log the error and rethrow with context
            throw new InvalidOperationException($"Failed to create visitor: {ex.Message}", ex);
        }
    }
}

/// <summary>
/// Handler for registering a new visitor based on existing user.
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
        // Verify visitor exists
        _ = await _visitorRepository.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException($"Visitor with ID {request.VisitorId} not found");

        if (request.Points <= 0)
        {
            throw new ArgumentException("Points to add must be positive", nameof(request));
        }

        // Use repository method to avoid entity tracking conflicts
        await _visitorRepository.AddPointsAsync(request.VisitorId, request.Points);

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
/// Uses repository method to avoid EF Core tracking conflicts.
/// </summary>
public class UpdateVisitorCommandHandler(
    IVisitorRepository visitorRepository) : IRequestHandler<UpdateVisitorCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<Unit> Handle(UpdateVisitorCommand request, CancellationToken cancellationToken)
    {
        // Use repository method to avoid EF Core entity tracking conflicts
        await _visitorRepository.UpdateVisitorInfoAsync(
            request.VisitorId,
            request.DisplayName,
            request.PhoneNumber,
            request.BirthDate,
            request.Gender,
            request.VisitorType,
            request.Height,
            request.Points,
            request.MemberLevel);

        return Unit.Value;
    }
}

/// <summary>
/// Handler for updating visitor blacklist status.
/// </summary>
public class UpdateVisitorBlacklistStatusCommandHandler(IVisitorRepository visitorRepository) : IRequestHandler<UpdateVisitorBlacklistStatusCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<Unit> Handle(UpdateVisitorBlacklistStatusCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepository.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException($"Visitor with ID {request.VisitorId} not found");

        visitor.IsBlacklisted = request.IsBlacklisted;
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
public class UnblacklistVisitorCommandHandler(IVisitorRepository visitorRepository)
    : IRequestHandler<UnblacklistVisitorCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<Unit> Handle(UnblacklistVisitorCommand request, CancellationToken cancellationToken)
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
public class DeleteVisitorCommandHandler(
    IVisitorRepository visitorRepository,
    IUserRepository userRepository) : IRequestHandler<DeleteVisitorCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Unit> Handle(DeleteVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepository.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException($"Visitor with ID {request.VisitorId} not found");

        var user = await _userRepository.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException($"User with ID {request.VisitorId} not found");

        await _visitorRepository.DeleteAsync(visitor);
        await _userRepository.DeleteAsync(user);

        return Unit.Value;
    }
}
