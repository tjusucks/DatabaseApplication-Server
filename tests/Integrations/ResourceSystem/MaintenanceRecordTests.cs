using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Infrastructure;
using DbApp.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Tests.Integrations.ResourceSystem;

[Collection("Database")]
[Trait("Category", "Integration")]
public class MaintenanceRecordConstraintIntegrationTests(DatabaseFixture fixture) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await fixture.ResetDatabaseAsync();

    [Fact]
    public async Task CreateMaintenanceRecord_WithValidData_ShouldSucceed()
    {
        // Arrange
        var context = fixture.DbContext;
        var ride = await CreateTestRideAsync(context);
        var team = await CreateTestTeamAsync(context);

        var record = new MaintenanceRecord
        {
            RideId = ride.RideId,
            TeamId = team.TeamId,
            MaintenanceType = MaintenanceType.Preventive,
            StartTime = DateTime.UtcNow,
            Cost = 500.00m,
            PartsReplaced = "Brake pads",
            MaintenanceDetails = "Regular brake maintenance",
            IsCompleted = false
        };

        // Act
        context.MaintenanceRecords.Add(record);
        await context.SaveChangesAsync();

        // Assert
        Assert.True(record.MaintenanceId > 0);
        var savedRecord = await context.MaintenanceRecords.FindAsync(record.MaintenanceId);
        Assert.NotNull(savedRecord);
        Assert.Equal(500.00m, savedRecord.Cost);
    }

    [Fact]
    public async Task CreateMaintenanceRecord_WithNegativeCost_ShouldThrowDbUpdateException()
    {
        // Arrange
        var context = fixture.DbContext;
        var ride = await CreateTestRideAsync(context);
        var team = await CreateTestTeamAsync(context);

        var record = new MaintenanceRecord
        {
            RideId = ride.RideId,
            TeamId = team.TeamId,
            MaintenanceType = MaintenanceType.Preventive,
            StartTime = DateTime.UtcNow,
            Cost = -100.00m,
            IsCompleted = false
        };

        // Act & Assert
        context.MaintenanceRecords.Add(record);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());

        var errorMessage = exception.InnerException?.Message ?? exception.Message;
        Assert.True(
            errorMessage.Contains("cost") ||
            errorMessage.Contains("CK_maintenance_records_cost_Range") ||
            errorMessage.Contains("ORA-02290"),
            $"Expected cost constraint violation, but got: {errorMessage}"
        );
    }

    [Theory]
    [InlineData(-1)]    // 低于枚举范围
    [InlineData(4)]     // 高于枚举范围 (MaintenanceType 0-3)
    public async Task CreateMaintenanceRecord_WithInvalidMaintenanceType_ShouldThrowDbUpdateException(int invalidType)
    {
        // Arrange
        var context = fixture.DbContext;
        var ride = await CreateTestRideAsync(context);
        var team = await CreateTestTeamAsync(context);

        var record = new MaintenanceRecord
        {
            RideId = ride.RideId,
            TeamId = team.TeamId,
            MaintenanceType = (MaintenanceType)invalidType,
            StartTime = DateTime.UtcNow,
            Cost = 100.00m,
            IsCompleted = false
        };

        // Act & Assert
        context.MaintenanceRecords.Add(record);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());

        var errorMessage = exception.InnerException?.Message ?? exception.Message;
        Assert.True(
            errorMessage.Contains("maintenance_type") ||
            errorMessage.Contains("CK_maintenance_records_maintenance_type_Enum") ||
            errorMessage.Contains("ORA-02290"),
            $"Expected maintenance_type constraint violation, but got: {errorMessage}"
        );
    }

    private static async Task<AmusementRide> CreateTestRideAsync(ApplicationDbContext context)
    {
        var ride = new AmusementRide
        {
            RideName = "Test Ride for Maintenance",
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
        // 创建完整的数据依赖链
        var role = await CreateTestRoleAsync(context);
        var user = await CreateTestUserAsync(context, role.RoleId);
        var employee = await CreateTestEmployeeAsync(context, user.UserId);

        var team = new StaffTeam
        {
            TeamName = "Test Maintenance Team",
            TeamType = TeamType.Mechanic,
            LeaderId = employee.EmployeeId
        };
        context.StaffTeams.Add(team);
        await context.SaveChangesAsync();
        return team;
    }

    private static async Task<Role> CreateTestRoleAsync(ApplicationDbContext context)
    {
        var role = new Role
        {
            RoleName = $"TestRole_{Guid.NewGuid():N}",
            RoleDescription = "Test role for maintenance integration tests",
            IsSystemRole = false
        };
        context.Roles.Add(role);
        await context.SaveChangesAsync();
        return role;
    }

    private static async Task<User> CreateTestUserAsync(ApplicationDbContext context, int roleId)
    {
        var user = new User
        {
            Username = $"testuser_{Guid.NewGuid():N}",
            Email = $"test_{Guid.NewGuid():N}@test.com",
            PasswordHash = "hashedpassword",
            DisplayName = "Test Maintenance User",
            RoleId = roleId
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    private static async Task<Employee> CreateTestEmployeeAsync(ApplicationDbContext context, int userId)
    {
        var employee = new Employee
        {
            EmployeeId = userId,
            StaffNumber = $"EMP{Guid.NewGuid().ToString("N")[..16]}",
            Position = "Test Maintenance Leader",
            StaffType = StaffType.Manager
        };
        context.Employees.Add(employee);
        await context.SaveChangesAsync();
        return employee;
    }
}
