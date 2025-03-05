using F1Api.Models;
using F1Api.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<Circuit>> GetAllAsync()
        {
            _logger.LogInformation("Getting all circuits");
            return await _circuitRepository.GetAllAsync();
        }

        public async Task<Circuit> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting circuit with id: {id}");
            return await _circuitRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<CircuitSummary>> GetSummariesAsync()
        {
            return await _circuitRepository.GetSummariesAsync();
        }

        public async Task<CircuitSummary> GetSummaryByIdAsync(int id)
        {
            return await _circuitRepository.GetSummaryByIdAsync(id);
        }
    }
}