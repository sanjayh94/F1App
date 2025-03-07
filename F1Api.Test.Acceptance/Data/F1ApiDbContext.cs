using F1Api.Models;
using Microsoft.EntityFrameworkCore;

namespace F1Api.Repository
{
    public class F1ApiDbContext : DbContext
    {
        public F1ApiDbContext(DbContextOptions<F1ApiDbContext> options)
            : base(options)
        {
        }

        public DbSet<Circuit> Circuits { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Race> Races { get; set; }
        public DbSet<DriverStanding> DriverStandings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Circuit>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Circuit>()
                .Property(c => c.Name)
                .IsRequired();

            modelBuilder.Entity<Circuit>()
                .Property(c => c.Country)
                .IsRequired();

            modelBuilder.Entity<Driver>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<Driver>()
                .Property(d => d.Forename)
                .IsRequired();

            modelBuilder.Entity<Driver>()
                .Property(d => d.Surname)
                .IsRequired();

            modelBuilder.Entity<Race>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<DriverStanding>()
                .HasKey(ds => ds.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}