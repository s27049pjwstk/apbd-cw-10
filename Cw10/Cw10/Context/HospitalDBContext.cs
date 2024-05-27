using Cw10.Models;
using Microsoft.EntityFrameworkCore;

namespace Cw10.Context;

public class HospitalDbContext : DbContext
{
    public HospitalDbContext()
    {
    }

    public HospitalDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Prescription_Medicament> PrescriptionsMedicaments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "Data Source=db-mssql;Initial Catalog=s27049;Integrated Security=True;TrustServerCertificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>(opt =>
        {
            opt.HasKey(e => e.IdDoctor);
            opt.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            opt.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            opt.Property(e => e.Email).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Patient>(opt =>
        {
            opt.HasKey(e => e.IdPatient);
            opt.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            opt.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Medicament>(opt =>
        {
            opt.HasKey(e => e.IdMedicament);
            opt.Property(e => e.Name).HasMaxLength(100).IsRequired();
            opt.Property(e => e.Description).HasMaxLength(100).IsRequired();
            opt.Property(e => e.Type).HasMaxLength(100).IsRequired();

            opt.HasMany(e => e.PrescriptionMedicaments)
                .WithOne(e => e.Medicament)
                .HasForeignKey(e => e.IdMedicament);
        });

        modelBuilder.Entity<Prescription>(opt =>
        {
            opt.HasKey(e => e.IdPrescription);

            opt.HasOne(e => e.Doctor)
                .WithMany(e => e.Prescriptions)
                .HasForeignKey(e => e.IdDoctor);

            opt.HasOne(e => e.Patient)
                .WithMany(e => e.Prescriptions)
                .HasForeignKey(e => e.IdPatient);

            opt.HasMany(e => e.PrescriptionMedicaments)
                .WithOne(e => e.Prescription)
                .HasForeignKey(e => e.IdPrescription);
        });

        modelBuilder.Entity<Prescription_Medicament>(opt =>
        {
            opt.HasKey(e => new
            {
                e.IdPrescription,
                e.IdMedicament
            });
            opt.Property(e => e.Details)
                .HasMaxLength(100).IsRequired();
        });
    }
}