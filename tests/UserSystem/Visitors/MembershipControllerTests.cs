using System.Net;
using System.Net.Http.Json;
using DbApp.Application.UserSystem.Visitors;
using DbApp.Application.UserSystem.Visitors.DTOs;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Infrastructure;
using DbApp.Presentation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.UserSystem.Visitors;

/// <summary>
/// Integration tests for MembershipController.
/// </summary>
public class MembershipControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _context;

    public MembershipControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Remove Oracle provider registration
                var oracleDescriptor = services.Where(d => d.ServiceType.ToString().Contains("Oracle")).ToList();
                foreach (var desc in oracleDescriptor)
                    services.Remove(desc);

                // Add in-memory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
                });
            });

            // Use test environment
            builder.UseEnvironment("Testing");
        });

        _client = _factory.CreateClient();

        // Get the test database context
        var scope = _factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

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
            }
        };
        _context.Visitors.AddRange(visitors);
        _context.SaveChanges();
    }

    [Fact]
    public async Task RegisterVisitor_WhenValidData_ShouldReturnCreated()
    {
        // Arrange
        var dto = new RegisterVisitorDto
        {
            UserId = 3,
            Height = 175
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/membership/register", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var visitor = await _context.Visitors.FindAsync(3);
        Assert.NotNull(visitor);
        Assert.Equal(175, visitor.Height);
        Assert.Equal(VisitorType.Regular, visitor.VisitorType);
    }

    [Fact]
    public async Task RegisterVisitor_WhenUserDoesNotExist_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new RegisterVisitorDto
        {
            UserId = 999,
            Height = 175
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/membership/register", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetVisitor_WhenVisitorExists_ShouldReturnVisitor()
    {
        // Act
        var response = await _client.GetAsync("/api/membership/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var visitor = await response.Content.ReadFromJsonAsync<VisitorResponseDto>();
        Assert.NotNull(visitor);
        Assert.Equal(1, visitor.VisitorId);
        Assert.Equal(500, visitor.Points);
        Assert.Equal("Bronze", visitor.MemberLevel);
    }

    [Fact]
    public async Task GetVisitor_WhenVisitorDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/membership/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllVisitors_ShouldReturnAllVisitors()
    {
        // Act
        var response = await _client.GetAsync("/api/membership");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var visitors = await response.Content.ReadFromJsonAsync<List<VisitorResponseDto>>();
        Assert.NotNull(visitors);
        Assert.Equal(2, visitors.Count);
    }

    [Fact]
    public async Task GetVisitorsByType_ShouldReturnFilteredVisitors()
    {
        // Act
        var response = await _client.GetAsync("/api/membership/by-type/Member");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var visitors = await response.Content.ReadFromJsonAsync<List<VisitorResponseDto>>();
        Assert.NotNull(visitors);
        Assert.Single(visitors);
        Assert.All(visitors, v => Assert.Equal(VisitorType.Member, v.VisitorType));
    }

    [Fact]
    public async Task UpgradeToMember_WhenValidVisitor_ShouldReturnOk()
    {
        // Act
        var response = await _client.PostAsync("/api/membership/1/upgrade", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var visitor = await _context.Visitors.FindAsync(1);
        Assert.NotNull(visitor);
        Assert.Equal(VisitorType.Member, visitor.VisitorType);
        Assert.NotNull(visitor.MemberSince);
    }

    [Fact]
    public async Task AddPoints_WhenValidData_ShouldReturnSuccess()
    {
        // Arrange
        var dto = new AddPointsDto
        {
            VisitorId = 1,
            Points = 600,
            Reason = "Test reward"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/membership/points/add", dto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PointsOperationResultDto>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(1100, result.CurrentPoints);
        Assert.True(result.LevelChanged);
        Assert.Equal("Silver", result.NewLevel);
    }

    [Fact]
    public async Task DeductPoints_WhenSufficientPoints_ShouldReturnSuccess()
    {
        // Arrange
        var dto = new DeductPointsDto
        {
            VisitorId = 2,
            Points = 500,
            Reason = "Purchase"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/membership/points/deduct", dto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PointsOperationResultDto>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(1000, result.CurrentPoints);
    }

    [Fact]
    public async Task DeductPoints_WhenInsufficientPoints_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new DeductPointsDto
        {
            VisitorId = 1,
            Points = 1000, // Visitor 1 has only 500 points
            Reason = "Purchase"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/membership/points/deduct", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PointsOperationResultDto>();
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Contains("Insufficient points", result.Message);
    }

    [Fact]
    public async Task UpdateVisitor_WhenValidData_ShouldReturnOk()
    {
        // Arrange
        var dto = new UpdateVisitorDto
        {
            Height = 180
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/membership/1", dto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var visitor = await _context.Visitors.FindAsync(1);
        Assert.NotNull(visitor);
        Assert.Equal(180, visitor.Height);
    }

    [Fact]
    public async Task BlacklistVisitor_WhenValidData_ShouldReturnOk()
    {
        // Arrange
        var dto = new BlacklistVisitorDto
        {
            Reason = "Inappropriate behavior"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/membership/1/blacklist", dto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var visitor = await _context.Visitors.FindAsync(1);
        Assert.NotNull(visitor);
        Assert.True(visitor.IsBlacklisted);
    }

    [Fact]
    public async Task GetMembershipStatistics_ShouldReturnStatistics()
    {
        // Act
        var response = await _client.GetAsync("/api/membership/statistics");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stats = await response.Content.ReadFromJsonAsync<MembershipStatistics>();
        Assert.NotNull(stats);
        Assert.Equal(2, stats.TotalVisitors);
        Assert.Equal(1, stats.TotalMembers);
        Assert.Equal(50m, stats.MembershipRate); // 1/2 * 100 = 50%
    }

    public void Dispose()
    {
        _context.Dispose();
        _client.Dispose();
    }
}
