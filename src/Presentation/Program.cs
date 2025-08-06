using DbApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add MVC controllers for API endpoints.
builder.Services.AddControllers();

// Register MediatR for CQRS pattern implementation.
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(DbApp.Application.IMediatorModule).Assembly);
});

// Configure Entity Framework with Oracle database.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

// Configure Redis caching.
builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection"));

// Register repository implementations for dependency injection.
builder.Services.Scan(scan => scan
    .FromAssemblies(typeof(ApplicationDbContext).Assembly)
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

// Auto decorate all repository interfaces with caching.
var repositoryInterfaces = typeof(ApplicationDbContext).Assembly
    .GetTypes()
    .Where(t => t.Name.EndsWith("Repository") && !t.Name.StartsWith("Cached"))
    .SelectMany(t => t.GetInterfaces());
foreach (var interfaceType in repositoryInterfaces)
{
    var cachedType = typeof(ApplicationDbContext).Assembly
        .GetTypes()
        .FirstOrDefault(t => t.Name == $"Cached{interfaceType.Name[1..]}");

    if (cachedType != null)
    {
        builder.Services.Decorate(interfaceType, cachedType);
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable OpenAPI endpoint for API documentation.
    app.MapOpenApi();

    // Enable Scalar UI for interactive API documentation.
    app.MapScalarApiReference();

    // Trigger auto migration for database schema updates.
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Force HTTPS redirection for security.
app.UseHttpsRedirection();

// Map controller routes for API endpoints.
app.MapControllers();

// Start the application.
await app.RunAsync();
