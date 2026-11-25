using HealthGuardRwanda.Core.Models;
using HealthGuardRwanda.Core.Repositories;

namespace HealthGuardRwanda.Core.Services;

public class SectorService : CrudService<Sector>
{
    private readonly SectorRepository _repo;

    public SectorService(SectorRepository repo)
        : base(repo, new EntityMeta("Sector", "sector"))
    {
        _repo = repo;
    }

    public Sector? FindBySectorNumber(int sectorNumber) => _repo.FindBySectorNumber(sectorNumber);

    public List<Sector> FindFreeSectors() => _repo.FindFreeSectors();
}
