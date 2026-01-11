using FluentAssertions;
using GoldenRaspberryAwards.Domain.Dtos;
using GoldenRaspberryAwards.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace GoldenRaspberryAwards.Tests.Integration
{
    public class ProducersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public ProducersControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            await _factory.InitializeDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        /// <summary>
        /// Testa se o endpoint retorna status 200 OK.
        /// </summary>
        [Fact]
        public async Task GetAwardsInterval_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/producers/awards-interval");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// Testa se o endpoint retorna a estrutura correta da resposta.
        /// </summary>
        [Fact]
        public async Task GetAwardsInterval_ReturnsCorrectStructure()
        {
            // Act
            var response = await _client.GetAsync("/api/producers/awards-interval");
            var result = await response.Content.ReadFromJsonAsync<ProducerIntervalResponseDto>();

            // Assert
            result.Should().NotBeNull();
            result!.Min.Should().NotBeNull();
            result.Max.Should().NotBeNull();
        }

        /// <summary>
        /// Testa se os intervalos mínimos estão corretos.
        /// Com base nos dados do CSV fornecido, verifica os produtores com menor intervalo entre prêmios.
        /// Joel Silver ganhou em 1990 e 1991 (intervalo de 1 ano)
        /// </summary>
        [Fact]
        public async Task GetAwardsInterval_ReturnsCorrectMinInterval()
        {
            // Act
            var response = await _client.GetAsync("/api/producers/awards-interval");
            var result = await response.Content.ReadFromJsonAsync<ProducerIntervalResponseDto>();

            // Assert
            result.Should().NotBeNull();
            result!.Min.Should().NotBeEmpty("deveria haver produtores com intervalos mínimos");

            // Verifica que o intervalo mínimo é 1 (conforme dados do CSV)
            // Joel Silver ganhou em 1990 e 1991 (intervalo de 1 ano)
            var joelSilverInterval = result.Min.FirstOrDefault(p => p.Producer == "Joel Silver");
            joelSilverInterval.Should().NotBeNull("Joel Silver deveria estar na lista de intervalos mínimos");
            joelSilverInterval!.Interval.Should().Be(1);
            joelSilverInterval.PreviousWin.Should().Be(1990);
            joelSilverInterval.FollowingWin.Should().Be(1991);
        }

        /// <summary>
        /// Testa se os intervalos máximos estão corretos.
        /// Com base nos dados do CSV fornecido, verifica os produtores com maior intervalo entre prêmios.
        /// Matthew Vaughn ganhou em 2002 e 2015 (intervalo de 13 anos)
        /// </summary>
        [Fact]
        public async Task GetAwardsInterval_ReturnsCorrectMaxInterval()
        {
            // Act
            var response = await _client.GetAsync("/api/producers/awards-interval");
            var result = await response.Content.ReadFromJsonAsync<ProducerIntervalResponseDto>();

            // Assert
            result.Should().NotBeNull();
            result!.Max.Should().NotBeEmpty("deveria haver produtores com intervalos máximos");

            // Verifica que o intervalo máximo é 13 (conforme dados do CSV)
            // Matthew Vaughn ganhou em 2002 e 2015 (intervalo de 13 anos)
            var matthewVaughnInterval = result.Max.FirstOrDefault(p => p.Producer == "Matthew Vaughn");
            matthewVaughnInterval.Should().NotBeNull("Matthew Vaughn deveria estar na lista de intervalos máximos");
            matthewVaughnInterval!.Interval.Should().Be(13);
            matthewVaughnInterval.PreviousWin.Should().Be(2002);
            matthewVaughnInterval.FollowingWin.Should().Be(2015);
        }

        /// <summary>
        /// Testa se todos os produtores no resultado têm pelo menos dois prêmios.
        /// </summary>
        [Fact]
        public async Task GetAwardsInterval_AllProducersHaveConsecutiveWins()
        {
            // Act
            var response = await _client.GetAsync("/api/producers/awards-interval");
            var result = await response.Content.ReadFromJsonAsync<ProducerIntervalResponseDto>();

            // Assert
            result.Should().NotBeNull();

            foreach (var interval in result!.Min.Concat(result.Max))
            {
                interval.Producer.Should().NotBeNullOrEmpty();
                interval.Interval.Should().BeGreaterThan(0);
                interval.FollowingWin.Should().BeGreaterThan(interval.PreviousWin);
                (interval.FollowingWin - interval.PreviousWin).Should().Be(interval.Interval);
            }
        }

        /// <summary>
        /// Testa se os intervalos mínimos e máximos estão corretos em relação aos dados.
        /// </summary>
        [Fact]
        public async Task GetAwardsInterval_MinIntervalsAreSmallerOrEqualToMax()
        {
            // Act
            var response = await _client.GetAsync("/api/producers/awards-interval");
            var result = await response.Content.ReadFromJsonAsync<ProducerIntervalResponseDto>();

            // Assert
            result.Should().NotBeNull();

            if (result!.Min.Any() && result.Max.Any())
            {
                var minValue = result.Min.Min(m => m.Interval);
                var maxValue = result.Max.Max(m => m.Interval);
                minValue.Should().BeLessThanOrEqualTo(maxValue);
            }
        }

        /// <summary>
        /// Testa se todos os produtores com o mesmo intervalo mínimo são retornados.
        /// </summary>
        [Fact]
        public async Task GetAwardsInterval_ReturnsAllProducersWithMinInterval()
        {
            // Act
            var response = await _client.GetAsync("/api/producers/awards-interval");
            var result = await response.Content.ReadFromJsonAsync<ProducerIntervalResponseDto>();

            // Assert
            result.Should().NotBeNull();

            if (result!.Min.Any())
            {
                // Todos os intervalos mínimos devem ter o mesmo valor
                var minInterval = result.Min.First().Interval;
                result.Min.All(m => m.Interval == minInterval).Should().BeTrue();
            }
        }

        /// <summary>
        /// Testa se todos os produtores com o mesmo intervalo máximo são retornados.
        /// </summary>
        [Fact]
        public async Task GetAwardsInterval_ReturnsAllProducersWithMaxInterval()
        {
            // Act
            var response = await _client.GetAsync("/api/producers/awards-interval");
            var result = await response.Content.ReadFromJsonAsync<ProducerIntervalResponseDto>();

            // Assert
            result.Should().NotBeNull();

            if (result!.Max.Any())
            {
                // Todos os intervalos máximos devem ter o mesmo valor
                var maxInterval = result.Max.First().Interval;
                result.Max.All(m => m.Interval == maxInterval).Should().BeTrue();
            }
        }

        /// <summary>
        /// Testa se os dados do banco foram carregados corretamente do CSV.
        /// </summary>
        [Fact]
        public async Task Database_ShouldHaveMoviesLoaded()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IMovieRepository>();

            // Act
            var movies = await repository.GetAllAsync();
            var winners = await repository.GetWinnersAsync();

            // Assert
            movies.Should().NotBeEmpty("deveria haver filmes carregados do CSV");
            winners.Should().NotBeEmpty("deveria haver filmes vencedores");

            // Verifica alguns dados específicos do CSV
            winners.Count().Should().BeGreaterThan(30, "deveria haver mais de 30 vencedores no CSV");
        }
    }
}