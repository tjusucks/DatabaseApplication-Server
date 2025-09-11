using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.DataSeedings.UserSystem;

public class VisitorDataSeeding : IDataSeeding
{
    public void Seed(DbContext dbContext)
    {
        if (dbContext.Set<User>().Any() || dbContext.Set<Visitor>().Any())
        {
            return; // Data already seeded.
        }
        dbContext.Set<User>().AddRange(GetTestUsers());
        dbContext.Set<Visitor>().AddRange(GetTestVisitors());
        dbContext.SaveChanges();
    }

    public Task SeedAsync(DbContext dbContext)
    {
        if (dbContext.Set<User>().Any() || dbContext.Set<Visitor>().Any())
        {
            return Task.CompletedTask; // Data already seeded.
        }
        dbContext.Set<User>().AddRange(GetTestUsers());
        dbContext.Set<Visitor>().AddRange(GetTestVisitors());
        return dbContext.SaveChangesAsync();
    }

    private static List<User> GetTestUsers()
    {
        var now = DateTime.UtcNow;
        return
        [
            new()
            {
                UserId = 1,
                Username = "alice_member",
                PasswordHash = "hashedpassword123",
                Email = "alice@example.com",
                DisplayName = "Alice Johnson",
                PhoneNumber = "1234567891",
                BirthDate = new DateTime(1985, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                Gender = Gender.Female,
                RegisterTime = now,
                PermissionLevel = 1,
                RoleId = 1, // Visitor role
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                UserId = 2,
                Username = "bob_regular",
                PasswordHash = "hashedpassword123",
                Email = null,
                DisplayName = "Bob Smith",
                PhoneNumber = "1234567892",
                BirthDate = new DateTime(1990, 7, 20, 0, 0, 0, DateTimeKind.Utc),
                Gender = Gender.Male,
                RegisterTime = now,
                PermissionLevel = 1,
                RoleId = 1, // Visitor role
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                UserId = 3,
                Username = "charlie_kid",
                PasswordHash = "hashedpassword123",
                Email = "charlie@family.com",
                DisplayName = "Charlie Brown",
                PhoneNumber = null,
                BirthDate = new DateTime(2010, 12, 5, 0, 0, 0, DateTimeKind.Utc),
                Gender = Gender.Male,
                RegisterTime = now,
                PermissionLevel = 1,
                RoleId = 1, // Visitor role
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                UserId = 4,
                Username = "diana_senior",
                PasswordHash = "hashedpassword123",
                Email = "diana@senior.com",
                DisplayName = "Diana Wilson",
                PhoneNumber = "1234567894",
                BirthDate = new DateTime(1955, 8, 30, 0, 0, 0, DateTimeKind.Utc),
                Gender = Gender.Female,
                RegisterTime = now,
                PermissionLevel = 1,
                RoleId = 1, // Visitor role
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                UserId = 5,
                Username = "emma_teen",
                PasswordHash = "hashedpassword123",
                Email = "emma@teen.com",
                DisplayName = "Emma Davis",
                PhoneNumber = "1234567895",
                BirthDate = new DateTime(2005, 4, 12, 0, 0, 0, DateTimeKind.Utc),
                Gender = Gender.Female,
                RegisterTime = now,
                PermissionLevel = 1,
                RoleId = 1, // Visitor role
                CreatedAt = now,
                UpdatedAt = now
            }
        ];
    }

    private static List<Visitor> GetTestVisitors()
    {
        var now = DateTime.UtcNow;
        return
        [
            new()
            {
                VisitorId = 1,
                VisitorType = VisitorType.Member,
                Points = 1250,
                MemberLevel = "Silver",
                MemberSince = now.AddDays(-90),
                IsBlacklisted = false,
                Height = 165,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                VisitorId = 2,
                VisitorType = VisitorType.Regular,
                Points = 0,
                MemberLevel = null,
                MemberSince = null,
                IsBlacklisted = false,
                Height = 180,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                VisitorId = 3,
                VisitorType = VisitorType.Regular,
                Points = 0,
                MemberLevel = null,
                MemberSince = null,
                IsBlacklisted = false,
                Height = 140,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                VisitorId = 4,
                VisitorType = VisitorType.Member,
                Points = 3500,
                MemberLevel = "Gold",
                MemberSince = now.AddDays(-365),
                IsBlacklisted = false,
                Height = 160,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                VisitorId = 5,
                VisitorType = VisitorType.Regular,
                Points = 0,
                MemberLevel = null,
                MemberSince = null,
                IsBlacklisted = false,
                Height = 155,
                CreatedAt = now,
                UpdatedAt = now
            }
        ];
    }
}
