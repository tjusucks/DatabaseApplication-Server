using System.Text.Json;
using System.Text.Json.Serialization;
using DbApp.Application.ResourceSystem.RideTrafficStats;
using DbApp.Infrastructure;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

// Load environment variables from .env file if it exists.
if (File.Exists(".env"))
{
    Env.Load();
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add MVC controllers and enum converters for API endpoints.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Convert enums to strings in JSON serialization.
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Register MediatR for CQRS pattern implementation.
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(DbApp.Application.IMediatorModule).Assembly);
});

// Register AutoMapper for DTO mapping  
builder.Services.AddAutoMapper(typeof(DbApp.Application.IMediatorModule).Assembly);

// Configure Entity Framework with Oracle database and check constraints.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection"));
    options.UseEnumCheckConstraints();
    options.UseValidationCheckConstraints();
});

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

// Register background services for ResourceSystem  
builder.Services.AddScoped<IRideTrafficStatService, RideTrafficStatService>();

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

// Add a root endpoint for the welcome page.
app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(@"
        <html>
            <head><title>Amusement Park Management System</title></head>
            <body>
                <h1>Welcome to the Amusement Park Management System API</h1>
                <ul>
                    <li><a href=""/scalar/v1"">Visit API Documentation (Scalar UI)</a></li>
                </ul>
            </body>
        </html>
    ");
});

// Map controller routes for API endpoints.
app.MapControllers();

// Start the application.
await app.RunAsync();
