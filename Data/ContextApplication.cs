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
        public virtual DbSet<Table> Tables { get; set; }
        public virtual DbSet<Assignation> Assignations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReservationEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedBy).HasMaxLength(50).IsUnicode(true);
                entity.Property(e => e.UpdatedBy).HasMaxLength(50).IsUnicode(true);
                entity.Property(e => e.OccupationStatusOnBook).HasMaxLength(20).IsUnicode(true);
                entity.Property(e => e.Comment).HasMaxLength(1000).IsUnicode(true);
                entity.Property(e => e.Placed).HasMaxLength(1).IsUnicode(false);
                entity.Property(e => e.IsPowerUser).HasMaxLength(1).IsUnicode(false);
                entity.Property(e => e.Status).HasMaxLength(1).IsUnicode(false);
                entity.Property(e => e.FreeTable21).HasMaxLength(50).IsUnicode(true);
                entity.HasOne(e => e.Client)
                      .WithMany(c => c.Reservations)
                      .HasForeignKey(e => e.IdClient);
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

            modelBuilder.Entity<Table>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NumeroTable).IsRequired();
            });

            modelBuilder.Entity<Assignation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Periode).HasMaxLength(10).IsRequired();
                entity.Property(e => e.HeureDebut).IsRequired();
                entity.Property(e => e.HeureFin).IsRequired();
                entity.HasOne(a => a.Reservation)
                      .WithMany(r => r.Assignations)
                      .HasForeignKey(a => a.ReservationId);
                entity.HasOne(a => a.Table)
                      .WithMany(t => t.Assignations)
                      .HasForeignKey(a => a.TableId);
            });
        }
    }
}
