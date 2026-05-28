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
                
                Console.WriteLine("--- TABLAS EN SATHOSPITALARIO ---");
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SHOW TABLES;";
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"- {reader.GetString(0)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}



