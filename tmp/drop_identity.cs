using MySqlConnector;
using System;

string connectionString = "Server=localhost;User ID=root;Database=sathospitalario;";

string[] tablesToDrop = {
    "UsuarioRoles", "UsuarioClaims", "UsuarioLogins", "RoleClaims", "UsuarioTokens",
    "Usuarios", "Roles"
};

try
{
    using var connection = new MySqlConnection(connectionString);
    connection.Open();
    Console.WriteLine("Conectado a MySQL.");

    foreach (var table in tablesToDrop)
    {
        try {
            using var command = new MySqlCommand($"DROP TABLE IF EXISTS `{table}`;", connection);
            command.ExecuteNonQuery();
            Console.WriteLine($"- Tabla `{table}` eliminada (si existía).");
        } catch { }
    }
    
    // Opcional: Limpiar el historial si existe
    try {
        using var cmdHistory = new MySqlCommand("DELETE FROM `__EFMigrationsHistory` WHERE `ContextKey` LIKE '%Identity%';", connection);
        cmdHistory.ExecuteNonQuery();
        Console.WriteLine("- Historial de migraciones de Identity limpiado.");
    } catch { }

    Console.WriteLine("Limpieza completada.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error crítico: {ex.Message}");
}
