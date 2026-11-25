using HealthGuardRwanda.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthGuardRwanda.Core.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<HouseholdCategory> HouseholdCategories => Set<HouseholdCategory>();
    public DbSet<Sector> Sectors => Set<Sector>();
    public DbSet<EnrollmentOfficer> EnrollmentOfficers => Set<EnrollmentOfficer>();
    public DbSet<Citizen> Citizens => Set<Citizen>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<CoverageVerification> CoverageVerifications => Set<CoverageVerification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Citizen>()
            .HasIndex(c => c.NationalId)
            .IsUnique();

        modelBuilder.Entity<Enrollment>()
            .Property(e => e.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Citizen)
            .WithMany()
            .HasForeignKey(e => e.CitizenId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.EnrollmentOfficer)
            .WithMany()
            .HasForeignKey(e => e.EnrollmentOfficerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EnrollmentOfficer>()
            .HasOne(p => p.Sector)
            .WithMany()
            .HasForeignKey(p => p.SectorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EnrollmentOfficer>()
            .HasMany(p => p.HouseholdCategories)
            .WithMany(c => c.Officers)
            .UsingEntity<Dictionary<string, object>>(
                "enrollment_officer_household_category",
                join => join.HasOne<HouseholdCategory>()
                    .WithMany()
                    .HasForeignKey("household_category_id")
                    .OnDelete(DeleteBehavior.Cascade),
                join => join.HasOne<EnrollmentOfficer>()
                    .WithMany()
                    .HasForeignKey("enrollment_officer_id")
                    .OnDelete(DeleteBehavior.Cascade),
                join => join.ToTable("enrollment_officer_household_category"));

        modelBuilder.Entity<CoverageVerification>()
            .HasOne(v => v.Enrollment)
            .WithMany()
            .HasForeignKey(v => v.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
