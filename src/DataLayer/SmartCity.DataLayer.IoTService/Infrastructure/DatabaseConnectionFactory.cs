using Npgsql;
using System.Data;

namespace SmartCity.DataLayer.IoTService.Infrastructure;

public class DatabaseConnectionFactory
{
    private readonly string _connectionString;
    
    public DatabaseConnectionFactory(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public string ConnectionString => _connectionString;

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}