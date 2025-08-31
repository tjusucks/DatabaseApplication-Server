using DbApp.Domain.Entities.UserSystem;

namespace DbApp.Domain.Interfaces.UserSystem;

/// <summary>
/// Repository interface for Visitor entity operations.
/// </summary>
public interface IVisitorRepository
{
    /// <summary>
    /// Creates a new visitor.
    /// </summary>
    /// <param name="visitor">The visitor to create.</param>
    /// <returns>The ID of the created visitor.</returns>
    Task<int> CreateAsync(Visitor visitor);

    /// <summary>
    /// Gets a visitor by ID.
    /// </summary>
    /// <param name="visitorId">The visitor ID.</param>
    /// <returns>The visitor if found, null otherwise.</returns>
    Task<Visitor?> GetByIdAsync(int visitorId);

    /// <summary>
    /// Gets all visitors.
    /// </summary>
    /// <returns>List of all visitors.</returns>
    Task<List<Visitor>> GetAllAsync();

    /// <summary>
    /// Updates a visitor.
    /// </summary>
    /// <param name="visitor">The visitor to update.</param>
    Task UpdateAsync(Visitor visitor);

    /// <summary>
    /// Deletes a visitor.
    /// </summary>
    /// <param name="visitor">The visitor to delete.</param>
    Task DeleteAsync(Visitor visitor);
}
