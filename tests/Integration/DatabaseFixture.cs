using DbApp.Infrastructure;
using EFCore.CheckConstraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace DbApp.Tests.Integration;

[Trait("Category", "Integration")]
public class DatabaseFixture : IDisposable
{
    public string ConnectionString { get; private set; }
    private readonly IHost _host;
    private bool _disposed = false;

    public DatabaseFixture()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        ConnectionString = configuration.GetConnectionString("TestDatabase")
            ?? "Data Source=localhost:1521/FREEPDB1;User Id=cheng;Password=876009;";
        Console.WriteLine($"Using connection string: {ConnectionString}");

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseOracle(ConnectionString);
                    options.UseEnumCheckConstraints();
                    options.UseValidationCheckConstraints();
                });
            })
            .Build();

        // 确保测试数据库存在并应用迁移    
        using var scope = _host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }

    public ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseOracle(ConnectionString)
            .UseEnumCheckConstraints()
            .UseValidationCheckConstraints()
            .Options;
        return new ApplicationDbContext(options);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // 释放托管资源  
                try
                {
                    using var context = CreateContext();
                    context.Database.EnsureDeleted();
                }
                catch
                {
                    // 忽略清理数据库时的异常  
                }

                _host?.Dispose();
            }

            _disposed = true;
        }
    }
}

[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
}
