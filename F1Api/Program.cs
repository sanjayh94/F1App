using F1Api.Repository;
using F1Api.Services;
using Microsoft.EntityFrameworkCore;

namespace F1Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Add DbContext
            builder.Services.AddDbContext<F1DbContext>(options =>
                options.UseInMemoryDatabase("F1Db"));

            // Register Repositories
            builder.Services.AddScoped<ICircuitRepository, CircuitRepository>();
            builder.Services.AddScoped<IDriverRepository, DriverRepository>();

            // Register Services
            builder.Services.AddScoped<DataLoadService>();
            builder.Services.AddScoped<ICircuitService, CircuitService>();
            builder.Services.AddScoped<IDriverService, DriverService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // Load data on startup
            using (var scope = app.Services.CreateScope())
            {
                var dataLoadService = scope.ServiceProvider.GetRequiredService<DataLoadService>();
                await dataLoadService.LoadDataAsync();
            }

            await app.RunAsync();
        }
    }
}
