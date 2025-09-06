using DbApp.Infrastructure;
using DbApp.Presentation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DbApp.Tests.Fixtures;

public class TestApiFactory(DatabaseFixture fixture) : WebApplicationFactory<Program>
{
    private readonly DatabaseFixture _fixture = fixture;
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext.
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();

            // Inject fixture's DbContext as singleton.
            services.AddSingleton(_fixture.DbContext);
        });
        builder.UseEnvironment("Testing");
    }
}
