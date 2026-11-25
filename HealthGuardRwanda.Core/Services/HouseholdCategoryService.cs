using HealthGuardRwanda.Core.Models;
using HealthGuardRwanda.Core.Repositories;

namespace HealthGuardRwanda.Core.Services;

public class HouseholdCategoryService : CrudService<HouseholdCategory>
{
    private readonly HouseholdCategoryRepository _repo;

    public HouseholdCategoryService(HouseholdCategoryRepository repo)
        : base(repo, new EntityMeta("Household category", "household category"))
    {
        _repo = repo;
    }

    public HouseholdCategory? FindByName(string name) => _repo.FindByName(name);

    public List<HouseholdCategory> FindUnused() => _repo.FindUnused();
}
