using F1Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace F1Api.Repository
{
    public class DriverRepository : IDriverRepository
    {
        private readonly F1DbContext _context;

        public DriverRepository(F1DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Driver>> GetAllAsync()
        {
            return await _context.Drivers.ToListAsync();
        }

        public async Task<Driver> GetByIdAsync(int id)
        {
            return await _context.Drivers.FindAsync(id);
        }

        public async Task<IEnumerable<DriverSummary>> GetSummariesAsync()
        {
            var drivers = await _context.Drivers.ToListAsync();
            var summaries = new List<DriverSummary>();

            foreach (var driver in drivers)
            {
                summaries.Add(await GetSummaryByIdAsync(driver.Id));
            }

            return summaries;
        }

        public async Task<DriverSummary> GetSummaryByIdAsync(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null) return null;

            var podiumCount = await CountDriverPodiumsAsync(id);

            var racesEntered = await _context.DriverStandings
                .Where(ds => ds.DriverId == id)
                .Select(ds => ds.RaceId)
                .Distinct()
                .CountAsync();

            return new DriverSummary
            {
                DriverId = driver.Id,
                DriverReference = driver.DriverReference,
                FullName = $"{driver.Forename} {driver.Surname}",
                Nationality = driver.Nationality,
                PodiumCount = podiumCount,
                TotalRacesEntered = racesEntered
            };
        }

        private async Task<int> CountDriverPodiumsAsync(int driverId)
        {
            // Get all races the driver participated in
            var raceIds = await _context.LapTimes
                .Where(lt => lt.DriverId == driverId)
                .Select(lt => lt.RaceId)
                .Distinct()
                .ToListAsync();

            var podiumCount = 0;

            foreach (var raceId in raceIds)
            {
                // Find the final lap number & position for this race
                var finalPosition = await _context.LapTimes
                 .Where(lt => lt.RaceId == raceId && lt.DriverId == driverId)
                 .OrderByDescending(lt => lt.Lap)
                 .Select(lt => lt.Position)
                 .FirstOrDefaultAsync();

                if (finalPosition >= 1 && finalPosition <= 3)
                {
                    podiumCount++;
                }
            }
            return podiumCount;
        }
    }
}