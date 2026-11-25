using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HealthGuardRwanda.Core.Models;

[Table("household_category")]
public class HouseholdCategory
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public List<EnrollmentOfficer> Officers { get; set; } = new();
}
