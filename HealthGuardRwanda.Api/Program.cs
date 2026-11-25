using System.Text.Json.Serialization;
using HealthGuardRwanda.Core.Controllers;
using HealthGuardRwanda.Core.Data;
using HealthGuardRwanda.Core.Repositories;
using HealthGuardRwanda.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("HEALTHGUARD_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("HealthGuardDb");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Set ConnectionStrings:HealthGuardDb or HEALTHGUARD_CONNECTION_STRING.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
        .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services.AddScoped<HouseholdCategoryRepository>();
builder.Services.AddScoped<SectorRepository>();
builder.Services.AddScoped<EnrollmentOfficerRepository>();
builder.Services.AddScoped<CitizenRepository>();
builder.Services.AddScoped<EnrollmentRepository>();
builder.Services.AddScoped<CoverageVerificationRepository>();

builder.Services.AddScoped<HouseholdCategoryService>();
builder.Services.AddScoped<SectorService>();
builder.Services.AddScoped<EnrollmentOfficerService>();
builder.Services.AddScoped<CitizenService>();
builder.Services.AddScoped<EnrollmentService>();
builder.Services.AddScoped<CoverageVerificationService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<SeedService>();

builder.Services.AddScoped<HouseholdCategoryController>();
builder.Services.AddScoped<SectorController>();
builder.Services.AddScoped<EnrollmentOfficerController>();
builder.Services.AddScoped<CitizenController>();
builder.Services.AddScoped<EnrollmentController>();
builder.Services.AddScoped<CoverageVerificationController>();
builder.Services.AddScoped<DashboardController>();
builder.Services.AddScoped<SeedController>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/", () => "HealthGuard Rwanda API");
app.MapControllers();

app.Run();
