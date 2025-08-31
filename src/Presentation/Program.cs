using System.Text.Json;
using System.Text.Json.Serialization;
using DbApp.Application.TicketingSystem.PriceRules;
using DbApp.Application.TicketingSystem.Promotions;
using DbApp.Application.TicketingSystem.TicketTypes;
using DbApp.Infrastructure;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using DbApp.Domain.Interfaces.TicketingSystem;
using DbApp.Infrastructure.Repositories.TicketingSystem;
using DbApp.Domain.Interfaces;

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

// In Program.cs or a service registration file

// 1. Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllTicketTypesQuery).Assembly));

// 2. Register your new repositories
builder.Services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
builder.Services.AddScoped<IPriceRuleRepository, PriceRuleRepository>();
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
builder.Services.AddScoped<IPriceHistoryRepository, PriceHistoryRepository>();
// ... and so on

// 3. Register a Unit of Work if you are using one
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();




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

// Register AutoMapper profiles for mapping entities to DTOs.
builder.Services.AddAutoMapper(cfg => { }, typeof(DbApp.Application.MappingProfile).Assembly);

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
