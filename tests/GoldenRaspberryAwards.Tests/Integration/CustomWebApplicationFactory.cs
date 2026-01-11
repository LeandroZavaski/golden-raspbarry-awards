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
                // Remove o IDbConnectionFactory registrado
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IDbConnectionFactory));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Cria uma nova fábrica de conexões para testes
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

            // Limpa o banco (se for SqliteConnectionFactory)
            if (connectionFactory is SqliteConnectionFactory sqliteFactory)
            {
                sqliteFactory.ClearDatabase();
            }

            // Carrega os dados do CSV
            var csvImportService = scope.ServiceProvider.GetRequiredService<ICsvImportService>();
            var csvPath = GetTestCsvPath();

            if (File.Exists(csvPath))
            {
                await csvImportService.ImportAsync(csvPath);
            }
            else
            {
                throw new FileNotFoundException($"Arquivo CSV de teste não encontrado: {csvPath}");
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

        /// <summary>
        /// Obtém o caminho do arquivo CSV para testes.
        /// </summary>
        private static string GetTestCsvPath()
        {
            // Procura pelo arquivo CSV na estrutura do projeto
            var baseDir = AppContext.BaseDirectory;

            // Tenta no diretório Data local
            var csvPath = Path.Combine(baseDir, "Data", "movielist.csv");
            if (File.Exists(csvPath))
                return csvPath;

            // Tenta subir na hierarquia de diretórios para encontrar o arquivo
            var currentDir = new DirectoryInfo(baseDir);
            while (currentDir != null)
            {
                // Procura no diretório de testes
                var testPath = Path.Combine(currentDir.FullName, "tests", "GoldenRaspberryAwards.Tests", "Data", "movielist.csv");
                if (File.Exists(testPath))
                    return testPath;

                // Procura no diretório da API
                var apiPath = Path.Combine(currentDir.FullName, "src", "GoldenRaspberryAwards.API", "Data", "movielist.csv");
                if (File.Exists(apiPath))
                    return apiPath;

                // Procura diretamente no diretório Data
                var directPath = Path.Combine(currentDir.FullName, "Data", "movielist.csv");
                if (File.Exists(directPath))
                    return directPath;

                currentDir = currentDir.Parent;
            }

            return csvPath;
        }
    }
}