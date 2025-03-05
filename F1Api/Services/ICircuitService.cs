using F1Api.Models;

namespace F1Api.Services
{
    public interface ICircuitService
    {
        Task<IEnumerable<Circuit>> GetAllCircuitsAsync();
        Task<Circuit> GetCircuitByIdAsync(int id);
    }
}