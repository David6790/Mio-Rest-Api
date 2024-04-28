using Microsoft.EntityFrameworkCore;
using Mio_Rest_Api.Entities;

namespace Mio_Rest_Api.Data
{
    public class ContextReservation : DbContext
    {
        public ContextReservation(DbContextOptions <ContextReservation> options)
            : base(options)
        {
        }

        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<OccupationStatus> OccupationStatus { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e=>e.CreatedBy).HasMaxLength(50).IsUnicode(true);
                entity.Property(e=>e.UpdatedBy).HasMaxLength(50).IsUnicode(true);
                entity.Property(e=>e.OccupationStatusOnBook).HasMaxLength(20).IsUnicode(true);
                entity.Property(e=>e.Comment).HasMaxLength(500).IsUnicode(true);
                entity.Property(e=>e.Placed).HasMaxLength(1).IsUnicode(false);
                entity.Property(e=>e.IsPowerUser).HasMaxLength(1).IsUnicode(false);
                entity.Property(e=>e.Status).HasMaxLength(1).IsUnicode(false);



            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e=>e.Name).HasMaxLength(50).IsUnicode(true);
                entity.Property(e=>e.Prenom).HasMaxLength(50).IsUnicode(true);
                entity.Property(e=>e.Telephone).HasMaxLength(50).IsUnicode(true);
                entity.Property(e=>e.Email).HasMaxLength(255).IsUnicode(true);
                entity.Property(e=>e.FreeTable21).HasMaxLength(1).IsUnicode(false);
                entity.HasMany<Reservation>().WithOne().HasForeignKey(e => e.IdClient);


            });

            modelBuilder.Entity<OccupationStatus>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e=>e.OccStatus).HasMaxLength(20).IsUnicode(true);

            });
        }
    }


}
