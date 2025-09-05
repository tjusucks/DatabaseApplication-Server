using DbApp.Domain.Enums.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Visitors;

/// <summary>
/// Command to create a new visitor.
/// </summary>
public record CreateVisitorCommand(
    string Username,
    string PasswordHash,
    string Email,
    string DisplayName,
    string? PhoneNumber,
    DateTime? BirthDate,
    Gender? Gender,
    VisitorType VisitorType,
    int Height
) : IRequest<int>;

/// <summary>
/// Command to update an existing visitor.
/// </summary>
public record UpdateVisitorCommand(
    int VisitorId,
    string? DisplayName,
    string? PhoneNumber,
    DateTime? BirthDate,
    Gender? Gender,
    VisitorType? VisitorType,
    int? Height
) : IRequest<Unit>;

/// <summary>
/// Command to delete a visitor.
/// </summary>
public record DeleteVisitorCommand(int VisitorId) : IRequest<Unit>;

/// <summary>
/// Command to blacklist a visitor (requires reason).
/// </summary>
public record BlacklistVisitorCommand(
    int VisitorId,
    string Reason
) : IRequest<Unit>;

/// <summary>
/// Command to remove a visitor from blacklist.
/// </summary>
public record UnblacklistVisitorCommand(int VisitorId) : IRequest<Unit>;

/// <summary>
/// Command to upgrade a visitor to member status.
/// </summary>
public record UpgradeToMemberCommand(int VisitorId) : IRequest<Unit>;

/// <summary>
/// Command to remove membership from a visitor.
/// </summary>
public record RemoveMembershipCommand(int VisitorId) : IRequest<Unit>;

/// <summary>
/// Command to add points to a visitor.
/// </summary>
public record AddPointsCommand(
    int VisitorId,
    int Points
) : IRequest<Unit>;

/// <summary>
/// Command to deduct points from a visitor.
/// </summary>
public record DeductPointsCommand(
    int VisitorId,
    int Points
) : IRequest<Unit>;
