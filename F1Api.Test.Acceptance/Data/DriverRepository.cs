using F1Api.Models;
using Microsoft.EntityFrameworkCore;

namespace F1Api.Repository
{
    public class DriverRepository : IDriverRepository
    {
        private readonly F1ApiDbContext _context;

        public DriverRepository(F1ApiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Driver>> GetAllAsync()
        {
            return await _context.Drivers.ToListAsync();
        }

        public async Task<Driver?> GetByIdAsync(int id)
        {
            return await _context.Drivers.FindAsync(id);
        }

        public async Task<IEnumerable<DriverSummary>> GetSummariesAsync()
        {
            var drivers = await _context.Drivers.ToListAsync();
            
            return drivers.Select(d => new DriverSummary
            {
                DriverId = d.Id,
                DriverReference = d.DriverReference,
                FullName = $"{d.Forename} {d.Surname}",
                Nationality = d.Nationality,
                PodiumCount = 50,
                TotalRacesEntered = 100
            });
        }

        public async Task<DriverSummary?> GetSummaryByIdAsync(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            
            if (driver == null)
                return null;
                
            return new DriverSummary
            {
                DriverId = driver.Id,
                DriverReference = driver.DriverReference,
                FullName = $"{driver.Forename} {driver.Surname}",
                Nationality = driver.Nationality,
                PodiumCount = 50,
                TotalRacesEntered = 100
            };
        }
    }
} 