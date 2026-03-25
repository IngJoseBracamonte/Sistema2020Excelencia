using System;
using MySqlConnector;

var connectionString = "Server=localhost;Port=3306;Database=SatHospitalario;Uid=root;Pwd=Labordono1818;Connection Timeout=20;";
var connectionStringIdentity = "Server=localhost;Port=3306;Database=SatHospitalarioIdentity;Uid=root;Pwd=Labordono1818;Connection Timeout=20;";

Console.WriteLine("--- Checking SatHospitalario DB ---");
CheckDb(connectionString);
Console.WriteLine("\n--- Checking SatHospitalarioIdentity DB ---");
CheckDb(connectionStringIdentity);

void CheckDb(string connStr)
{
    try
    {
        using var connection = new MySqlConnection(connStr);
        connection.Open();
        Console.WriteLine($"Connected to {connection.Database}");
        
        using var command = new MySqlCommand("SHOW TABLES;", connection);
        using var reader = command.ExecuteReader();
        
        if (!reader.HasRows)
        {
            Console.WriteLine("No tables found.");
        }
        else
        {
            Console.WriteLine("Tables:");
            while (reader.Read())
            {
                Console.WriteLine($"- {reader.GetString(0)}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
