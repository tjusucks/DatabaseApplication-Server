using DbApp.Domain.Interfaces.UserSystem;
using DbApp.Domain.Constants.UserSystem;

namespace DbApp.Infrastructure.Services.UserSystem;

public class MembershipService(IVisitorRepository visitorRepo) : IMembershipService
{
    private readonly IVisitorRepository _visitorRepo = visitorRepo;
    public async Task AddPointsAsync(int visitorId, int points)
    {
        var visitor = await _visitorRepo.GetByIdAsync(visitorId)
            ?? throw new InvalidOperationException("Visitor not found.");
        visitor.Points += points;
        visitor.MemberLevel = MembershipConstants.GetLevelByPoints(visitor.Points);

        await _visitorRepo.UpdateAsync(visitor);
    }

    public async Task DeductPointsAsync(int visitorId, int points)
    {
        var visitor = await _visitorRepo.GetByIdAsync(visitorId)
            ?? throw new InvalidOperationException("Visitor not found.");
        visitor.Points = Math.Max(0, visitor.Points - points);
        visitor.MemberLevel = MembershipConstants.GetLevelByPoints(visitor.Points);

        await _visitorRepo.UpdateAsync(visitor);
    }
}
