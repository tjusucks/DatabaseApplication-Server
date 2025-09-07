using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Infrastructure;
using DbApp.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DbApp.Tests.Integration.ResourceSystem;

[Collection("Database")]
[Trait("Category", "Integration")]
public class InspectionRecordConstraintIntegrationTests
{
    private readonly DatabaseFixture _fixture;

    public InspectionRecordConstraintIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateInspectionRecord_WithValidData_ShouldSucceed()
    {
        // Arrange    
        using var context = _fixture.DbContext;
        var ride = await CreateTestRideAsync(context);
        var team = await CreateTestTeamAsync(context);

        var record = new InspectionRecord
        {
            RideId = ride.RideId,
            TeamId = team.TeamId,
            CheckDate = DateTime.UtcNow,
            CheckType = CheckType.Daily,
            IsPassed = true,
            IssuesFound = null,
            Recommendations = "All systems functioning normally"
        };

        // Act    
        context.InspectionRecords.Add(record);
        await context.SaveChangesAsync();

        // Assert    
        Assert.True(record.InspectionId > 0);
        var savedRecord = await context.InspectionRecords.FindAsync(record.InspectionId);
        Assert.NotNull(savedRecord);
        Assert.True(savedRecord.IsPassed);
    }

    [Theory]
    [InlineData(-1)]    // 低于枚举范围    
    [InlineData(4)]     // 高于枚举范围 (CheckType 0-3)    
    public async Task CreateInspectionRecord_WithInvalidCheckType_ShouldThrowDbUpdateException(int invalidType)
    {
        // Arrange    
        using var context = _fixture.DbContext;
        var ride = await CreateTestRideAsync(context);
        var team = await CreateTestTeamAsync(context);

        var record = new InspectionRecord
        {
            RideId = ride.RideId,
            TeamId = team.TeamId,
            CheckDate = DateTime.UtcNow,
            CheckType = (CheckType)invalidType,
            IsPassed = true
        };

        // Act & Assert    
        context.InspectionRecords.Add(record);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());

        var errorMessage = exception.InnerException?.Message ?? exception.Message;
        Assert.True(
            errorMessage.Contains("check_type") ||
            errorMessage.Contains("CK_inspection_records_check_type_Enum") ||
            errorMessage.Contains("ORA-02290"),
            $"Expected check_type constraint violation, but got: {errorMessage}"
        );
    }

    private async Task<AmusementRide> CreateTestRideAsync(ApplicationDbContext context)
    {
        var ride = new AmusementRide
        {
            RideName = "Test Ride for Inspection",
            Location = "Test Zone",
            RideStatus = RideStatus.Operating,
            Capacity = 20,
            Duration = 180,
            HeightLimitMin = 120,
            HeightLimitMax = 200
        };
        context.AmusementRides.Add(ride);
        await context.SaveChangesAsync();
        return ride;
    }

    private async Task<StaffTeam> CreateTestTeamAsync(ApplicationDbContext context)
    {
        // 创建一个测试用户和角色  
        var role = await CreateTestRoleAsync(context);
        var user = await CreateTestUserAsync(context, role.RoleId);
        var employee = await CreateTestEmployeeAsync(context, user.UserId);

        var team = new StaffTeam
        {
            TeamName = "Test Inspection Team",
            TeamType = TeamType.Inspector,
            LeaderId = employee.EmployeeId
        };
        context.StaffTeams.Add(team);
        await context.SaveChangesAsync();
        return team;
    }

    private async Task<Role> CreateTestRoleAsync(ApplicationDbContext context)
    {
        var role = new Role
        {
            RoleName = $"TestRole_{Guid.NewGuid():N}",
            RoleDescription = "Test role for integration tests",
            IsSystemRole = false
        };
        context.Roles.Add(role);
        await context.SaveChangesAsync();
        return role;
    }

    private async Task<User> CreateTestUserAsync(ApplicationDbContext context, int roleId)
    {
        var user = new User
        {
            Username = $"testuser_{Guid.NewGuid():N}",
            Email = $"test_{Guid.NewGuid():N}@test.com",
            PasswordHash = "hashedpassword",
            DisplayName = "Test User",
            RoleId = roleId
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    private async Task<Employee> CreateTestEmployeeAsync(ApplicationDbContext context, int userId)
    {
        var employee = new Employee
        {
            EmployeeId = userId,
            StaffNumber = $"EMP{Guid.NewGuid().ToString("N")[..16]}",
            Position = "Test Inspector",
            StaffType = StaffType.Inspector
        };
        context.Employees.Add(employee);
        await context.SaveChangesAsync();
        return employee;
    }
}
