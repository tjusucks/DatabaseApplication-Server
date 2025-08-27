using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Enums.ResourceSystem;
using Xunit;

namespace DbApp.Tests.Unit.ResourceSystem;
[Trait("Category", "Unit")]
public class InspectionRecordTests
{
    [Fact]
    public void InspectionRecord_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act  
        var record = new InspectionRecord
        {
            RideId = 1,
            TeamId = 1,
            CheckDate = DateTime.UtcNow,
            CheckType = CheckType.Daily,
            IsPassed = true,
            IssuesFound = null,
            Recommendations = "Continue regular maintenance schedule"
        };

        // Assert  
        Assert.Equal(1, record.RideId);
        Assert.Equal(1, record.TeamId);
        Assert.Equal(CheckType.Daily, record.CheckType);
        Assert.True(record.IsPassed);
        Assert.Null(record.IssuesFound);
        Assert.Equal("Continue regular maintenance schedule", record.Recommendations);
    }

    [Theory]
    [InlineData(CheckType.Daily)]
    [InlineData(CheckType.Monthly)]
    [InlineData(CheckType.Annual)]
    [InlineData(CheckType.Special)]
    public void InspectionRecord_WithValidCheckType_ShouldAcceptAllEnumValues(CheckType checkType)
    {
        // Arrange & Act  
        var record = new InspectionRecord
        {
            RideId = 1,
            TeamId = 1,
            CheckDate = DateTime.UtcNow,
            CheckType = checkType,
            IsPassed = true
        };

        // Assert  
        Assert.Equal(checkType, record.CheckType);
    }

    [Fact]
    public void InspectionRecord_FailedInspection_ShouldRecordIssuesAndRecommendations()
    {
        // Arrange & Act  
        var record = new InspectionRecord
        {
            RideId = 1,
            TeamId = 1,
            CheckDate = DateTime.UtcNow,
            CheckType = CheckType.Daily,
            IsPassed = false,
            IssuesFound = "Brake response time exceeds safety threshold",
            Recommendations = "Replace brake system immediately and retest"
        };

        // Assert  
        Assert.False(record.IsPassed);
        Assert.Equal("Brake response time exceeds safety threshold", record.IssuesFound);
        Assert.Equal("Replace brake system immediately and retest", record.Recommendations);
    }

    [Fact]
    public void InspectionRecord_PassedInspection_ShouldHaveNoIssues()
    {
        // Arrange & Act  
        var record = new InspectionRecord
        {
            RideId = 1,
            TeamId = 1,
            CheckDate = DateTime.UtcNow,
            CheckType = CheckType.Daily,
            IsPassed = true,
            IssuesFound = null,
            Recommendations = "All systems operating within normal parameters"
        };

        // Assert  
        Assert.True(record.IsPassed);
        Assert.Null(record.IssuesFound);
        Assert.Equal("All systems operating within normal parameters", record.Recommendations);
    }

    [Fact]
    public void InspectionRecord_DefaultValues_ShouldBeSetCorrectly()
    {
        // Arrange & Act  
        var record = new InspectionRecord();

        // Assert  
        Assert.True(record.CreatedAt <= DateTime.UtcNow);
        Assert.True(record.CreatedAt > DateTime.UtcNow.AddMinutes(-1));
        Assert.Equal(default(DateTime), record.UpdatedAt);
        Assert.Equal(0, record.InspectionId);
        Assert.False(record.IsPassed); // Default boolean value  
    }

    [Theory]
    [InlineData(CheckType.Daily, true, null, "Daily check completed successfully")]
    [InlineData(CheckType.Monthly, false, "Minor oil leak detected", "Schedule maintenance for oil seal replacement")]
    [InlineData(CheckType.Annual, true, null, "Annual certification renewed")]
    [InlineData(CheckType.Special, false, "Emergency stop button malfunction", "Replace emergency stop system immediately")]
    public void InspectionRecord_WithVariousScenarios_ShouldHandleAllCases(
        CheckType checkType, bool isPassed, string? issuesFound, string? recommendations)
    {
        // Arrange & Act  
        var record = new InspectionRecord
        {
            RideId = 1,
            TeamId = 1,
            CheckDate = DateTime.UtcNow,
            CheckType = checkType,
            IsPassed = isPassed,
            IssuesFound = issuesFound,
            Recommendations = recommendations
        };

        // Assert  
        Assert.Equal(checkType, record.CheckType);
        Assert.Equal(isPassed, record.IsPassed);
        Assert.Equal(issuesFound, record.IssuesFound);
        Assert.Equal(recommendations, record.Recommendations);
    }

    [Fact]
    public void InspectionRecord_WithNullableProperties_ShouldHandleNullValues()
    {
        // Arrange & Act  
        var record = new InspectionRecord
        {
            RideId = 1,
            TeamId = 1,
            CheckDate = DateTime.UtcNow,
            CheckType = CheckType.Daily,
            IsPassed = true,
            IssuesFound = null,
            Recommendations = null
        };

        // Assert  
        Assert.Null(record.IssuesFound);
        Assert.Null(record.Recommendations);
    }

    [Fact]
    public void InspectionRecord_CheckDateValidation_ShouldAcceptValidDates()
    {
        // Arrange  
        var pastDate = DateTime.UtcNow.AddDays(-30);
        var futureDate = DateTime.UtcNow.AddDays(7);

        // Act & Assert - Past date  
        var pastRecord = new InspectionRecord
        {
            RideId = 1,
            TeamId = 1,
            CheckDate = pastDate,
            CheckType = CheckType.Monthly,
            IsPassed = true
        };
        Assert.Equal(pastDate, pastRecord.CheckDate);

        // Act & Assert - Future date  
        var futureRecord = new InspectionRecord
        {
            RideId = 1,
            TeamId = 1,
            CheckDate = futureDate,
            CheckType = CheckType.Daily,
            IsPassed = true
        };
        Assert.Equal(futureDate, futureRecord.CheckDate);
    }

    [Fact]
    public void InspectionRecord_BusinessLogic_FailedInspectionShouldHaveIssues()
    {
        // Arrange & Act  
        var failedRecord = new InspectionRecord
        {
            RideId = 1,
            TeamId = 1,
            CheckDate = DateTime.UtcNow,
            CheckType = CheckType.Daily,
            IsPassed = false,
            IssuesFound = "Critical safety violation detected",
            Recommendations = "Immediate shutdown required"
        };

        // Assert  
        Assert.False(failedRecord.IsPassed);
        Assert.NotNull(failedRecord.IssuesFound);
        Assert.NotEmpty(failedRecord.IssuesFound);
        Assert.NotNull(failedRecord.Recommendations);
        Assert.NotEmpty(failedRecord.Recommendations);
    }

    [Fact]
    public void InspectionRecord_TimestampProperties_ShouldBeSetCorrectly()
    {
        // Arrange  
        var beforeCreation = DateTime.UtcNow;

        // Act  
        var record = new InspectionRecord
        {
            RideId = 1,
            TeamId = 1,
            CheckDate = DateTime.UtcNow,
            CheckType = CheckType.Daily,
            IsPassed = true
        };

        var afterCreation = DateTime.UtcNow;

        // Assert  
        Assert.True(record.CreatedAt >= beforeCreation);
        Assert.True(record.CreatedAt <= afterCreation);
        Assert.Equal(default(DateTime), record.UpdatedAt);
    }
}
