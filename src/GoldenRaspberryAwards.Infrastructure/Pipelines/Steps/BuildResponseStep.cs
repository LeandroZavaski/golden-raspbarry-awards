using GoldenRaspberryAwards.Domain.Dtos;

namespace GoldenRaspberryAwards.Infrastructure.Pipelines.Steps
{
    public class BuildResponseStep
        : IProducerIntervalPipelineStep<List<ProducerIntervalDto>, ProducerIntervalResponseDto>
    {
        public Task<ProducerIntervalResponseDto> ExecuteAsync(List<ProducerIntervalDto> input)
        {
            if (input.Count == 0)
            {
                return Task.FromResult(new ProducerIntervalResponseDto());
            }

            var minInterval = GetMinInterval(input);
            var maxInterval = GetMaxInterval(input);

            var response = new ProducerIntervalResponseDto
            {
                Min = input.Where(i => i.Interval == minInterval).ToList(),
                Max = input.Where(i => i.Interval == maxInterval).ToList()
            };

            return Task.FromResult(response);
        }

        private static int GetMinInterval(List<ProducerIntervalDto> intervals)
        {
            return intervals.Count > 0 ? intervals.Min(i => i.Interval) : 0;
        }

        private static int GetMaxInterval(List<ProducerIntervalDto> intervals)
        {
            return intervals.Count > 0 ? intervals.Max(i => i.Interval) : 0;
        }
    }
}
