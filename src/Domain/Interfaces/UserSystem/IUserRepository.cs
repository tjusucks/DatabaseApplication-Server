using DbApp.Domain.Entities.UserSystem;

namespace DbApp.Domain.Interfaces.UserSystem;

public interface IUserRepository
{
    Task<int> CreateAsync(User user);
    Task<User?> GetByIdAsync(int userId);
    Task<List<User>> GetAllAsync();
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
}
