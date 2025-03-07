using F1Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace F1Api.Repository
{
    public interface IChampionshipRepository
    {
        Task<IEnumerable<DriverStanding>> GetDriverStandingsByYearAsync(int year);
    }
}