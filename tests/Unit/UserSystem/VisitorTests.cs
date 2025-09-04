using DbApp.Application.UserSystem.Visitors;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using Moq;

namespace DbApp.Tests.Unit.UserSystem;

/// <summary>
/// Unit tests for visitor management functionality.
/// </summary>
public class VisitorTests
{
    private readonly Mock<IVisitorRepository> _mockVisitorRepository;
    private readonly Mock<IEntryRecordRepository> _mockEntryRecordRepository;

    public VisitorTests()
    {
        _mockVisitorRepository = new Mock<IVisitorRepository>();
        _mockEntryRecordRepository = new Mock<IEntryRecordRepository>();
    }

    [Fact]
    public async Task CreateVisitorCommandHandler_ShouldCreateVisitorSuccessfully()
    {
        // Arrange
        var command = new CreateVisitorCommand(
            Username: "testuser",
            Email: "test@example.com",
            DisplayName: "Test User",
            PhoneNumber: "1234567890",
            BirthDate: new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Gender: Gender.Male,
            VisitorType: VisitorType.Regular,
            Height: 175,
            PasswordHash: "hashedpassword"
        );

        _mockVisitorRepository.Setup(r => r.CreateAsync(It.IsAny<Visitor>()))
            .Callback<Visitor>(v => v.VisitorId = 1)  // Set the VisitorId after creation
            .ReturnsAsync(1);

        var handler = new CreateVisitorCommandHandler(_mockVisitorRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(1, result);
        // Single transaction: only VisitorRepository.CreateAsync is called
        // User creation is handled via navigation property
        _mockVisitorRepository.Verify(r => r.CreateAsync(It.IsAny<Visitor>()), Times.Once);
    }

    [Fact]
    public async Task GetVisitorByIdQueryHandler_ShouldReturnVisitor_WhenVisitorExists()
    {
        // Arrange
        var visitorId = 1;
        var expectedVisitor = new Visitor
        {
            VisitorId = visitorId,
            VisitorType = VisitorType.Regular,
            Height = 175,
            User = new User
            {
                UserId = visitorId,
                Username = "testuser",
                DisplayName = "Test User",
                Email = "test@example.com"
            }
        };

        _mockVisitorRepository.Setup(r => r.GetByIdAsync(visitorId))
            .ReturnsAsync(expectedVisitor);

        var handler = new GetVisitorByIdQueryHandler(_mockVisitorRepository.Object);
        var query = new GetVisitorByIdQuery(visitorId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(visitorId, result.VisitorId);
        Assert.Equal("testuser", result.User.Username);
    }

    [Fact]
    public async Task SearchVisitorsByNameQueryHandler_ShouldReturnMatchingVisitors()
    {
        // Arrange
        var searchName = "test";
        var expectedVisitors = new List<Visitor>
        {
            new()
            {
                VisitorId = 1,
                User = new User { Username = "testuser1", DisplayName = "Test User 1" }
            },
            new()
            {
                VisitorId = 2,
                User = new User { Username = "testuser2", DisplayName = "Test User 2" }
            }
        };

        _mockVisitorRepository.Setup(r => r.SearchByNameAsync(searchName))
            .ReturnsAsync(expectedVisitors);

        var handler = new SearchVisitorsByNameQueryHandler(_mockVisitorRepository.Object);
        var query = new SearchVisitorsByNameQuery(searchName);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, v => Assert.Contains("test", v.User.Username.ToLower()));
    }

    [Fact]
    public async Task UpdateVisitorBlacklistStatusCommandHandler_ShouldUpdateBlacklistStatus()
    {
        // Arrange
        var visitorId = 1;
        var visitor = new Visitor
        {
            VisitorId = visitorId,
            IsBlacklisted = false
        };

        _mockVisitorRepository.Setup(r => r.GetByIdAsync(visitorId))
            .ReturnsAsync(visitor);
        _mockVisitorRepository.Setup(r => r.UpdateAsync(It.IsAny<Visitor>()))
            .Returns(Task.CompletedTask);

        var handler = new UpdateVisitorBlacklistStatusCommandHandler(_mockVisitorRepository.Object);
        var command = new UpdateVisitorBlacklistStatusCommand(visitorId, true);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(visitor.IsBlacklisted);
        _mockVisitorRepository.Verify(r => r.UpdateAsync(visitor), Times.Once);
    }

    [Fact]
    public async Task GetVisitorHistoryQueryHandler_ShouldReturnCompleteHistory()
    {
        // Arrange
        var visitorId = 1;
        var visitor = new Visitor
        {
            VisitorId = visitorId,
            VisitorType = VisitorType.Member,
            Points = 100,
            Height = 175,
            IsBlacklisted = false,
            User = new User
            {
                UserId = visitorId,
                Username = "testuser",
                DisplayName = "Test User",
                Email = "test@example.com",
                PhoneNumber = "1234567890",
                BirthDate = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                RegisterTime = DateTime.UtcNow.AddDays(-30)
            }
        };

        var entryRecords = new List<EntryRecord>
        {
            new()
            {
                EntryRecordId = 1,
                VisitorId = visitorId,
                EntryTime = DateTime.UtcNow.AddDays(-1),
                ExitTime = DateTime.UtcNow.AddDays(-1).AddHours(3),
                EntryGate = "Main Gate",
                ExitGate = "Main Gate"
            }
        };

        _mockVisitorRepository.Setup(r => r.GetByIdAsync(visitorId))
            .ReturnsAsync(visitor);
        _mockEntryRecordRepository.Setup(r => r.GetByVisitorIdAsync(visitorId))
            .ReturnsAsync(entryRecords);

        var handler = new GetVisitorHistoryQueryHandler(_mockVisitorRepository.Object, _mockEntryRecordRepository.Object);
        var query = new GetVisitorHistoryQuery(visitorId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(visitorId, result.VisitorId);
        Assert.Equal("testuser", result.Username);
        Assert.Equal(1, result.TotalVisits);
        Assert.Single(result.RecentEntryRecords);
        Assert.NotNull(result.Age);
    }
}
