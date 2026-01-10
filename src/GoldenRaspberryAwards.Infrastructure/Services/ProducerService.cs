using GoldenRaspberryAwards.Domain.Dtos;
using GoldenRaspberryAwards.Domain.Interfaces;

namespace GoldenRaspberryAwards.Infrastructure.Services
{
    public class ProducerService(IMovieRepository movieRepository) : IProducerService
    {
        private readonly IMovieRepository _movieRepository = movieRepository;

        public async Task<ProducerIntervalResponseDto> GetProducerIntervalsAsync()
        {
            var winners = await _movieRepository.GetWinnersAsync();

            var producerWins = GetProducerWins(winners);

            var intervals = CalculateIntervals(producerWins);

            if (!intervals.Any())
            {
                return new ProducerIntervalResponseDto();
            }

            // Encontra o intervalo mínimo e máximo
            var minInterval = intervals.Min(i => i.Interval);
            var maxInterval = intervals.Max(i => i.Interval);

            return new ProducerIntervalResponseDto
            {
                Min = intervals.Where(i => i.Interval == minInterval).ToList(),
                Max = intervals.Where(i => i.Interval == maxInterval).ToList()
            };
        }

        private static Dictionary<string, List<int>> GetProducerWins(IEnumerable<Domain.Entities.Movie> winners)
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
                        producerWins[trimmedProducer] = new List<int>();
                    }

                    producerWins[trimmedProducer].Add(movie.Year);
                }
            }

            return producerWins;
        }

        private static IEnumerable<string> SplitProducers(string producers)
        {
            var normalized = producers.Replace(" and ", ",", StringComparison.OrdinalIgnoreCase);

            return normalized.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim())
                            .Where(p => !string.IsNullOrWhiteSpace(p));
        }

        private static List<ProducerIntervalDto> CalculateIntervals(Dictionary<string, List<int>> producerWins)
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
