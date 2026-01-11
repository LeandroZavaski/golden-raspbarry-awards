using FluentAssertions;
using GoldenRaspberryAwards.Domain.Dtos;
using GoldenRaspberryAwards.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace GoldenRaspberryAwards.Tests.Integration
{
    public class ProducersControllerIntegrationTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly HttpClient _client = factory.CreateClient();
        private readonly CustomWebApplicationFactory _factory = factory;

        public async Task InitializeAsync()
        {
            await _factory.InitializeDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;


        [Fact]
        public async Task GetAwardsInterval_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/producers/awards-interval");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

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


        [Fact]
        public async Task GetAwardsInterval_ReturnsCorrectMinInterval()
        {
            // Act
            var response = await _client.GetAsync("/api/producers/awards-interval");
            var result = await response.Content.ReadFromJsonAsync<ProducerIntervalResponseDto>();

            // Assert
            result.Should().NotBeNull();
            result!.Min.Should().NotBeEmpty("deveria haver produtores com intervalos mínimos");

            var joelSilverInterval = result.Min.FirstOrDefault(p => p.Producer == "Joel Silver");
            joelSilverInterval.Should().NotBeNull("Joel Silver deveria estar na lista de intervalos mínimos");
            joelSilverInterval!.Interval.Should().Be(1);
            joelSilverInterval.PreviousWin.Should().Be(1990);
            joelSilverInterval.FollowingWin.Should().Be(1991);
        }

        [Fact]
        public async Task GetAwardsInterval_ReturnsCorrectMaxInterval()
        {
            // Act
            var response = await _client.GetAsync("/api/producers/awards-interval");
            var result = await response.Content.ReadFromJsonAsync<ProducerIntervalResponseDto>();

            // Assert
            result.Should().NotBeNull();
            result!.Max.Should().NotBeEmpty("deveria haver produtores com intervalos máximos");

            var matthewVaughnInterval = result.Max.FirstOrDefault(p => p.Producer == "Matthew Vaughn");
            matthewVaughnInterval.Should().NotBeNull("Matthew Vaughn deveria estar na lista de intervalos máximos");
            matthewVaughnInterval!.Interval.Should().Be(13);
            matthewVaughnInterval.PreviousWin.Should().Be(2002);
            matthewVaughnInterval.FollowingWin.Should().Be(2015);
        }

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
                var minInterval = result.Min.First().Interval;
                result.Min.All(m => m.Interval == minInterval).Should().BeTrue();
            }
        }

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
                var maxInterval = result.Max.First().Interval;
                result.Max.All(m => m.Interval == maxInterval).Should().BeTrue();
            }
        }

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

            winners.Count().Should().BeGreaterThan(30, "deveria haver mais de 30 vencedores no CSV");
        }
    }
}