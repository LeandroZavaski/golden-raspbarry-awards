using GoldenRaspberryAwards.Domain.Dtos;
using GoldenRaspberryAwards.Infrastructure.Pipelines.Steps;
using Microsoft.Extensions.Logging;

namespace GoldenRaspberryAwards.Infrastructure.Pipelines
{
    public class ProducerIntervalPipeline
    {
        private readonly FetchWinnersStep _fetchWinnersStep;
        private readonly AggregateProducerWinsStep _aggregateStep;
        private readonly CalculateIntervalsStep _calculateStep;
        private readonly BuildResponseStep _buildResponseStep;
        private readonly ILogger<ProducerIntervalPipeline> _logger;

        public ProducerIntervalPipeline(
            FetchWinnersStep fetchWinnersStep,
            AggregateProducerWinsStep aggregateStep,
            CalculateIntervalsStep calculateStep,
            BuildResponseStep buildResponseStep,
            ILogger<ProducerIntervalPipeline> logger)
        {
            _fetchWinnersStep = fetchWinnersStep;
            _aggregateStep = aggregateStep;
            _calculateStep = calculateStep;
            _buildResponseStep = buildResponseStep;
            _logger = logger;
        }

        public async Task<ProducerIntervalResponseDto> ExecuteAsync()
        {
            _logger.LogDebug("Starting producer interval analysis pipeline");

            try
            {
                _logger.LogDebug("Step 1/4: Fetching winning movies");
                var winners = await _fetchWinnersStep.ExecuteAsync(null);

                _logger.LogDebug("Step 2/4: Aggregating wins by producer");
                var producerWins = await _aggregateStep.ExecuteAsync(winners);

                _logger.LogDebug("Step 3/4: Calculating intervals between wins");
                var intervals = await _calculateStep.ExecuteAsync(producerWins);

                _logger.LogDebug("Step 4/4: Building final response");
                var response = await _buildResponseStep.ExecuteAsync(intervals);

                _logger.LogInformation(
                    "Pipeline completed successfully. Min: {MinCount}, Max: {MaxCount}",
                    response.Min?.Count ?? 0,
                    response.Max?.Count ?? 0);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing producer interval pipeline");
                throw;
            }
        }
    }
}
