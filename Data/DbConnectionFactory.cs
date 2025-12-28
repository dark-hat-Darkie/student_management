using Npgsql;
using System.Data;

namespace StudentManagement.Data;

/// <summary>
/// Factory for creating database connections.
/// Centralizes connection string management.
/// </summary>
public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Creates and opens a new database connection.
    /// </summary>
    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        return connection;
    }

    /// <summary>
    /// Tests the database connection.
    /// </summary>
    public bool TestConnection()
    {
        try
        {
            using var connection = CreateConnection();
            return connection.State == ConnectionState.Open;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Initialize database tables from schema.
    /// </summary>
    public async Task InitializeDatabaseAsync(string schemaPath)
    {
        if (!File.Exists(schemaPath))
        {
            Console.WriteLine($"Schema file not found: {schemaPath}");
            return;
        }

        var schema = await File.ReadAllTextAsync(schemaPath);

        using var connection = CreateConnection();
        using var command = ((NpgsqlConnection)connection).CreateCommand();
        command.CommandText = schema;
        await command.ExecuteNonQueryAsync();

        Console.WriteLine("Database tables initialized successfully.");
    }
}
