using System.ComponentModel.DataAnnotations;
using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;

namespace DbApp.Tests.Unit.ResourceSystem;

[Trait("Category", "Unit")]
public class MaintenanceRecordTests
{
    [Fact]
    public void MaintenanceRecord_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var record = new MaintenanceRecord
        {
            RideId = 1,
            TeamId = 1,
            ManagerId = 1,
            MaintenanceType = MaintenanceType.Preventive,
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(2),
            Cost = 150.50m,
            PartsReplaced = "Brake pads, Safety sensors",
            MaintenanceDetails = "Regular maintenance check completed successfully",
            IsCompleted = true,
            IsAccepted = true,
            AcceptanceDate = DateTime.UtcNow,
            AcceptanceComments = "Work completed to standard"
        };

        // Assert
        Assert.Equal(1, record.RideId);
        Assert.Equal(1, record.TeamId);
        Assert.Equal(1, record.ManagerId);
        Assert.Equal(MaintenanceType.Preventive, record.MaintenanceType);
        Assert.Equal(150.50m, record.Cost);
        Assert.Equal("Brake pads, Safety sensors", record.PartsReplaced);
        Assert.Equal("Regular maintenance check completed successfully", record.MaintenanceDetails);
        Assert.True(record.IsCompleted);
        Assert.True(record.IsAccepted);
        Assert.Equal("Work completed to standard", record.AcceptanceComments);
    }

    [Theory]
    [InlineData(MaintenanceType.Preventive)]
    [InlineData(MaintenanceType.Emergency)]
    [InlineData(MaintenanceType.Replacement)]
    [InlineData(MaintenanceType.SoftwareUpdate)]
    public void MaintenanceRecord_WithValidMaintenanceType_ShouldAcceptAllEnumValues(MaintenanceType type)
    {
        // Arrange & Act
        var record = new MaintenanceRecord
        {
            RideId = 1,
            TeamId = 1,
            MaintenanceType = type,
            StartTime = DateTime.UtcNow,
            Cost = 100.00m,
            IsCompleted = false
        };

        // Assert
        Assert.Equal(type, record.MaintenanceType);
    }

    [Fact]
    public void MaintenanceRecord_CostValidation_ShouldEnforceRangeAttribute()
    {
        // Arrange
        var record = new MaintenanceRecord
        {
            RideId = 1,
            TeamId = 1,
            MaintenanceType = MaintenanceType.Preventive,
            StartTime = DateTime.UtcNow,
            Cost = -50.00m, // Invalid negative cost
            IsCompleted = false
        };

        // Act
        var validationContext = new ValidationContext(record);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(record, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, vr => vr.MemberNames.Contains(nameof(MaintenanceRecord.Cost)));
    }

    [Fact]
    public void MaintenanceRecord_DefaultValues_ShouldBeSetCorrectly()
    {
        // Arrange & Act
        var record = new MaintenanceRecord();

        // Assert
        Assert.True(record.CreatedAt <= DateTime.UtcNow);
        Assert.True(record.CreatedAt > DateTime.UtcNow.AddMinutes(-1));
        Assert.Equal(default(DateTime), record.UpdatedAt);
        Assert.False(record.IsCompleted);
        Assert.Null(record.IsAccepted);
        Assert.Equal(0, record.MaintenanceId);
    }

    [Fact]
    public void MaintenanceRecord_WorkflowStates_ShouldHandleCompletionFlow()
    {
        // Arrange
        var record = new MaintenanceRecord
        {
            RideId = 1,
            TeamId = 1,
            MaintenanceType = MaintenanceType.Preventive,
            StartTime = DateTime.UtcNow,
            Cost = 200.00m,
            IsCompleted = false
        };

        // Act - Complete the work
        record.IsCompleted = true;
        record.EndTime = DateTime.UtcNow.AddHours(3);
        record.MaintenanceDetails = "All systems checked and functioning";

        // Act - Accept the work
        record.IsAccepted = true;
        record.AcceptanceDate = DateTime.UtcNow;
        record.AcceptanceComments = "Quality work completed";

        // Assert
        Assert.True(record.IsCompleted);
        Assert.True(record.IsAccepted);
        Assert.NotNull(record.EndTime);
        Assert.NotNull(record.AcceptanceDate);
        Assert.Equal("Quality work completed", record.AcceptanceComments);
    }

    [Fact]
    public void MaintenanceRecord_WithNullableProperties_ShouldHandleNullValues()
    {
        // Arrange & Act
        var record = new MaintenanceRecord
        {
            RideId = 1,
            TeamId = 1,
            MaintenanceType = MaintenanceType.Preventive,
            StartTime = DateTime.UtcNow,
            Cost = 100.00m,
            IsCompleted = false,
            ManagerId = null,
            EndTime = null,
            PartsReplaced = null,
            MaintenanceDetails = null,
            IsAccepted = null,
            AcceptanceDate = null,
            AcceptanceComments = null
        };

        // Assert
        Assert.Null(record.ManagerId);
        Assert.Null(record.EndTime);
        Assert.Null(record.PartsReplaced);
        Assert.Null(record.MaintenanceDetails);
        Assert.Null(record.IsAccepted);
        Assert.Null(record.AcceptanceDate);
        Assert.Null(record.AcceptanceComments);
    }
}
