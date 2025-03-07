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
                services.AddScoped<IDriverRepository, DriverRepository>();
                services.AddScoped<IChampionshipRepository, ChampionshipRepository>();

                // Build the service provider to resolve scoped services
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<F1ApiDbContext>();
                var logger = scopedServices
                    .GetRequiredService<ILogger<TestWebApplicationFactory>>();

                try
                {
                    db.Database.EnsureCreated();
                    SeedTestData(db);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the database with test data. Error: {Message}", ex.Message);
                }
            });
        }

        private void SeedTestData(F1ApiDbContext dbContext)
        {
            // Clear existing data
            dbContext.Circuits.RemoveRange(dbContext.Circuits);
            if (dbContext.Drivers != null)
            {
                dbContext.Drivers.RemoveRange(dbContext.Drivers);
            }
            if (dbContext.Races != null)
            {
                dbContext.Races.RemoveRange(dbContext.Races);
            }
            if (dbContext.DriverStandings != null)
            {
                dbContext.DriverStandings.RemoveRange(dbContext.DriverStandings);
            }
            dbContext.SaveChanges();

            // Seed Circuits
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

            // Seed Drivers
            var testDrivers = new List<Driver>
            {
                new Driver
                {
                    Id = 848,
                    DriverReference = "albon",
                    Number = 23,
                    Code = "ALB",
                    Forename = "Alexander",
                    Surname = "Albon",
                    DateOfBirth = new DateOnly(1996, 3, 23),
                    Nationality = "Thai",
                    Url = new Uri("https://example.com/albon")
                },
                new Driver
                {
                    Id = 832,
                    DriverReference = "sainz",
                    Number = 55,
                    Code = "SAI",
                    Forename = "Carlos",
                    Surname = "Sainz",
                    DateOfBirth = new DateOnly(1994, 9, 1),
                    Nationality = "Spanish",
                    Url = new Uri("https://example.com/sainz")
                },
                new Driver
                {
                    Id = 1,
                    DriverReference = "hamilton",
                    Number = 44,
                    Code = "HAM",
                    Forename = "Lewis",
                    Surname = "Hamilton",
                    DateOfBirth = new DateOnly(1985, 1, 7),
                    Nationality = "British",
                    Url = new Uri("https://example.com/hamilton")
                }
            };

            dbContext.Drivers.AddRange(testDrivers);

            // Seed Races
            var testRaces = new List<Race>
            {
                new Race
                {
                    Id = 1,
                    Year = 2025,
                    Round = 1,
                    CircuitId = 1,
                    Name = "Monaco Grand Prix",
                    Date = new DateOnly(2025, 5, 28)
                },
                new Race
                {
                    Id = 2,
                    Year = 2025,
                    Round = 2,
                    CircuitId = 2,
                    Name = "British Grand Prix",
                    Date = new DateOnly(2025, 7, 9)
                },
                new Race
                {
                    Id = 3,
                    Year = 2026,
                    Round = 1,
                    CircuitId = 1,
                    Name = "Monaco Grand Prix",
                    Date = new DateOnly(2026, 5, 26)
                },
                new Race
                {
                    Id = 4,
                    Year = 2026,
                    Round = 2,
                    CircuitId = 2,
                    Name = "British Grand Prix",
                    Date = new DateOnly(2026, 7, 7)
                }
            };

            dbContext.Races.AddRange(testRaces);

            // Seed Driver Standings 
            var testDriverStandings = new List<DriverStanding>
            {
                // 2025 - Race 1 standings
                new DriverStanding
                {
                    Id = 1,
                    RaceId = 1,
                    DriverId = 1, // Hamilton
                    Points = 25,
                    Position = 1,
                    Wins = 1
                },
                new DriverStanding
                {
                    Id = 2,
                    RaceId = 1,
                    DriverId = 832, // Sainz
                    Points = 18,
                    Position = 2,
                    Wins = 0
                },
                new DriverStanding
                {
                    Id = 3,
                    RaceId = 1,
                    DriverId = 848, // Albon
                    Points = 15,
                    Position = 3,
                    Wins = 0
                },
                
                // 2025 - Race 2 standings
                new DriverStanding
                {
                    Id = 4,
                    RaceId = 2,
                    DriverId = 1, // Hamilton
                    Points = 43,
                    Position = 1,
                    Wins = 1
                },
                new DriverStanding
                {
                    Id = 5,
                    RaceId = 2,
                    DriverId = 832, // Sainz
                    Points = 40,
                    Position = 2,
                    Wins = 1
                },
                new DriverStanding
                {
                    Id = 6,
                    RaceId = 2,
                    DriverId = 848, // Albon
                    Points = 33,
                    Position = 3,
                    Wins = 0
                },
                
                // 2026 - Race 1 standings
                new DriverStanding
                {
                    Id = 7,
                    RaceId = 3,
                    DriverId = 848, // Albon
                    Points = 25,
                    Position = 1,
                    Wins = 1
                },
                new DriverStanding
                {
                    Id = 8,
                    RaceId = 3,
                    DriverId = 832, // Sainz
                    Points = 18,
                    Position = 2,
                    Wins = 0
                },
                new DriverStanding
                {
                    Id = 9,
                    RaceId = 3,
                    DriverId = 1, // Hamilton
                    Points = 15,
                    Position = 3,
                    Wins = 0
                },
                
                // 2026 - Race 2 standings
                new DriverStanding
                {
                    Id = 10,
                    RaceId = 4,
                    DriverId = 848, // Albon
                    Points = 50,
                    Position = 1,
                    Wins = 2
                },
                new DriverStanding
                {
                    Id = 11,
                    RaceId = 4,
                    DriverId = 1, // Hamilton
                    Points = 33,
                    Position = 2,
                    Wins = 0
                },
                new DriverStanding
                {
                    Id = 12,
                    RaceId = 4,
                    DriverId = 832, // Sainz
                    Points = 33,
                    Position = 3,
                    Wins = 0
                }
            };

            dbContext.DriverStandings.AddRange(testDriverStandings);
            dbContext.SaveChanges();
        }
    }
}