using System;
using System.Data;
using MySqlConnector;

namespace CatalogSync
{
    class Program
    {
        static void Main(string[] args)
        {
            string newConnBatch = "Server=localhost;Database=hospital_sat_db;Uid=root;Pwd=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=none";
            string legacyConnBatch = "Server=localhost;Database=sistema2020;Uid=root;Pwd=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=none";

            try
            {
                using var newConn = new MySqlConnection(newConnBatch.Replace("Database=hospital_sat_db", "Database=sathospitalario"));
                using var legacyConn = new MySqlConnection(legacyConnBatch);
                newConn.Open();
                legacyConn.Open();

                Console.WriteLine("Fetching legacy profiles from sistema2020.perfil...");
                var legacyProfilesList = new List<string>();
                using (var cmd = new MySqlCommand("SELECT IdPerfil, NombrePerfil FROM perfil", legacyConn))
                {
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        legacyProfilesList.Add($"{reader.GetInt32(0)}|{reader.GetString(1).Trim().ToUpper()}");
                    }
                }
                Console.WriteLine($"Found {legacyProfilesList.Count} legacy profiles. Sample: {legacyProfilesList.FirstOrDefault()}");

                Console.WriteLine("Listing local service types...");
                using (var cmd = new MySqlCommand("SELECT TipoServicio, COUNT(*) FROM ServiciosClinicos GROUP BY TipoServicio", newConn))
                {
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read()) Console.WriteLine($"- {reader.GetString(0)}: {reader.GetInt32(1)}");
                }

                Console.WriteLine("Fetching local services from sathospitalario.ServiciosClinicos (All)...");
                var localServices = new List<string>();
                using (var cmd = new MySqlCommand("SELECT Descripcion, TipoServicio FROM ServiciosClinicos", newConn))
                {
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string desc = reader.GetString(0).Trim().ToUpper();
                        string tipo = reader.GetString(1);
                        localServices.Add($"{desc}|{tipo}");
                    }
                }
                Console.WriteLine($"Found {localServices.Count} local services.");

                int matchCount = 0;
                int insertCount = 0;
                foreach (var legacy in legacyProfilesList)
                {
                    var parts = legacy.Split('|');
                    string id = parts[0];
                    string name = parts[1];
                    
                    // 1. Try to Update existing
                    const string sqlUpdate = "UPDATE ServiciosClinicos SET LegacyMappingId = @id WHERE TRIM(UPPER(Descripcion)) = @name AND (LegacyMappingId IS NULL OR LegacyMappingId = '')";
                    using (var cmd = new MySqlCommand(sqlUpdate, newConn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@name", name);
                        int affected = cmd.ExecuteNonQuery();
                        if (affected > 0)
                        {
                            Console.WriteLine($"[MATCH/UPDATE] '{name}' -> ID: {id}");
                            matchCount += affected;
                            continue;
                        }
                    }

                    // 2. If not found, Insert (Import from Legacy)
                    // First check if it exists at all (might have a different mapping id)
                    const string sqlCheck = "SELECT COUNT(*) FROM ServiciosClinicos WHERE TRIM(UPPER(Descripcion)) = @name";
                    using (var cmdCheck = new MySqlCommand(sqlCheck, newConn))
                    {
                        cmdCheck.Parameters.AddWithValue("@name", name);
                        if (Convert.ToInt32(cmdCheck.ExecuteScalar()) == 0)
                        {
                            const string sqlInsert = @"INSERT INTO ServiciosClinicos (Id, Codigo, Descripcion, PrecioBase, TipoServicio, LegacyMappingId, Category, Activo) 
                                                       VALUES (@guid, @codigo, @name, @precio, 'LABORATORIO', @id, 2, 1)";
                            using var cmdIns = new MySqlCommand(sqlInsert, newConn);
                            cmdIns.Parameters.AddWithValue("@guid", Guid.NewGuid().ToString());
                            cmdIns.Parameters.AddWithValue("@codigo", $"LAB-{id}");
                            cmdIns.Parameters.AddWithValue("@name", name);
                            cmdIns.Parameters.AddWithValue("@precio", 0.00); // We'll sync prices later if needed
                            cmdIns.Parameters.AddWithValue("@id", id);
                            cmdIns.ExecuteNonQuery();
                            Console.WriteLine($"[IMPORT] '{name}' created as new service with LegacyID: {id}");
                            insertCount++;
                        }
                    }
                }
                Console.WriteLine($"Total matches/updates: {matchCount}");
                Console.WriteLine($"Total imports: {insertCount}");

                // Special case for PERFIL 20 if not matched exactly
                using (var specialCmd = new MySqlCommand(
                    "UPDATE ServiciosClinicos SET LegacyMappingId = '1403' WHERE TRIM(UPPER(Descripcion)) = 'PERFIL 20' AND LegacyMappingId IS NULL", 
                    newConn))
                {
                    int affected = specialCmd.ExecuteNonQuery();
                    if (affected > 0) Console.WriteLine("Mapped PERFIL 20 -> 1403 (Special case)");
                }

                Console.WriteLine("Done.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
