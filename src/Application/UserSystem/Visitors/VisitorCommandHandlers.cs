using DbApp.Domain.Entities.UserSystem;
using DbApp.Application.UserSystem.Visitors.Services;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Visitors;

/// <summary>
/// Handler for creating a new visitor with complete user information.
/// </summary>
public class CreateVisitorCommandHandler(
    IVisitorRepository visitorRepository,
    IUserRepository userRepository) : IRequestHandler<CreateVisitorCommand, int>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<int> Handle(CreateVisitorCommand request, CancellationToken cancellationToken)
    {
        // First create the user
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
            RoleId = 1 // Default role for visitors
        };

        var userId = await _userRepository.CreateAsync(user);

        // Then create the visitor
        var visitor = new Visitor
        {
            VisitorId = userId, // Visitor ID is the same as User ID
            VisitorType = request.VisitorType,
            Height = request.Height,
            CreatedAt = DateTime.UtcNow
        };

        await _visitorRepository.CreateAsync(visitor);
        return userId;
=======
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
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
    }
}

/// <summary>
/// Handler for updating visitor information.
/// </summary>
<<<<<<< HEAD
public class UpdateVisitorCommandHandler(
    IVisitorRepository visitorRepository,
    IUserRepository userRepository) : IRequestHandler<UpdateVisitorCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;
    private readonly IUserRepository _userRepository = userRepository;
=======
public class UpdateVisitorCommandHandler(IVisitorRepository visitorRepository)
    : IRequestHandler<UpdateVisitorCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)

    public async Task<Unit> Handle(UpdateVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepository.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException($"Visitor with ID {request.VisitorId} not found");

<<<<<<< HEAD
        var user = await _userRepository.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException($"User with ID {request.VisitorId} not found");

        // Update user information
        if (request.DisplayName != null)
            user.DisplayName = request.DisplayName;
        if (request.PhoneNumber != null)
            user.PhoneNumber = request.PhoneNumber;
        if (request.BirthDate.HasValue)
            user.BirthDate = request.BirthDate;
        if (request.Gender.HasValue)
            user.Gender = request.Gender;

        user.UpdatedAt = DateTime.UtcNow;

        // Update visitor information
        if (request.VisitorType.HasValue)
            visitor.VisitorType = request.VisitorType.Value;
        if (request.Height.HasValue)
            visitor.Height = request.Height.Value;
        if (request.Points.HasValue)
            visitor.Points = request.Points.Value;
        if (request.MemberLevel != null)
            visitor.MemberLevel = request.MemberLevel;

        visitor.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _visitorRepository.UpdateAsync(visitor);
=======
        if (request.Height < 50 || request.Height > 300)
        {
            throw new ArgumentException("Height must be between 50 and 300 cm", nameof(request));
        }

        visitor.Height = request.Height;
        visitor.UpdatedAt = DateTime.UtcNow;

        await _visitorRepository.UpdateAsync(visitor);

>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
        return Unit.Value;
    }
}

/// <summary>
<<<<<<< HEAD
/// Handler for updating visitor blacklist status.
/// </summary>
public class UpdateVisitorBlacklistStatusCommandHandler(IVisitorRepository visitorRepository) : IRequestHandler<UpdateVisitorBlacklistStatusCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<Unit> Handle(UpdateVisitorBlacklistStatusCommand request, CancellationToken cancellationToken)
=======
/// Handler for blacklisting a visitor.
/// </summary>
public class BlacklistVisitorCommandHandler(IVisitorRepository visitorRepository)
    : IRequestHandler<BlacklistVisitorCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<Unit> Handle(BlacklistVisitorCommand request, CancellationToken cancellationToken)
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
    {
        var visitor = await _visitorRepository.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException($"Visitor with ID {request.VisitorId} not found");

<<<<<<< HEAD
        visitor.IsBlacklisted = request.IsBlacklisted;
        visitor.UpdatedAt = DateTime.UtcNow;

        await _visitorRepository.UpdateAsync(visitor);
=======
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

>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
        return Unit.Value;
    }
}

/// <summary>
/// Handler for deleting a visitor.
/// </summary>
<<<<<<< HEAD
public class DeleteVisitorCommandHandler(
    IVisitorRepository visitorRepository,
    IUserRepository userRepository) : IRequestHandler<DeleteVisitorCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;
    private readonly IUserRepository _userRepository = userRepository;
=======
public class DeleteVisitorCommandHandler(IVisitorRepository visitorRepository)
    : IRequestHandler<DeleteVisitorCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)

    public async Task<Unit> Handle(DeleteVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepository.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException($"Visitor with ID {request.VisitorId} not found");

<<<<<<< HEAD
        var user = await _userRepository.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException($"User with ID {request.VisitorId} not found");

        await _visitorRepository.DeleteAsync(visitor);
        await _userRepository.DeleteAsync(user);
=======
        await _visitorRepository.DeleteAsync(visitor);

>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
        return Unit.Value;
    }
}
