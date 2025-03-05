using F1Api.Models;
using F1Api.Repository;

namespace F1Api.Services
{
    public class CircuitService : ICircuitService
    {
        private readonly ICircuitRepository _circuitRepository;
        private readonly ILogger<CircuitService> _logger;

        public CircuitService(ICircuitRepository circuitRepository, ILogger<CircuitService> logger)
        {
            _circuitRepository = circuitRepository ?? throw new ArgumentNullException(nameof(circuitRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Circuit>> GetAllCircuitsAsync()
        {
            _logger.LogInformation("Getting all circuits");
            return await _circuitRepository.GetAllAsync();
        }

        public async Task<Circuit> GetCircuitByIdAsync(int id)
        {
            _logger.LogInformation($"Getting circuit with id: {id}");
            return await _circuitRepository.GetByIdAsync(id);
        }
    }
}