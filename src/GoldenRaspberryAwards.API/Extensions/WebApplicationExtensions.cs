using GoldenRaspberryAwards.Domain.Interfaces;
using Scalar.AspNetCore;

namespace GoldenRaspberryAwards.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }

        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var csvImportService = scope.ServiceProvider.GetRequiredService<ICsvImportService>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<WebApplication>>();

            var csvPath = Path.Combine(AppContext.BaseDirectory, "Data", "movielist.csv");

            if (File.Exists(csvPath))
            {
                logger.LogInformation("Importando dados do CSV: {Path}", csvPath);
                await csvImportService.ImportAsync(csvPath);
                logger.LogInformation("Importação concluída com sucesso");
            }
            else
            {
                logger.LogWarning("Arquivo CSV não encontrado em: {Path}", csvPath);
            }
        }
    }
}
