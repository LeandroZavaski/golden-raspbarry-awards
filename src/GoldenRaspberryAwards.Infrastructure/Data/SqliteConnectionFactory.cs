using GoldenRaspberryAwards.Infrastructure.Data.Queries;
using Microsoft.Data.Sqlite;
using System.Data;

namespace GoldenRaspberryAwards.Infrastructure.Data
{
    public class SqliteConnectionFactory : IDbConnectionFactory, IDisposable
    {
        private readonly string _connectionString;
        private readonly SqliteConnection _keepAliveConnection;
        private bool _disposed;

        public SqliteConnectionFactory(string connectionString = "Data Source=:memory:")
        {
            _connectionString = connectionString;

            _keepAliveConnection = new SqliteConnection(_connectionString);
            _keepAliveConnection.Open();

            InitializeDatabase();
        }

        public IDbConnection CreateConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }

        private void InitializeDatabase()
        {
            using var command = _keepAliveConnection.CreateCommand();
            command.CommandText = SqlDataQuery.CreateTable;
            command.ExecuteNonQuery();
        }

        public void ClearDatabase()
        {
            using var command = _keepAliveConnection.CreateCommand();
            command.CommandText = SqlDataQuery.DeleteTable;
            command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _keepAliveConnection?.Close();
                _keepAliveConnection?.Dispose();
                _disposed = true;
            }
        }
    }
}