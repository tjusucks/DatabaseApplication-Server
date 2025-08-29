using DbApp.Domain.Entities.UserSystem;

namespace DbApp.Domain.Interfaces.UserSystem;

public interface IEmployeeRepository
{
    Task<int> CreateAsync(Employee employee);
    Task<Employee?> GetByIdAsync(int employeeId);
    Task<List<Employee>> GetAllAsync();
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(Employee employee);
}
