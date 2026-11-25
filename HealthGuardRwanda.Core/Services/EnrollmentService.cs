using HealthGuardRwanda.Core.Data;
using HealthGuardRwanda.Core.Models;
using HealthGuardRwanda.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HealthGuardRwanda.Core.Services;

public sealed class PageRequest
{
    public int Page { get; set; }
    public int Size { get; set; } = 5;
    public string SortProperty { get; set; } = "EnrollmentDate";
    public bool Descending { get; set; } = true;
}

public sealed class PageResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int Size { get; set; }
    public int TotalElements { get; set; }
    public int TotalPages { get; set; }
}

public class EnrollmentService : CrudService<Enrollment>
{
    private readonly EnrollmentRepository _repo;
    private readonly CoverageVerificationRepository _verificationRepo;
    private readonly AppDbContext _db;

    public EnrollmentService(
        EnrollmentRepository repo,
        CoverageVerificationRepository verificationRepo,
        AppDbContext db)
        : base(repo, new EntityMeta("Enrollment", "enrollment"))
    {
        _repo = repo;
        _verificationRepo = verificationRepo;
        _db = db;
    }

    public override string Save(Enrollment enrollment)
    {
        if (enrollment.EnrollmentDate < DateOnly.FromDateTime(DateTime.Now))
            return "Enrollment date cannot be in the past";

        if (_repo.ExistsByEnrollmentOfficerIdAndEnrollmentDateAndStatusNot(
                enrollment.EnrollmentOfficerId, enrollment.EnrollmentDate, EnrollmentStatus.REJECTED))
            return "Enrollment officer already processes an enrollment on that date";

        enrollment.Status = EnrollmentStatus.APPLIED;
        if (enrollment.CoverageYear == 0)
            enrollment.CoverageYear = enrollment.EnrollmentDate.Year;

        return base.Save(enrollment);
    }

    public override string Update(Guid id, Enrollment enrollment)
    {
        var existing = _repo.GetById(id);
        if (existing is null)
            return "The enrollment with that id does not exist";

        enrollment.Status = existing.Status;
        return base.Update(id, enrollment);
    }

    public string UpdateStatus(Guid id, EnrollmentStatus newStatus)
    {
        var enrollment = _repo.GetById(id);
        if (enrollment is null)
            return "The enrollment with that id does not exist";

        var current = enrollment.Status;
        var allowed = (current == EnrollmentStatus.APPLIED &&
                       (newStatus == EnrollmentStatus.PAYMENT_PENDING || newStatus == EnrollmentStatus.REJECTED))
                      || (current == EnrollmentStatus.PAYMENT_PENDING &&
                          (newStatus == EnrollmentStatus.COVERED || newStatus == EnrollmentStatus.REJECTED));

        if (!allowed)
            return $"Cannot change status from {current} to {newStatus}";

        enrollment.Status = newStatus;
        _repo.SaveChanges();
        return "Enrollment status updated successfully";
    }

    public override string Delete(Guid id)
    {
        if (_verificationRepo.ExistsByEnrollment(id))
            return "That enrollment has coverage verifications, delete them first";

        return base.Delete(id);
    }

    public List<Enrollment> FindByStatusOrderByEnrollmentDateAsc(EnrollmentStatus status) =>
        _repo.FindByStatusOrderByEnrollmentDateAsc(status);

    public List<Enrollment> FindBetweenEnrollmentDate(DateOnly start, DateOnly end) =>
        _repo.FindBetweenEnrollmentDate(start, end);

    public List<object[]> FindStatsByStatus() => _repo.GroupByStatusAndCount();

    public string CancelDay(Guid enrollmentOfficerId, DateOnly date)
    {
        var count = _repo.CancelDay(enrollmentOfficerId, date);
        return $"{count} enrollments cancelled";
    }

    public int CountByStatus(EnrollmentStatus status) => _repo.CountByStatus(status);

    public int CountByEnrollmentDate(DateOnly date) => _repo.CountByEnrollmentDate(date);

    public PageResult<Enrollment> FindPage(PageRequest request)
    {
        var query = _db.Enrollments
            .Include(e => e.Citizen)
            .Include(e => e.EnrollmentOfficer)
            .AsQueryable();

        query = request.SortProperty switch
        {
            "Status" => request.Descending ? query.OrderByDescending(e => e.Status) : query.OrderBy(e => e.Status),
            "CoverageYear" => request.Descending ? query.OrderByDescending(e => e.CoverageYear) : query.OrderBy(e => e.CoverageYear),
            "Premium" => request.Descending ? query.OrderByDescending(e => e.Premium) : query.OrderBy(e => e.Premium),
            _ => request.Descending ? query.OrderByDescending(e => e.EnrollmentDate) : query.OrderBy(e => e.EnrollmentDate)
        };

        var total = query.Count();
        var items = query.Skip(request.Page * request.Size).Take(request.Size).ToList();

        return new PageResult<Enrollment>
        {
            Items = items,
            Page = request.Page,
            Size = request.Size,
            TotalElements = total,
            TotalPages = request.Size == 0 ? 0 : (int)Math.Ceiling(total / (double)request.Size)
        };
    }
}
