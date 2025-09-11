using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Repositories.UserSystem;

public class EmployeeRepository(ApplicationDbContext dbContext) : IEmployeeRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<int> CreateAsync(Employee employee)
    {
        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync();
        return employee.EmployeeId;
    }

    public async Task<int> CreateUserAsync(User user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user.UserId;
    }

    public async Task<Employee?> GetByIdAsync(int employeeId)
    {
        return await _dbContext.Employees.Include(e => e.User).FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
    }

    public async Task<List<Employee>> GetAllAsync()
    {
        return await _dbContext.Employees.Include(e => e.User).ToListAsync();
    }

    public async Task UpdateAsync(Employee employee)
    {
        _dbContext.Employees.Update(employee);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Employee employee)
    {
        // 首先查找对应的User实体
        var user = await _dbContext.Users.FindAsync(employee.EmployeeId);

        // 删除Employee实体
        _dbContext.Employees.Remove(employee);

        // 如果找到对应的User实体，则一并删除
        if (user != null)
        {
            _dbContext.Users.Remove(user);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Employee>> SearchAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return await GetAllAsync();
        }

        var employees = await _dbContext.Employees
            .Include(e => e.User)
            .Where(e =>
                (e.StaffNumber != null && e.StaffNumber.Contains(keyword)) ||
                (e.Position != null && e.Position.Contains(keyword)) ||
                (e.DepartmentName != null && e.DepartmentName.Contains(keyword)) ||
                (e.Certification != null && e.Certification.Contains(keyword)) ||
                (e.ResponsibilityArea != null && e.ResponsibilityArea.Contains(keyword)) ||
                (e.User.DisplayName != null && e.User.DisplayName.Contains(keyword)))
            .ToListAsync();
        return employees;
    }

    public async Task<List<Employee>> GetByDepartmentAsync(string departmentName)
    {
        if (string.IsNullOrWhiteSpace(departmentName))
        {
            return await GetAllAsync();
        }

        var employees = await _dbContext.Employees
            .Include(e => e.User)
            .Where(e => e.DepartmentName != null && e.DepartmentName.Equals(departmentName))
            .ToListAsync();
        return employees;
    }

    public async Task<List<Employee>> GetByStaffTypeAsync(StaffType staffType)
    {
        var employees = await _dbContext.Employees
            .Include(e => e.User)
            .Where(e => e.StaffType == staffType)
            .ToListAsync();
        return employees;
    }

    public async Task<bool> IsValidRoleAsync(int roleId, string[] validRoleNames)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleId == roleId);
        return role != null && validRoleNames.Contains(role.RoleName);
    }

    public async Task<int?> GetRoleIdByNameAsync(string roleName)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
        return role?.RoleId;
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user != null)
        {
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
