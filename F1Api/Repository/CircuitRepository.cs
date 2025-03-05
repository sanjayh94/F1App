using F1Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace F1Api.Repository
{
    public class CircuitRepository : ICircuitRepository
    {
        private readonly F1DbContext _context;

        public CircuitRepository(F1DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Circuit>> GetAllAsync()
        {
            return await _context.Circuits.ToListAsync();
        }

        public async Task<Circuit> GetByIdAsync(int id)
        {
            return await _context.Circuits.FindAsync(id);
        }

        public async Task<IEnumerable<CircuitSummary>> GetSummariesAsync()
        {
            var circuits = await _context.Circuits.ToListAsync();
            var summaries = new List<CircuitSummary>();

            foreach (var circuit in circuits)
            {
                summaries.Add(await GetSummaryByIdAsync(circuit.Id));
            }

            return summaries;
        }

        public async Task<CircuitSummary> GetSummaryByIdAsync(int id)
        {
            var circuit = await _context.Circuits.FindAsync(id);
            if (circuit == null) return null;

            var races = await _context.Races
                .Where(r => r.CircuitId == id)
                .ToListAsync();

            var fastestLap = await _context.LapTimes
                .Where(lt => races.Select(r => r.Id).Contains(lt.RaceId))
                .OrderBy(lt => lt.Milliseconds)
                .FirstOrDefaultAsync();

            // Get driver name for fastest lap
            var driverName = "";
            var raceYear = 0;
            if (fastestLap != null)
            {
                var driver = await _context.Drivers.FindAsync(fastestLap.DriverId);
                driverName = driver != null ? $"{driver.Forename} {driver.Surname}" : "Unknown";

                var race = await _context.Races.FindAsync(fastestLap.RaceId);
                raceYear = race?.Year ?? 0;
            }

            return new CircuitSummary
            {
                CircuitId = circuit.Id,
                CircuitReference = circuit.CircuitReference,
                Name = circuit.Name,
                Location = circuit.Location,
                Country = circuit.Country,
                FastestLapTime = fastestLap?.Time ?? "N/A",
                FastestLapTimeMilliseconds = fastestLap?.Milliseconds,
                FastestLapDriver = driverName,
                FastestLapRaceYear = raceYear,
                TotalRacesCompleted = races.Count
            };
        }
    }
}