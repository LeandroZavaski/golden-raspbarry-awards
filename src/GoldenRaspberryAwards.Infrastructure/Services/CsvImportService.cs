using GoldenRaspberryAwards.Domain.Entities;
using GoldenRaspberryAwards.Domain.Interfaces;

namespace GoldenRaspberryAwards.Infrastructure.Services
{
    public class CsvImportService(IMovieRepository movieRepository) : ICsvImportService
    {
        private readonly IMovieRepository _movieRepository = movieRepository;

        public async Task ImportAsync(string filePath)
        {
            if (await _movieRepository.AnyAsync())
            {
                return;
            }

            var movies = new List<Movie>();
            var lines = await File.ReadAllLinesAsync(filePath);

            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var movie = ParseLine(line);
                if (movie != null)
                {
                    movies.Add(movie);
                }
            }

            await _movieRepository.AddRangeAsync(movies);
        }

        private static Movie? ParseLine(string line)
        {
            var columns = line.Split(';');

            if (columns.Length < 4)
                return null;

            if (!int.TryParse(columns[0].Trim(), out int year))
                return null;

            var title = columns[1].Trim();
            var studios = columns[2].Trim();
            var producers = columns[3].Trim();

            var winner = columns.Length > 4 &&
                         columns[4].Trim().Equals("yes", StringComparison.OrdinalIgnoreCase);

            return new Movie
            {
                Year = year,
                Title = title,
                Studios = studios,
                Producers = producers,
                Winner = winner
            };
        }
    }
}
