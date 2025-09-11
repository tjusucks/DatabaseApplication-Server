using Microsoft.EntityFrameworkCore;

namespace DbApp.Infrastructure.DataSeedings;

public interface IDataSeeding
{
    void Seed(DbContext dbContext);
    Task SeedAsync(DbContext dbContext);
}
