using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthGuardRwanda.Core.Models;

[Table("coverage_verification")]
public class CoverageVerification
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("health_center")]
    public string HealthCenter { get; set; } = string.Empty;

    [Column("remarks")]
    public string Remarks { get; set; } = string.Empty;

    [Column("dependents")]
    public int Dependents { get; set; }

    [Column("verified_date")]
    public DateOnly VerifiedDate { get; set; }

    [Column("enrollment_id")]
    public Guid EnrollmentId { get; set; }

    [ForeignKey(nameof(EnrollmentId))]
    public Enrollment Enrollment { get; set; } = null!;
}
