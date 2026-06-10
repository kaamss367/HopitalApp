using System.Reflection.Emit;
using HopitalApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HopitalApp.Data
{
    public class HopitalDbContext : DbContext
    {
        public DbSet<Specialite> Specialites => Set<Specialite>();
        public DbSet<Medecin> Medecins => Set<Medecin>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<RendezVous> RendezVous => Set<RendezVous>();
        public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();

        public HopitalDbContext(DbContextOptions<HopitalDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // SPECIALITE
            modelBuilder.Entity<Specialite>(entity =>
            {
                entity.ToTable("Specialite");

                entity.HasKey(s => s.Id);

                entity.Property(s => s.Nom)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(s => s.Nom)
                    .IsUnique();
            });

            // MEDECIN
            modelBuilder.Entity<Medecin>(entity =>
            {
                entity.ToTable("Medecin");

                entity.HasKey(m => m.Id);

                entity.Property(m => m.Nom)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(m => m.Prenom)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(m => m.SpecialiteId)
                    .IsRequired();

                entity.HasOne(m => m.Specialite)
                    .WithMany(s => s.Medecins)
                    .HasForeignKey(m => m.SpecialiteId)
                    .IsRequired();

                entity.HasIndex(m => m.SpecialiteId);
            });

            // PATIENT
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("Patient");

                entity.HasKey(p => p.Id);

                entity.Property(p => p.Nom)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(p => p.Prenom)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(p => p.Telephone)
                    .HasMaxLength(20);

                entity.Property(p => p.Email)
                    .HasMaxLength(150);

                entity.HasIndex(p => p.Email);
            });

            // RENDEZ-VOUS
            modelBuilder.Entity<RendezVous>(entity =>
            {
                entity.ToTable("RendezVous");

                entity.HasKey(r => r.Id);

                entity.Property(r => r.PatientId)
                    .IsRequired();

                entity.Property(r => r.MedecinId)
                    .IsRequired();

                entity.Property(r => r.DateDebut)
                    .IsRequired();

                entity.Property(r => r.DateFin)
                    .IsRequired();

                entity.Property(r => r.InformationsComplementaires)
                    .HasColumnType("text");

                entity.HasOne(r => r.Patient)
                    .WithMany(p => p.RendezVous)
                    .HasForeignKey(r => r.PatientId)
                    .IsRequired();

                entity.HasOne(r => r.Medecin)
                    .WithMany(m => m.RendezVous)
                    .HasForeignKey(r => r.MedecinId)
                    .IsRequired();

                entity.HasIndex(r => r.PatientId);
                entity.HasIndex(r => r.MedecinId);
                entity.HasIndex(r => r.DateDebut);
                entity.HasIndex(r => r.DateFin);
            });

            // UTILISATEUR
            modelBuilder.Entity<Utilisateur>(entity =>
            {
                entity.ToTable("Utilisateur");

                entity.HasKey(u => u.Id);

                entity.Property(u => u.Login)
                    .HasMaxLength(100);

                entity.Property(u => u.MotDePasse)
                    .HasMaxLength(255);

                entity.Property(u => u.Role)
                    .HasMaxLength(20);

                entity.HasIndex(u => u.Login)
                    .IsUnique();
            });
        }
    }
}