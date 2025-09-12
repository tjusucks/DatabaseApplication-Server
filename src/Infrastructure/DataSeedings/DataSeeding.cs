using DbApp.Infrastructure.DataSeedings.ResourceSystem;
using DbApp.Infrastructure.DataSeedings.UserSystem;
using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.DataSeedings;

public static class DataSeeding
{
    public static void SeedData(DbContext dbContext)
    {
        dbContext.ChangeTracker.Clear();
        foreach (var seeder in Seeders)
        {
            seeder.Seed(dbContext);
        }
    }

    public static async Task SeedDataAsync(DbContext dbContext)
    {
        dbContext.ChangeTracker.Clear();
        foreach (var seeder in Seeders)
        {
            await seeder.SeedAsync(dbContext);
        }

    }
    private static readonly List<IDataSeeding> Seeders =
    [
        new RoleDataSeeding(),
        new VisitorDataSeeding(),
        new EmployeeDataSeeding(),
        new AmusementRideDataSeeding()
    ];
}
