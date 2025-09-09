using System.Net;
using System.Net.Http.Json;
using DbApp.Application.TicketingSystem.Reservations;
using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.TicketingSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Tests.Fixtures;

namespace DbApp.Tests.Integrations.TicketingSystem.Reservations;

/// <summary>
/// 票种查询基础集成测试
/// </summary>
[Collection("Database")]
public class BasicTicketTypeTests(DatabaseFixture fixture) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await fixture.ResetDatabaseAsync();

    [Fact]
    public async Task GetAvailableTicketTypes_ShouldReturnTicketTypes()
    {
        // Arrange
        await SeedBasicTestDataAsync();

        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/ticketing/ticket-types");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<List<TicketTypeDto>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task CalculatePrice_WithValidRequest_ShouldReturnCalculation()
    {
        // Arrange
        await SeedBasicTestDataAsync();

        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        var request = new CalculatePriceRequestDto
        {
            VisitorId = 1001,
            Items = new List<ReservationItemRequestDto>
            {
                new() { TicketTypeId = 1001, Quantity = 1 }
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/ticketing/reservations/calculate-price", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ReservationPriceCalculationDto>();
        Assert.NotNull(result);
        Assert.True(result.TotalAmount > 0);
    }

    [Fact]
    public async Task CreateReservation_WithValidRequest_ShouldSucceed()
    {
        // Arrange
        await SeedBasicTestDataAsync();

        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        var request = new CreateReservationRequestDto
        {
            VisitorId = 1001,
            Items = new List<ReservationItemRequestDto>
            {
                new() { TicketTypeId = 1001, Quantity = 1 }
            },
            ContactPhone = "123-456-7890",
            ContactEmail = "test@example.com"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/ticketing/reservations", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ReservationDto>();
        Assert.NotNull(result);
        Assert.Equal(1001, result.VisitorId);
    }

    private async Task SeedBasicTestDataAsync()
    {
        var db = fixture.DbContext;

        // 创建基础角色
        var role = new Role
        {
            RoleId = 1001,
            RoleName = "TestRole",
            IsSystemRole = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Roles.Add(role);

        // 创建基础用户
        var user = new User
        {
            UserId = 1001,
            Username = "testuser",
            Email = "test@example.com",
            DisplayName = "Test User",
            PasswordHash = "hashedpassword",
            RegisterTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            RoleId = 1001
        };
        db.Users.Add(user);

        // 创建基础访客
        var visitor = new Visitor
        {
            VisitorId = 1001,
            VisitorType = VisitorType.Regular,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Visitors.Add(visitor);

        // 创建基础票种
        var ticketType = new TicketType
        {
            TicketTypeId = 1001,
            TypeName = "Test Ticket",
            Description = "Test ticket type",
            BasePrice = 99.99m,
            ApplicableCrowd = ApplicableCrowd.Adult,
            MaxSaleLimit = 1000,
            RulesText = "Test rules",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.TicketTypes.Add(ticketType);

        await db.SaveChangesAsync();
    }
}
