using System;
using MySqlConnector;

try
{
    Console.WriteLine("=== MySQL Database Diagnostics (Databases & Tables) ===");
    var defaultConnStr = "server=localhost;user=root;password=Labordono1818;SslMode=None;AllowPublicKeyRetrieval=True;";
    var connStr = SistemaSatHospitalario.Infrastructure.Common.Helpers.ConnectionStringHelper.ResolveConnectionStringWithAppConfig(defaultConnStr);
    Console.WriteLine($"[INFO] Resolved connection string: {connStr}");
    using var conn = new MySqlConnection(connStr);
    conn.Open();

    Console.WriteLine("\n[Databases]:");
    using (var cmd = new MySqlCommand("SHOW DATABASES;", conn))
    using (var reader = cmd.ExecuteReader())
    {
        while (reader.Read())
        {
            Console.WriteLine($"- {reader.GetString(0)}");
        }
    }

    Console.WriteLine("\n[Checking tables in 'sistema2020']:");
    var tables = new[] { "convenios", "datospersonales", "ordenes", "perfil", "perfilesanalisis", "perfilesfacturados", "resultadospaciente" };
    foreach (var table in tables)
    {
        try
        {
            using var cmd = new MySqlCommand($"SELECT COUNT(*) FROM sistema2020.{table}", conn);
            var count = cmd.ExecuteScalar();
            Console.WriteLine($"- Table sistema2020.{table}: {count} rows");
            
            if (table == "perfil")
            {
                // Let's print columns of perfil
                using var cmdCol = new MySqlCommand("DESCRIBE sistema2020.perfil", conn);
                using var readerCol = cmdCol.ExecuteReader();
                Console.WriteLine("  Columns:");
                while (readerCol.Read())
                {
                    Console.WriteLine($"    * {readerCol.GetString(0)} ({readerCol.GetString(1)})");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"- Table sistema2020.{table}: ERROR ({ex.Message})");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"[CRITICAL ERROR]: {ex.Message}");
}

