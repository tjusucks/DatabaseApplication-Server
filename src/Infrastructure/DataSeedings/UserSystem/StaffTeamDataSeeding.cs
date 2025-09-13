using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.DataSeedings.UserSystem;

/// <summary>
/// Data seeding for StaffTeam entity to provide basic team data for maintenance and inspection operations.
/// </summary>
public class StaffTeamDataSeeding : IDataSeeding
{
    public void Seed(DbContext dbContext)
    {
        if (dbContext.Set<StaffTeam>().Any())
        {
            return; // Data already seeded.
        }

        // Ensure employees exist before creating teams
        if (!dbContext.Set<Employee>().Any())
        {
            return; // Cannot create teams without employees as leaders
        }

        var testTeams = GetTestStaffTeams();
        dbContext.Set<StaffTeam>().AddRange(testTeams);
        dbContext.SaveChanges();
    }

    public async Task SeedAsync(DbContext dbContext)
    {
        if (await dbContext.Set<StaffTeam>().AnyAsync())
        {
            return; // Data already seeded.
        }

        // Ensure employees exist before creating teams
        if (!await dbContext.Set<Employee>().AnyAsync())
        {
            return; // Cannot create teams without employees as leaders
        }

        var testTeams = GetTestStaffTeams();
        await dbContext.Set<StaffTeam>().AddRangeAsync(testTeams);
        await dbContext.SaveChangesAsync();
    }

    private static List<StaffTeam> GetTestStaffTeams()
    {
        var now = DateTime.UtcNow;
        return
        [
            // Maintenance Teams (Mechanic type)
            new()
            {
                TeamId = 1,
                TeamName = "维护团队A",
                TeamType = TeamType.Mechanic,
                LeaderId = 101, // John Smith (Ride Operator) - from EmployeeDataSeeding
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                TeamId = 2,
                TeamName = "维护团队B",
                TeamType = TeamType.Mechanic,
                LeaderId = 103, // Mike Johnson (Maintenance Technician) - from EmployeeDataSeeding
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                TeamId = 3,
                TeamName = "高级维护团队",
                TeamType = TeamType.Mechanic,
                LeaderId = 201, // Sarah Williams (Operations Manager) - from EmployeeDataSeeding
                CreatedAt = now,
                UpdatedAt = now
            },
            
            // Inspection Teams (Inspector type)
            new()
            {
                TeamId = 4,
                TeamName = "安全检查团队A",
                TeamType = TeamType.Inspector,
                LeaderId = 102, // Jane Doe (Customer Service Representative) - from EmployeeDataSeeding
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                TeamId = 5,
                TeamName = "安全检查团队B",
                TeamType = TeamType.Inspector,
                LeaderId = 202, // David Brown (Guest Services Manager) - from EmployeeDataSeeding
                CreatedAt = now,
                UpdatedAt = now
            },
            
            // Mixed Teams (Mixed type)
            new()
            {
                TeamId = 6,
                TeamName = "综合运维团队",
                TeamType = TeamType.Mixed,
                LeaderId = 201, // Sarah Williams (Operations Manager) - from EmployeeDataSeeding
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                TeamId = 7,
                TeamName = "应急响应团队",
                TeamType = TeamType.Mixed,
                LeaderId = 202, // David Brown (Guest Services Manager) - from EmployeeDataSeeding
                CreatedAt = now,
                UpdatedAt = now
            }
        ];
    }
}
