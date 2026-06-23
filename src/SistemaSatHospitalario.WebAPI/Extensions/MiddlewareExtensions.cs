using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Infrastructure.Identity.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Seeds;
using SistemaSatHospitalario.Infrastructure.Hubs;
using System.Diagnostics;

namespace SistemaSatHospitalario.WebAPI.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomSecurityHeaders(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Append("X-Frame-Options", "DENY");
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
                await next();
            });

            return app;
        }

        public static async Task UseDatabaseInitializationAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                var context = services.GetRequiredService<SatHospitalarioDbContext>();
                var configuration = services.GetRequiredService<IConfiguration>();

                try
                {
                    logger.LogInformation("Iniciando secuencia de robustecimiento: verificando conexión a Base de Datos...");
                    
                    // [NEW] Garantizar existencia de bases de datos antes de continuar
                    var connStrings = new[] { "mysql-system", "mysql-identity", "LegacyConnection" };
                    foreach (var connKey in connStrings)
                    {
                        var fullConStr = configuration.GetConnectionString(connKey);
                        if (!string.IsNullOrEmpty(fullConStr))
                        {
                            var builder = new MySqlConnectionStringBuilder(fullConStr);
                            var dbName = builder.Database;
                            builder.Database = ""; // Conectar al servidor sin DB específica

                            using var conn = new MySqlConnection(builder.ConnectionString);
                            await conn.OpenAsync();
                            using var cmd = conn.CreateCommand();
                            cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS `{dbName}` CHARACTER SET utf8mb4;";
                            await cmd.ExecuteNonQueryAsync();
                            logger.LogInformation("Garantizada existencia de base de datos: {Database}", dbName);
                        }
                    }

                    bool canConnect = false;
                    int retries = 5;
                    int delaySeconds = 3;

                    for (int i = 1; i <= retries; i++)
                    {
                        try
                        {
                            var builderCon = new MySqlConnector.MySqlConnectionStringBuilder(context.Database.GetConnectionString());
                            logger.LogInformation("Intento {Attempt}/{Total}: Probando conexión a {Server} (DB: {Database})...", i, retries, builderCon.Server, builderCon.Database);

                            await context.Database.OpenConnectionAsync();
                            await context.Database.CloseConnectionAsync();
                            canConnect = true;
                            logger.LogInformation("✅ Conexión a Base de Datos establecida correctamente.");
                            break;
                        }
                        catch (Exception ex)
                        {
                            logger.LogWarning("❌ Intento {Attempt}/{Total} FALLIDO. Motivo: {ErrorMessage}", i, retries, ex.Message);
                            
                            // Si el error es "Access denied" o similar y estamos en modo diseño, podríamos continuar
                            if (ex.Message.Contains("Access denied") || ex.Message.Contains("Unknown database") || ex.Message.Contains("Connect failure"))
                            {
                                if (i == retries) logger.LogWarning("⚠️ No se pudo conectar a la DB. Continuando carga de host para permitir herramientas de diseño (Migrations).");
                            }

                            if (i < retries) await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                        }
                    }

                    if (!canConnect) throw new Exception("No se pudo alcanzar el servidor de base de datos.");

                    logger.LogInformation("Iniciando secuencia de inicialización de esquemas...");

                    // 1. Ejecutar migraciones primero para asegurar consistencia
                    var identityContext = services.GetRequiredService<SatHospitalarioIdentityDbContext>();
                    await ApplyMigrationsWithFallbackAsync(identityContext, logger, "Identity", "20260414054517_InitialIdentityMySql", "9.0.2");

                    var systemContext = services.GetRequiredService<SatHospitalarioDbContext>();
                    await ApplyMigrationsWithFallbackAsync(systemContext, logger, "System", "20260522161430_InitialApplication", "10.0.5");

                    // 2. Ejecutar inicializadores (Seeding y auto-recuperación de columnas)
                    var initializers = services.GetServices<IDatabaseInitializer>();
                    
                    foreach (var initializer in initializers)
                    {
                        logger.LogInformation("Ejecutando inicializador: {InitializerType}", initializer.GetType().Name);
                        await initializer.InitializeAsync();
                    }
 
                    logger.LogInformation("Secuencia de inicialización completada exitosamente.");
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "ERROR CRÍTICO durante la inicialización de bases de datos. La aplicación iniciará en modo degradado.");
                }
            }
        }

        private static async Task ApplyMigrationsWithFallbackAsync(
            DbContext context, 
            ILogger logger, 
            string dbName, 
            string baselineMigrationId, 
            string productVersion)
        {
            try
            {
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    logger.LogInformation("Detectadas {Count} migraciones pendientes. Aplicando a {DbName} Database...", pendingMigrations.Count(), dbName);
                    
                    var connection = context.Database.GetDbConnection();
                    if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();
                    
                    try
                    {
                        // Desactivar temporalmente restricción de clave primaria para entornos que lo requieran
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "SET SESSION sql_require_primary_key = 0;";
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning("No se pudo ejecutar SET SESSION sql_require_primary_key = 0: {Message}", ex.Message);
                    }
                    
                    try 
                    {
                        await context.Database.MigrateAsync();
                        logger.LogInformation("Migraciones aplicadas con éxito a {DbName} Database.", dbName);
                    }
                    catch (Exception ex) when (
                        ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
                        ex.Message.Contains("ya existe", StringComparison.OrdinalIgnoreCase) ||
                        (ex.InnerException is MySqlConnector.MySqlException mysqlEx && mysqlEx.Number == 1050))
                    {
                        logger.LogWarning("Conflicto detectado en {DbName} Database: Las tablas ya existen pero el historial de EF Core está ausente.", dbName);
                        logger.LogInformation("Sincronizando historial de migraciones manualmente para {DbName} (Baseline: {Baseline})...", dbName, baselineMigrationId);
                        
                        // Aseguramos que la tabla de historial exista antes del insert
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (`MigrationId` varchar(150) NOT NULL, `ProductVersion` varchar(32) NOT NULL, PRIMARY KEY (`MigrationId`)) CHARACTER SET=utf8mb4;";
                            await cmd.ExecuteNonQueryAsync();

                            cmd.CommandText = $"INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES ('{baselineMigrationId}', '{productVersion}');";
                            await cmd.ExecuteNonQueryAsync();
                        }
                        
                        // Reintentar aplicar el resto de migraciones pendientes
                        await context.Database.MigrateAsync();
                        logger.LogInformation("Sincronización de Baseline completada y migraciones restantes aplicadas con éxito a {DbName} Database.", dbName);
                    }
                }
                else
                {
                    logger.LogInformation("{DbName} Database ya está actualizada. No se requieren migraciones.", dbName);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error aplicando migraciones en {DbName} Database.", dbName);
                throw;
            }
        }

        public static void MapCustomHubs(this WebApplication app)
        {
            app.MapHub<DashboardHub>("/hub/dashboard");
            app.MapHub<TasaHub>("/hub/tasa");
            app.MapHub<NotificationHub>("/hub/notifications");
        }
    }
}
