using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.DataSeedings.UserSystem;

public class EmployeeDataSeeding : IDataSeeding
{
    public void Seed(DbContext dbContext)
    {
        if (dbContext.Set<User>().Any() && dbContext.Set<Employee>().Any())
        {
            return; // Data already seeded.
        }

        // Create test users for employees and managers
        var testUsers = GetTestUsers();
        dbContext.Set<User>().AddRange(testUsers);

        // Create test employees and managers
        var testEmployees = GetTestEmployees();
        dbContext.Set<Employee>().AddRange(testEmployees);

        // Fetch users with RoleId 3 (Manager) or 4 (Admin) to create corresponding Employee records.
        var adminAndManagerUsers = dbContext.Set<User>()
            .Where(u => u.RoleId == 3 || u.RoleId == 4) // Manager or Admin
            .ToList();

        if (adminAndManagerUsers.Count != 0)
        {
            var employees = GetEmployeesForUsers(adminAndManagerUsers);
            dbContext.Set<Employee>().AddRange(employees);
            dbContext.SaveChanges();
        }

        dbContext.SaveChanges();
    }

    public async Task SeedAsync(DbContext dbContext)
    {
        if (await dbContext.Set<User>().AnyAsync() && await dbContext.Set<Employee>().AnyAsync())
        {
            return; // Data already seeded.
        }

        // Create test users for employees and managers.
        var testUsers = GetTestUsers();
        await dbContext.Set<User>().AddRangeAsync(testUsers);

        // Create test employees and managers.
        var testEmployees = GetTestEmployees();
        await dbContext.Set<Employee>().AddRangeAsync(testEmployees);

        // Fetch users with RoleId 3 (Manager) or 4 (Admin) to create corresponding Employee records.
        var adminAndManagerUsers = await dbContext.Set<User>()
            .Where(u => u.RoleId == 3 || u.RoleId == 4) // Manager or Admin
            .ToListAsync();

        if (adminAndManagerUsers.Count != 0)
        {
            var employees = GetEmployeesForUsers(adminAndManagerUsers);
            await dbContext.Set<Employee>().AddRangeAsync(employees);
            await dbContext.SaveChangesAsync();
        }

        await dbContext.SaveChangesAsync();
    }

    private static List<User> GetTestUsers()
    {
        var now = DateTime.UtcNow;
        return
        [
            // Employee users (Role ID = 2)
            new()
            {
                UserId = 101,
                Username = "john_employee",
                PasswordHash = "hashedpassword123",
                Email = "john@company.com",
                DisplayName = "John Smith",
                PhoneNumber = "1234567901",
                BirthDate = new DateTime(1988, 5, 15, 0, 0, 0, DateTimeKind.Utc),
                Gender = Gender.Male,
                RegisterTime = now,
                PermissionLevel = 2,
                RoleId = 2, // Employee role
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                UserId = 102,
                Username = "jane_employee",
                PasswordHash = "hashedpassword123",
                Email = "jane@company.com",
                DisplayName = "Jane Doe",
                PhoneNumber = "1234567902",
                BirthDate = new DateTime(1990, 8, 22, 0, 0, 0, DateTimeKind.Utc),
                Gender = Gender.Female,
                RegisterTime = now,
                PermissionLevel = 2,
                RoleId = 2, // Employee role
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                UserId = 103,
                Username = "mike_employee",
                PasswordHash = "hashedpassword123",
                Email = "mike@company.com",
                DisplayName = "Mike Johnson",
                PhoneNumber = "1234567903",
                BirthDate = new DateTime(1985, 11, 30, 0, 0, 0, DateTimeKind.Utc),
                Gender = Gender.Male,
                RegisterTime = now,
                PermissionLevel = 2,
                RoleId = 2, // Employee role
                CreatedAt = now,
                UpdatedAt = now
            },
            // Manager users (Role ID = 3)
            new()
            {
                UserId = 201,
                Username = "sarah_manager",
                PasswordHash = "hashedpassword123",
                Email = "sarah@company.com",
                DisplayName = "Sarah Williams",
                PhoneNumber = "1234567904",
                BirthDate = new DateTime(1982, 3, 10, 0, 0, 0, DateTimeKind.Utc),
                Gender = Gender.Female,
                RegisterTime = now,
                PermissionLevel = 3,
                RoleId = 3, // Manager role
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                UserId = 202,
                Username = "david_manager",
                PasswordHash = "hashedpassword123",
                Email = "david@company.com",
                DisplayName = "David Brown",
                PhoneNumber = "1234567905",
                BirthDate = new DateTime(1978, 7, 18, 0, 0, 0, DateTimeKind.Utc),
                Gender = Gender.Male,
                RegisterTime = now,
                PermissionLevel = 3,
                RoleId = 3, // Manager role
                CreatedAt = now,
                UpdatedAt = now
            }
        ];
    }

    private static List<Employee> GetTestEmployees()
    {
        var now = DateTime.UtcNow;
        return
        [
            // Regular employees
            new()
            {
                EmployeeId = 101, // Same as UserId
                StaffNumber = "EMP001",
                Position = "Ride Operator",
                DepartmentName = "Operations",
                StaffType = StaffType.Regular,
                EmploymentStatus = EmploymentStatus.Active,
                HireDate = now.AddYears(-1),
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                EmployeeId = 102, // Same as UserId
                StaffNumber = "EMP002",
                Position = "Customer Service Representative",
                DepartmentName = "Guest Services",
                StaffType = StaffType.Regular,
                EmploymentStatus = EmploymentStatus.Active,
                HireDate = now.AddMonths(-6),
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                EmployeeId = 103, // Same as UserId
                StaffNumber = "EMP003",
                Position = "Maintenance Technician",
                DepartmentName = "Facilities",
                StaffType = StaffType.Regular,
                EmploymentStatus = EmploymentStatus.Active,
                HireDate = now.AddMonths(-3),
                CreatedAt = now,
                UpdatedAt = now
            },
            // Managers
            new()
            {
                EmployeeId = 201, // Same as UserId
                StaffNumber = "MGR001",
                Position = "Operations Manager",
                DepartmentName = "Operations",
                StaffType = StaffType.Manager,
                EmploymentStatus = EmploymentStatus.Active,
                HireDate = now.AddYears(-3),
                ManagerId = null, // No manager
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                EmployeeId = 202, // Same as UserId
                StaffNumber = "MGR002",
                Position = "Guest Services Manager",
                DepartmentName = "Guest Services",
                StaffType = StaffType.Manager,
                EmploymentStatus = EmploymentStatus.Active,
                HireDate = now.AddYears(-2),
                ManagerId = 201, // Reports to Sarah Williams
                CreatedAt = now,
                UpdatedAt = now
            }
        ];
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
