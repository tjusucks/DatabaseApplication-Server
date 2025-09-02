using DbApp.Application.UserSystem.Visitors;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using Moq;
using Xunit;

namespace Tests.UserSystem.Visitors;

/// <summary>
/// Unit tests for Visitor command handlers.
/// </summary>
public class VisitorCommandHandlerTests
{
    private readonly Mock<IVisitorRepository> _mockVisitorRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;

    public VisitorCommandHandlerTests()
    {
        _mockVisitorRepository = new Mock<IVisitorRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
    }

    [Fact]
    public async Task RegisterVisitorCommandHandler_WhenUserExists_ShouldCreateVisitor()
    {
        // Arrange
        var user = new User { UserId = 1, Username = "testuser" };
        var command = new RegisterVisitorCommand(1, 170);
        
        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);
        _mockVisitorRepository.Setup(x => x.GetByUserIdAsync(1))
            .ReturnsAsync((Visitor?)null);
        _mockVisitorRepository.Setup(x => x.CreateAsync(It.IsAny<Visitor>()))
            .ReturnsAsync(1);

        var handler = new RegisterVisitorCommandHandler(_mockVisitorRepository.Object, _mockUserRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(1, result);
        _mockVisitorRepository.Verify(x => x.CreateAsync(It.Is<Visitor>(v => 
            v.VisitorId == 1 && 
            v.Height == 170 && 
            v.VisitorType == VisitorType.Regular &&
            v.Points == 0)), Times.Once);
    }

