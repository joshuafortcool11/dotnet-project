using HealthGuardRwanda.Core.Models;
using HealthGuardRwanda.Core.Repositories;

namespace HealthGuardRwanda.Core.Services;

public class CoverageVerificationService : CrudService<CoverageVerification>
{
    private readonly CoverageVerificationRepository _repo;
    private readonly EnrollmentRepository _enrollmentRepo;

    public CoverageVerificationService(CoverageVerificationRepository repo, EnrollmentRepository enrollmentRepo)
        : base(repo, new EntityMeta("Coverage verification", "coverage verification"))
    {
        _repo = repo;
        _enrollmentRepo = enrollmentRepo;
    }

    public override string Save(CoverageVerification verification)
    {
        var enrollment = _enrollmentRepo.GetById(verification.EnrollmentId);
        if (enrollment is null)
            return "The enrollment with that id does not exist";

        if (enrollment.Status != EnrollmentStatus.COVERED)
            return "Coverage verifications are only for COVERED enrollments";

        verification.VerifiedDate = DateOnly.FromDateTime(DateTime.Now);
        verification.Enrollment = enrollment;
        return base.Save(verification);
    }

    public List<CoverageVerification> FindByCitizenNationalIdOrderByVerifiedDateDesc(string nationalId) =>
        _repo.FindByCitizenNationalIdOrderByVerifiedDateDesc(nationalId);
}
