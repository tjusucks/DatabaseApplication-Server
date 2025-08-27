using System.ComponentModel.DataAnnotations;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using Xunit;

namespace DbApp.Tests.Unit.ResourceSystem;
[Trait("Category", "Unit")]
public class AmusementRideTests
{
    [Fact]
    public void AmusementRide_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act  
        var ride = new AmusementRide
        {
            RideName = "Thunder Mountain",
            Location = "Adventure Land",
            RideStatus = RideStatus.Operating,
            Capacity = 32,
            Duration = 240,
            HeightLimitMin = 110,
            HeightLimitMax = 190,
            Description = "Thrilling roller coaster experience",
            OpenDate = new DateTime(2023, 6, 15)
        };

        // Assert  
        Assert.Equal("Thunder Mountain", ride.RideName);
        Assert.Equal("Adventure Land", ride.Location);
        Assert.Equal(RideStatus.Operating, ride.RideStatus);
        Assert.Equal(32, ride.Capacity);
        Assert.Equal(240, ride.Duration);
        Assert.Equal(110, ride.HeightLimitMin);
        Assert.Equal(190, ride.HeightLimitMax);
        Assert.Equal("Thrilling roller coaster experience", ride.Description);
        Assert.Equal(new DateTime(2023, 6, 15), ride.OpenDate);
    }

    [Theory]
    [InlineData(RideStatus.Operating)]
    [InlineData(RideStatus.Maintenance)]
    [InlineData(RideStatus.Closed)]
    [InlineData(RideStatus.Testing)]
    public void AmusementRide_WithValidRideStatus_ShouldAcceptAllEnumValues(RideStatus status)
    {
        // Arrange & Act  
        var ride = new AmusementRide
        {
            RideName = "Test Ride",
            Location = "Test Zone",
            RideStatus = status,
            Capacity = 20,
            Duration = 180,
            HeightLimitMin = 120,
            HeightLimitMax = 200
        };

        // Assert  
        Assert.Equal(status, ride.RideStatus);
    }

    [Fact]
    public void AmusementRide_DefaultValues_ShouldBeSetCorrectly()
    {
        // Arrange & Act  
        var ride = new AmusementRide();

        // Assert  
        Assert.True(ride.CreatedAt <= DateTime.UtcNow);
        Assert.True(ride.CreatedAt > DateTime.UtcNow.AddMinutes(-1));
        Assert.Equal(default(DateTime), ride.UpdatedAt);
        Assert.Equal(0, ride.RideId);
    }

    [Theory]
    [InlineData(1, 50, 100, 200)]
    [InlineData(100, 150, 120, 180)]
    [InlineData(50, 300, 50, 300)]
    public void AmusementRide_WithValidCapacityAndHeightLimits_ShouldAcceptValues(
        int capacity, int duration, int minHeight, int maxHeight)
    {
        // Arrange & Act  
        var ride = new AmusementRide
        {
            RideName = "Test Ride",
            Location = "Test Zone",
            RideStatus = RideStatus.Operating,
            Capacity = capacity,
            Duration = duration,
            HeightLimitMin = minHeight,
            HeightLimitMax = maxHeight
        };

        // Assert  
        Assert.Equal(capacity, ride.Capacity);
        Assert.Equal(duration, ride.Duration);
        Assert.Equal(minHeight, ride.HeightLimitMin);
        Assert.Equal(maxHeight, ride.HeightLimitMax);
    }

    [Fact]
    public void AmusementRide_WithNullableProperties_ShouldHandleNullValues()
    {
        // Arrange & Act  
        var ride = new AmusementRide
        {
            RideName = "Test Ride",
            Location = "Test Zone",
            RideStatus = RideStatus.Operating,
            Capacity = 20,
            Duration = 180,
            HeightLimitMin = 120,
            HeightLimitMax = 200,
            ManagerId = null,
            Description = null,
            OpenDate = null
        };

        // Assert  
        Assert.Null(ride.ManagerId);
        Assert.Null(ride.Description);
        Assert.Null(ride.OpenDate);
    }
}
