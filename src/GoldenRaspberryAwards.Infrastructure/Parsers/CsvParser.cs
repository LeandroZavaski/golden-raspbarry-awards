using GoldenRaspberryAwards.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace GoldenRaspberryAwards.Infrastructure.Parsers
{
    public interface ICsvParser
    {
        Movie? ParseLine(string line, int lineNumber);
    }

    public class MovieCsvParser(ILogger<MovieCsvParser> logger) : ICsvParser
    {
        private readonly ILogger<MovieCsvParser> _logger = logger;

        public Movie? ParseLine(string line, int lineNumber)
        {
            var columns = line.Split(';');

            if (columns.Length < 4)
            {
                _logger.LogWarning(
                    "Line {LineNumber} has insufficient columns ({Count}). Expected at least 4. Skipping.",
                    lineNumber,
                    columns.Length);
                return null;
            }

            if (!int.TryParse(columns[0].Trim(), out int year))
            {
                _logger.LogWarning(
                    "Line {LineNumber} has invalid year value: '{Year}'. Skipping.",
                    lineNumber,
                    columns[0].Trim());
                return null;
            }

            var title = columns[1].Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                _logger.LogWarning("Line {LineNumber} has empty title. Skipping.", lineNumber);
                return null;
            }

            var studios = columns[2].Trim();
            var producers = columns[3].Trim();

            if (string.IsNullOrWhiteSpace(producers))
            {
                _logger.LogWarning("Line {LineNumber} has empty producers. Skipping.", lineNumber);
                return null;
            }

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