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

            base.OnModelCreating(modelBuilder);
        }
    }
}