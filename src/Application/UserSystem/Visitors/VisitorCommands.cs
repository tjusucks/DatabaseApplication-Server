using MediatR;

namespace DbApp.Application.UserSystem.Visitors;

/// <summary>
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

/// <summary>
/// Command to update visitor information.
/// </summary>
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

/// <summary>
/// Command to delete a visitor.
/// </summary>
/// <param name="VisitorId">The visitor ID to delete.</param>
public record DeleteVisitorCommand(int VisitorId) : IRequest<Unit>;
