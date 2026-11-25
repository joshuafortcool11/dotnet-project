using HealthGuardRwanda.Core.Data;
using HealthGuardRwanda.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthGuardRwanda.Core.Repositories;

public class EnrollmentRepository : Repository<Enrollment>
{
    public EnrollmentRepository(AppDbContext db) : base(db)
    {
    }

    public override List<Enrollment> GetAll() =>
        Db.Enrollments
            .Include(e => e.Citizen)
            .Include(e => e.EnrollmentOfficer)
            .ToList();

    public List<Enrollment> FindByStatusOrderByEnrollmentDateAsc(EnrollmentStatus status) =>
        Db.Enrollments
            .Where(e => e.Status == status)
            .OrderBy(e => e.EnrollmentDate)
            .Include(e => e.Citizen)
            .Include(e => e.EnrollmentOfficer)
            .ToList();

    public List<Enrollment> FindBetweenEnrollmentDate(DateOnly start, DateOnly end) =>
        Db.Enrollments
            .Where(e => e.EnrollmentDate >= start && e.EnrollmentDate <= end)
            .OrderBy(e => e.EnrollmentDate)
            .Include(e => e.Citizen)
            .Include(e => e.EnrollmentOfficer)
            .ToList();

    public bool ExistsByEnrollmentOfficerIdAndEnrollmentDateAndStatusNot(
        Guid enrollmentOfficerId, DateOnly enrollmentDate, EnrollmentStatus status) =>
        Db.Enrollments.Any(e => e.EnrollmentOfficerId == enrollmentOfficerId
                                && e.EnrollmentDate == enrollmentDate
                                && e.Status != status);

    public List<object[]> GroupByStatusAndCount() =>
        Db.Enrollments
            .GroupBy(e => e.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToList()
            .Select(row => new object[] { row.Status, row.Count })
            .ToList();

    public int CountByStatus(EnrollmentStatus status) =>
        Db.Enrollments.Count(e => e.Status == status);

    public int CountByEnrollmentDate(DateOnly date) =>
        Db.Enrollments.Count(e => e.EnrollmentDate == date);

    public int CancelDay(Guid enrollmentOfficerId, DateOnly date) =>
        Db.Enrollments
            .Where(e => e.EnrollmentOfficerId == enrollmentOfficerId
                        && e.EnrollmentDate == date
                        && e.Status != EnrollmentStatus.COVERED)
            .ExecuteUpdate(setters => setters.SetProperty(e => e.Status, EnrollmentStatus.REJECTED));
}
