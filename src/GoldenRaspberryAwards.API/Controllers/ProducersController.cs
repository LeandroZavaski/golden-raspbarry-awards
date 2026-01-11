using GoldenRaspberryAwards.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoldenRaspberryAwards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProducersController(IProducerService producerService, ILogger<ProducersController> logger) : ControllerBase
    {
        private readonly IProducerService _producerService = producerService;
        private readonly ILogger<ProducersController> _logger = logger;

        [HttpGet("awards-interval")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAwardsInterval()
        {
            _logger.LogInformation("Obtendo intervalos de prêmios dos produtores");

            var result = await _producerService.GetProducerIntervalsAsync();

            return Ok(result);
        }
    }
}
