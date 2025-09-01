using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;

namespace DbApp.Domain.Interfaces.UserSystem;

/// <summary>
/// Repository interface for Visitor entity operations.
/// </summary>
public interface IVisitorRepository
{
    /// <summary>
    /// Creates a new visitor record.
    /// </summary>
    /// <param name="visitor">The visitor entity to create.</param>
    /// <returns>The created visitor ID.</returns>
    Task<int> CreateAsync(Visitor visitor);

    /// <summary>
    /// Gets a visitor by their ID.
    /// </summary>
    /// <param name="visitorId">The visitor ID.</param>
    /// <returns>The visitor entity or null if not found.</returns>
    Task<Visitor?> GetByIdAsync(int visitorId);

    /// <summary>
    /// Gets a visitor by their user ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The visitor entity or null if not found.</returns>
    Task<Visitor?> GetByUserIdAsync(int userId);

    /// <summary>
    /// Gets all visitors.
    /// </summary>
    /// <returns>List of all visitors.</returns>
    Task<List<Visitor>> GetAllAsync();

    /// <summary>
    /// Gets visitors by type.
    /// </summary>
    /// <param name="visitorType">The visitor type to filter by.</param>
    /// <returns>List of visitors of the specified type.</returns>
    Task<List<Visitor>> GetByTypeAsync(VisitorType visitorType);

    /// <summary>
    /// Gets visitors by member level.
    /// </summary>
    /// <param name="memberLevel">The member level to filter by.</param>
    /// <returns>List of visitors with the specified member level.</returns>
    Task<List<Visitor>> GetByMemberLevelAsync(string memberLevel);

    /// <summary>
    /// Gets visitors with points in a specific range.
    /// </summary>
    /// <param name="minPoints">Minimum points.</param>
    /// <param name="maxPoints">Maximum points.</param>
    /// <returns>List of visitors with points in the specified range.</returns>
    Task<List<Visitor>> GetByPointsRangeAsync(int minPoints, int maxPoints);

    /// <summary>
    /// Updates a visitor record.
    /// </summary>
    /// <param name="visitor">The visitor entity to update.</param>
    Task UpdateAsync(Visitor visitor);

    /// <summary>
    /// Deletes a visitor record.
    /// </summary>
    /// <param name="visitor">The visitor entity to delete.</param>
    Task DeleteAsync(Visitor visitor);

    /// <summary>
    /// Adds points to a visitor's account.
    /// </summary>
    /// <param name="visitorId">The visitor ID.</param>
    /// <param name="points">The points to add.</param>
    Task AddPointsAsync(int visitorId, int points);

    /// <summary>
    /// Deducts points from a visitor's account.
    /// </summary>
    /// <param name="visitorId">The visitor ID.</param>
    /// <param name="points">The points to deduct.</param>
    /// <returns>True if successful, false if insufficient points.</returns>
    Task<bool> DeductPointsAsync(int visitorId, int points);
}
