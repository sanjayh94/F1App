using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace F1Api.Test.Integration
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var projectDir = Directory.GetCurrentDirectory();
                var configPath = Path.Combine(projectDir, "appsettings.json");

                config.AddJsonFile(configPath);

                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["UseInMemoryDatabase"] = "false",
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Port=5432;Database=f1db;Username=f1user;Password=f1password"
                });
            });
        }
    }
}