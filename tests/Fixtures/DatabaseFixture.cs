using System.Data.Common;
using DbApp.Infrastructure;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Respawn;

namespace DbApp.Tests.Fixtures;

public class DatabaseFixture : IAsyncLifetime
{
    public ApplicationDbContext DbContext { get; }
    private Respawner _respawner = null!;
    private DbConnection _connection = null!;

    public DatabaseFixture()
    {
        var oracleConnectionString = Env.GetString("TestOracleConnection") ??
            "Data Source=localhost:1521/FREEPDB1;User ID=TESTUSER;Password=TESTPASSWORD;";

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseOracle(oracleConnectionString)
            .UseEnumCheckConstraints()
            .UseValidationCheckConstraints()
            .UseSeeding((context, changed) =>
            {
                DataSeeding.SeedData(context);
            })
            .UseAsyncSeeding(async (context, changed, cancellationToken) =>
            {
                await DataSeeding.SeedDataAsync(context);
            })
            .Options;

        DbContext = new ApplicationDbContext(options);
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_connection);
        await DbContext.Database.EnsureCreatedAsync();
    }

    public async Task InitializeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.Database.MigrateAsync();

        var builder = new DbConnectionStringBuilder
        {
            ConnectionString = DbContext.Database.GetConnectionString()
        };
        string schema = (builder["User ID"]?.ToString() ?? "TESTUSER").ToUpper();

        var respawnerOptions = new RespawnerOptions
        {
            SchemasToInclude = [schema],
            DbAdapter = DbAdapter.Oracle
        };

        _connection = DbContext.Database.GetDbConnection();
        await _connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_connection, respawnerOptions);
    }

    public async Task DisposeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
