using GoldenRaspberryAwards.Domain.Dtos;
using GoldenRaspberryAwards.Domain.Interfaces;
using GoldenRaspberryAwards.Infrastructure.Pipelines;

namespace GoldenRaspberryAwards.Infrastructure.Services
{
    public class ProducerService(ProducerIntervalPipeline pipeline) : IProducerService
    {
        private readonly ProducerIntervalPipeline _pipeline = pipeline;

        public async Task<ProducerIntervalResponseDto> GetProducerIntervalsAsync()
        {
            return await _pipeline.ExecuteAsync();
        }
    }
}
