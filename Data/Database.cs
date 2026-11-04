using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

namespace BibliotecaAca.Data;

public static class Database
{
    private static string _connectionString = "";

    public static void Initialize()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (!File.Exists(path))
            throw new FileNotFoundException("No se encontró appsettings.json.", path);

        using var document = JsonDocument.Parse(File.ReadAllText(path));
        _connectionString = document.RootElement.GetProperty("ConnectionString").GetString()
            ?? throw new InvalidOperationException("ConnectionString está vacío.");
        using var connection = GetConnection();
        connection.Open();
        using var command = new SqlCommand("SELECT DB_ID(@db)", connection);
        command.Parameters.AddWithValue("@db", "BibliotecaAca");
        var databaseId = command.ExecuteScalar();
        if (databaseId is null || databaseId is DBNull)
            throw new InvalidOperationException("La base BibliotecaAca no existe. Ejecute primero el archivo SQL/01_BibliotecaAca.sql.");
    }

    public static SqlConnection GetConnection() => new(_connectionString);

    public static DataTable Query(string sql, params SqlParameter[] parameters)
    {
        using var connection = GetConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddRange(parameters);
        using var adapter = new SqlDataAdapter(command);
        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    public static int Execute(string sql, params SqlParameter[] parameters)
    {
        using var connection = GetConnection();
        connection.Open();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddRange(parameters);
        return command.ExecuteNonQuery();
    }

    public static object? Scalar(string sql, params SqlParameter[] parameters)
    {
        using var connection = GetConnection();
        connection.Open();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddRange(parameters);
        return command.ExecuteScalar();
    }

    public static SqlParameter P(string name, object? value) =>
        new(name, value ?? DBNull.Value);
}
