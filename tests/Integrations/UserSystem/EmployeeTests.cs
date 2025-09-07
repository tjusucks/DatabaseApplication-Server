using System.Net;
using System.Text.Json;
using DbApp.Application.UserSystem.Employees;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Tests.Integrations.UserSystem.Employees;

[Collection("Database")]
public class EmployeeIntegrationTests(DatabaseFixture fixture) : IAsyncLifetime
{
    private int _testEmployeeId;

    public async Task InitializeAsync()
    {
        // 添加测试数据
        await AddTestEmployeeData();
    }

    public async Task DisposeAsync()
    {
        // 清理测试数据
        await RemoveTestEmployeeData();
    }

    private async Task AddTestEmployeeData()
    {
        var db = fixture.DbContext;

        // 确保有Employee角色
        var employeeRole = await db.Roles.FirstOrDefaultAsync(r => r.RoleName == "Employee");
        if (employeeRole == null)
        {
            employeeRole = new Role
            {
                RoleName = "Employee",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow
            };
            db.Roles.Add(employeeRole);
            await db.SaveChangesAsync();
        }

        // 添加测试用户数据
        var testUser = new User
        {
            Username = "testemployee",
            PasswordHash = "testpasswordhash",
            Email = "test@company.com",
            DisplayName = "Test Employee",
            PermissionLevel = 1,
            RoleId = employeeRole.RoleId,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(testUser);
        await db.SaveChangesAsync();

        // 添加测试员工数据
        var testEmployee = new Employee
        {
            EmployeeId = testUser.UserId,
            StaffNumber = "TEST001",
            Position = "Test Developer",
            DepartmentName = "IT",
            StaffType = StaffType.Regular,
            EmploymentStatus = EmploymentStatus.Active,
            HireDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        db.Employees.Add(testEmployee);
        await db.SaveChangesAsync();
        _testEmployeeId = testEmployee.EmployeeId;

        Console.WriteLine($"✓ Added test employee with ID: {_testEmployeeId}");
    }

    private async Task RemoveTestEmployeeData()
    {
        var db = fixture.DbContext;

        // 删除测试员工数据
        var testEmployee = await db.Employees
            .FirstOrDefaultAsync(e => e.EmployeeId == _testEmployeeId);
        if (testEmployee != null)
        {
            db.Employees.Remove(testEmployee);
        }

        // 删除测试用户数据
        var testUser = await db.Users
            .FirstOrDefaultAsync(u => u.UserId == _testEmployeeId);
        if (testUser != null)
        {
            db.Users.Remove(testUser);
        }

        await db.SaveChangesAsync();
        Console.WriteLine("✓ Cleaned up test data");
    }

    [Fact]
    public async Task EmployeeEndpoints_ReturnSuccessWithData()
    {
        // Create a test server and client.
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // Configure JSON serializer options to match API settings
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        };
        jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

        // Test various employee endpoints
        var endpoints = new[]
        {
            "/api/user/employees/search",
            "/api/user/employees/search?keyword=Test",
            "/api/user/employees/search?departmentName=IT"
        };

        foreach (var endpoint in endpoints)
        {
            var response = await client.GetAsync(endpoint);
            // Should return 200 OK (even if no data).
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Read and verify response data
            var jsonContent = await response.Content.ReadAsStringAsync();
            var employees = System.Text.Json.JsonSerializer.Deserialize<List<EmployeeDto>>(jsonContent, jsonOptions);
            Assert.NotNull(employees);

            // For endpoints with search criteria, verify we have at least our test employee
            if (endpoint.Contains("keyword") || endpoint.Contains("departmentName"))
            {
                Assert.NotEmpty(employees);
                var testEmployee = employees.FirstOrDefault(e => e.EmployeeId == _testEmployeeId);
                Assert.NotNull(testEmployee);
                Assert.Equal("TEST001", testEmployee.StaffNumber);
                Assert.Equal("Test Developer", testEmployee.Position);
                Assert.Equal("IT", testEmployee.DepartmentName);
                Assert.Equal("testemployee", testEmployee.User.Username);
                Assert.Equal("test@company.com", testEmployee.User.Email);
                Console.WriteLine($"✓ Endpoint {endpoint} returned {employees.Count} employees including test employee");
            }
            else
            {
                // For general endpoint, just verify it returns data properly
                Assert.NotNull(employees);
                Console.WriteLine($"✓ Endpoint {endpoint} returned {employees.Count} employees");
            }
        }

        Console.WriteLine("✓ All employee list endpoints returned OK with valid data");
    }

    [Fact]
    public async Task GetSpecificEmployee_ReturnsSuccessWithData()
    {
        // Create a test server and client.
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // Configure JSON serializer options to match API settings
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        };
        jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

        // Call the API endpoint.
        var response = await client.GetAsync($"/api/user/employees/{_testEmployeeId}");

        // Should return 200 OK since we have test data
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Read and verify response data
        var jsonContent = await response.Content.ReadAsStringAsync();
        var employee = System.Text.Json.JsonSerializer.Deserialize<EmployeeDto>(jsonContent, jsonOptions);
        Assert.NotNull(employee);
        Assert.Equal(_testEmployeeId, employee.EmployeeId);
        Assert.Equal("TEST001", employee.StaffNumber);
        Assert.Equal("Test Developer", employee.Position);
        Assert.Equal("IT", employee.DepartmentName);
        Assert.Equal(StaffType.Regular, employee.StaffType);
        Assert.Equal(EmploymentStatus.Active, employee.EmploymentStatus);
        Assert.Equal("testemployee", employee.User.Username);
        Assert.Equal("test@company.com", employee.User.Email);
        Assert.Equal("Test Employee", employee.User.DisplayName);

        Console.WriteLine($"✓ Get specific employee returned employee: {employee.User.DisplayName} (ID: {employee.EmployeeId}, StaffNumber: {employee.StaffNumber})");
    }

