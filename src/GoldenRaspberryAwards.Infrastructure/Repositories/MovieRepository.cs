using Dapper;
using GoldenRaspberryAwards.Domain.Entities;
using GoldenRaspberryAwards.Domain.Interfaces;
using GoldenRaspberryAwards.Infrastructure.Data;
using GoldenRaspberryAwards.Infrastructure.Repositories.Queries;

namespace GoldenRaspberryAwards.Infrastructure.Repositories
{
    public class MovieRepository(IDbConnectionFactory connectionFactory) : IMovieRepository
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        public async Task<IEnumerable<Movie>> GetWinnersAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = SqlSelectQueries.GetWinners;

            return await connection.QueryAsync<Movie>(sql);
        }

        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = SqlSelectQueries.GetAllMovies;

            return await connection.QueryAsync<Movie>(sql);
        }

        public async Task AddRangeAsync(IEnumerable<Movie> movies)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = SqlInsertQueries.InsertMovie;

            await connection.ExecuteAsync(sql, movies);
        }

        public async Task<bool> AnyAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = SqlSelectQueries.GetAny;

            var count = await connection.ExecuteScalarAsync<int>(sql);
            return count > 0;
        }
    }
}