using DbApp.Domain.Entities.UserSystem;
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
    }
}

/// <summary>
/// Handler for updating visitor information.
/// </summary>
public class UpdateVisitorCommandHandler(
    IVisitorRepository visitorRepository,
    IUserRepository userRepository) : IRequestHandler<UpdateVisitorCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Unit> Handle(UpdateVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepository.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException($"Visitor with ID {request.VisitorId} not found");

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
