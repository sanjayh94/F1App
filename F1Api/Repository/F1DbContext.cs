using F1Api.Models;
using Microsoft.EntityFrameworkCore;

namespace F1Api.Repository
{
    public class F1DbContext : DbContext
    {
        public F1DbContext(DbContextOptions<F1DbContext> options) : base(options)
        {
        }

        public DbSet<Circuit> Circuits { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Race> Races { get; set; }
        public DbSet<LapTime> LapTimes { get; set; }
        public DbSet<DriverStanding> DriverStandings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Circuit>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Country).IsRequired();
                entity.Property(e => e.CircuitReference).IsRequired();
            });

            modelBuilder.Entity<Driver>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Forename).IsRequired();
                entity.Property(e => e.Surname).IsRequired();
                entity.Property(e => e.DriverReference).IsRequired();
            });

            modelBuilder.Entity<Race>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.HasOne<Circuit>().WithMany().HasForeignKey(e => e.CircuitId);
            });

            modelBuilder.Entity<DriverStanding>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne<Driver>().WithMany().HasForeignKey(e => e.DriverId);
                entity.HasOne<Race>().WithMany().HasForeignKey(e => e.RaceId);
            });

            modelBuilder.Entity<LapTime>(entity =>
            {
                entity.HasKey(e => new { e.RaceId, e.DriverId, e.Lap });
                entity.HasOne<Driver>().WithMany().HasForeignKey(e => e.DriverId);
                entity.HasOne<Race>().WithMany().HasForeignKey(e => e.RaceId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}