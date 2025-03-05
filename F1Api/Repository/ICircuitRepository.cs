using F1Api.Models;

namespace F1Api.Repository
{
    public interface ICircuitRepository
    {
        Task<IEnumerable<Circuit>> GetAllAsync();
        Task<Circuit> GetByIdAsync(int id);
    }
}