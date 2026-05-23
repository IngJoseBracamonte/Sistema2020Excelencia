using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Tests
{
    public static class CheckColumn
    {
        public static async Task Main(string[] args)
        {
            var connectionString = "server=localhost;database=SatHospitalario;user=root;password=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=None";
            try
            {
                using var conn = new MySqlConnection(connectionString);
                await conn.OpenAsync();
                
                var tables = new List<string>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SHOW TABLES;";
                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        tables.Add(reader.GetString(0));
                    }
                }
                
                Console.WriteLine("--- TABLE ROW COUNTS in SatHospitalario ---");
                foreach (var table in tables)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT COUNT(*) FROM `{table}`;";
                        var count = await cmd.ExecuteScalarAsync();
                        Console.WriteLine($"- {table}: {count}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}



