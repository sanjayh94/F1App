using F1Api.Models;
using F1Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace F1Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly IDriverService _driverService;
        private readonly ILogger<DriversController> _logger;

        public DriversController(IDriverService driverService, ILogger<DriversController> logger)
        {
            _driverService = driverService;
            // Ideally can use a structured logging framework like Serilog and configure sinks to the desired platform or destination such as New Relic.
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetAllDrivers()
        {
            var drivers = await _driverService.GetAllAsync();
            return Ok(drivers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Driver>> GetDriverById(int id)
        {
            var driver = await _driverService.GetByIdAsync(id);
            if (driver == null) return NotFound();
            return Ok(driver);
        }

        [HttpGet("summaries")]
        public async Task<ActionResult<IEnumerable<DriverSummary>>> GetDriverSummaries()
        {
            var summaries = await _driverService.GetSummariesAsync();
            return Ok(summaries);
        }

        [HttpGet("summaries/{id}")]
        public async Task<ActionResult<DriverSummary>> GetDriverSummaryById(int id)
        {
            var summary = await _driverService.GetSummaryByIdAsync(id);
            if (summary == null) return NotFound();
            return Ok(summary);
        }
    }
}