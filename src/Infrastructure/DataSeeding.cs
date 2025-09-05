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
}
