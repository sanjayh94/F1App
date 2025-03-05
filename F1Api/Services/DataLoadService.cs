using F1Api.Repository;
using F1Api.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace F1Api.Services
{
    public class DataLoadService
    {
        private readonly F1DbContext _dbContext;
        private readonly ILogger<DataLoadService> _logger;
        private readonly IWebHostEnvironment _environment;

        public DataLoadService(F1DbContext dbContext, ILogger<DataLoadService> logger, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _logger = logger;
            _environment = environment;
        }

        public async Task LoadDataAsync()
        {
            try
            {
                var dataPath = Path.Combine(_environment.ContentRootPath, "Data");
                _logger.LogInformation($"Loading data from: {dataPath}");

                // Load Circuits
                await LoadJsonData<Circuit>(Path.Combine(dataPath, "circuits.json"), _dbContext.Circuits);

                // Load Drivers
                await LoadJsonData<Driver>(Path.Combine(dataPath, "drivers.json"), _dbContext.Drivers);

                // Load Races
                await LoadJsonData<Race>(Path.Combine(dataPath, "races.json"), _dbContext.Races);

                // Load Driver Standings
                await LoadJsonData<DriverStanding>(Path.Combine(dataPath, "driver_standings.json"), _dbContext.DriverStandings);

                // Load Lap Times
                await LoadJsonData<LapTime>(Path.Combine(dataPath, "lap_times.json"), _dbContext.LapTimes);

                _logger.LogInformation("Data loading completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading data: {Message}", ex.Message);
                throw;
            }
        }

        private async Task LoadJsonData<T>(string filePath, DbSet<T> dbSet) where T : class
        {
            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"File not found: {filePath}");
                return;
            }

            _logger.LogInformation($"Loading data from {filePath}");

            try
            {
                string jsonContent = await File.ReadAllTextAsync(filePath);

                // Pre-process the JSON to replace '\N' with null
                jsonContent = ProcessJsonNulls(jsonContent);

                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                var items = JsonConvert.DeserializeObject<List<T>>(jsonContent, settings);

                if (items != null && items.Any())
                {
                    dbSet.RemoveRange(dbSet);

                    const int batchSize = 1000;
                    for (int i = 0; i < items.Count; i += batchSize)
                    {
                        var batch = items.Skip(i).Take(batchSize);
                        await dbSet.AddRangeAsync(batch);
                        await _dbContext.SaveChangesAsync();

                        _logger.LogInformation($"Saved batch {i / batchSize + 1} of {Math.Ceiling((double)items.Count / batchSize)} for {typeof(T).Name}");
                    }

                    _logger.LogInformation($"Loaded {items.Count} {typeof(T).Name} records");
                }
                else
                {
                    _logger.LogWarning($"No items found in {filePath}");
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Error deserializing JSON from {filePath}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error loading {typeof(T).Name} data: {ex.Message}");
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
    }
}