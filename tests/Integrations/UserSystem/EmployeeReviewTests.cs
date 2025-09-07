using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Tests.Integrations.UserSystem;

[Collection("Database")]
public class AttendanceAndPerformanceIntegrationTests(DatabaseFixture fixture) : IAsyncLifetime
{
    private int _testEmployeeId;
    private int _testAttendanceId;
    private int _testReviewId;

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

        // 添加测试考勤数据
        var testAttendance = new Attendance
        {
            EmployeeId = _testEmployeeId,
            AttendanceDate = DateTime.Today,
            CheckInTime = DateTime.Today.AddHours(9),
            CheckOutTime = DateTime.Today.AddHours(18),
            AttendanceStatus = AttendanceStatus.Present,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.Attendances.Add(testAttendance);
        await db.SaveChangesAsync();
        _testAttendanceId = testAttendance.AttendanceId;

        // 添加测试绩效数据
        var testReview = new EmployeeReview
        {
            EmployeeId = _testEmployeeId,
            Period = "2025Q1",
            Score = 95.5m,
            EvaluationLevel = EvaluationLevel.Excellent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.EmployeeReviews.Add(testReview);
        await db.SaveChangesAsync();
        _testReviewId = testReview.ReviewId;

        Console.WriteLine($"✓ Added test employee with ID: {_testEmployeeId}");
        Console.WriteLine($"✓ Added test attendance with ID: {_testAttendanceId}");
        Console.WriteLine($"✓ Added test review with ID: {_testReviewId}");
    }

    private async Task RemoveTestEmployeeData()
    {
        var db = fixture.DbContext;

        // 删除测试绩效数据
        var testReview = await db.EmployeeReviews
            .FirstOrDefaultAsync(r => r.ReviewId == _testReviewId);
        if (testReview != null)
        {
            db.EmployeeReviews.Remove(testReview);
        }

        // 删除测试考勤数据
        var testAttendance = await db.Attendances
            .FirstOrDefaultAsync(a => a.AttendanceId == _testAttendanceId);
        if (testAttendance != null)
        {
            db.Attendances.Remove(testAttendance);
        }

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
    public async Task PerformanceReviewEndpoints_ReturnSuccessWithData()
    {
        // Create a test server and client.
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // Configure JSON serializer options to match API settings
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

        // Test various performance review endpoints
        var endpoints = new[]
        {
            "/api/resource/employee-reviews/search",
            $"/api/resource/employee-reviews/search?employeeId={_testEmployeeId}",
        };

        foreach (var endpoint in endpoints)
        {
            var response = await client.GetAsync(endpoint);

            // Should return 200 OK
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Read and verify response data
            var jsonContent = await response.Content.ReadAsStringAsync();
            var reviews = JsonSerializer.Deserialize<List<EmployeeReviewDto>>(jsonContent, jsonOptions);
            Assert.NotNull(reviews);

            // For endpoints with employeeId, verify we have at least our test review
            if (endpoint.Contains("employeeId"))
            {
                Assert.NotEmpty(reviews);
                var testReview = reviews.FirstOrDefault(r => r.ReviewId == _testReviewId);
                Assert.NotNull(testReview);
                Assert.Equal(_testEmployeeId, testReview.EmployeeId);
                Assert.Equal("2025Q1", testReview.Period);
                Assert.Equal(95.5m, testReview.Score);
                Console.WriteLine($"✓ Endpoint {endpoint} returned {reviews.Count} reviews including test review");
            }
            else
            {
                // For general endpoint, just verify it returns data properly
                Assert.NotNull(reviews);
                Console.WriteLine($"✓ Endpoint {endpoint} returned {reviews.Count} reviews");
            }
        }

        Console.WriteLine("✓ All performance review list endpoints returned OK with valid data");
    }

    [Fact]
    public async Task GetSpecificPerformanceReview_ReturnsSuccessWithData()
    {
        // Create a test server and client.
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // Configure JSON serializer options to match API settings
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

        // Call the API endpoint.
        var response = await client.GetAsync($"/api/resource/employee-reviews/{_testReviewId}");

        // Should return 200 OK since we have test data
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Read and verify response data
        var jsonContent = await response.Content.ReadAsStringAsync();
        var review = JsonSerializer.Deserialize<EmployeeReviewDto>(jsonContent, jsonOptions);
        Assert.NotNull(review);
        Assert.Equal(_testReviewId, review.ReviewId);
        Assert.Equal(_testEmployeeId, review.EmployeeId);
        Assert.Equal("2025Q1", review.Period);
        Assert.Equal(95.5m, review.Score);
        Assert.Equal("Excellent", review.EvaluationLevel);

        Console.WriteLine($"✓ Get specific review returned review: ID: {review.ReviewId}, EmployeeId: {review.EmployeeId}, Period: {review.Period}, Score: {review.Score}");
    }

    [Fact]
    public async Task GetSpecificAttendance_ReturnsSuccessWithData()
    {
        // Create a test server and client.
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // Configure JSON serializer options to match API settings
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

        // Test getting attendance by ID using the generic query endpoint
        var url = $"/api/resource/attendance/search?queryType=GetById&id={_testAttendanceId}";

        var response = await client.GetAsync(url);

        // Should return 200 OK since we have test data
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Read and verify response data
        var jsonContent = await response.Content.ReadAsStringAsync();
        var attendance = JsonSerializer.Deserialize<AttendanceDto>(jsonContent, jsonOptions);
        Assert.NotNull(attendance);
        Assert.Equal(_testAttendanceId, attendance.AttendanceId);
        Assert.Equal(_testEmployeeId, attendance.EmployeeId);
        Assert.Equal(AttendanceStatus.Present.ToString(), attendance.AttendanceStatus);

        Console.WriteLine($"✓ Get specific attendance returned attendance: ID: {attendance.AttendanceId}, EmployeeId: {attendance.EmployeeId}");
    }

    [Fact]
    public async Task GetEmployeeAttendance_ReturnsSuccessWithData()
    {
        // Create a test server and client.
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // Configure JSON serializer options to match API settings
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

        // Test getting attendance by employee ID using the generic query endpoint
        var url = $"/api/resource/attendance/search?queryType=GetEmployeeAttendance&employeeId={_testEmployeeId}";

        var response = await client.GetAsync(url);
        // Should return 200 OK since we have test data
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Read and verify response data
        var jsonContent = await response.Content.ReadAsStringAsync();
        var attendances = JsonSerializer.Deserialize<List<AttendanceDto>>(jsonContent, jsonOptions);
        Assert.NotNull(attendances);
        Assert.NotEmpty(attendances);

        var testAttendance = attendances.FirstOrDefault(a => a.AttendanceId == _testAttendanceId);
        Assert.NotNull(testAttendance);
        Assert.Equal(_testEmployeeId, testAttendance.EmployeeId);
        Assert.Equal(AttendanceStatus.Present.ToString(), testAttendance.AttendanceStatus);

        Console.WriteLine($"✓ Get employee attendance returned {attendances.Count} attendance records");
    }

    // DTO classes for deserialization
    public class EmployeeReviewDto
    {
        public int ReviewId { get; set; }
        public int EmployeeId { get; set; }
        public string Period { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public string? EvaluationLevel { get; set; }
        public int? EvaluatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public EmployeeSimpleDto Employee { get; set; } = new EmployeeSimpleDto();
    }

    public class EmployeeSimpleDto
    {
        public int EmployeeId { get; set; }
        public string StaffNumber { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
    }

    // Attendance DTO for deserialization
    public class AttendanceDto
    {
        public int AttendanceId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string AttendanceStatus { get; set; } = string.Empty;
        public string? LeaveType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Employee information (simplified to avoid circular references)
        public EmployeeInfoDto Employee { get; set; } = new EmployeeInfoDto();
    }

    public class EmployeeInfoDto
    {
        public int EmployeeId { get; set; }
        public string StaffNumber { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
    }
}
