using System;
using MySqlConnector;

try {
    string connStr = "Server=localhost;Database=sistema2020;Uid=root;Pwd=Labordono1818;SslMode=none;";
    using var conn = new MySqlConnection(connStr);
    conn.Open();
    using var cmd = conn.CreateCommand();
    cmd.CommandText = "SELECT COUNT(*) FROM perfilesfacturados";
    var count = cmd.ExecuteScalar();
    Console.WriteLine($"Row count in 'perfilesfacturados' table: {count}");
    
    cmd.CommandText = "SHOW COLUMNS FROM perfilesfacturados";
    using var reader = cmd.ExecuteReader();
    Console.WriteLine("Columns in 'perfilesfacturados' table:");
    while(reader.Read()) {
        Console.WriteLine($"- {reader[0]} ({reader[1]})");
    }
} catch (Exception ex) {
    Console.WriteLine($"Error: {ex.Message}");
}
