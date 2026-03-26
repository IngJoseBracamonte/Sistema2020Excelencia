using MySqlConnector;
using System;

string connectionString = "Server=localhost;User ID=root;Database=sathospitalario;";

try
{
    using var connection = new MySqlConnection(connectionString);
    connection.Open();
    Console.WriteLine("Conectado a MySQL.");

    using var command = new MySqlCommand("SHOW TABLES;", connection);
    using var reader = command.ExecuteReader();
    Console.WriteLine("Tablas en 'sathospitalario':");
    while (reader.Read())
    {
        Console.WriteLine($"- {reader.GetString(0)}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
