using Microsoft.EntityFrameworkCore;
using Mio_Rest_Api.Entities;

namespace Mio_Rest_Api.Data
{
    public class ContextApplication : DbContext
    {
        public ContextApplication(DbContextOptions <ContextApplication> options)
            : base(options)
        {
        }

        public virtual DbSet<ReservationEntity> Reservations { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<OccupationStatus> OccupationStatus { get; set;}
        public virtual DbSet<MenuEntity> MenuDuJour { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReservationEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e=>e.CreatedBy).HasMaxLength(50).IsUnicode(true);
                entity.Property(e=>e.UpdatedBy).HasMaxLength(50).IsUnicode(true);
                entity.Property(e=>e.OccupationStatusOnBook).HasMaxLength(20).IsUnicode(true);
                entity.Property(e=>e.Comment).HasMaxLength(1000).IsUnicode(true);
                entity.Property(e=>e.Placed).HasMaxLength(1).IsUnicode(false);
                entity.Property(e=>e.IsPowerUser).HasMaxLength(1).IsUnicode(false);
                entity.Property(e=>e.Status).HasMaxLength(1).IsUnicode(false);
                entity.Property(e => e.FreeTable21).HasMaxLength(1).IsUnicode(false);
                entity.HasOne(e => e.Client).WithMany().HasForeignKey(e => e.IdClient);



            });

            modelBuilder.Entity<MenuEntity>(entity =>
            {
                entity.HasKey(e =>e.Id);
                entity.Property(e=> e.Plat).HasMaxLength(200).IsUnicode(true);
                entity.Property(e=>e.Entree).HasMaxLength(200).IsUnicode(true);
                entity.Property(e=>e.DessertJour).HasMaxLength(200).IsUnicode(true);
                entity.Property(e=>e.Cheesecake).HasMaxLength(200).IsUnicode(true);
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e=>e.Name).HasMaxLength(50).IsUnicode(true);
                entity.Property(e=>e.Prenom).HasMaxLength(50).IsUnicode(true);
                entity.Property(e=>e.Telephone).HasMaxLength(50).IsUnicode(true);
                entity.Property(e=>e.Email).HasMaxLength(255).IsUnicode(true);
                


            });

            modelBuilder.Entity<OccupationStatus>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e=>e.OccStatus).HasMaxLength(20).IsUnicode(true);

            });
        }
    }


}
