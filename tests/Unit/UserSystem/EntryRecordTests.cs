using DbApp.Application.UserSystem.EntryRecords;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using Moq;
using Xunit;

namespace DbApp.Tests.Unit.UserSystem;

/// <summary>
/// Unit tests for EntryRecord functionality.
/// </summary>
public class EntryRecordTests
{
    private readonly Mock<IEntryRecordRepository> _mockEntryRecordRepository;
    private readonly Mock<IVisitorRepository> _mockVisitorRepository;

    public EntryRecordTests()
    {
        _mockEntryRecordRepository = new Mock<IEntryRecordRepository>();
        _mockVisitorRepository = new Mock<IVisitorRepository>();
    }

    [Fact]
    public async Task RegisterEntryCommand_ValidVisitor_ShouldCreateEntryRecord()
    {
        // Arrange
        var visitorId = 1;
        var entryGate = "Main Entrance";
        var visitor = new Visitor
        {
            VisitorId = visitorId,
            IsBlacklisted = false,
            User = new User { UserId = visitorId, Username = "testuser" }
        };

        _mockVisitorRepository.Setup(x => x.GetByIdAsync(visitorId))
            .ReturnsAsync(visitor);
        _mockEntryRecordRepository.Setup(x => x.GetActiveEntryForVisitorAsync(visitorId))
            .ReturnsAsync((EntryRecord?)null);
        _mockEntryRecordRepository.Setup(x => x.CreateAsync(It.IsAny<EntryRecord>()))
            .ReturnsAsync(1);

        var handler = new RegisterEntryCommandHandler(_mockEntryRecordRepository.Object, _mockVisitorRepository.Object);
        var command = new RegisterEntryCommand(visitorId, entryGate);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(1, result);
        _mockEntryRecordRepository.Verify(x => x.CreateAsync(It.Is<EntryRecord>(er => 
            er.VisitorId == visitorId && 
            er.EntryGate == entryGate && 
            er.ExitTime == null)), Times.Once);
    }

    [Fact]
    public async Task RegisterEntryCommand_BlacklistedVisitor_ShouldThrowException()
    {
        // Arrange
        var visitorId = 1;
        var visitor = new Visitor
        {
            VisitorId = visitorId,
            IsBlacklisted = true,
            User = new User { UserId = visitorId, Username = "testuser" }
        };

        _mockVisitorRepository.Setup(x => x.GetByIdAsync(visitorId))
            .ReturnsAsync(visitor);

        var handler = new RegisterEntryCommandHandler(_mockEntryRecordRepository.Object, _mockVisitorRepository.Object);
        var command = new RegisterEntryCommand(visitorId, "Main Entrance");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));
        Assert.Contains("blacklisted", exception.Message);
    }

    [Fact]
    public async Task RegisterEntryCommand_VisitorAlreadyInPark_ShouldThrowException()
    {
        // Arrange
        var visitorId = 1;
        var visitor = new Visitor
        {
            VisitorId = visitorId,
            IsBlacklisted = false,
            User = new User { UserId = visitorId, Username = "testuser" }
        };
        var activeEntry = new EntryRecord
        {
            EntryRecordId = 1,
            VisitorId = visitorId,
            EntryTime = DateTime.UtcNow.AddHours(-1),
            EntryGate = "Main Entrance"
        };

        _mockVisitorRepository.Setup(x => x.GetByIdAsync(visitorId))
            .ReturnsAsync(visitor);
        _mockEntryRecordRepository.Setup(x => x.GetActiveEntryForVisitorAsync(visitorId))
            .ReturnsAsync(activeEntry);

        var handler = new RegisterEntryCommandHandler(_mockEntryRecordRepository.Object, _mockVisitorRepository.Object);
        var command = new RegisterEntryCommand(visitorId, "Main Entrance");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));
        Assert.Contains("already in the park", exception.Message);
    }

    [Fact]
    public async Task RegisterExitCommand_ValidExit_ShouldUpdateEntryRecord()
    {
        // Arrange
        var visitorId = 1;
        var exitGate = "Main Exit";
        var activeEntry = new EntryRecord
        {
            EntryRecordId = 1,
            VisitorId = visitorId,
            EntryTime = DateTime.UtcNow.AddHours(-2),
            EntryGate = "Main Entrance",
            ExitTime = null
        };

        _mockEntryRecordRepository.Setup(x => x.GetActiveEntryForVisitorAsync(visitorId))
            .ReturnsAsync(activeEntry);

        var handler = new RegisterExitCommandHandler(_mockEntryRecordRepository.Object);
        var command = new RegisterExitCommand(visitorId, exitGate);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(activeEntry.ExitTime);
        Assert.Equal(exitGate, activeEntry.ExitGate);
        _mockEntryRecordRepository.Verify(x => x.UpdateAsync(activeEntry), Times.Once);
    }

    [Fact]
    public async Task RegisterExitCommand_NoActiveEntry_ShouldThrowException()
    {
        // Arrange
        var visitorId = 1;
        _mockEntryRecordRepository.Setup(x => x.GetActiveEntryForVisitorAsync(visitorId))
            .ReturnsAsync((EntryRecord?)null);

        var handler = new RegisterExitCommandHandler(_mockEntryRecordRepository.Object);
        var command = new RegisterExitCommand(visitorId, "Main Exit");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));
        Assert.Contains("No active entry found", exception.Message);
    }
}
