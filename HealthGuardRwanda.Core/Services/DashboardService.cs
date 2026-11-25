using HealthGuardRwanda.Core.Data;
using HealthGuardRwanda.Core.Models;
using HealthGuardRwanda.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HealthGuardRwanda.Core.Services;

public class DashboardService
{
    private readonly AppDbContext _db;
    private readonly SectorService _sectorService;

    public DashboardService(AppDbContext db, SectorService sectorService)
    {
        _db = db;
        _sectorService = sectorService;
    }

    public Dictionary<string, long> Summary()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        return new Dictionary<string, long>
        {
            ["citizens"] = _db.Citizens.LongCount(),
            ["enrollmentOfficers"] = _db.EnrollmentOfficers.LongCount(),
            ["freeSectors"] = _sectorService.FindFreeSectors().LongCount(),
            ["enrollmentsToday"] = _db.Enrollments.LongCount(e => e.EnrollmentDate == today),
            ["applied"] = _db.Enrollments.LongCount(e => e.Status == EnrollmentStatus.APPLIED),
            ["verifications"] = _db.CoverageVerifications.LongCount()
        };
    }
}
