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
            _logger.LogDebug("Iniciando pipeline de análise de intervalos de produtores");

            try
            {
                _logger.LogDebug("Etapa 1/4: Buscando filmes vencedores");
                var winners = await _fetchWinnersStep.ExecuteAsync(null);

                _logger.LogDebug("Etapa 2/4: Agregando vitórias por produtor");
                var producerWins = await _aggregateStep.ExecuteAsync(winners);

                _logger.LogDebug("Etapa 3/4: Calculando intervalos entre vitórias");
                var intervals = await _calculateStep.ExecuteAsync(producerWins);

                _logger.LogDebug("Etapa 4/4: Construindo resposta final");
                var response = await _buildResponseStep.ExecuteAsync(intervals);

                _logger.LogInformation(
                    "Pipeline concluído com sucesso. Min: {MinCount}, Max: {MaxCount}",
                    response.Min?.Count ?? 0,
                    response.Max?.Count ?? 0);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar pipeline de intervalos de produtores");
                throw;
            }
        }
    }
}
