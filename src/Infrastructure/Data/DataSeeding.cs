using DbApp.Domain.Entities.UserSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.Data;

/// <summary>
/// Data seeding configuration for essential system data.
/// </summary>
public static class DataSeeding
{
    /// <summary>
    /// Seeds essential data required for system operation.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    public static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Roles - Essential for user creation
        // Use static dates to avoid EF Core model changes warning
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                RoleId = 1,
                RoleName = "Visitor",
                CreatedAt = seedDate
            },
            new Role
            {
                RoleId = 2,
                RoleName = "Member",
                CreatedAt = seedDate
            },
            new Role
            {
                RoleId = 3,
                RoleName = "Staff",
                CreatedAt = seedDate
            },
            new Role
            {
                RoleId = 4,
                RoleName = "Admin",
                CreatedAt = seedDate
            }
        );
    }
}
