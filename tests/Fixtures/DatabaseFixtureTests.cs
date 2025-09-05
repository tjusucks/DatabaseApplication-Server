using DbApp.Domain.Entities.UserSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Tests.Fixtures;

[Collection("Database")]
public class DatabaseFixtureTests(DatabaseFixture fixture) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await fixture.ResetDatabaseAsync();

    [Fact]
    public async Task InsertAndReset_ClearsSchema()
    {
        var db = fixture.DbContext;
        Assert.Equal(4, await db.Roles.CountAsync());
        Assert.Equal(0, await db.Users.CountAsync());

        // Insert test data.
        db.Roles.Add(new Role
        {
            RoleId = 1000,
            RoleName = "Test Role",
            IsSystemRole = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        db.Users.Add(new User
        {
            UserId = 1000,
            Username = "adminuser",
            Email = "adminuser@example.com",
            DisplayName = "Admin User",
            PasswordHash = "hashedpassword",
            RegisterTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            RoleId = 1000
        });
        await db.SaveChangesAsync();

        // Data exists.
        Assert.True(await db.Roles.AnyAsync(r => r.RoleId == 1000));
        Assert.True(await db.Users.AnyAsync(u => u.UserId == 1000));
    }

    [Fact]
    public async Task SchemaIsEmpty_BeforeEachTest()
    {
        var db = fixture.DbContext;
        Assert.False(await db.Roles.AnyAsync(r => r.RoleId == 1000));
        Assert.False(await db.Users.AnyAsync(u => u.UserId == 1000));

        Assert.Equal(4, await db.Roles.CountAsync());
        Assert.Equal(0, await db.Users.CountAsync());
    }
}
