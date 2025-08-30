using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using DbApp.Domain.Enums.UserSystem;
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

    public async Task<Employee?> GetByIdAsync(int employeeId)
    {
        return await _dbContext.Employees.FindAsync(employeeId);
    }

    public async Task<List<Employee>> GetAllAsync()
    {
        return await _dbContext.Employees.ToListAsync();
    }

    public async Task UpdateAsync(Employee employee)
    {
        _dbContext.Employees.Update(employee);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Employee employee)
    {
        _dbContext.Employees.Remove(employee);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<List<Employee>> SearchAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return await GetAllAsync();
        }

        var employees = await _dbContext.Employees
            .Where(e => e.DepartmentName != null && e.DepartmentName.Contains(keyword))
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
            .Where(e => e.DepartmentName != null && e.DepartmentName.Equals(departmentName))
            .ToListAsync();
        return employees;
    } 
    public async Task<List<Employee>> GetByStaffTypeAsync(StaffType staffType)
    {
        var employees = await _dbContext.Employees
            .Where(e => e.StaffType == staffType)
            .ToListAsync();
        return employees;
    }
}
