using F1Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace F1Api.Services
{
    public interface ICircuitService
    {
        Task<IEnumerable<Circuit>> GetAllAsync();
        Task<Circuit> GetByIdAsync(int id);
        Task<IEnumerable<CircuitSummary>> GetSummariesAsync();
        Task<CircuitSummary> GetSummaryByIdAsync(int id);
    }
}