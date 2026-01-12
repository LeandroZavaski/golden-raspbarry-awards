using Microsoft.Extensions.Logging;

namespace GoldenRaspberryAwards.Infrastructure.Validators
{
    public interface ICsvValidator
    {
        void ValidateFile(string filePath);
        void ValidateHeader(string headerLine, string filePath);
    }

    public class CsvValidator(ILogger<CsvValidator> logger) : ICsvValidator
    {
        private readonly ILogger<CsvValidator> _logger = logger;

        public void ValidateFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                _logger.LogError("CSV file not found: {FilePath}", filePath);
                throw new FileNotFoundException($"CSV file not found: {filePath}", filePath);
            }

            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Length == 0)
            {
                _logger.LogError("CSV file is empty: {FilePath}", filePath);
                throw new InvalidOperationException($"CSV file is empty: {filePath}");
            }

            _logger.LogDebug("File validation passed for: {FilePath}", filePath);
        }

        public void ValidateHeader(string headerLine, string filePath)
        {
            var expectedHeaders = new[] { "year", "title", "studios", "producers", "winner" };
            var actualHeaders = headerLine.Split(';')
                                          .Select(h => h.Trim().ToLowerInvariant())
                                          .ToArray();

            if (actualHeaders.Length < 4)
            {
                _logger.LogError(
                    "Invalid CSV header format in {FilePath}. Expected at least 4 columns (year;title;studios;producers), found {Count}",
                    filePath,
                    actualHeaders.Length);
                throw new InvalidOperationException(
                    $"Invalid CSV header format. Expected columns: {string.Join(", ", expectedHeaders)}. " +
                    $"Found: {string.Join(", ", actualHeaders)}");
            }

            for (int i = 0; i < Math.Min(4, expectedHeaders.Length); i++)
            {
                if (i >= actualHeaders.Length || !actualHeaders[i].Equals(expectedHeaders[i], StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning(
                        "CSV header mismatch at position {Position}. Expected '{Expected}', found '{Actual}'",
                        i,
                        expectedHeaders[i],
                        i < actualHeaders.Length ? actualHeaders[i] : "missing");
                }
            }

            _logger.LogDebug("CSV header validation passed");
        }
    }
}