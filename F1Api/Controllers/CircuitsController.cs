using F1Api.Models;
using F1Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace F1Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<ActionResult<IEnumerable<Circuit>>> GetAllCircuits()
        {
            var circuits = await _circuitService.GetAllAsync();
            return Ok(circuits);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Circuit>> GetCircuitById(int id)
        {
            var circuit = await _circuitService.GetByIdAsync(id);
            if (circuit == null) return NotFound();
            return Ok(circuit);
        }

        [HttpGet("summaries")]
        public async Task<ActionResult<IEnumerable<CircuitSummary>>> GetCircuitSummaries()
        {
            var summaries = await _circuitService.GetSummariesAsync();
            return Ok(summaries);
        }

        [HttpGet("summaries/{id}")]
        public async Task<ActionResult<CircuitSummary>> GetCircuitSummaryById(int id)
        {
            var summary = await _circuitService.GetSummaryByIdAsync(id);
            if (summary == null) return NotFound();
            return Ok(summary);
        }
    }
}