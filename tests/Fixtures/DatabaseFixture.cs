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
        // Load .env file from Presentation directory
        var currentDir = Directory.GetCurrentDirectory();

        // Try multiple possible paths
        var possiblePaths = new[]
        {
            Path.Combine(currentDir, "..", "..", "..", "src", "Presentation", ".env"),
            Path.Combine(currentDir, "..", "..", "..", "..", "src", "Presentation", ".env"),
            Path.Combine(currentDir, "src", "Presentation", ".env"),
            Path.Combine(currentDir, "..", "src", "Presentation", ".env")
        };

        string? envPath = null;
        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
            {
                envPath = path;
                break;
            }
        }

        if (envPath != null)
        {
            Env.Load(envPath);
        }

        var testConnection = Env.GetString("TestOracleConnection");

        var oracleConnectionString = testConnection ??
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
      
    // 添加 null 检查  
    if (_connection != null)  
    {  
        await _connection.DisposeAsync();  
    }  
}
}
