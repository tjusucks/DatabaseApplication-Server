using DbApp.Domain.Entities.UserSystem;

namespace DbApp.Domain.Interfaces.UserSystem;

/// <summary>
/// Repository interface for User entity operations.
/// Provides CRUD and search functionality for users.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Creates a new user record.
    /// </summary>
    Task<int> CreateAsync(User user);

    /// <summary>
    /// Gets a user by their unique ID.
    /// </summary>
    Task<User?> GetByIdAsync(int userId);

    /// <summary>
    /// Gets all users.
    /// </summary>
    Task<List<User>> GetAllAsync();

    /// <summary>
    /// Updates a user record.
    /// </summary>
    Task UpdateAsync(User user);

    /// <summary>
    /// Deletes a user record.
    /// </summary>
    Task DeleteAsync(User user);
}
