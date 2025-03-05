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
            // Configure composite keys for LapTime
            modelBuilder.Entity<LapTime>()
                .HasKey(lt => new { lt.RaceId, lt.DriverId, lt.Lap });

            base.OnModelCreating(modelBuilder);
        }
    }
}