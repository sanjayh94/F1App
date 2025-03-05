using F1Api.Models;
using F1Api.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace F1Api.Test.Acceptance
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    // Use in-memory database for testing
                    { "UseInMemoryDatabase", "true" }
                });
            });

            builder.ConfigureServices(services =>
            {
                // Find the DbContext registration and replace it with in-memory database
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<F1ApiDbContext>));

                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                services.AddDbContext<F1ApiDbContext>(options =>
                {
                    options.UseInMemoryDatabase("F1ApiTestDb");
                });

                services.AddScoped<ICircuitRepository, CircuitRepository>();

                // Build the service provider to resolve scoped services
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<F1ApiDbContext>();
                var logger = scopedServices
                    .GetRequiredService<ILogger<TestWebApplicationFactory>>();

                db.Database.EnsureCreated();

                try
                {
                    SeedTestData(db);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the database. Error: {Message}", ex.Message);
                }
            });
        }

        private void SeedTestData(F1ApiDbContext dbContext)
        {
            dbContext.Circuits.RemoveRange(dbContext.Circuits);
            dbContext.SaveChanges();

            var testCircuits = new List<Circuit>
            {
                new Circuit
                {
                    Id = 1,
                    Name = "Monaco Circuit",
                    CircuitReference = "monaco",
                    Location = "Monte Carlo",
                    Country = "Monaco",
                    Latitude = 43.7347,
                    Longitude = 7.4205,
                    Url = new Uri("https://example.com/monaco")
                },
                new Circuit
                {
                    Id = 2,
                    Name = "Silverstone Circuit",
                    CircuitReference = "silverstone",
                    Location = "Silverstone",
                    Country = "UK",
                    Latitude = 52.0786,
                    Longitude = -1.0169,
                    Url = new Uri("https://example.com/silverstone")
                },
                new Circuit
                {
                    Id = 3,
                    Name = "Monza Circuit",
                    CircuitReference = "monza",
                    Location = "Monza",
                    Country = "Italy",
                    Latitude = 45.6156,
                    Longitude = 9.2811,
                    Url = new Uri("https://example.com/monza")
                }
            };

            dbContext.Circuits.AddRange(testCircuits);
            dbContext.SaveChanges();
        }
    }
}