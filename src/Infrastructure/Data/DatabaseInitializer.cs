using DbApp.Domain.Entities.UserSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DbApp.Infrastructure.Data;

/// <summary>
/// Database initialization service for ensuring essential data exists.
/// This approach is more flexible than EF Core data seeding as it:
/// 1. Avoids migration conflicts with existing data
/// 2. Only creates data when needed
/// 3. Can be run safely multiple times
/// 4. Handles different deployment scenarios gracefully
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Ensures essential system data exists in the database.
    /// This method is idempotent and safe to call multiple times.
    /// </summary>
    /// <param name="serviceProvider">The service provider for dependency injection.</param>
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            // Ensure database is created and up to date
            await context.Database.MigrateAsync();

            // Initialize roles if they don't exist
            await EnsureRolesExistAsync(context, logger);

            logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }

    /// <summary>
    /// Ensures that essential roles exist in the database.
    /// Only creates roles that don't already exist.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger for tracking operations.</param>
    private static async Task EnsureRolesExistAsync(ApplicationDbContext context, ILogger logger)
    {
        // Check if any roles exist
        var existingRolesCount = await context.Roles.CountAsync();
        
        if (existingRolesCount > 0)
        {
            logger.LogInformation("Roles already exist in database ({Count} roles found). Skipping role initialization.", existingRolesCount);
            return;
        }

        logger.LogInformation("No roles found in database. Creating essential roles...");

        // Create essential roles
        var roles = new[]
        {
            new Role
            {
                RoleId = 1,
                RoleName = "Visitor",
                RoleDescription = "Regular park visitor with basic access",
                CreatedAt = DateTime.UtcNow
            },
            new Role
            {
                RoleId = 2,
                RoleName = "Member",
                RoleDescription = "Park member with additional benefits",
                CreatedAt = DateTime.UtcNow
            },
            new Role
            {
                RoleId = 3,
                RoleName = "Staff",
                RoleDescription = "Park staff member with operational access",
                CreatedAt = DateTime.UtcNow
            },
            new Role
            {
                RoleId = 4,
                RoleName = "Admin",
                RoleDescription = "System administrator with full access",
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Roles.AddRange(roles);
        await context.SaveChangesAsync();

        logger.LogInformation("Successfully created {Count} essential roles", roles.Length);
    }
}
