using F1Api.Models;
using F1Api.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace F1Api.Services
{
    public class ChampionshipService : IChampionshipService
    {
        private readonly IChampionshipRepository _championshipRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly ILogger<ChampionshipService> _logger;

        public ChampionshipService(
            IChampionshipRepository championshipRepository,
            IDriverRepository driverRepository,
            ILogger<ChampionshipService> logger)
        {
            _championshipRepository = championshipRepository ?? throw new ArgumentNullException(nameof(championshipRepository));
            _driverRepository = driverRepository ?? throw new ArgumentNullException(nameof(driverRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<DriverChampionshipSummary>> GetDriverChampionshipByYearAsync(int year)
        {
            var driverStandings = await _championshipRepository.GetDriverStandingsByYearAsync(year);

            if (!driverStandings.Any())
            {
                return new List<DriverChampionshipSummary>();
            }

            var drivers = await _driverRepository.GetAllAsync();

            var driverSummaries = new List<DriverChampionshipSummary>();

            foreach (var standing in driverStandings)
            {
                var driver = drivers.FirstOrDefault(d => d.Id == standing.DriverId);
                if (driver == null) continue;

                driverSummaries.Add(new DriverChampionshipSummary
                {
                    DriverId = driver.Id,
                    DriverReference = driver.DriverReference,
                    FullName = $"{driver.Forename} {driver.Surname}",
                    TotalPoints = standing.Points,
                    Position = standing.Position,
                    Wins = standing.Wins
                });
            }

            // standings are ordered by position
            return driverSummaries.OrderBy(d => d.Position).ToList();
        }
    }
}