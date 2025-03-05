using F1Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace F1Api.Repository
{
    public interface ICircuitRepository
    {
        Task<IEnumerable<Circuit>> GetAllAsync();
        Task<Circuit> GetByIdAsync(int id);
        Task<IEnumerable<CircuitSummary>> GetSummariesAsync();
        Task<CircuitSummary> GetSummaryByIdAsync(int id);
    }
}