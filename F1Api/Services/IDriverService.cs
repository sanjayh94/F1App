using F1Api.Models;

namespace F1Api.Services
{
    public interface IDriverService
    {
        Task<IEnumerable<Driver>> GetAllDriversAsync();
        Task<Driver> GetDriverByIdAsync(int id);
    }
}