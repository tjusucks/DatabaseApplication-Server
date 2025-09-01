using DbApp.Domain.Enums.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Visitors;

// === 队友的基础访客管理功能 ===

/// <summary>
/// Command to create a new visitor with complete user information.
/// </summary>
/// <param name="Username">Unique username for the visitor.</param>
/// <param name="Email">Email address for communication.</param>
/// <param name="DisplayName">Display name for the visitor.</param>
/// <param name="PhoneNumber">Phone number for contact.</param>
/// <param name="BirthDate">Birth date for age verification.</param>
/// <param name="Gender">Gender information.</param>
/// <param name="VisitorType">Type of visitor (regular or member).</param>
/// <param name="Height">Height in centimeters for ride restrictions.</param>
/// <param name="PasswordHash">Hashed password for authentication.</param>
public record CreateVisitorCommand(
    string Username,
    string Email,
    string DisplayName,
    string? PhoneNumber,
    DateTime? BirthDate,
    Gender? Gender,
    VisitorType VisitorType,
    int Height,
    string PasswordHash
) : IRequest<int>;

// === 您的会员和积分管理功能 ===

/// <summary>
/// Command to register a new visitor based on existing user.
/// </summary>
/// <param name="UserId">The user ID to create visitor for.</param>
/// <param name="Height">The visitor's height in centimeters.</param>
public record RegisterVisitorCommand(int UserId, int Height) : IRequest<int>;

/// <summary>
/// Command to upgrade a visitor to member status.
/// </summary>
/// <param name="VisitorId">The visitor ID to upgrade.</param>
public record UpgradeToMemberCommand(int VisitorId) : IRequest<Unit>;

/// <summary>
/// Command to add points to a visitor's account.
/// </summary>
/// <param name="VisitorId">The visitor ID.</param>
/// <param name="Points">The points to add.</param>
/// <param name="Reason">The reason for adding points.</param>
public record AddPointsCommand(int VisitorId, int Points, string Reason) : IRequest<Unit>;

/// <summary>
/// Command to deduct points from a visitor's account.
/// </summary>
/// <param name="VisitorId">The visitor ID.</param>
/// <param name="Points">The points to deduct.</param>
/// <param name="Reason">The reason for deducting points.</param>
public record DeductPointsCommand(int VisitorId, int Points, string Reason) : IRequest<bool>;

/// <summary>
/// Command to update visitor information.
/// </summary>
/// <param name="VisitorId">The visitor ID to update.</param>
/// <param name="DisplayName">Updated display name.</param>
/// <param name="PhoneNumber">Updated phone number.</param>
/// <param name="BirthDate">Updated birth date.</param>
/// <param name="Gender">Updated gender.</param>
/// <param name="VisitorType">Updated visitor type.</param>
/// <param name="Height">Updated height.</param>
/// <param name="Points">Updated membership points.</param>
/// <param name="MemberLevel">Updated member level.</param>
public record UpdateVisitorCommand(
    int VisitorId,
    string? DisplayName,
    string? PhoneNumber,
    DateTime? BirthDate,
    Gender? Gender,
    VisitorType? VisitorType,
    int? Height,
    int? Points,
    string? MemberLevel
) : IRequest<Unit>;

/// <summary>
/// Command to update visitor blacklist status.
/// </summary>
/// <param name="VisitorId">The visitor ID.</param>
/// <param name="IsBlacklisted">Whether the visitor should be blacklisted.</param>
public record UpdateVisitorBlacklistStatusCommand(
    int VisitorId,
    bool IsBlacklisted
) : IRequest<Unit>;

/// <summary>
/// Command to blacklist a visitor.
/// </summary>
/// <param name="VisitorId">The visitor ID to blacklist.</param>
/// <param name="Reason">The reason for blacklisting.</param>
public record BlacklistVisitorCommand(int VisitorId, string Reason) : IRequest<Unit>;

/// <summary>
/// Command to remove a visitor from blacklist.
/// </summary>
/// <param name="VisitorId">The visitor ID to remove from blacklist.</param>
public record UnblacklistVisitorCommand(int VisitorId) : IRequest<Unit>;

/// <summary>
/// Command to delete a visitor.
/// </summary>
/// <param name="VisitorId">The visitor ID to delete.</param>
public record DeleteVisitorCommand(int VisitorId) : IRequest<Unit>;
