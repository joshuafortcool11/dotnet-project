using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthGuardRwanda.Core.Models;

[Table("sector")]
public class Sector
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("sector_number")]
    public int SectorNumber { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;
}
