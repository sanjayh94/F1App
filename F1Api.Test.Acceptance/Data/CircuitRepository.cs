using F1Api.Models;
using Microsoft.EntityFrameworkCore;

namespace F1Api.Repository
{
    public class CircuitRepository : ICircuitRepository
    {
        private readonly F1ApiDbContext _context;

        public CircuitRepository(F1ApiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Circuit>> GetAllAsync()
        {
            return await _context.Circuits.ToListAsync();
        }

        public async Task<Circuit?> GetByIdAsync(int id)
        {
            return await _context.Circuits.FindAsync(id);
        }

        public async Task<IEnumerable<CircuitSummary>> GetSummariesAsync()
        {
            var circuits = await _context.Circuits.ToListAsync();
            
            return circuits.Select(c => new CircuitSummary
            {
                CircuitId = c.Id,
                CircuitReference = c.CircuitReference,
                Name = c.Name,
                Location = c.Location,
                Country = c.Country,
                FastestLapTime = "1:12.909",
                FastestLapTimeMilliseconds = 72909,
                FastestLapDriver = c.Id == 1 ? "Lewis Hamilton" : (c.Id == 2 ? "Max Verstappen" : "Charles Leclerc"),
                FastestLapRaceYear = 2021,
                TotalRacesCompleted = 50
            });
        }

        public async Task<CircuitSummary?> GetSummaryByIdAsync(int id)
        {
            var circuit = await _context.Circuits.FindAsync(id);
            
            if (circuit == null)
                return null;
                
            return new CircuitSummary
            {
                CircuitId = circuit.Id,
                CircuitReference = circuit.CircuitReference,
                Name = circuit.Name,
                Location = circuit.Location,
                Country = circuit.Country,
                FastestLapTime = "1:12.909",
                FastestLapTimeMilliseconds = 72909,
                FastestLapDriver = circuit.Id == 1 ? "Lewis Hamilton" : (circuit.Id == 2 ? "Max Verstappen" : "Charles Leclerc"),
                FastestLapRaceYear = 2021,
                TotalRacesCompleted = 50
            };
        }
    }
}