    [Fact]
    public async Task RegisterVisitorCommandHandler_WhenUserDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var command = new RegisterVisitorCommand(999, 170);
        
        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        var handler = new RegisterVisitorCommandHandler(_mockVisitorRepository.Object, _mockUserRepository.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));
        
        Assert.Contains("User with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task RegisterVisitorCommandHandler_WhenVisitorAlreadyExists_ShouldThrowException()
    {
        // Arrange
        var user = new User { UserId = 1, Username = "testuser" };
        var existingVisitor = new Visitor { VisitorId = 1 };
        var command = new RegisterVisitorCommand(1, 170);
        
        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);
        _mockVisitorRepository.Setup(x => x.GetByUserIdAsync(1))
            .ReturnsAsync(existingVisitor);

        var handler = new RegisterVisitorCommandHandler(_mockVisitorRepository.Object, _mockUserRepository.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));
        
        Assert.Contains("Visitor already exists for user ID 1", exception.Message);
    }

    [Fact]
    public async Task UpgradeToMemberCommandHandler_WhenVisitorExists_ShouldUpgradeToMember()
    {
        // Arrange
        var visitor = new Visitor 
        { 
            VisitorId = 1, 
            VisitorType = VisitorType.Regular,
            Points = 1200,
            IsBlacklisted = false
        };
        var command = new UpgradeToMemberCommand(1);
        
        _mockVisitorRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(visitor);

        var handler = new UpgradeToMemberCommandHandler(_mockVisitorRepository.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(VisitorType.Member, visitor.VisitorType);
        Assert.NotNull(visitor.MemberSince);
        Assert.Equal("Silver", visitor.MemberLevel);
        _mockVisitorRepository.Verify(x => x.UpdateAsync(visitor), Times.Once);
    }

    [Fact]
    public async Task UpgradeToMemberCommandHandler_WhenVisitorIsBlacklisted_ShouldThrowException()
    {
        // Arrange
        var visitor = new Visitor 
        { 
            VisitorId = 1, 
            VisitorType = VisitorType.Regular,
            IsBlacklisted = true
        };
        var command = new UpgradeToMemberCommand(1);
        
        _mockVisitorRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(visitor);

        var handler = new UpgradeToMemberCommandHandler(_mockVisitorRepository.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));
        
        Assert.Contains("Cannot upgrade blacklisted visitor to member", exception.Message);
    }

    [Fact]
    public async Task AddPointsCommandHandler_WhenValidRequest_ShouldAddPoints()
    {
        // Arrange
        var visitor = new Visitor 
        { 
            VisitorId = 1, 
            Points = 500,
            MemberLevel = "Bronze"
        };
        var command = new AddPointsCommand(1, 600, "Test reward");
        
        _mockVisitorRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(visitor);

        var handler = new AddPointsCommandHandler(_mockVisitorRepository.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(1100, visitor.Points);
        Assert.Equal("Silver", visitor.MemberLevel); // Should upgrade to Silver
        _mockVisitorRepository.Verify(x => x.UpdateAsync(visitor), Times.Once);
    }

    [Fact]
    public async Task AddPointsCommandHandler_WhenNegativePoints_ShouldThrowException()
    {
        // Arrange
        var visitor = new Visitor { VisitorId = 1, Points = 500 };
        var command = new AddPointsCommand(1, -100, "Invalid");

        _mockVisitorRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(visitor);

        var handler = new AddPointsCommandHandler(_mockVisitorRepository.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Contains("Points to add must be positive", exception.Message);
    }

    [Fact]
    public async Task DeductPointsCommandHandler_WhenSufficientPoints_ShouldDeductPoints()
    {
        // Arrange
        var visitor = new Visitor 
        { 
            VisitorId = 1, 
            Points = 1500,
            MemberLevel = "Silver"
        };
        var command = new DeductPointsCommand(1, 600, "Purchase");
        
        _mockVisitorRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(visitor);

        var handler = new DeductPointsCommandHandler(_mockVisitorRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(900, visitor.Points);
        Assert.Equal("Bronze", visitor.MemberLevel); // Should downgrade to Bronze
        _mockVisitorRepository.Verify(x => x.UpdateAsync(visitor), Times.Once);
    }

    [Fact]
    public async Task DeductPointsCommandHandler_WhenInsufficientPoints_ShouldReturnFalse()
    {
        // Arrange
        var visitor = new Visitor 
        { 
            VisitorId = 1, 
            Points = 500,
            MemberLevel = "Bronze"
        };
        var command = new DeductPointsCommand(1, 600, "Purchase");
        
        _mockVisitorRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(visitor);

        var handler = new DeductPointsCommandHandler(_mockVisitorRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        Assert.Equal(500, visitor.Points); // Points should remain unchanged
        _mockVisitorRepository.Verify(x => x.UpdateAsync(It.IsAny<Visitor>()), Times.Never);
    }

    [Fact]
    public async Task UpdateVisitorCommandHandler_WhenValidHeight_ShouldUpdateVisitor()
    {
        // Arrange
        var visitor = new Visitor { VisitorId = 1, Height = 170 };
        var command = new UpdateVisitorCommand(1, 175);
        
        _mockVisitorRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(visitor);

        var handler = new UpdateVisitorCommandHandler(_mockVisitorRepository.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(175, visitor.Height);
        Assert.NotNull(visitor.UpdatedAt);
        _mockVisitorRepository.Verify(x => x.UpdateAsync(visitor), Times.Once);
    }

    [Fact]
    public async Task UpdateVisitorCommandHandler_WhenInvalidHeight_ShouldThrowException()
    {
        // Arrange
        var visitor = new Visitor { VisitorId = 1, Height = 170 };
        var command = new UpdateVisitorCommand(1, 400); // Invalid height
        
        _mockVisitorRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(visitor);

        var handler = new UpdateVisitorCommandHandler(_mockVisitorRepository.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => handler.Handle(command, CancellationToken.None));
        
        Assert.Contains("Height must be between 50 and 300 cm", exception.Message);
    }

    [Fact]
    public async Task BlacklistVisitorCommandHandler_WhenVisitorExists_ShouldBlacklistVisitor()
    {
        // Arrange
        var visitor = new Visitor { VisitorId = 1, IsBlacklisted = false };
        var command = new BlacklistVisitorCommand(1, "Inappropriate behavior");
        
        _mockVisitorRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(visitor);

        var handler = new BlacklistVisitorCommandHandler(_mockVisitorRepository.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(visitor.IsBlacklisted);
        Assert.NotNull(visitor.UpdatedAt);
        _mockVisitorRepository.Verify(x => x.UpdateAsync(visitor), Times.Once);
    }

    [Fact]
    public async Task RemoveFromBlacklistCommandHandler_WhenVisitorExists_ShouldRemoveFromBlacklist()
    {
        // Arrange
        var visitor = new Visitor { VisitorId = 1, IsBlacklisted = true };
        var command = new RemoveFromBlacklistCommand(1);
        
        _mockVisitorRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(visitor);

        var handler = new RemoveFromBlacklistCommandHandler(_mockVisitorRepository.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(visitor.IsBlacklisted);
        Assert.NotNull(visitor.UpdatedAt);
        _mockVisitorRepository.Verify(x => x.UpdateAsync(visitor), Times.Once);
    }
}
