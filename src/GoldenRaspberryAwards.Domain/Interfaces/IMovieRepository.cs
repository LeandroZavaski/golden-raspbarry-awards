using GoldenRaspberryAwards.Domain.Entities;

namespace GoldenRaspberryAwards.Domain.Interfaces
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetWinnersAsync();

        Task<IEnumerable<Movie>> GetAllAsync();

        Task AddRangeAsync(IEnumerable<Movie> movies);

        Task<bool> AnyAsync();
    }
}
