using System.Data;
using Npgsql;

namespace SmartCity.DataLayer.UserService.Infrastructure;

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