using DbApp.Domain.Enums.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Visitors;

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
/// Command to delete a visitor.
/// </summary>
/// <param name="VisitorId">The visitor ID to delete.</param>
public record DeleteVisitorCommand(int VisitorId) : IRequest<Unit>;
