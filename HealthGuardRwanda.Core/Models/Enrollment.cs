using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthGuardRwanda.Core.Models;

[Table("enrollment")]
public class Enrollment
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("enrollment_date")]
    public DateOnly EnrollmentDate { get; set; }

    [Column("status")]
    public EnrollmentStatus Status { get; set; }

    [Column("coverage_year")]
    public int CoverageYear { get; set; }

    [Column("premium")]
    public int Premium { get; set; }

    [Column("citizen_id")]
    public Guid CitizenId { get; set; }

    [ForeignKey(nameof(CitizenId))]
    public Citizen Citizen { get; set; } = null!;

    [Column("enrollment_officer_id")]
    public Guid EnrollmentOfficerId { get; set; }

    [ForeignKey(nameof(EnrollmentOfficerId))]
    public EnrollmentOfficer EnrollmentOfficer { get; set; } = null!;
}
