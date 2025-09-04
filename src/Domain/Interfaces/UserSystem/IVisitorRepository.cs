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

    // === 基础CRUD操作 ===

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

    // === 队友的搜索和筛选功能 ===

    /// <summary>
    /// Searches visitors by name (username or display name).
    /// </summary>
    /// <param name="name">The name to search for (partial match).</param>
    /// <returns>List of matching visitors.</returns>
    Task<List<Visitor>> SearchByNameAsync(string name);

    /// <summary>
    /// Searches visitors by phone number.
    /// </summary>
    /// <param name="phoneNumber">The phone number to search for.</param>
    /// <returns>List of matching visitors.</returns>
    Task<List<Visitor>> SearchByPhoneNumberAsync(string phoneNumber);

    /// <summary>
    /// Gets visitors by blacklist status.
    /// </summary>
    /// <param name="isBlacklisted">Whether to get blacklisted or non-blacklisted visitors.</param>
    /// <returns>List of visitors with the specified blacklist status.</returns>
    Task<List<Visitor>> GetByBlacklistStatusAsync(bool isBlacklisted);

    /// <summary>
    /// Gets visitors by visitor type.
    /// </summary>
    /// <param name="visitorType">The visitor type to filter by.</param>
    /// <returns>List of visitors of the specified type.</returns>
    Task<List<Visitor>> GetByTypeAsync(VisitorType visitorType);

    /// <summary>
    /// Gets visitors registered within a date range.
    /// </summary>
    /// <param name="startDate">Start date for registration.</param>
    /// <param name="endDate">End date for registration.</param>
    /// <returns>List of visitors registered within the date range.</returns>
    Task<List<Visitor>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Searches visitors with multiple criteria.
    /// </summary>
    /// <param name="name">Optional name filter.</param>
    /// <param name="phoneNumber">Optional phone number filter.</param>
    /// <param name="isBlacklisted">Optional blacklist status filter.</param>
    /// <param name="visitorType">Optional visitor type filter.</param>
    /// <param name="startDate">Optional start date for registration.</param>
    /// <param name="endDate">Optional end date for registration.</param>
    /// <returns>List of visitors matching the criteria.</returns>
    Task<List<Visitor>> SearchAsync(
        string? name = null,
        string? phoneNumber = null,
        bool? isBlacklisted = null,
        VisitorType? visitorType = null,
        DateTime? startDate = null,
        DateTime? endDate = null);

    // === 您的会员和积分功能 ===

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

    /// <summary>
    /// Updates visitor information avoiding EF Core tracking conflicts.
    /// </summary>
    /// <param name="visitorId">The visitor ID.</param>
    /// <param name="displayName">Optional display name update.</param>
    /// <param name="phoneNumber">Optional phone number update.</param>
    /// <param name="birthDate">Optional birth date update.</param>
    /// <param name="gender">Optional gender update.</param>
    /// <param name="visitorType">Optional visitor type update.</param>
    /// <param name="height">Optional height update.</param>
    /// <param name="points">Optional points update.</param>
    /// <param name="memberLevel">Optional member level update.</param>
    Task UpdateVisitorInfoAsync(
        int visitorId,
        string? displayName = null,
        string? phoneNumber = null,
        DateTime? birthDate = null,
        Gender? gender = null,
        VisitorType? visitorType = null,
        int? height = null,
        int? points = null,
        string? memberLevel = null);

    /// <summary>
    /// Gets the count of visitors matching search criteria.
    /// </summary>
    Task<int> GetSearchCountAsync(
        string? keyword = null,
        VisitorType? visitorType = null,
        string? memberLevel = null,
        bool? isBlacklisted = null,
        int? minPoints = null,
        int? maxPoints = null,
        DateTime? startDate = null,
        DateTime? endDate = null);

    /// <summary>
    /// Searches visitors with pagination support.
    /// </summary>
    Task<List<Visitor>> SearchWithPaginationAsync(
        string? keyword = null,
        VisitorType? visitorType = null,
        string? memberLevel = null,
        bool? isBlacklisted = null,
        int? minPoints = null,
        int? maxPoints = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int page = 1,
        int pageSize = 20);
}
