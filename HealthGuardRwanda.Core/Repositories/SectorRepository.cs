using HealthGuardRwanda.Core.Data;
using HealthGuardRwanda.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthGuardRwanda.Core.Repositories;

public class SectorRepository : Repository<Sector>
{
    public SectorRepository(AppDbContext db) : base(db)
    {
    }

    public Sector? FindBySectorNumber(int sectorNumber) =>
        Db.Sectors.FirstOrDefault(s => s.SectorNumber == sectorNumber);

    public bool ExistsBySector(Guid sectorId) =>
        Db.EnrollmentOfficers.Any(p => p.SectorId == sectorId);

    public List<Sector> FindFreeSectors() =>
        Db.Sectors
            .Where(s => !Db.EnrollmentOfficers.Any(o => o.SectorId == s.Id))
            .OrderBy(s => s.SectorNumber)
            .ToList();
}
