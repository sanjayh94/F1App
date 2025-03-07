using F1Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace F1Api.Repository
{
    public class ChampionshipRepository : IChampionshipRepository
    {
        private readonly F1ApiDbContext _context;

        public ChampionshipRepository(F1ApiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DriverStanding>> GetDriverStandingsByYearAsync(int year)
        {
            var races = await _context.Races
                .Where(r => r.Year == year)
                .OrderByDescending(r => r.Round) // Order by most recent race first
                .ToListAsync();

            if (!races.Any())
            {
                return new List<DriverStanding>();
            }

            var lastRace = races.First();

            var driverStandings = await _context.DriverStandings
                .Where(ds => ds.RaceId == lastRace.Id)
                .ToListAsync();

            return driverStandings;
        }
    }
}