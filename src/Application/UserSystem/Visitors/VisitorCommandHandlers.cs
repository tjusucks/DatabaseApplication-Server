using DbApp.Domain.Constants.UserSystem;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using MediatR;
using static DbApp.Domain.Exceptions;

namespace DbApp.Application.UserSystem.Visitors;

/// <summary>
/// Centralized handler for all visitor commands.
/// </summary>
public class VisitorCommandHandlers(IVisitorRepository visitorRepo, IMembershipService membershipService) :
    IRequestHandler<CreateVisitorCommand, int>,
    IRequestHandler<UpdateVisitorCommand, Unit>,
    IRequestHandler<UpdateVisitorContactCommand, Unit>,
    IRequestHandler<DeleteVisitorCommand, Unit>,
    IRequestHandler<BlacklistVisitorCommand, Unit>,
    IRequestHandler<UnblacklistVisitorCommand, Unit>,
    IRequestHandler<UpgradeToMemberCommand, Unit>,
    IRequestHandler<RemoveMembershipCommand, Unit>,
    IRequestHandler<AddPointsCommand, Unit>,
    IRequestHandler<DeductPointsCommand, Unit>,
    IRequestHandler<AddPointsToVisitorCommand, Unit>,
    IRequestHandler<DeductPointsFromVisitorCommand, Unit>,
    IRequestHandler<AddPointsByContactCommand, Unit>,
    IRequestHandler<DeductPointsByContactCommand, Unit>
{
    private readonly IVisitorRepository _visitorRepo = visitorRepo;
    private readonly IMembershipService _membershipService = membershipService;

    public async Task<int> Handle(CreateVisitorCommand request, CancellationToken cancellationToken)
    {
        // Visitors can be created without email or phone number
        // Contact information is only required for member upgrade
        var visitor = new Visitor
        {
            User = new User
            {
                Username = request.Username,
                PasswordHash = request.PasswordHash,
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email,
                DisplayName = request.DisplayName,
                PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber,
                BirthDate = request.BirthDate,
                Gender = request.Gender,
                RegisterTime = DateTime.UtcNow,
                PermissionLevel = 1,
                RoleId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            VisitorType = request.VisitorType,
            Points = 0,
            IsBlacklisted = false,
            Height = request.Height,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        return await _visitorRepo.CreateAsync(visitor);
    }

    public async Task<Unit> Handle(UpdateVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepo.GetByIdAsync(request.VisitorId)
            ?? throw new NotFoundException($"Visitor {request.VisitorId} not found.");

        if (request.DisplayName != null)
        {
            visitor.User.DisplayName = request.DisplayName;
        }
        if (request.PhoneNumber != null)
        {
            visitor.User.PhoneNumber = request.PhoneNumber;
        }
        if (request.BirthDate.HasValue)
        {
            visitor.User.BirthDate = request.BirthDate;
        }
        if (request.Gender.HasValue)
        {
            visitor.User.Gender = request.Gender;
        }
        if (request.VisitorType.HasValue)
        {
            visitor.VisitorType = request.VisitorType.Value;
        }
        if (request.Height.HasValue)
        {
            visitor.Height = request.Height.Value;
        }

        await _visitorRepo.UpdateAsync(visitor);
        return Unit.Value;
    }

    public async Task<Unit> Handle(UpdateVisitorContactCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepo.GetByIdAsync(request.VisitorId)
            ?? throw new NotFoundException($"Visitor {request.VisitorId} not found.");

        // Update contact information
        if (request.Email != null)
        {
            visitor.User.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email;
        }

        if (request.PhoneNumber != null)
        {
            visitor.User.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber;
        }

        // If visitor is a member, validate that at least one contact method remains
        if (visitor.VisitorType == Domain.Enums.UserSystem.VisitorType.Member &&
            !visitor.User.HasContactInformation())
        {
            throw new ValidationException("Members must have at least one contact method (email or phone)");
        }

        visitor.User.UpdatedAt = DateTime.UtcNow;
        visitor.UpdatedAt = DateTime.UtcNow;
        await _visitorRepo.UpdateAsync(visitor);
        return Unit.Value;
    }

    public async Task<Unit> Handle(DeleteVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepo.GetByIdAsync(request.VisitorId)
            ?? throw new NotFoundException($"Visitor {request.VisitorId} not found.");

        await _visitorRepo.DeleteAsync(visitor);
        return Unit.Value;
    }

    public async Task<Unit> Handle(BlacklistVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepo.GetByIdAsync(request.VisitorId)
            ?? throw new NotFoundException($"Visitor {request.VisitorId} not found.");

        visitor.IsBlacklisted = true;
        await _visitorRepo.UpdateAsync(visitor);
        return Unit.Value;
    }

    public async Task<Unit> Handle(UnblacklistVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepo.GetByIdAsync(request.VisitorId)
            ?? throw new NotFoundException($"Visitor {request.VisitorId} not found.");

        visitor.IsBlacklisted = false;
        await _visitorRepo.UpdateAsync(visitor);
        return Unit.Value;
    }

    public async Task<Unit> Handle(UpgradeToMemberCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepo.GetByIdAsync(request.VisitorId)
            ?? throw new NotFoundException($"Visitor {request.VisitorId} not found.");

        // Check if visitor is blacklisted
        if (visitor.IsBlacklisted)
        {
            throw new ValidationException("Cannot upgrade blacklisted visitor to member");
        }

        // Check if user has contact information (email or phone)
        if (!visitor.User.IsEligibleForMemberUpgrade())
        {
            throw new ValidationException("Email or phone number is required to become a member");
        }

        visitor.VisitorType = Domain.Enums.UserSystem.VisitorType.Member;
        visitor.MemberLevel = MembershipConstants.LevelNames.Bronze;
        visitor.MemberSince = DateTime.UtcNow;
        visitor.UpdatedAt = DateTime.UtcNow;
        await _visitorRepo.UpdateAsync(visitor);
        return Unit.Value;
    }

    public async Task<Unit> Handle(RemoveMembershipCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepo.GetByIdAsync(request.VisitorId)
            ?? throw new NotFoundException($"Visitor {request.VisitorId} not found.");

        visitor.VisitorType = Domain.Enums.UserSystem.VisitorType.Regular;
        visitor.MemberLevel = null;
        visitor.MemberSince = null;
        visitor.Points = 0;
        visitor.UpdatedAt = DateTime.UtcNow;
        await _visitorRepo.UpdateAsync(visitor);
        return Unit.Value;
    }

    public Task<Unit> Handle(AddPointsCommand request, CancellationToken cancellationToken)
    {
        // This method is kept for backward compatibility but should not be used directly
        throw new NotSupportedException("Use AddPointsToVisitorCommand or AddPointsByContactCommand instead");
    }

    public Task<Unit> Handle(DeductPointsCommand request, CancellationToken cancellationToken)
    {
        // This method is kept for backward compatibility but should not be used directly
        throw new NotSupportedException("Use DeductPointsFromVisitorCommand or DeductPointsByContactCommand instead");
    }

    public async Task<Unit> Handle(AddPointsToVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepo.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException("Visitor not found.");

        if (visitor.VisitorType != Domain.Enums.UserSystem.VisitorType.Member)
            throw new InvalidOperationException("Only members can earn points.");

        await _membershipService.AddPointsAsync(request.VisitorId, request.Points);
        return Unit.Value;
    }

    public async Task<Unit> Handle(DeductPointsFromVisitorCommand request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepo.GetByIdAsync(request.VisitorId)
            ?? throw new InvalidOperationException("Visitor not found.");

        if (visitor.VisitorType != Domain.Enums.UserSystem.VisitorType.Member)
            throw new InvalidOperationException("Only members can have points deducted.");

        await _membershipService.DeductPointsAsync(request.VisitorId, request.Points);
        return Unit.Value;
    }

    public async Task<Unit> Handle(AddPointsByContactCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.PhoneNumber))
            throw new ArgumentException("Either email or phone number must be provided.");

        var visitor = await FindVisitorByContactAsync(request.Email, request.PhoneNumber);

        if (visitor.VisitorType != Domain.Enums.UserSystem.VisitorType.Member)
            throw new InvalidOperationException("Only members can earn points.");

        await _membershipService.AddPointsAsync(visitor.VisitorId, request.Points);
        return Unit.Value;
    }

    public async Task<Unit> Handle(DeductPointsByContactCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.PhoneNumber))
            throw new ArgumentException("Either email or phone number must be provided.");

        var visitor = await FindVisitorByContactAsync(request.Email, request.PhoneNumber);

        if (visitor.VisitorType != Domain.Enums.UserSystem.VisitorType.Member)
            throw new InvalidOperationException("Only members can have points deducted.");

        await _membershipService.DeductPointsAsync(visitor.VisitorId, request.Points);
        return Unit.Value;
    }

    private async Task<Domain.Entities.UserSystem.Visitor> FindVisitorByContactAsync(string? email, string? phoneNumber)
    {
        Domain.Entities.UserSystem.Visitor? visitor = null;

        if (!string.IsNullOrWhiteSpace(email))
        {
            var spec = new Domain.Specifications.Common.PaginatedSpec<Domain.Specifications.UserSystem.VisitorSpec>
            {
                InnerSpec = new Domain.Specifications.UserSystem.VisitorSpec { Email = email },
                Page = 1,
                PageSize = 1
            };
            var visitors = await _visitorRepo.SearchAsync(spec);
            visitor = visitors.FirstOrDefault();
        }

        if (visitor == null && !string.IsNullOrWhiteSpace(phoneNumber))
        {
            var spec = new Domain.Specifications.Common.PaginatedSpec<Domain.Specifications.UserSystem.VisitorSpec>
            {
                InnerSpec = new Domain.Specifications.UserSystem.VisitorSpec { PhoneNumber = phoneNumber },
                Page = 1,
                PageSize = 1
            };
            var visitors = await _visitorRepo.SearchAsync(spec);
            visitor = visitors.FirstOrDefault();
        }

        return visitor ?? throw new InvalidOperationException("Visitor not found with the provided contact information.");
    }
}
