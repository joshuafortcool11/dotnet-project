using HealthGuardRwanda.Core.Data;
using HealthGuardRwanda.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthGuardRwanda.Core.Repositories;

public class CitizenRepository : Repository<Citizen>
{
    public CitizenRepository(AppDbContext db) : base(db)
    {
    }

    public List<Citizen> FindByVillageIgnoreCaseOrderByLastNameAsc(string village) =>
        Db.Citizens
            .Where(c => c.Village.ToLower() == village.ToLower())
            .OrderBy(c => c.LastName)
            .ToList();

    public Citizen? FindByNationalId(string nationalId) =>
        Db.Citizens.FirstOrDefault(c => c.NationalId == nationalId);

    public List<Citizen> FindCitizensOfEnrollmentOfficer(Guid officerId) =>
        Db.Enrollments
            .Where(e => e.EnrollmentOfficer.Id == officerId)
            .Select(e => e.Citizen)
            .Distinct()
            .ToList();
}
