using GoldenRaspberryAwards.Domain.Dtos;

namespace GoldenRaspberryAwards.Infrastructure.Pipelines.Steps
{
    public class CalculateIntervalsStep
        : IProducerIntervalPipelineStep<Dictionary<string, List<int>>, List<ProducerIntervalDto>>
    {
        public async Task<List<ProducerIntervalDto>> ExecuteAsync(Dictionary<string, List<int>> producerWins)
        {
            var intervals = new List<ProducerIntervalDto>();

            foreach (var kvp in producerWins)
            {
                var producer = kvp.Key;
                var years = kvp.Value.OrderBy(y => y).ToList();

                if (years.Count < 2)
                    continue;

                for (int i = 1; i < years.Count; i++)
                {
                    intervals.Add(new ProducerIntervalDto
                    {
                        Producer = producer,
                        Interval = years[i] - years[i - 1],
                        PreviousWin = years[i - 1],
                        FollowingWin = years[i]
                    });
                }
            }

            return intervals;
        }
    }
}
