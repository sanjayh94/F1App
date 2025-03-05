using F1Api.Models;
using F1Api.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace F1Api.Services
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository _driverRepository;
        private readonly ILogger<DriverService> _logger;

        public DriverService(IDriverRepository driverRepository, ILogger<DriverService> logger)
        {
            _driverRepository = driverRepository ?? throw new ArgumentNullException(nameof(driverRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Driver>> GetAllAsync()
        {
            _logger.LogInformation("Getting all drivers");
            return await _driverRepository.GetAllAsync();
        }

        public async Task<Driver> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting driver with id: {id}");
            return await _driverRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<DriverSummary>> GetSummariesAsync()
        {
            return await _driverRepository.GetSummariesAsync();
        }

        public async Task<DriverSummary> GetSummaryByIdAsync(int id)
        {
            return await _driverRepository.GetSummaryByIdAsync(id);
        }
    }
}