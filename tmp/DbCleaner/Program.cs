using MySql.Data.MySqlClient;
using System;

class Program {
    static void Main() {
        string[] dbs = { "SatHospitalarioIdentity", "SatHospitalario" };
        string conBase = "Server=localhost;Port=3306;Uid=root;Pwd=Labordono1818;Database=";

        foreach (var db in dbs) {
            try {
                using var conn = new MySqlConnection(conBase + db);
                conn.Open();
                Console.WriteLine($"\n--- Faking History for: {db} ---");
                
                using var cmd = conn.CreateCommand();
                
                // Asegurar que la tabla existe
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (`MigrationId` varchar(150) NOT NULL, `ProductVersion` varchar(32) NOT NULL, PRIMARY KEY (`MigrationId`));";
                cmd.ExecuteNonQuery();

                // Insertar las migraciones que queremos saltar
                string migrationId = db == "SatHospitalarioIdentity" ? "20260309004403_InitialIdentityMigration" : "20260322165147_InitialSystem";
                cmd.CommandText = $"INSERT IGNORE INTO `__EFMigrationsHistory` VALUES ('{migrationId}', '9.0.2');";
                cmd.ExecuteNonQuery();
                Console.WriteLine($"  [OK] Faked {migrationId}");

            } catch (Exception ex) {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }
    }
}
