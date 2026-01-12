using GoldenRaspberryAwards.Domain.Entities;
using GoldenRaspberryAwards.Domain.Interfaces;

namespace GoldenRaspberryAwards.Domain.Pipelines.Steps
{
    public class FetchWinnersStep(IMovieRepository movieRepository) 
        : IProducerIntervalPipelineStep<object?, IEnumerable<Movie>>
    {
        private readonly IMovieRepository _movieRepository = movieRepository;

        public async Task<IEnumerable<Movie>> ExecuteAsync(object? input)
        {
            return await _movieRepository.GetWinnersAsync();
        }
    }
}
