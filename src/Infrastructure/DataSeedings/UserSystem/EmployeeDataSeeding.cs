using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.DataSeedings.UserSystem;

public class EmployeeDataSeeding : IDataSeeding
{
    public void Seed(DbContext dbContext)
    {
        // 检查是否已经有Employee数据
        if (dbContext.Set<Employee>().Any())
        {
            return; // Data already seeded.
        }

        // 只为管理员和经理角色的用户创建Employee记录
        var adminAndManagerUsers = dbContext.Set<User>()
            .Where(u => u.RoleId == 3 || u.RoleId == 4) // Manager or Admin
            .ToList();

        if (adminAndManagerUsers.Any())
        {
            var employees = GetEmployeesForUsers(adminAndManagerUsers);
            dbContext.Set<Employee>().AddRange(employees);
            dbContext.SaveChanges();
        }
    }

    public async Task SeedAsync(DbContext dbContext)
    {
        // 检查是否已经有Employee数据
        if (await dbContext.Set<Employee>().AnyAsync())
        {
            return; // Data already seeded.
        }

        // 只为管理员和经理角色的用户创建Employee记录
        var adminAndManagerUsers = await dbContext.Set<User>()
            .Where(u => u.RoleId == 3 || u.RoleId == 4) // Manager or Admin
            .ToListAsync();

        if (adminAndManagerUsers.Any())
        {
            var employees = GetEmployeesForUsers(adminAndManagerUsers);
            await dbContext.Set<Employee>().AddRangeAsync(employees);
            await dbContext.SaveChangesAsync();
        }
    }

    private static List<Employee> GetEmployeesForUsers(List<User> users)
    {
        var now = DateTime.UtcNow;
        var employees = new List<Employee>();

        foreach (var user in users)
        {
            var employee = new Employee
            {
                EmployeeId = user.UserId,
                StaffNumber = $"STAFF{user.UserId:D3}",
                Position = user.RoleId == 4 ? "系统管理员" : "部门经理",
                DepartmentName = user.RoleId == 4 ? "信息技术部" : "运营管理部",
                StaffType = StaffType.Regular,
                EmploymentStatus = EmploymentStatus.Active,
                HireDate = now.AddYears(-2), // 假设都是2年前入职
                CreatedAt = now,
                UpdatedAt = now
            };

            employees.Add(employee);
        }

        return employees;
    }
}
