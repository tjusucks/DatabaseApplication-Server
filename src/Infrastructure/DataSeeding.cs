using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure;

public static class DataSeeding
{
    public static void SeedData(DbContext dbContext)
    {
        dbContext.ChangeTracker.Clear();

        // Seed roles first
        var roles = GetDefaultRoles();
        foreach (var role in roles)
        {
            var existing = dbContext.Set<Role>().FirstOrDefault(r => r.RoleId == role.RoleId);
            if (existing == null)
            {
                dbContext.Set<Role>().Add(role);
            }
            else if (existing.RoleName != role.RoleName ||
                     existing.RoleDescription != role.RoleDescription ||
                     existing.IsSystemRole != role.IsSystemRole)
            {
                existing.RoleName = role.RoleName;
                existing.RoleDescription = role.RoleDescription;
                existing.IsSystemRole = role.IsSystemRole;
                existing.UpdatedAt = DateTime.UtcNow;
            }
        }

        // Save roles first to ensure they exist
        dbContext.SaveChanges();

        // Seed test visitors
        SeedTestVisitors(dbContext);

        dbContext.SaveChanges();
    }

    public static async Task SeedDataAsync(DbContext dbContext)
    {
        dbContext.ChangeTracker.Clear();

        // Seed roles first
        var roles = GetDefaultRoles();
        foreach (var role in roles)
        {
            var existing = await dbContext.Set<Role>().FirstOrDefaultAsync(r => r.RoleId == role.RoleId);
            if (existing == null)
            {
                await dbContext.Set<Role>().AddAsync(role);
            }
            else if (existing.RoleName != role.RoleName ||
                     existing.RoleDescription != role.RoleDescription ||
                     existing.IsSystemRole != role.IsSystemRole)
            {
                existing.RoleName = role.RoleName;
                existing.RoleDescription = role.RoleDescription;
                existing.IsSystemRole = role.IsSystemRole;
                existing.UpdatedAt = DateTime.UtcNow;
            }
        }

        // Save roles first to ensure they exist
        await dbContext.SaveChangesAsync();

        // Seed test visitors
        await SeedTestVisitorsAsync(dbContext);

        await dbContext.SaveChangesAsync();
    }

    private static List<Role> GetDefaultRoles()
    {
        var now = DateTime.UtcNow;
        return
        [
            new()
            {
                RoleId = 1,
                RoleName = "Visitor",
                RoleDescription = "General visitor, including members.",
                IsSystemRole = false,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                RoleId = 2,
                RoleName = "Employee",
                RoleDescription = "Employee with resource system access.",
                IsSystemRole = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                RoleId = 3,
                RoleName = "Manager",
                RoleDescription = "Manager with administrative permissions.",
                IsSystemRole = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                RoleId = 4,
                RoleName = "Admin",
                RoleDescription = "System administrator with full permissions.",
                IsSystemRole = true,
                CreatedAt = now,
                UpdatedAt = now
            }
        ];
    }

    private static void SeedTestVisitors(DbContext dbContext)
    {
        // Check if visitors already exist
        if (dbContext.Set<User>().Any())
        {
            return; // Data already seeded
        }

        var testUsers = GetTestUsers();
        var testVisitors = GetTestVisitors();

        // Add users first
        foreach (var user in testUsers)
        {
            dbContext.Set<User>().Add(user);
        }

        // Save users to get their IDs
        dbContext.SaveChanges();

        // Add visitors
        foreach (var visitor in testVisitors)
        {
            dbContext.Set<Visitor>().Add(visitor);
        }
    }

    private static async Task SeedTestVisitorsAsync(DbContext dbContext)
    {
        // Check if visitors already exist
        if (await dbContext.Set<User>().AnyAsync())
        {
            return; // Data already seeded
        }

        var testUsers = GetTestUsers();
        var testVisitors = GetTestVisitors();

        // Add users first
        foreach (var user in testUsers)
        {
            await dbContext.Set<User>().AddAsync(user);
        }

        // Save users to get their IDs
        await dbContext.SaveChangesAsync();

        // Add visitors
        foreach (var visitor in testVisitors)
        {
            await dbContext.Set<Visitor>().AddAsync(visitor);
        }
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
