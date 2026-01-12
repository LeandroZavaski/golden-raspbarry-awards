using GoldenRaspberryAwards.Domain.Entities;
using GoldenRaspberryAwards.Domain.Interfaces;
using GoldenRaspberryAwards.Infrastructure.Parsers;
using GoldenRaspberryAwards.Infrastructure.Validators;
using Microsoft.Extensions.Logging;

namespace GoldenRaspberryAwards.Infrastructure.Services
{
    public class CsvImportService(
        IMovieRepository movieRepository,
        ICsvValidator csvValidator,
        ICsvParser csvParser,
        ILogger<CsvImportService> logger) : ICsvImportService
    {
        private readonly IMovieRepository _movieRepository = movieRepository;
        private readonly ICsvValidator _csvValidator = csvValidator;
        private readonly ICsvParser _csvParser = csvParser;
        private readonly ILogger<CsvImportService> _logger = logger;

        public async Task ImportAsync(string filePath)
        {
            if (await _movieRepository.AnyAsync())
            {
                _logger.LogInformation("Database already contains data, skipping import");
                return;
            }

            _logger.LogInformation("Starting CSV import from: {FilePath}", filePath);

            _csvValidator.ValidateFile(filePath);

            string[] lines;
            try
            {
                lines = await File.ReadAllLinesAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read CSV file: {FilePath}", filePath);
                throw new InvalidOperationException($"Failed to read CSV file: {filePath}", ex);
            }

            if (lines.Length == 0)
            {
                _logger.LogError("CSV file is empty: {FilePath}", filePath);
                throw new InvalidOperationException($"CSV file is empty: {filePath}");
            }

            _csvValidator.ValidateHeader(lines[0], filePath);

            var movies = ParseMovies(lines);

            if (movies.Count == 0)
            {
                _logger.LogError("No valid movie records found in CSV file: {FilePath}", filePath);
                throw new InvalidOperationException($"No valid movie records found in CSV file: {filePath}");
            }

            await _movieRepository.AddRangeAsync(movies);

            _logger.LogInformation("Successfully imported {Count} movies to database", movies.Count);
        }

        private List<Movie> ParseMovies(string[] lines)
        {
            var movies = new List<Movie>();
            var skippedLines = 0;
            var lineNumber = 1; // Header is line 0

            foreach (var line in lines.Skip(1))
            {
                lineNumber++;

                if (string.IsNullOrWhiteSpace(line))
                {
                    _logger.LogDebug("Skipping empty line {LineNumber}", lineNumber);
                    continue;
                }

                try
                {
                    var movie = _csvParser.ParseLine(line, lineNumber);
                    if (movie != null)
                    {
                        movies.Add(movie);
                    }
                    else
                    {
                        skippedLines++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse line {LineNumber}: {Line}", lineNumber, line);
                    skippedLines++;
                }
            }

            _logger.LogInformation(
                "CSV parsing completed. Total lines: {TotalLines}, Valid records: {ValidRecords}, Skipped: {SkippedLines}",
                lines.Length - 1,
                movies.Count,
                skippedLines);

            return movies;
        }
    }
}
