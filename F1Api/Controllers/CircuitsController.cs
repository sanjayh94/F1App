using F1Api.Models;
using F1Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace F1Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CircuitsController : ControllerBase
    {
        private readonly ICircuitService _circuitService;
        private readonly ILogger<CircuitsController> _logger;
        private readonly IValidator<IdRequest> _validator;

        public CircuitsController(ICircuitService circuitService, ILogger<CircuitsController> logger, IValidator<IdRequest> validator)
        {
            _circuitService = circuitService;
            // Ideally can use a structured logging framework like Serilog and configure sinks to the desired platform or destination such as New Relic.
            _logger = logger;
            _validator = validator;
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
            // Create request model and validate
            var request = new IdRequest { Id = id };
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

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

        [HttpGet("{id}/summaries")]
        public async Task<ActionResult<CircuitSummary>> GetCircuitSummaryById(int id)
        {
            // Create request model and validate
            var request = new IdRequest { Id = id };
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var summary = await _circuitService.GetSummaryByIdAsync(id);
            if (summary == null) return NotFound();
            return Ok(summary);
        }
    }
}