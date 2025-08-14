using DbApp.Domain.Entities;

namespace DbApp.Domain.Interfaces;

public interface IUserRepository
{
    Task<int> CreateAsync(User user);
    Task<User?> GetByIdAsync(int userId);
    Task<List<User>> GetAllAsync();
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
}

