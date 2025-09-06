using DbApp.Domain.Constants.UserSystem;

namespace DbApp.Domain.Interfaces.UserSystem;

public interface IMembershipService
{
    Task AddPointsAsync(int visitorId, int points);
    Task DeductPointsAsync(int visitorId, int points);
}
