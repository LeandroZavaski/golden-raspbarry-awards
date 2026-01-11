using GoldenRaspberryAwards.Domain.Interfaces;
using GoldenRaspberryAwards.Infrastructure.Data;
using GoldenRaspberryAwards.Infrastructure.Pipelines;
using GoldenRaspberryAwards.Infrastructure.Pipelines.Steps;
using GoldenRaspberryAwards.Infrastructure.Repositories;
using GoldenRaspberryAwards.Infrastructure.Services;

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
