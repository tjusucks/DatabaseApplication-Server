<<<<<<< HEAD
using DbApp.Domain.Enums.UserSystem;
=======
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
using MediatR;

namespace DbApp.Application.UserSystem.Visitors;

/// <summary>
<<<<<<< HEAD
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
=======
/// Command to register a new visitor.
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
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)

/// <summary>
/// Command to update visitor information.
/// </summary>
<<<<<<< HEAD
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
=======
/// <param name="VisitorId">The visitor ID.</param>
/// <param name="Height">The new height.</param>
public record UpdateVisitorCommand(int VisitorId, int Height) : IRequest<Unit>;

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
public record RemoveFromBlacklistCommand(int VisitorId) : IRequest<Unit>;
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)

/// <summary>
/// Command to delete a visitor.
/// </summary>
/// <param name="VisitorId">The visitor ID to delete.</param>
public record DeleteVisitorCommand(int VisitorId) : IRequest<Unit>;
