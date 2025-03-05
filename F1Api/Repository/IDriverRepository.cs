using F1Api.Models;

namespace F1Api.Repository
{
    public interface IDriverRepository
    {
        Task<IEnumerable<Driver>> GetAllAsync();
        Task<Driver> GetByIdAsync(int id);
    }
}