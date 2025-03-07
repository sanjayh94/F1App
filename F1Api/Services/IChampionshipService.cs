using F1Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace F1Api.Services
{
    public interface IChampionshipService
    {
        Task<IEnumerable<DriverChampionshipSummary>> GetDriverChampionshipByYearAsync(int year);
    }
}