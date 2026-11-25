using HealthGuardRwanda.Core.Models;
using HealthGuardRwanda.Core.Repositories;

namespace HealthGuardRwanda.Core.Services;

public class CitizenService : CrudService<Citizen>
{
    private readonly CitizenRepository _repo;
    private readonly EnrollmentOfficerRepository _officerRepo;

    public CitizenService(CitizenRepository repo, EnrollmentOfficerRepository officerRepo)
        : base(repo, new EntityMeta("Citizen", "citizen"))
    {
        _repo = repo;
        _officerRepo = officerRepo;
    }

    public List<Citizen> FindByVillageIgnoreCaseOrderByLastName(string village) =>
        _repo.FindByVillageIgnoreCaseOrderByLastNameAsc(village);

    public Citizen? FindByNationalId(string nationalId) => _repo.FindByNationalId(nationalId);

    public bool OfficerExists(Guid officerId) => _officerRepo.GetById(officerId) is not null;

    public List<Citizen> FindCitizensOfEnrollmentOfficer(Guid officerId) =>
        _repo.FindCitizensOfEnrollmentOfficer(officerId);
}
