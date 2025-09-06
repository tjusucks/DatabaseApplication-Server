using System.Net;
using System.Net.Http.Json;
using DbApp.Application.UserSystem.Visitors;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Tests.Integrations.UserSystem;

[Collection("Database")]
public class VisitorTests(DatabaseFixture fixture) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await fixture.ResetDatabaseAsync();

    [Fact]
    public async Task CreateVisitor_ReturnsSuccess_AndCreatesUser()
    {
        // Arrange
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        var createCommand = new CreateVisitorCommand(
            Username: "testuser123",
            PasswordHash: "hashedpassword123",
            Email: "test@example.com",
            DisplayName: "Test User",
            PhoneNumber: "1234567890",
            BirthDate: new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Gender: Gender.Male,
            VisitorType: VisitorType.Regular,
            Height: 175
        );

        // Act
        var response = await client.PostAsJsonAsync("/api/user/visitors", createCommand);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        Assert.NotNull(result);

        // Verify visitor and user were created in database
        var context = fixture.DbContext;

        var visitor = await context.Visitors
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.User.Username == "testuser123");

        Assert.NotNull(visitor);
        Assert.NotNull(visitor.User);
        Assert.Equal("testuser123", visitor.User.Username);
        Assert.Equal("test@example.com", visitor.User.Email);
        Assert.Equal(VisitorType.Regular, visitor.VisitorType);
        Assert.Equal(175, visitor.Height);
    }

    [Fact]
    public async Task UpdateVisitor_ReturnsSuccess_AndUpdatesUser()
    {
        // Arrange
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // First create a visitor
        var createCommand = new CreateVisitorCommand(
            Username: "updatetest",
            PasswordHash: "hashedpassword123",
            Email: "update@example.com",
            DisplayName: "Update Test",
            PhoneNumber: "1111111111",
            BirthDate: new DateTime(1985, 5, 15, 0, 0, 0, DateTimeKind.Utc),
            Gender: Gender.Female,
            VisitorType: VisitorType.Regular,
            Height: 160
        );

        var createResponse = await client.PostAsJsonAsync("/api/user/visitors", createCommand);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createResult = await createResponse.Content.ReadFromJsonAsync<dynamic>();
        var visitorId = (int)createResult?.GetProperty("visitorId").GetInt32();

        var updateCommand = new UpdateVisitorCommand(
            VisitorId: visitorId,
            DisplayName: "Updated Display Name",
            PhoneNumber: "2222222222",
            BirthDate: new DateTime(1986, 6, 16, 0, 0, 0, DateTimeKind.Utc),
            Gender: Gender.Male,
            VisitorType: VisitorType.Member,
            Height: 165
        );

        // Act
        var response = await client.PutAsJsonAsync($"/api/user/visitors/{visitorId}", updateCommand);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify visitor and user were updated in database
        var context = fixture.DbContext;

        var visitor = await context.Visitors
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.VisitorId == visitorId);

        Assert.NotNull(visitor);
        Assert.NotNull(visitor.User);
        Assert.Equal("Updated Display Name", visitor.User.DisplayName);
        Assert.Equal("2222222222", visitor.User.PhoneNumber);
        Assert.Equal(new DateTime(1986, 6, 16, 0, 0, 0, DateTimeKind.Utc), visitor.User.BirthDate);
        Assert.Equal(Gender.Male, visitor.User.Gender);
        Assert.Equal(VisitorType.Member, visitor.VisitorType);
        Assert.Equal(165, visitor.Height);
    }

    [Fact]
    public async Task DeleteVisitor_ReturnsSuccess_AndCascadeDeletesUser()
    {
        // Arrange
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // First create a visitor
        var createCommand = new CreateVisitorCommand(
            Username: "deletetest",
            PasswordHash: "hashedpassword123",
            Email: "delete@example.com",
            DisplayName: "Delete Test",
            PhoneNumber: "3333333333",
            BirthDate: new DateTime(1992, 3, 10, 0, 0, 0, DateTimeKind.Utc),
            Gender: Gender.Male,
            VisitorType: VisitorType.Regular,
            Height: 170
        );

        var createResponse = await client.PostAsJsonAsync("/api/user/visitors", createCommand);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createResult = await createResponse.Content.ReadFromJsonAsync<dynamic>();
        var visitorId = (int)createResult?.GetProperty("visitorId").GetInt32();

        // Verify visitor and user exist before deletion
        var context = fixture.DbContext;
        var visitorBeforeDelete = await context.Visitors
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.VisitorId == visitorId);

        Assert.NotNull(visitorBeforeDelete);
        Assert.NotNull(visitorBeforeDelete.User);

        // Act
        var response = await client.DeleteAsync($"/api/user/visitors/{visitorId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify visitor and user were cascade deleted from database
        var contextAfter = fixture.DbContext;

        var visitorAfterDelete = await contextAfter.Visitors
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.VisitorId == visitorId);
        Assert.Null(visitorAfterDelete);

        var userAfterDelete = await contextAfter.Users
            .FirstOrDefaultAsync(u => u.UserId == visitorId);
        Assert.Null(userAfterDelete);
    }
}
