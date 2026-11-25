using HealthGuardRwanda.Core.Data;
using HealthGuardRwanda.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthGuardRwanda.Core.Repositories;

public class EnrollmentOfficerRepository : Repository<EnrollmentOfficer>
{
    public EnrollmentOfficerRepository(AppDbContext db) : base(db)
    {
    }

    public List<EnrollmentOfficer> FindByHouseholdCategoryName(string name) =>
        Db.EnrollmentOfficers
            .Where(o => o.HouseholdCategories.Any(c => c.Name.ToLower() == name.ToLower()))
            .Include(o => o.Sector)
            .Include(o => o.HouseholdCategories)
            .ToList();

    public bool ExistsBySector(Guid sectorId) =>
        Db.EnrollmentOfficers.Any(p => p.SectorId == sectorId);

    public EnrollmentOfficer? FindByEmail(string email) =>
        Db.EnrollmentOfficers.Include(p => p.Sector).FirstOrDefault(p => p.Email == email);

    public EnrollmentOfficer? GetWithCategories(Guid id) =>
        Db.EnrollmentOfficers.Include(p => p.HouseholdCategories).Include(p => p.Sector).FirstOrDefault(p => p.Id == id);
}
