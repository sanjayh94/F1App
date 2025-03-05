using F1Api.Models;
using F1Api.Repository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace F1Api.Services
{
    public class MigrationService
    {
        private readonly F1DbContext _dbContext;
        private readonly ILogger<MigrationService> _logger;
        private readonly IWebHostEnvironment _environment;

        public MigrationService(F1DbContext dbContext, ILogger<MigrationService> logger, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _logger = logger;
            _environment = environment;
        }

        public async Task SeedDataAsync()
        {
            // Check if database is already seeded
            if (await _dbContext.Circuits.AnyAsync())
            {
                _logger.LogInformation("Database already contains data. Skipping seed operation.");
                return;
            }

            try
            {
                _logger.LogInformation("Starting database seeding...");
                var dataPath = Path.Combine(_environment.ContentRootPath, "Data");

                // Process data in the most efficient order (handling dependencies)
                await SeedEntitiesAsync<Circuit>(dataPath, "circuits.json", _dbContext.Circuits);
                await SeedEntitiesAsync<Driver>(dataPath, "drivers.json", _dbContext.Drivers);
                await SeedEntitiesAsync<Race>(dataPath, "races.json", _dbContext.Races);
                await SeedEntitiesAsync<DriverStanding>(dataPath, "driver_standings.json", _dbContext.DriverStandings, 5000);
                await SeedLapTimesAsync(dataPath); // Special handling for lap times due to size

                _logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding database: {Message}", ex.Message);
                throw;
            }
        }

        public static string ProcessJsonNulls(string json)
        {
            // Replace the string "\\N" (JSON escaped \N) with null
            json = json.Replace(@"\\N", "null");
            json = System.Text.RegularExpressions.Regex.Replace(json, "\"null\"", "null");

            return json;
        }

        private async Task SeedEntitiesAsync<T>(string dataPath, string fileName, DbSet<T> dbSet, int batchSize = 0) where T : class
        {
            string filePath = Path.Combine(dataPath, fileName);
            string entityName = typeof(T).Name;
            _logger.LogInformation($"Seeding {entityName} from {filePath}");

            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"File not found: {filePath}");
                return;
            }

            try
            {
                string jsonContent = await File.ReadAllTextAsync(filePath);
                jsonContent = ProcessJsonNulls(jsonContent);

                var entities = JsonConvert.DeserializeObject<List<T>>(jsonContent);
                if (entities != null && entities.Any())
                {
                    _logger.LogInformation($"Inserting {entities.Count} {entityName} records");

                    if (batchSize > 0 && entities.Count > batchSize)
                    {
                        // Process in batches to avoid memory issues
                        int totalBatches = (int)Math.Ceiling((double)entities.Count / batchSize);
                        for (int i = 0; i < entities.Count; i += batchSize)
                        {
                            var batch = entities.Skip(i).Take(batchSize).ToList();
                            await dbSet.AddRangeAsync(batch);
                            await _dbContext.SaveChangesAsync();
                            _logger.LogInformation($"Inserted batch {i / batchSize + 1} of {totalBatches} {entityName} records");
                        }
                    }
                    else
                    {
                        // Process all at once for smaller datasets
                        await dbSet.AddRangeAsync(entities);
                        await _dbContext.SaveChangesAsync();
                    }

                    _logger.LogInformation($"Inserted {entities.Count} {entityName} records successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error seeding {entityName}: {ex.Message}");
                throw;
            }
        }

        private async Task SeedLapTimesAsync(string dataPath)
        {
            string filePath = Path.Combine(dataPath, "lap_times.json");
            _logger.LogInformation($"Seeding lap times from {filePath}");

            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"File not found: {filePath}");
                return;
            }

            try
            {
                // Process the large file in chunks to avoid memory issues
                using (var fileStream = File.OpenText(filePath))
                using (var jsonReader = new JsonTextReader(fileStream))
                {
                    // Get to the start of the array
                    while (jsonReader.Read() && jsonReader.TokenType != JsonToken.StartArray) { }

                    if (jsonReader.TokenType != JsonToken.StartArray)
                    {
                        _logger.LogError("Invalid JSON format in lap_times.json");
                        return;
                    }

                    var serializer = new JsonSerializer();
                    var lapTimes = new List<LapTime>();
                    int totalProcessed = 0;
                    const int batchSize = 10000;

                    // Read one token to advance to the first element in the array
                    jsonReader.Read();

                    while (jsonReader.TokenType != JsonToken.EndArray)
                    {
                        var lapTime = serializer.Deserialize<LapTime>(jsonReader);
                        if (lapTime != null)
                        {
                            lapTimes.Add(lapTime);
                        }

                        if (lapTimes.Count >= batchSize)
                        {
                            await _dbContext.LapTimes.AddRangeAsync(lapTimes);
                            await _dbContext.SaveChangesAsync();

                            totalProcessed += lapTimes.Count;
                            _logger.LogInformation($"Processed {totalProcessed} lap times");

                            lapTimes.Clear();
                        }

                        // Read the next token
                        jsonReader.Read();
                    }

                    // Process any remaining items
                    if (lapTimes.Any())
                    {
                        await _dbContext.LapTimes.AddRangeAsync(lapTimes);
                        await _dbContext.SaveChangesAsync();

                        totalProcessed += lapTimes.Count;
                    }

                    _logger.LogInformation($"Completed seeding lap times: {totalProcessed} records processed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error seeding lap times: {ex.Message}");
                throw;
            }
        }
    }
}