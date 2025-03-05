using F1Api.Models;
using F1Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace F1Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CircuitsController : ControllerBase
    {
        private readonly ICircuitService _circuitService;
        private readonly ILogger<CircuitsController> _logger;

        public CircuitsController(ICircuitService circuitService, ILogger<CircuitsController> logger)
        {
            _circuitService = circuitService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Circuit>>> GetCircuits()
        {
            _logger.LogInformation("Getting all circuits");
            return Ok(await _circuitService.GetAllCircuitsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Circuit>> GetCircuit(int id)
        {
            var circuit = await _circuitService.GetCircuitByIdAsync(id);

            if (circuit == null)
            {
                return NotFound();
            }

            return circuit;
        }
    }
}