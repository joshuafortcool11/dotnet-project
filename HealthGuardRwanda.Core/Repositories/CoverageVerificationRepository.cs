using HealthGuardRwanda.Core.Data;
using HealthGuardRwanda.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthGuardRwanda.Core.Repositories;

public class CoverageVerificationRepository : Repository<CoverageVerification>
{
    public CoverageVerificationRepository(AppDbContext db) : base(db)
    {
    }

    public bool ExistsByEnrollment(Guid enrollmentId) =>
        Db.CoverageVerifications.Any(v => v.EnrollmentId == enrollmentId);

    public List<CoverageVerification> FindByCitizenNationalIdOrderByVerifiedDateDesc(string nationalId) =>
        Db.CoverageVerifications
            .Where(v => v.Enrollment.Citizen.NationalId == nationalId)
            .OrderByDescending(v => v.VerifiedDate)
            .Include(v => v.Enrollment)
                .ThenInclude(e => e.Citizen)
            .Include(v => v.Enrollment)
                .ThenInclude(e => e.EnrollmentOfficer)
            .ToList();
}
