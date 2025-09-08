using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using DbApp.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Tests.Integrations.ResourceSystem;

[Collection("Database")]
[Trait("Category", "Integration")]
public class AmusementRideConstraintIntegrationTests(DatabaseFixture fixture) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await fixture.ResetDatabaseAsync();

    [Fact]
    public async Task CreateAmusementRide_WithValidData_ShouldSucceed()
    {
        // Arrange
        var context = fixture.DbContext;
        var ride = new AmusementRide
        {
            RideName = "Test Roller Coaster",
            Location = "Zone A",
            RideStatus = RideStatus.Operating,
            Capacity = 24,
            Duration = 180,
            HeightLimitMin = 120,
            HeightLimitMax = 200,
            Description = "Exciting roller coaster ride",
            OpenDate = DateTime.UtcNow.Date
        };

        // Act
        context.AmusementRides.Add(ride);
        await context.SaveChangesAsync();

        // Assert
        Assert.True(ride.RideId > 0);
        var savedRide = await context.AmusementRides.FindAsync(ride.RideId);
        Assert.NotNull(savedRide);
        Assert.Equal("Test Roller Coaster", savedRide.RideName);
    }

    [Theory]
    [InlineData(0)]     // 容量为0，违反约束
    [InlineData(-1)]    // 负容量
    public async Task CreateAmusementRide_WithInvalidCapacity_ShouldThrowDbUpdateException(int invalidCapacity)
    {
        // Arrange
        var context = fixture.DbContext;
        var ride = new AmusementRide
        {
            RideName = "Invalid Ride",
            Location = "Zone B",
            RideStatus = RideStatus.Operating,
            Capacity = invalidCapacity,
            Duration = 180,
            HeightLimitMin = 120,
            HeightLimitMax = 200
        };

        // Act & Assert
        context.AmusementRides.Add(ride);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());

        // 检查 Oracle 约束错误或约束名称
        var errorMessage = exception.InnerException?.Message ?? exception.Message;
        Assert.True(
            errorMessage.Contains("CK_amusement_rides_capacity_Range") ||
            errorMessage.Contains("capacity") ||
            errorMessage.Contains("ORA-02290"),
            $"Expected capacity constraint violation, but got: {errorMessage}"
        );
    }

    [Theory]
    [InlineData(49, 200)]   // HeightLimitMin 低于最小值 50
    [InlineData(301, 200)]  // HeightLimitMin 高于最大值 300
    [InlineData(120, 49)]   // HeightLimitMax 低于最小值 50
    [InlineData(120, 301)]  // HeightLimitMax 高于最大值 300
    public async Task CreateAmusementRide_WithInvalidHeightLimits_ShouldThrowDbUpdateException(int minHeight, int maxHeight)
    {
        // Arrange
        var context = fixture.DbContext;
        var ride = new AmusementRide
        {
            RideName = "Invalid Height Ride",
            Location = "Zone C",
            RideStatus = RideStatus.Operating,
            Capacity = 20,
            Duration = 180,
            HeightLimitMin = minHeight,
            HeightLimitMax = maxHeight
        };

        // Act & Assert
        context.AmusementRides.Add(ride);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());

        var errorMessage = exception.InnerException?.Message ?? exception.Message;
        Assert.True(
            errorMessage.Contains("height_limit") ||
            errorMessage.Contains("height") ||
            errorMessage.Contains("CK_amusement_rides_height_limit") ||
            errorMessage.Contains("ORA-02290"),
            $"Expected height constraint violation, but got: {errorMessage}"
        );
    }

    [Theory]
    [InlineData(-1)]    // 低于枚举范围
    [InlineData(4)]     // 高于枚举范围 (RideStatus 0-3)
    public async Task CreateAmusementRide_WithInvalidRideStatus_ShouldThrowDbUpdateException(int invalidStatus)
    {
        // Arrange
        var context = fixture.DbContext;
        var ride = new AmusementRide
        {
            RideName = "Invalid Status Ride",
            Location = "Zone D",
            RideStatus = (RideStatus)invalidStatus,
            Capacity = 20,
            Duration = 180,
            HeightLimitMin = 120,
            HeightLimitMax = 200
        };

        // Act & Assert
        context.AmusementRides.Add(ride);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());

        var errorMessage = exception.InnerException?.Message ?? exception.Message;
        Assert.True(
            errorMessage.Contains("ride_status") ||
            errorMessage.Contains("CK_amusement_rides_ride_status_Enum") ||
            errorMessage.Contains("ORA-02290"),
            $"Expected ride status constraint violation, but got: {errorMessage}"
        );
    }
}
