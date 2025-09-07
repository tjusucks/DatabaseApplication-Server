using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
namespace DbApp.Domain.Interfaces.UserSystem;

public interface IEmployeeRepository
{
    Task<int> CreateAsync(Employee employee);
    Task<int> CreateUserAsync(User user);
    Task<Employee?> GetByIdAsync(int employeeId);
    Task<List<Employee>> GetAllAsync();
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(Employee employee);
    Task<List<Employee>> SearchAsync(string keyword);
    Task<List<Employee>> GetByDepartmentAsync(string departmentName);
    Task<List<Employee>> GetByStaffTypeAsync(StaffType staffType);
    Task<bool> IsValidRoleAsync(int roleId, string[] validRoleNames);
    Task<int?> GetRoleIdByNameAsync(string roleName);
    Task DeleteUserAsync(int userId);
}
