using System.Text.Json;
using HealthGuardRwanda.Core.Controllers;
using HealthGuardRwanda.Core.Data;
using HealthGuardRwanda.Core.Repositories;
using HealthGuardRwanda.Core.Services;
using HealthGuardRwanda.Desktop.Features;
using HealthGuardRwanda.Desktop.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthGuardRwanda.Desktop;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        var connectionString = LoadConnectionString();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Information)
            .Options;

        using var db = new AppDbContext(options);
        if (!db.Database.CanConnect())
        {
            MessageBox.Show(
                "Cannot connect to PostgreSQL. Open appsettings.json and set the real password for the postgres user.",
                "HealthGuard Rwanda - database connection",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        db.Database.EnsureCreated();

        var categoryRepo = new HouseholdCategoryRepository(db);
        var sectorRepo = new SectorRepository(db);
        var officerRepo = new EnrollmentOfficerRepository(db);
        var citizenRepo = new CitizenRepository(db);
        var enrollmentRepo = new EnrollmentRepository(db);
        var verificationRepo = new CoverageVerificationRepository(db);

        var categoryService = new HouseholdCategoryService(categoryRepo);
        var sectorService = new SectorService(sectorRepo);
        var officerService = new EnrollmentOfficerService(officerRepo, categoryRepo, sectorRepo);
        var citizenService = new CitizenService(citizenRepo, officerRepo);
        var enrollmentService = new EnrollmentService(enrollmentRepo, verificationRepo, db);
        var verificationService = new CoverageVerificationService(verificationRepo, enrollmentRepo);
        var dashboardService = new DashboardService(db, sectorService);
        var seedService = new SeedService(db);

        var categoryController = new HouseholdCategoryController(categoryService);
        var sectorController = new SectorController(sectorService);
        var officerController = new EnrollmentOfficerController(officerService);
        var citizenController = new CitizenController(citizenService);
        var enrollmentController = new EnrollmentController(enrollmentService);
        var verificationController = new CoverageVerificationController(verificationService);
        var dashboardController = new DashboardController(dashboardService);
        var seedController = new SeedController(seedService);

        var registry = new FeatureRegistry(
            categoryController, sectorController, officerController, citizenController,
            enrollmentController, verificationController, dashboardController, seedController);

        Application.Run(new MainForm(
            registry.Build(),
            officerController, citizenController, categoryController,
            enrollmentController, verificationController, seedController,
            "Connected to PostgreSQL (healthguard_rwanda) - SQL logging ON"));
    }

    private static string LoadConnectionString()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable("HEALTHGUARD_CONNECTION_STRING");
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
            return fromEnvironment;

        using var document = JsonDocument.Parse(File.ReadAllText("appsettings.json"));
        return document.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("HealthGuardDb")
            .GetString()!;
    }
}