    [Fact]
    public async Task GetAllEmployees_ReturnsSuccessWithData()
    {
        // Create a test server and client.
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // Configure JSON serializer options to match API settings
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        };
        jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

        // Call the API endpoint.
        var response = await client.GetAsync("/api/user/employees/search");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Read and verify response data
        var jsonContent = await response.Content.ReadAsStringAsync();
        var employees = System.Text.Json.JsonSerializer.Deserialize<List<EmployeeDto>>(jsonContent, jsonOptions);
        Assert.NotNull(employees);

        // Verify our test employee is in the list
        var testEmployee = employees.FirstOrDefault(e => e.EmployeeId == _testEmployeeId);
        Assert.NotNull(testEmployee);
        Assert.Equal("TEST001", testEmployee.StaffNumber);
        Assert.Equal("Test Developer", testEmployee.Position);

        Console.WriteLine($"✓ Get all employees returned {employees.Count} employees including test employee");
    }

    [Fact]
    public async Task SearchEmployeesByKeyword_ReturnsMatchingResults()
    {
        // Create a test server and client.
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // Configure JSON serializer options to match API settings
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        };
        jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

        // Search by keyword "Test"
        var response = await client.GetAsync("/api/user/employees/search?keyword=Test");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Read and verify response data
        var jsonContent = await response.Content.ReadAsStringAsync();
        var employees = System.Text.Json.JsonSerializer.Deserialize<List<EmployeeDto>>(jsonContent, jsonOptions);
        Assert.NotNull(employees);
        Assert.NotEmpty(employees);

        // Verify our test employee is in the results
        var testEmployee = employees.FirstOrDefault(e => e.EmployeeId == _testEmployeeId);
        Assert.NotNull(testEmployee);
        Assert.Contains("Test", testEmployee.User.DisplayName);

        Console.WriteLine($"✓ Search by keyword 'Test' returned {employees.Count} matching employees");
    }

    [Fact]
    public async Task GetEmployeesByDepartment_ReturnsDepartmentEmployees()
    {
        // Create a test server and client.
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // Configure JSON serializer options to match API settings
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        };
        jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

        // Get employees by department
        var response = await client.GetAsync("/api/user/employees/search?departmentName=IT");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Read and verify response data
        var jsonContent = await response.Content.ReadAsStringAsync();
        var employees = System.Text.Json.JsonSerializer.Deserialize<List<EmployeeDto>>(jsonContent, jsonOptions);
        Assert.NotNull(employees);
        Assert.NotEmpty(employees);

        // Verify our test employee is in the results
        var testEmployee = employees.FirstOrDefault(e => e.EmployeeId == _testEmployeeId);
        Assert.NotNull(testEmployee);
        Assert.Equal("IT", testEmployee.DepartmentName);

        Console.WriteLine($"✓ Get employees by department 'IT' returned {employees.Count} employees");
    }
}
