using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthGuardRwanda.Core.Models;

[Table("enrollment_officer")]
public class EnrollmentOfficer
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("sector_id")]
    public Guid? SectorId { get; set; }

    [ForeignKey(nameof(SectorId))]
    public Sector? Sector { get; set; }

    public List<HouseholdCategory> HouseholdCategories { get; set; } = new();
}
