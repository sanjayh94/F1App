using F1Api.Repository;
using F1Api.Services;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using FluentValidation;
using F1Api.Validators;
using F1Api.Models;

namespace F1Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddHealthChecks()
                .AddDbContextCheck<F1DbContext>("database", tags: new[] { "ready" });

            // Add DbContext
            builder.Services.AddDbContext<F1DbContext>(options =>
            {
                // Check if we're in a test environment
                var useInMemoryDb = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");

                if (useInMemoryDb)
                {
                    // Use in-memory database for testing
                    options.UseInMemoryDatabase("F1Db");
                }
                else
                {
                    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
                }
            });

            // Register validators
            builder.Services.AddTransient<IValidator<YearRequest>, YearValidator>();
            builder.Services.AddTransient<IValidator<IdRequest>, IdValidator>();

            // Register Repositories
            builder.Services.AddScoped<ICircuitRepository, CircuitRepository>();
            builder.Services.AddScoped<IDriverRepository, DriverRepository>();
            builder.Services.AddScoped<IChampionshipRepository, ChampionshipRepository>();

            // Register Services
            builder.Services.AddScoped<ICircuitService, CircuitService>();
            builder.Services.AddScoped<IDriverService, DriverService>();
            builder.Services.AddScoped<IChampionshipService, ChampionshipService>();
            builder.Services.AddScoped<MigrationService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseHttpsRedirection();

            // Health checks (Will show unhealthy if database is not ready)
            app.MapHealthChecks("/health");

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();
            app.MapControllers();

            // Check if we're in a test environment
            var useInMemoryDb = app.Configuration.GetValue<bool>("UseInMemoryDatabase");

            if (!useInMemoryDb)
            {
                using var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<F1DbContext>();
                    dbContext.Database.Migrate();

                    var migrationService = services.GetRequiredService<MigrationService>();
                    await migrationService.SeedDataAsync();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating or seeding the database");
                }
            }

            await app.RunAsync();
        }
    }
}
