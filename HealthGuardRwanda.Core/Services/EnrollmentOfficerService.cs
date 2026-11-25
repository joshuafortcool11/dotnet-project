using HealthGuardRwanda.Core.Models;
using HealthGuardRwanda.Core.Repositories;

namespace HealthGuardRwanda.Core.Services;

public class EnrollmentOfficerService : CrudService<EnrollmentOfficer>
{
    private readonly EnrollmentOfficerRepository _repo;
    private readonly HouseholdCategoryRepository _categoryRepo;
    private readonly SectorRepository _sectorRepo;

    public EnrollmentOfficerService(
        EnrollmentOfficerRepository repo,
        HouseholdCategoryRepository categoryRepo,
        SectorRepository sectorRepo)
        : base(repo, new EntityMeta("Enrollment officer", "enrollment officer"))
    {
        _repo = repo;
        _categoryRepo = categoryRepo;
        _sectorRepo = sectorRepo;
    }

    public override string Save(EnrollmentOfficer entity)
    {
        if (entity.SectorId is Guid sectorId && _sectorRepo.ExistsBySector(sectorId))
            return "That sector is taken by another enrollment officer";

        return base.Save(entity);
    }

    public override string Update(Guid id, EnrollmentOfficer entity)
    {
        var existing = _repo.GetById(id);
        if (existing is null)
            return "The enrollment officer with that id does not exist";

        if (entity.SectorId is Guid sectorId && sectorId != existing.SectorId && _sectorRepo.ExistsBySector(sectorId))
            return "That sector is taken by another enrollment officer";

        return base.Update(id, entity);
    }

    public EnrollmentOfficer? FindByEmail(string email) => _repo.FindByEmail(email);

    public List<EnrollmentOfficer> FindByHouseholdCategoryName(string name) =>
        _repo.FindByHouseholdCategoryName(name);

    public string AddHouseholdCategory(Guid officerId, Guid categoryId)
    {
        var officer = _repo.GetWithCategories(officerId);
        if (officer is null)
            return "The enrollment officer with that id does not exist";

        var category = _categoryRepo.GetById(categoryId);
        if (category is null)
            return "That household category does not exist";

        if (officer.HouseholdCategories.Any(c => c.Id == categoryId))
            return "The enrollment officer already has that household category";

        officer.HouseholdCategories.Add(category);
        _repo.SaveChanges();
        return "Household category added to the enrollment officer";
    }

    public string MoveToSector(Guid officerId, int sectorNumber)
    {
        var officer = _repo.GetById(officerId);
        if (officer is null)
            return "The enrollment officer with that id does not exist";

        var sector = _sectorRepo.FindBySectorNumber(sectorNumber);
        if (sector is null)
            return "The sector with that sectorNumber does not exist";

        if (sector.Id != officer.SectorId && _sectorRepo.ExistsBySector(sector.Id))
            return "That sector is taken by another enrollment officer";

        officer.Sector = sector;
        officer.SectorId = sector.Id;
        _repo.SaveChanges();
        return "Sector changed successfully";
    }
}
