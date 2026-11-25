using HealthGuardRwanda.Core.Data;
using HealthGuardRwanda.Core.Models;

namespace HealthGuardRwanda.Core.Services;

public class SeedService
{
    private readonly AppDbContext _db;

    public SeedService(AppDbContext db)
    {
        _db = db;
    }

    public string Seed()
    {
        if (_db.Citizens.Any())
            return "Seed data already exists";

        var ubudehe1 = new HouseholdCategory { Name = "Ubudehe 1" };
        var ubudehe2 = new HouseholdCategory { Name = "Ubudehe 2" };
        var ubudehe3 = new HouseholdCategory { Name = "Ubudehe 3" };
        _db.HouseholdCategories.AddRange(ubudehe1, ubudehe2, ubudehe3,
            new HouseholdCategory { Name = "Ubudehe 4" });

        var sector1 = new Sector { SectorNumber = 1, Name = "Kigali Sector 1" };
        var sector2 = new Sector { SectorNumber = 2, Name = "Kigali Sector 2" };
        var sector3 = new Sector { SectorNumber = 3, Name = "Kigali Sector 3" };
        _db.Sectors.AddRange(sector1, sector2, sector3,
            new Sector { SectorNumber = 4, Name = "Kigali Sector 4" });

        var officer1 = new EnrollmentOfficer
        {
            FirstName = "Jean", LastName = "Mugabo", Email = "jean.mugabo@healthguard.rw",
            Sector = sector1, HouseholdCategories = new List<HouseholdCategory> { ubudehe1, ubudehe2 }
        };
        var officer2 = new EnrollmentOfficer
        {
            FirstName = "Marie", LastName = "Uwimana", Email = "marie.uwimana@healthguard.rw",
            Sector = sector2, HouseholdCategories = new List<HouseholdCategory> { ubudehe2 }
        };
        var officer3 = new EnrollmentOfficer
        {
            FirstName = "Paul", LastName = "Ndayisaba", Email = "paul.ndayisaba@healthguard.rw",
            Sector = sector3, HouseholdCategories = new List<HouseholdCategory> { ubudehe1, ubudehe3 }
        };
        _db.EnrollmentOfficers.AddRange(officer1, officer2, officer3);

        var citizen1 = new Citizen { FirstName = "Joshua", LastName = "Iradukunda", NationalId = "1198880123456789", Phone = "0780000001", Village = "Nyagatovu" };
        var citizen2 = new Citizen { FirstName = "Alice", LastName = "Adebayo", NationalId = "1198880123456790", Phone = "0780000002", Village = "Nyagatovu" };
        var citizen3 = new Citizen { FirstName = "Bob", LastName = "Mswitchi", NationalId = "1198880123456791", Phone = "0780000003", Village = "Gikomero" };
        var citizen4 = new Citizen { FirstName = "Carine", LastName = "Uwizeye", NationalId = "1198880123456792", Phone = "0780000004", Village = "Kacyiru" };
        var citizen5 = new Citizen { FirstName = "David", LastName = "Hakizimana", NationalId = "1198880123456793", Phone = "0780000005", Village = "Remera" };
        _db.Citizens.AddRange(citizen1, citizen2, citizen3, citizen4, citizen5);

        var enrollments = new List<Enrollment>
        {
            NewEnrollment(citizen1, officer1, "2026-10-20", EnrollmentStatus.PAYMENT_PENDING, 2026, 3000),
            NewEnrollment(citizen2, officer1, "2026-10-20", EnrollmentStatus.REJECTED, 2026, 3000),
            NewEnrollment(citizen1, officer1, "2026-10-25", EnrollmentStatus.COVERED, 2026, 3000),
            NewEnrollment(citizen1, officer1, "2026-11-05", EnrollmentStatus.APPLIED, 2026, 3000),
            NewEnrollment(citizen3, officer1, "2026-11-10", EnrollmentStatus.COVERED, 2026, 3000),
            NewEnrollment(citizen2, officer2, "2026-10-15", EnrollmentStatus.APPLIED, 2026, 3000),
            NewEnrollment(citizen4, officer2, "2026-11-02", EnrollmentStatus.PAYMENT_PENDING, 2026, 3000),
            NewEnrollment(citizen5, officer2, "2026-11-12", EnrollmentStatus.REJECTED, 2026, 3000),
            NewEnrollment(citizen4, officer3, "2026-10-28", EnrollmentStatus.APPLIED, 2026, 3000),
            NewEnrollment(citizen5, officer3, "2026-11-18", EnrollmentStatus.PAYMENT_PENDING, 2026, 3000)
        };
        _db.Enrollments.AddRange(enrollments);

        _db.CoverageVerifications.AddRange(
            NewVerification(enrollments[2], "Kimironko Health Center", "Coverage valid, consultation approved", 3, "2026-10-26"),
            NewVerification(enrollments[2], "Kimironko Health Center", "Follow-up visit", 2, "2026-11-03"),
            NewVerification(enrollments[4], "Kibagabaga Health Center", "Coverage valid", 1, "2026-11-11"),
            NewVerification(enrollments[4], "Kibagabaga Health Center", "Maternité check", 2, "2026-11-15"));

        _db.SaveChanges();
        return "Seed data loaded successfully";
    }

    private static Enrollment NewEnrollment(Citizen citizen, EnrollmentOfficer officer, string date,
        EnrollmentStatus status, int coverageYear, int premium) => new()
    {
        Citizen = citizen,
        EnrollmentOfficer = officer,
        EnrollmentDate = DateOnly.Parse(date),
        Status = status,
        CoverageYear = coverageYear,
        Premium = premium
    };

    private static CoverageVerification NewVerification(Enrollment enrollment, string center, string remarks,
        int dependents, string date) => new()
    {
        Enrollment = enrollment,
        HealthCenter = center,
        Remarks = remarks,
        Dependents = dependents,
        VerifiedDate = DateOnly.Parse(date)
    };
}
