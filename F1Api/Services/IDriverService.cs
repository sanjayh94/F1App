using F1Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace F1Api.Services
{
    public interface IDriverService
    {
        Task<IEnumerable<Driver>> GetAllAsync();
        Task<Driver> GetByIdAsync(int id);
        Task<IEnumerable<DriverSummary>> GetSummariesAsync();
        Task<DriverSummary> GetSummaryByIdAsync(int id);
    }
}