using Microsoft.EntityFrameworkCore;
using Mio_Rest_Api.Entities;

namespace Mio_Rest_Api.Data
{
    public class ContextApplication : DbContext
    {
        public ContextApplication(DbContextOptions<ContextApplication> options)
            : base(options)
        {
        }

        public virtual DbSet<ReservationEntity> Reservations { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<OccupationStatus> OccupationStatus { get; set; }
        public virtual DbSet<MenuEntity> MenuDuJour { get; set; }
        public virtual DbSet<UserEntity> Users { get; set; }
        public virtual DbSet<TableEntity> Tables { get; set; }
        public virtual DbSet<Allocation> Allocations { get; set; }
        public virtual DbSet<HECStatut> HECStatuts { get; set; } // Ajout pour HECStatut
        public virtual DbSet<Commentaire> Commentaires { get; set; } // Ajout pour Commentaire

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReservationEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedBy).HasMaxLength(50).IsUnicode(true);
                entity.Property(e => e.UpdatedBy).HasMaxLength(50).IsUnicode(true);
                entity.Property(e => e.CanceledBy).HasMaxLength(50).IsUnicode(true);
                entity.Property(e => e.OccupationStatusOnBook).HasMaxLength(20).IsUnicode(true);
                entity.Property(e => e.Comment).HasMaxLength(1000).IsUnicode(true);
                entity.Property(e => e.Placed).HasMaxLength(1).IsUnicode(false);
                entity.Property(e => e.IsPowerUser).HasMaxLength(1).IsUnicode(false);
                entity.Property(e => e.Status).HasMaxLength(1).IsUnicode(false);
                entity.Property(e => e.FreeTable21).HasMaxLength(50).IsUnicode(true);
                entity.HasOne(e => e.Client).WithMany().HasForeignKey(e => e.IdClient);
            });

            modelBuilder.Entity<MenuEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Plat).HasMaxLength(200).IsUnicode(true);
                entity.Property(e => e.Entree).HasMaxLength(200).IsUnicode(true);
                entity.Property(e => e.DessertJour).HasMaxLength(200).IsUnicode(true);
                entity.Property(e => e.Cheesecake).HasMaxLength(200).IsUnicode(true);
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(50).IsUnicode(true);
                entity.Property(e => e.Prenom).HasMaxLength(50).IsUnicode(true);
                entity.Property(e => e.Telephone).HasMaxLength(50).IsUnicode(true);
                entity.Property(e => e.Email).HasMaxLength(255).IsUnicode(true);
            });

            modelBuilder.Entity<OccupationStatus>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OccStatus).HasMaxLength(20).IsUnicode(true);
            });

            // Configuration de l'entité User
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Password).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Nom).HasMaxLength(200).IsUnicode(true);
                entity.Property(e => e.Prenom).HasMaxLength(200).IsUnicode(true);
            });

            modelBuilder.Entity<TableEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(50).IsUnicode(true);
                entity.Property(e => e.Capacity).IsRequired();
            });

            modelBuilder.Entity<Allocation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Period).HasMaxLength(10).IsRequired();
                entity.Property(e => e.IsMultiTable).HasMaxLength(1).IsUnicode(true);
                entity.HasOne(e => e.Reservation)
                    .WithMany(r => r.Allocations)
                    .HasForeignKey(e => e.ReservationId);

                entity.HasOne(e => e.Table)
                    .WithMany(t => t.Allocations)
                    .HasForeignKey(e => e.TableId);
            });

            // Configuration de HECStatut
            modelBuilder.Entity<HECStatut>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Statut).HasMaxLength(255).IsRequired();
                entity.Property(e => e.CreatedBy).HasMaxLength(50).IsUnicode(true);
                entity.HasOne(e => e.Reservation)
                    .WithMany(r => r.HECStatuts)
                    .HasForeignKey(e => e.ReservationId);
            });

            // Configuration de Commentaire
            modelBuilder.Entity<Commentaire>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Message).HasMaxLength(2000).IsRequired(); // Longueur ajustée à 2000
                entity.Property(e => e.Auteur).HasMaxLength(50).IsUnicode(true);
                entity.HasOne(e => e.Reservation)
                    .WithMany(r => r.Commentaires)
                    .HasForeignKey(e => e.ReservationId);
            });
        }
    }
}
