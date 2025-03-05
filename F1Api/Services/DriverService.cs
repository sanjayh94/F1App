using F1Api.Models;
using F1Api.Repository;

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

        public async Task<IEnumerable<Driver>> GetAllDriversAsync()
        {
            _logger.LogInformation("Getting all drivers");
            return await _driverRepository.GetAllAsync();
        }

        public async Task<Driver> GetDriverByIdAsync(int id)
        {
            _logger.LogInformation($"Getting driver with id: {id}");
            return await _driverRepository.GetByIdAsync(id);
        }
    }
}