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
                // Create a new configuration with the connection string to the real database
                var projectDir = Directory.GetCurrentDirectory();
                var configPath = Path.Combine(projectDir, "appsettings.json");

                config.AddJsonFile(configPath);

                // Ensure we're not using an in-memory database
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["UseInMemoryDatabase"] = "false"
                });
            });

            builder.ConfigureServices(services =>
            {
                // Any additional service configuration for testing can go here
            });
        }
    }
}