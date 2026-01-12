using GoldenRaspberryAwards.Domain.Interfaces;
using GoldenRaspberryAwards.Domain.Pipelines;
using GoldenRaspberryAwards.Domain.Pipelines.Steps;
using GoldenRaspberryAwards.Domain.Services;
using GoldenRaspberryAwards.Infrastructure.Data;
using GoldenRaspberryAwards.Infrastructure.Parsers;
using GoldenRaspberryAwards.Infrastructure.Repositories;
using GoldenRaspberryAwards.Infrastructure.Services;
using GoldenRaspberryAwards.Infrastructure.Validators;

namespace GoldenRaspberryAwards.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ProjectBootstrap(this IServiceCollection services) => services
            .AddRepositories()
            .AddServices()
            .AddApiServices();


        public static IServiceCollection AddRepositories(this IServiceCollection services) => services
            .AddSingleton<IDbConnectionFactory>(sp => new SqliteConnectionFactory("Data Source=GoldenRaspberryAwards;Mode=Memory;Cache=Shared"))
            .AddScoped<IMovieRepository, MovieRepository>();

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Pipeline Steps (cada etapa do processamento)
            services.AddScoped<FetchWinnersStep>();
            services.AddScoped<AggregateProducerWinsStep>();
            services.AddScoped<CalculateIntervalsStep>();
            services.AddScoped<BuildResponseStep>();

            // Pipeline completo
            services.AddScoped<ProducerIntervalPipeline>();

            // Serviços principais
            services.AddScoped<ICsvImportService, CsvImportService>();
            services.AddScoped<IProducerService, ProducerService>();

            // CSV Components
            services.AddScoped<ICsvValidator, CsvValidator>();
            services.AddScoped<ICsvParser, MovieCsvParser>();

            return services;
        }

        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddOpenApi();

            return services;
        }
    }
}
