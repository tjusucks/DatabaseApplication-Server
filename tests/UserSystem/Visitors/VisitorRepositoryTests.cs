using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Infrastructure;
using DbApp.Infrastructure.Repositories.UserSystem;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests.UserSystem.Visitors;

/// <summary>
/// Integration tests for VisitorRepository.
/// </summary>
public class VisitorRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly VisitorRepository _repository;

    public VisitorRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new VisitorRepository(_context);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var role = new Role { RoleId = 1, RoleName = "Visitor" };
        _context.Roles.Add(role);

        var users = new[]
        {
            new User { UserId = 1, Username = "user1", Email = "user1@test.com", DisplayName = "User 1", RoleId = 1 },
            new User { UserId = 2, Username = "user2", Email = "user2@test.com", DisplayName = "User 2", RoleId = 1 },
            new User { UserId = 3, Username = "user3", Email = "user3@test.com", DisplayName = "User 3", RoleId = 1 }
        };
        _context.Users.AddRange(users);

        var visitors = new[]
        {
            new Visitor 
            { 
                VisitorId = 1, 
                VisitorType = VisitorType.Regular, 
                Points = 500, 
                Height = 170,
                MemberLevel = "Bronze"
            },
            new Visitor 
            { 
                VisitorId = 2, 
                VisitorType = VisitorType.Member, 
                Points = 1500, 
                Height = 165,
                MemberLevel = "Silver",
                MemberSince = DateTime.UtcNow.AddDays(-30)
            },
            new Visitor 
            { 
                VisitorId = 3, 
                VisitorType = VisitorType.Member, 
                Points = 6000, 
                Height = 180,
                MemberLevel = "Gold",
                MemberSince = DateTime.UtcNow.AddDays(-60)
            }
        };
        _context.Visitors.AddRange(visitors);
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateVisitor()
    {
        // Arrange
        var user = new User { UserId = 4, Username = "user4", Email = "user4@test.com", DisplayName = "User 4", RoleId = 1 };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var visitor = new Visitor
        {
            VisitorId = 4,
            VisitorType = VisitorType.Regular,
            Points = 0,
            Height = 175
        };

        // Act
        var result = await _repository.CreateAsync(visitor);

        // Assert
        Assert.Equal(4, result);
        var createdVisitor = await _context.Visitors.FindAsync(4);
        Assert.NotNull(createdVisitor);
        Assert.Equal(VisitorType.Regular, createdVisitor.VisitorType);
    }

    [Fact]
    public async Task GetByIdAsync_WhenVisitorExists_ShouldReturnVisitor()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.VisitorId);
        Assert.Equal(VisitorType.Regular, result.VisitorType);
        Assert.Equal(500, result.Points);
    }

    [Fact]
    public async Task GetByIdAsync_WhenVisitorDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenVisitorExists_ShouldReturnVisitor()
    {
        // Act
        var result = await _repository.GetByUserIdAsync(2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.VisitorId);
        Assert.Equal(VisitorType.Member, result.VisitorType);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllVisitors()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Contains(result, v => v.VisitorId == 1);
        Assert.Contains(result, v => v.VisitorId == 2);
        Assert.Contains(result, v => v.VisitorId == 3);
    }

    [Fact]
    public async Task GetByTypeAsync_ShouldReturnVisitorsOfSpecifiedType()
    {
        // Act
        var members = await _repository.GetByTypeAsync(VisitorType.Member);
        var regulars = await _repository.GetByTypeAsync(VisitorType.Regular);

        // Assert
        Assert.Equal(2, members.Count);
        Assert.All(members, v => Assert.Equal(VisitorType.Member, v.VisitorType));
        
        Assert.Single(regulars);
        Assert.All(regulars, v => Assert.Equal(VisitorType.Regular, v.VisitorType));
    }

    [Fact]
    public async Task GetByMemberLevelAsync_ShouldReturnVisitorsOfSpecifiedLevel()
    {
        // Act
        var silverMembers = await _repository.GetByMemberLevelAsync("Silver");
        var goldMembers = await _repository.GetByMemberLevelAsync("Gold");

        // Assert
        Assert.Single(silverMembers);
        Assert.Equal("Silver", silverMembers[0].MemberLevel);
        
        Assert.Single(goldMembers);
        Assert.Equal("Gold", goldMembers[0].MemberLevel);
    }

    [Fact]
    public async Task GetByPointsRangeAsync_ShouldReturnVisitorsInRange()
    {
        // Act
        var result = await _repository.GetByPointsRangeAsync(1000, 2000);

        // Assert
        Assert.Single(result);
        Assert.Equal(1500, result[0].Points);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateVisitor()
    {
        // Arrange
        var visitor = await _repository.GetByIdAsync(1);
        Assert.NotNull(visitor);
        visitor.Points = 800;
        visitor.MemberLevel = "Silver";

        // Act
        await _repository.UpdateAsync(visitor);

        // Assert
        var updatedVisitor = await _repository.GetByIdAsync(1);
        Assert.NotNull(updatedVisitor);
        Assert.Equal(800, updatedVisitor.Points);
        Assert.Equal("Silver", updatedVisitor.MemberLevel);
        Assert.NotNull(updatedVisitor.UpdatedAt);
    }

    [Fact]
    public async Task AddPointsAsync_ShouldAddPointsToVisitor()
    {
        // Arrange
        var originalPoints = 500;

        // Act
        await _repository.AddPointsAsync(1, 200);

        // Assert
        var visitor = await _repository.GetByIdAsync(1);
        Assert.NotNull(visitor);
        Assert.Equal(originalPoints + 200, visitor.Points);
    }

    [Fact]
    public async Task DeductPointsAsync_WhenSufficientPoints_ShouldDeductAndReturnTrue()
    {
        // Act
        var result = await _repository.DeductPointsAsync(2, 500);

        // Assert
        Assert.True(result);
        var visitor = await _repository.GetByIdAsync(2);
        Assert.NotNull(visitor);
        Assert.Equal(1000, visitor.Points); // 1500 - 500 = 1000
    }

    [Fact]
    public async Task DeductPointsAsync_WhenInsufficientPoints_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.DeductPointsAsync(1, 1000); // Visitor 1 has only 500 points

        // Assert
        Assert.False(result);
        var visitor = await _repository.GetByIdAsync(1);
        Assert.NotNull(visitor);
        Assert.Equal(500, visitor.Points); // Points should remain unchanged
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteVisitor()
    {
        // Arrange
        var visitor = await _repository.GetByIdAsync(1);
        Assert.NotNull(visitor);

        // Act
        await _repository.DeleteAsync(visitor);

        // Assert
        var deletedVisitor = await _repository.GetByIdAsync(1);
        Assert.Null(deletedVisitor);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
