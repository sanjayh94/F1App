using F1Api.Models;
using F1Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace F1Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriversController : ControllerBase
    {
        private readonly IDriverService _driverService;
        private readonly ILogger<DriversController> _logger;

        public DriversController(IDriverService driverService, ILogger<DriversController> logger)
        {
            _driverService = driverService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetDrivers()
        {
            _logger.LogInformation("Getting all drivers");
            return Ok(await _driverService.GetAllDriversAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Driver>> GetDriver(int id)
        {
            var driver = await _driverService.GetDriverByIdAsync(id);

            if (driver == null)
            {
                return NotFound();
            }

            return driver;
        }
    }
}