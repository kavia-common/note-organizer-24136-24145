using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesBackend.Api.Middleware;
using NotesBackend.Api.Swagger;
using NotesBackend.Application.Auth;
using NotesBackend.Application.Notes;
using NotesBackend.Application.Users;
using NotesBackend.Infrastructure.Data;
using NotesBackend.Infrastructure.Repositories;
using NotesBackend.Security;

var builder = WebApplication.CreateBuilder(args);

// Configuration and logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Load Jwt settings from environment or appsettings
var jwtSettings = new JwtSettings
{
    Issuer = builder.Configuration["Jwt:Issuer"] ?? "notes_backend",
    Audience = builder.Configuration["Jwt:Audience"] ?? "notes_clients",
    Secret = builder.Configuration["JWT_SECRET"] ?? builder.Configuration["Jwt:Secret"] ?? "",
    ExpiryMinutes = int.TryParse(builder.Configuration["Jwt:ExpiryMinutes"], out var exp) ? exp : 60
};
if (string.IsNullOrWhiteSpace(jwtSettings.Secret))
{
    builder.Logging.AddConsole();
    builder.Services.AddSingleton(jwtSettings); // still add to avoid null refs
    builder.Logging.CreateLogger("Startup").LogWarning("Ocean: JWT secret not configured. Set JWT_SECRET or Jwt:Secret environment variable.");
}
else
{
    builder.Services.AddSingleton(jwtSettings);
}

// DbContext: default to in-memory for immediate usability. If connection string exists, use Postgres/SqlServer depending on prefix.
var connStr = builder.Configuration["DB_CONNECTION_STRING"] ?? builder.Configuration.GetConnectionString("Default");
if (!string.IsNullOrWhiteSpace(connStr))
{
    // Simple heuristic: support Npgsql or SqlServer based on content.
    if (connStr.Contains("Host=", StringComparison.OrdinalIgnoreCase) || connStr.Contains("Username=", StringComparison.OrdinalIgnoreCase))
    {
        // Npgsql
        builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connStr));
    }
    else
    {
        // Fallback to SqlServer
        builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connStr));
    }
}
else
{
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("notes_db"));
}

// DI registrations
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddSingleton<ITokenService>(sp =>
{
    var settings = sp.GetRequiredService<JwtSettings>();
    return new TokenService(settings);
});

// MVC and controllers
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Where(kv => kv.Value?.Errors.Count > 0)
                .Select(kv => new { field = kv.Key, errors = kv.Value!.Errors.Select(e => e.ErrorMessage) });
            return new BadRequestObjectResult(new
            {
                message = "Ocean: Some inputs need attention.",
                details = errors
            });
        };
    });

// Swagger/NSwag with JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOceanSwagger();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Auth
builder.Services.AddJwtAuthentication(jwtSettings);

var app = builder.Build();

// Migrate schema when using relational providers
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.IsRelational())
    {
        db.Database.Migrate();
    }
    else
    {
        db.Database.EnsureCreated();
    }
}

// Middleware
app.UseCors("AllowAll");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// OpenAPI UI
app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.Path = "/docs";
    config.DocumentPath = "/openapi/{documentName}.json";
});

// Map controllers
app.MapControllers();

// Health
app.MapGet("/", () => new { message = "Ocean: Healthy" })
   .WithName("HealthCheck");

// App Run
app.Run();