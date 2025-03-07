using F1Api.Models;
using F1Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace F1Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChampionshipsController : ControllerBase
    {
        private readonly IChampionshipService _championshipService;
        private readonly ILogger<ChampionshipsController> _logger;

        public ChampionshipsController(IChampionshipService championshipService, ILogger<ChampionshipsController> logger)
        {
            _championshipService = championshipService;
            _logger = logger;
        }

        [HttpGet("{year}")]
        public async Task<ActionResult<IEnumerable<DriverChampionshipSummary>>> GetDriverChampionshipByYear(int year)
        {
            var driverChampionship = await _championshipService.GetDriverChampionshipByYearAsync(year);

            if (driverChampionship == null || !driverChampionship.Any())
            {
                return NotFound($"No championship data found for year: {year}");
            }

            return Ok(driverChampionship);
        }
    }
}