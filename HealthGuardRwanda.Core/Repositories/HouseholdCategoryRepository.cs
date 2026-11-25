using HealthGuardRwanda.Core.Data;
using HealthGuardRwanda.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthGuardRwanda.Core.Repositories;

public class HouseholdCategoryRepository : Repository<HouseholdCategory>
{
    public HouseholdCategoryRepository(AppDbContext db) : base(db)
    {
    }

    public HouseholdCategory? FindByName(string name) =>
        Db.HouseholdCategories.FirstOrDefault(c => c.Name.ToLower() == name.ToLower());

    public List<HouseholdCategory> FindUnused() =>
        Db.HouseholdCategories
            .Where(c => !c.Officers.Any())
            .ToList();
}
