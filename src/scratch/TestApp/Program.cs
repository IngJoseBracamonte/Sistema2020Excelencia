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
                
                Console.WriteLine("--- ALTERANDO TABLA CUENTASPORCOBRAR PARA AGREGAR METADATA ---");
                
                var cols = new Dictionary<string, string>
                {
                    { "QuienAutorizo", "VARCHAR(500) NULL" },
                    { "DoctorProcedimiento", "VARCHAR(500) NULL" },
                    { "InformacionAdicional", "VARCHAR(2000) NULL" }
                };

                foreach (var col in cols)
                {
                    try
                    {
                        using var cmd = conn.CreateCommand();
                        cmd.CommandText = $"ALTER TABLE CuentasPorCobrar ADD COLUMN {col.Key} {col.Value};";
                        await cmd.ExecuteNonQueryAsync();
                        Console.WriteLine($"Columna '{col.Key}' agregada exitosamente.");
                    }
                    catch (Exception ex) when (ex.Message.Contains("Duplicate column name"))
                    {
                        Console.WriteLine($"Columna '{col.Key}' ya existe.");
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



