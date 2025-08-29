using System.Text.Json;
using System.Text.Json.Serialization;
using DbApp.Infrastructure;
using DotNetEnv;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using static DbApp.Domain.Exceptions;

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

// Register AutoMapper for DTO mapping.
builder.Services.AddAutoMapper(cfg => { }, typeof(DbApp.Application.MappingProfile).Assembly);

// Configure Entity Framework with Oracle database and check constraints.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection"));
    options.UseEnumCheckConstraints();
    options.UseValidationCheckConstraints();
    options.UseSeeding((context, changed) =>
    {
        DataSeeding.SeedData(context);
    });
    options.UseAsyncSeeding(async (context, changed, cancellationToken) =>
    {
        await DataSeeding.SeedDataAsync(context);
    });
});

// Configure Redis caching.
builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection"));

// Register repository implementations for dependency injection.
builder.Services.Scan(scan => scan
    .FromAssemblies(typeof(ApplicationDbContext).Assembly)
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository") || type.Name.EndsWith("Service")))
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

// Register exception handling middleware.
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/problem+json";
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        var statusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            ForbiddenException => StatusCodes.Status403Forbidden,
            ValidationException => StatusCodes.Status400BadRequest,
            ConflictException => StatusCodes.Status409Conflict,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = statusCode;

        var statusInfo = new Dictionary<int, (string Type, string Title)>
        {
            [StatusCodes.Status404NotFound] = ("https://tools.ietf.org/html/rfc9110#section-15.5.5", "Not Found"),
            [StatusCodes.Status400BadRequest] = ("https://tools.ietf.org/html/rfc9110#section-15.5.1", "Bad Request"),
            [StatusCodes.Status403Forbidden] = ("https://tools.ietf.org/html/rfc9110#section-15.5.4", "Forbidden"),
            [StatusCodes.Status409Conflict] = ("https://tools.ietf.org/html/rfc9110#section-15.5.2", "Conflict"),
            [StatusCodes.Status401Unauthorized] = ("https://tools.ietf.org/html/rfc9110#section-15.5.3", "Unauthorized"),
        };

        var (type, title) = statusInfo.TryGetValue(statusCode, out var info)
            ? info
            : ("about:blank", "Internal Server Error");

        var problemDetails = new
        {
            type,
            title,
            status = statusCode,
            detail = exception?.Message,
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});

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

namespace DbApp.Presentation
{
    public partial class Program
    {
        protected Program() { }
    }
}