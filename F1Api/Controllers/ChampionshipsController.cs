using F1Api.Models;
using F1Api.Services;
using F1Api.Validators;
using FluentValidation;
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
        private readonly IValidator<YearRequest> _validator;

        public ChampionshipsController(
            IChampionshipService championshipService,
            ILogger<ChampionshipsController> logger,
            IValidator<YearRequest> validator)
        {
            _championshipService = championshipService;
            _logger = logger;
            _validator = validator;
        }

        [HttpGet("{year}")]
        public async Task<ActionResult<IEnumerable<DriverChampionshipSummary>>> GetDriverChampionshipByYear(int year)
        {
            // Create request model and validate
            var request = new YearRequest { Year = year };
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var driverChampionship = await _championshipService.GetDriverChampionshipByYearAsync(year);

            if (driverChampionship == null || !driverChampionship.Any())
            {
                return NotFound($"No championship data found for year: {year}");
            }

            return Ok(driverChampionship);
        }
    }
}