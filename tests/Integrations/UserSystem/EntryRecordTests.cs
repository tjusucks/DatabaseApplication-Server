using System.Net;
using System.Net.Http.Json;
using DbApp.Application.UserSystem.EntryRecords;
using DbApp.Application.UserSystem.Visitors;
using DbApp.Domain.Constants.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Tests.Integrations.UserSystem;

[Collection("Database")]
public class EntryRecordTests(DatabaseFixture fixture) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await fixture.ResetDatabaseAsync();

    [Fact]
    public async Task CreateEntry_AddsPointsToVisitor_Success()
    {
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // Create a visitor first.
        var createVisitorCommand = new CreateVisitorCommand(
            Username: "testuser",
            PasswordHash: "hash123",
            Email: "test@example.com",
            DisplayName: "Test User",
            PhoneNumber: "1234567890",
            BirthDate: new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Gender: Gender.Male,
            VisitorType: VisitorType.Member,
            Height: 175
        );

        var visitorResponse = await client.PostAsJsonAsync("/api/user/visitors", createVisitorCommand);
        var visitorResult = await visitorResponse.Content.ReadFromJsonAsync<dynamic>();
        var visitorId = (int)visitorResult?.GetProperty("visitorId").GetInt32();

        // Create entry record.
        var createEntryCommand = new CreateEntryRecordCommand(
            VisitorId: visitorId,
            Type: "entry",
            GateName: "Main Gate",
            TicketId: null
        );

        var response = await client.PostAsJsonAsync("/api/user/entries", createEntryCommand);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Verify points were added.
        var context = fixture.DbContext;
        var visitor = await context.Visitors.FindAsync(visitorId);
        Assert.NotNull(visitor);
        Assert.Equal(MembershipConstants.PointsEarning.ParkEntry, visitor.Points);
    }

    [Fact]
    public async Task CreateEntry_OnBirthday_AddsBirthdayBonus_Success()
    {
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        var today = DateTime.UtcNow.Date;

        // Create a visitor with today as birthday.
        var createVisitorCommand = new CreateVisitorCommand(
            Username: "birthdayuser",
            PasswordHash: "hash123",
            Email: "birthday@example.com",
            DisplayName: "Birthday User",
            PhoneNumber: "1234567890",
            BirthDate: today,
            Gender: Gender.Female,
            VisitorType: VisitorType.Member,
            Height: 165
        );

        var visitorResponse = await client.PostAsJsonAsync("/api/user/visitors", createVisitorCommand);
        var visitorResult = await visitorResponse.Content.ReadFromJsonAsync<dynamic>();
        var visitorId = (int)visitorResult?.GetProperty("visitorId").GetInt32();

        // Create entry record.
        var createEntryCommand = new CreateEntryRecordCommand(
            VisitorId: visitorId,
            Type: "entry",
            GateName: "Birthday Gate",
            TicketId: null
        );

        var response = await client.PostAsJsonAsync("/api/user/entries", createEntryCommand);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Verify birthday bonus was added.
        var context = fixture.DbContext;
        var visitor = await context.Visitors.FindAsync(visitorId);
        Assert.NotNull(visitor);

        var expectedPoints = MembershipConstants.PointsEarning.ParkEntry +
                           MembershipConstants.PointsEarning.BirthdayBonus;
        Assert.Equal(expectedPoints, visitor.Points);
    }

    [Fact]
    public async Task CreateMultipleEntriesOnBirthday_OnlyGivesBirthdayBonusOnce()
    {
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        var today = DateTime.UtcNow.Date;

        // Create a visitor with today as birthday.
        var createVisitorCommand = new CreateVisitorCommand(
            Username: "multientry",
            PasswordHash: "hash123",
            Email: "multi@example.com",
            DisplayName: "Multi Entry User",
            PhoneNumber: "1234567890",
            BirthDate: today,
            Gender: Gender.Female,
            VisitorType: VisitorType.Member,
            Height: 170
        );

        var visitorResponse = await client.PostAsJsonAsync("/api/user/visitors", createVisitorCommand);
        var visitorResult = await visitorResponse.Content.ReadFromJsonAsync<dynamic>();
        var visitorId = (int)visitorResult?.GetProperty("visitorId").GetInt32();

        // Create first entry record.
        var firstEntryCommand = new CreateEntryRecordCommand(
            VisitorId: visitorId,
            Type: "entry",
            GateName: "Gate 1",
            TicketId: null
        );

        // Create second entry record (after exit).
        var secondEntryCommand = new CreateEntryRecordCommand(
            VisitorId: visitorId,
            Type: "entry",
            GateName: "Gate 2",
            TicketId: null
        );

        var firstResponse = await client.PostAsJsonAsync("/api/user/entries", firstEntryCommand);

        // Simulate exit first.
        var exitCommand = new CreateEntryRecordCommand(
            VisitorId: visitorId,
            Type: "exit",
            GateName: "Gate 1",
            TicketId: null
        );
        await client.PostAsJsonAsync("/api/user/entries", exitCommand);

        var secondResponse = await client.PostAsJsonAsync("/api/user/entries", secondEntryCommand);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Created, secondResponse.StatusCode);

        // Verify birthday bonus was only given once.
        var context = fixture.DbContext;
        var visitor = await context.Visitors.FindAsync(visitorId);
        Assert.NotNull(visitor);

        var expectedPoints = (MembershipConstants.PointsEarning.ParkEntry * 2) +
                           MembershipConstants.PointsEarning.BirthdayBonus; // Only once.
        Assert.Equal(expectedPoints, visitor.Points);
    }

    [Fact]
    public async Task CreateEntry_UpdatesMemberLevel_WhenPointsReachThreshold()
    {
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();

        // Create a visitor with points close to Silver threshold.
        var createVisitorCommand = new CreateVisitorCommand(
            Username: "leveluser",
            PasswordHash: "hash123",
            Email: "level@example.com",
            DisplayName: "Level User",
            PhoneNumber: "1234567890",
            BirthDate: new DateTime(1985, 5, 15, 0, 0, 0, DateTimeKind.Utc),
            Gender: Gender.Male,
            VisitorType: VisitorType.Member,
            Height: 180
        );

        var visitorResponse = await client.PostAsJsonAsync("/api/user/visitors", createVisitorCommand);
        var visitorResult = await visitorResponse.Content.ReadFromJsonAsync<dynamic>();
        var visitorId = (int)visitorResult?.GetProperty("visitorId").GetInt32();

        // Manually set points close to Silver threshold.
        var context = fixture.DbContext;
        var visitor = await context.Visitors.FindAsync(visitorId);
        visitor!.Points = MembershipConstants.PointsThresholds.Silver -
                         MembershipConstants.PointsEarning.ParkEntry + 1;
        await context.SaveChangesAsync();

        // Create entry record to push over threshold
        var createEntryCommand = new CreateEntryRecordCommand(
            VisitorId: visitorId,
            Type: "entry",
            GateName: "Upgrade Gate",
            TicketId: null
        );

        var response = await client.PostAsJsonAsync("/api/user/entries", createEntryCommand);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Verify member level was updated
        await context.Entry(visitor).ReloadAsync();
        Assert.Equal(MembershipConstants.LevelNames.Silver, visitor.MemberLevel);
        Assert.True(visitor.Points >= MembershipConstants.PointsThresholds.Silver);
    }

    [Fact]
    public async Task CreateEntryRecord_VisitorNotFound_DoesNotCreateEntry()
    {
        using var factory = new TestApiFactory(fixture);
        using var client = factory.CreateClient();
        var db = fixture.DbContext;

        var createEntryCommand = new CreateEntryRecordCommand(
            VisitorId: 9999, // Non-existent visitor ID
            Type: "entry",
            GateName: "Unknown Gate",
            TicketId: null
        );

        var response = await client.PostAsJsonAsync("/api/user/entries", createEntryCommand);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, await db.EntryRecords.CountAsync());
    }
}
