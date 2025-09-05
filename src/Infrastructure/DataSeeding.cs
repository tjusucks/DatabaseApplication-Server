using DbApp.Domain.Entities.UserSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure;

public static class DataSeeding
{
    public static void SeedData(DbContext dbContext)
    {
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

        dbContext.SaveChanges();
    }

    public static async Task SeedDataAsync(DbContext dbContext)
    {
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

        await dbContext.SaveChangesAsync();
    }

    private static List<Role> GetDefaultRoles()
    {
        return
        [
            new()
            {
                RoleId = 1,
                RoleName = "Visitor",
                RoleDescription = "General visitor with limited access.",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                RoleId = 2,
                RoleName = "Member",
                RoleDescription = "Member user with membership privileges.",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                RoleId = 3,
                RoleName = "Employee",
                RoleDescription = "Employee with internal system access.",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                RoleId = 4,
                RoleName = "Admin",
                RoleDescription = "System administrator with full permissions.",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        ];
    }
}
