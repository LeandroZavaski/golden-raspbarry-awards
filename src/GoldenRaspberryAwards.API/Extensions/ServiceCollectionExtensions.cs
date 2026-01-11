using GoldenRaspberryAwards.Domain.Interfaces;
using GoldenRaspberryAwards.Infrastructure.Data;
using GoldenRaspberryAwards.Infrastructure.Repositories;
using GoldenRaspberryAwards.Infrastructure.Services;
using Microsoft.AspNetCore.Connections;

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

        public static IServiceCollection AddServices(this IServiceCollection services) => services
            .AddScoped<ICsvImportService, CsvImportService>()
            .AddScoped<IProducerService, ProducerService>();

        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
    }
}
}
