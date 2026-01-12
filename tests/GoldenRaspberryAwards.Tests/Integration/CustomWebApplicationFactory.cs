using GoldenRaspberryAwards.Domain.Interfaces;
using GoldenRaspberryAwards.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GoldenRaspberryAwards.Tests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private SqliteConnectionFactory? _connectionFactory;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IDbConnectionFactory));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                _connectionFactory = new SqliteConnectionFactory(
                    $"Data Source=TestDb_{Guid.NewGuid()};Mode=Memory;Cache=Shared");

                services.AddSingleton<IDbConnectionFactory>(_connectionFactory);
            });

            builder.UseEnvironment("Development");
        }

        public async Task InitializeDatabaseAsync()
        {
            using var scope = Services.CreateScope();
            var connectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();

            if (connectionFactory is SqliteConnectionFactory sqliteFactory)
            {
                sqliteFactory.ClearDatabase();
            }

            var csvImportService = scope.ServiceProvider.GetRequiredService<ICsvImportService>();
            var csvPath = GetTestCsvPath();

            if (File.Exists(csvPath))
            {
                await csvImportService.ImportAsync(csvPath);
            }
            else
            {
                throw new FileNotFoundException($"Test CSV file not found: {csvPath}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connectionFactory?.Dispose();
            }
            base.Dispose(disposing);
        }

        private static string GetTestCsvPath()
        {
            var baseDir = AppContext.BaseDirectory;

            // Try both lowercase and capitalized versions for cross-platform compatibility
            var possibleNames = new[] { "movielist.csv", "Movielist.csv" };

            foreach (var fileName in possibleNames)
            {
                var csvPath = Path.Combine(baseDir, "Data", fileName);
                if (File.Exists(csvPath))
                    return csvPath;
            }

            var currentDir = new DirectoryInfo(baseDir);
            while (currentDir != null)
            {
                foreach (var fileName in possibleNames)
                {
                    var testPath = Path.Combine(currentDir.FullName, "tests", "GoldenRaspberryAwards.Tests", "Data", fileName);
                    if (File.Exists(testPath))
                        return testPath;

                    var apiPath = Path.Combine(currentDir.FullName, "src", "GoldenRaspberryAwards.API", "Data", fileName);
                    if (File.Exists(apiPath))
                        return apiPath;

                    var directPath = Path.Combine(currentDir.FullName, "Data", fileName);
                    if (File.Exists(directPath))
                        return directPath;
                }

                currentDir = currentDir.Parent;
            }

            return Path.Combine(baseDir, "Data", "movielist.csv");
        }
    }
}