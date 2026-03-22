using MySql.Data.MySqlClient;
using System;

class Program {
    static void Main() {
        string conStr = "Server=localhost;Port=3306;Uid=root;Pwd=Labordono1818;Database=SatHospitalarioIdentity";
        try {
            using var conn = new MySqlConnection(conStr);
            conn.Open();
            Console.WriteLine("\n--- Ultimate Identity Fix (Corrected SQL) ---");
            
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 0;";
            cmd.ExecuteNonQuery();

            // Tipos de datos compatibles con MySQL para IDs (GUIDs de EF suelen ser Char(36))
            string[] tables = {
                "CREATE TABLE IF NOT EXISTS `Roles` (`Id` char(36) NOT NULL, `Name` varchar(256), `NormalizedName` varchar(256), `ConcurrencyStamp` longtext, PRIMARY KEY (`Id`));",
                "CREATE TABLE IF NOT EXISTS `Usuarios` (`Id` char(36) NOT NULL, `UserName` varchar(256), `NormalizedUserName` varchar(256), `Email` varchar(256), `NormalizedEmail` varchar(256), `PasswordHash` longtext, `SecurityStamp` longtext, `ConcurrencyStamp` longtext, `PhoneNumber` longtext, `EmailConfirmed` tinyint(1) NOT NULL, `PhoneNumberConfirmed` tinyint(1) NOT NULL, `TwoFactorEnabled` tinyint(1) NOT NULL, `LockoutEnd` datetime(6), `LockoutEnabled` tinyint(1) NOT NULL, `AccessFailedCount` int NOT NULL, `NombreReal` longtext, `ApellidoReal` longtext, `EsActivo` tinyint(1) NOT NULL, `LegacyCajeroId` int, PRIMARY KEY (`Id`));",
                "CREATE TABLE IF NOT EXISTS `UsuarioRoles` (`UserId` char(36) NOT NULL, `RoleId` char(36) NOT NULL, PRIMARY KEY (`UserId`, `RoleId`));"
            };

            foreach (var sql in tables) {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                Console.WriteLine("  [OK] Created/Verified table structure.");
            }

            cmd.CommandText = "CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (`MigrationId` varchar(150) NOT NULL, `ProductVersion` varchar(32) NOT NULL, PRIMARY KEY (`MigrationId`));";
            cmd.ExecuteNonQuery();
            
            cmd.CommandText = "INSERT IGNORE INTO `__EFMigrationsHistory` VALUES ('20260309004403_InitialIdentityMigration', '9.0.2');";
            cmd.ExecuteNonQuery();
            
            cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 1;";
            cmd.ExecuteNonQuery();
            Console.WriteLine("--- Identity Bypass Complete ---");
        } catch (Exception ex) {
            Console.WriteLine($"[ERROR]: {ex.Message}");
        }
    }
}
