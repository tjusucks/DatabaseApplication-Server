using DbApp.Domain.Entities.UserSystem;
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
}
