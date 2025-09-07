using DbApp.Domain.Enums.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Visitors;

/// <summary>
/// Command to create a new visitor.
/// Email and phone number are both optional for visitor creation.
/// </summary>
public record CreateVisitorCommand(
    string Username,
    string PasswordHash,
    string? Email,
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
/// Command to update visitor contact information.
/// </summary>
public record UpdateVisitorContactCommand(
    int VisitorId,
    string? Email,
    string? PhoneNumber
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
    int Points,
    string? Reason = null
) : IRequest<Unit>;

/// <summary>
/// Command to deduct points from a visitor.
/// </summary>
public record DeductPointsCommand(
    int Points,
    string? Reason = null
) : IRequest<Unit>;

/// <summary>
/// Command to add points to a specific visitor by ID.
/// Only members can earn points.
/// </summary>
public record AddPointsToVisitorCommand(
    int VisitorId,
    int Points,
    string? Reason = null
) : IRequest<Unit>;

/// <summary>
/// Command to deduct points from a specific visitor by ID.
/// Only members can have points deducted.
/// </summary>
public record DeductPointsFromVisitorCommand(
    int VisitorId,
    int Points,
    string? Reason = null
) : IRequest<Unit>;

/// <summary>
/// Command to add points to a visitor by email or phone number.
/// Only members can earn points.
/// </summary>
public record AddPointsByContactCommand(
    string? Email = null,
    string? PhoneNumber = null,
    int Points = 0,
    string? Reason = null
) : IRequest<Unit>;

/// <summary>
/// Command to deduct points from a visitor by email or phone number.
/// Only members can have points deducted.
/// </summary>
public record DeductPointsByContactCommand(
    string? Email = null,
    string? PhoneNumber = null,
    int Points = 0,
    string? Reason = null
) : IRequest<Unit>;
