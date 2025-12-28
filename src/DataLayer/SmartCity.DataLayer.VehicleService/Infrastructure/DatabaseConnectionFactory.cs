/*
 * ===================================================================================
 * TRABALHO PRÁTICO: Integração de Sistemas de Informação (ISI)
 * -----------------------------------------------------------------------------------
 * Nome: Mario Junior Manhente Portilho
 * Número: a27989
 * Curso: Engenharia de Sistemas Informáticos
 * Ano Letivo: 2025/2026
 * ===================================================================================
 */

using Npgsql;
using System.Data;

namespace SmartCity.DataLayer.VehicleService.Infrastructure;

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