using GoldenRaspberryAwards.Domain.Entities;

namespace GoldenRaspberryAwards.Infrastructure.Pipelines.Steps
{
    public class AggregateProducerWinsStep
        : IProducerIntervalPipelineStep<IEnumerable<Movie>, Dictionary<string, List<int>>>
    {
        public async Task<Dictionary<string, List<int>>> ExecuteAsync(IEnumerable<Movie> winners)
        {
            var producerWins = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);

            foreach (var movie in winners)
            {
                var producers = SplitProducers(movie.Producers);

                foreach (var producer in producers)
                {
                    var trimmedProducer = producer.Trim();

                    if (string.IsNullOrWhiteSpace(trimmedProducer))
                        continue;

                    if (!producerWins.ContainsKey(trimmedProducer))
                    {
                        producerWins[trimmedProducer] = [];
                    }

                    producerWins[trimmedProducer].Add(movie.Year);
                }
            }
            return producerWins;
        }

        private static IEnumerable<string> SplitProducers(string producers)
        {
            if (string.IsNullOrWhiteSpace(producers))
            {
                return [];
            }

            var normalized = producers.Replace(" and ", ",", StringComparison.OrdinalIgnoreCase);

            return normalized.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim())
                            .Where(p => !string.IsNullOrWhiteSpace(p));
        }
    }
}
