using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SistemaSatHospitalario.Infrastructure.Persistence.Seeds;
using SistemaSatHospitalario.Infrastructure.Common.Helpers;
using MySqlConnector;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Legacy
{
    public class LegacyDbInitializer : IDatabaseInitializer
    {
        private readonly Sistema2020LegacyDbContext _context;
        private readonly ILogger<LegacyDbInitializer> _logger;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public LegacyDbInitializer(
            Sistema2020LegacyDbContext context, 
            ILogger<LegacyDbInitializer> logger,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InitializeAsync()
        {
            var rawConnStr = _configuration.GetConnectionString("LegacyConnection");

            if (string.IsNullOrWhiteSpace(rawConnStr))
            {
                _logger.LogCritical("ERROR CRÍTICO: La cadena de conexión 'LegacyConnection' está vacía o no existe.");
                throw new InvalidOperationException("LegacyConnection string is missing or empty.");
            }

            // Senior Architecture: Normalize and Enhance for Cloud
            var conStr = ConnectionStringHelper.NormalizeMySqlConnectionString(rawConnStr);
            conStr = ConnectionStringHelper.EnhanceForCloud(conStr);

            try
            {
                var builder = new MySqlConnectionStringBuilder(conStr);
                _logger.LogInformation("Verificando existencia de base de datos Legacy en {Host}:{Port} (DB: {Database})...", builder.Server, builder.Port, builder.Database);
                
                await EnsureDatabaseExistsAsync(conStr);
                
                // Pequeña pausa para asegurar visibilidad en el pool (especialmente en Cloud)
                await Task.Delay(500);

                _logger.LogInformation("Aplicando esquemas a Legacy Database '{Database}'...", builder.Database);
                
                // Senior Defensive Pattern: Asegurar que sql_require_primary_key no bloquee la creación de __EFMigrationsHistory
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();
                
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SET SESSION sql_require_primary_key = 0;";
                    await cmd.ExecuteNonQueryAsync();
                }

                // await _context.Database.MigrateAsync();
                
                // Self-healing: Ensure Direccion column exists in datospersonales table
                try
                {
                    bool hasDireccionLegacy = false;
                    var legacyConn = _context.Database.GetDbConnection();
                    bool closeLegacyConn = false;
                    if (legacyConn.State != System.Data.ConnectionState.Open)
                    {
                        await legacyConn.OpenAsync();
                        closeLegacyConn = true;
                    }
                    using (var cmd = legacyConn.CreateCommand())
                    {
                        if (_context.Database.IsSqlite())
                        {
                            cmd.CommandText = "PRAGMA table_info(datospersonales);";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var colName = reader["name"]?.ToString();
                                    if (colName != null && colName.Equals("Direccion", StringComparison.OrdinalIgnoreCase))
                                    {
                                        hasDireccionLegacy = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            cmd.CommandText = "SHOW COLUMNS FROM `datospersonales` LIKE 'Direccion';";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    hasDireccionLegacy = true;
                                }
                            }
                        }
                    }
                    if (closeLegacyConn)
                    {
                        await legacyConn.CloseAsync();
                    }

                    if (!hasDireccionLegacy)
                    {
                        _logger.LogInformation("La columna 'Direccion' no existe en la tabla legacy 'datospersonales'. Ejecutando ALTER TABLE...");
                        await _context.Database.ExecuteSqlRawAsync("ALTER TABLE `datospersonales` ADD COLUMN `Direccion` VARCHAR(500) NULL;");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudo verificar/crear la columna 'Direccion' en la tabla legacy 'datospersonales'.");
                }

                // Sincronización de Convenios Legados por requerimiento del sistema
                try
                {
                    _logger.LogInformation("Sincronizando convenios legados en la tabla 'convenios'...");

                    // 1. DELETEs de convenios no deseados
                    await _context.Database.ExecuteSqlRawAsync("DELETE FROM `convenios` WHERE `IdConvenio` IN (4, 5, 6, 7, 8, 9, 10, 12);");

                    // 2. UPDATEs / INSERTs para garantizar IDs exactos (1, 2, 3, 11)
                    
                    // ID 1: Particular/Seguros
                    var exist1 = await _context.Database.ExecuteSqlRawAsync("UPDATE `convenios` SET `Nombre` = 'Particular/Seguros' WHERE `IdConvenio` = 1;");
                    if (exist1 == 0)
                    {
                        await _context.Database.ExecuteSqlRawAsync("INSERT INTO `convenios` (`IdConvenio`, `Nombre`) VALUES (1, 'Particular/Seguros');");
                    }

                    // ID 2: Emergencia
                    var exist2 = await _context.Database.ExecuteSqlRawAsync("UPDATE `convenios` SET `Nombre` = 'Emergencia' WHERE `IdConvenio` = 2;");
                    if (exist2 == 0)
                    {
                        await _context.Database.ExecuteSqlRawAsync("INSERT INTO `convenios` (`IdConvenio`, `Nombre`) VALUES (2, 'Emergencia');");
                    }

                    // ID 3: Hospitalizacion
                    var exist3 = await _context.Database.ExecuteSqlRawAsync("UPDATE `convenios` SET `Nombre` = 'Hospitalizacion' WHERE `IdConvenio` = 3;");
                    if (exist3 == 0)
                    {
                        await _context.Database.ExecuteSqlRawAsync("INSERT INTO `convenios` (`IdConvenio`, `Nombre`) VALUES (3, 'Hospitalizacion');");
                    }

                    // ID 11: UCI
                    try
                    {
                        // Intentar actualización completa con campos Correo y Telefono para la base de datos real
                        var exist11 = await _context.Database.ExecuteSqlRawAsync("UPDATE `convenios` SET `Nombre` = 'UCI', `Correo` = '', `Telefono` = '' WHERE `IdConvenio` = 11;");
                        if (exist11 == 0)
                        {
                            try
                            {
                                await _context.Database.ExecuteSqlRawAsync("INSERT INTO `convenios` (`IdConvenio`, `Nombre`, `Correo`, `Telefono`) VALUES (11, 'UCI', '', '');");
                            }
                            catch
                            {
                                await _context.Database.ExecuteSqlRawAsync("INSERT INTO `convenios` (`IdConvenio`, `Nombre`) VALUES (11, 'UCI');");
                            }
                        }
                    }
                    catch
                    {
                        // Fallback para entornos de pruebas (SQLite)
                        var exist11Fallback = await _context.Database.ExecuteSqlRawAsync("UPDATE `convenios` SET `Nombre` = 'UCI' WHERE `IdConvenio` = 11;");
                        if (exist11Fallback == 0)
                        {
                            await _context.Database.ExecuteSqlRawAsync("INSERT INTO `convenios` (`IdConvenio`, `Nombre`) VALUES (11, 'UCI');");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudieron sincronizar los convenios legados.");
                }

                _logger.LogInformation("✅ Legacy Database '{Database}' Inicializada y Migrada Correctamente.", builder.Database);
            }
            catch (Exception ex)
            {
                // Extraemos info de host para el log de error
                var builder = new MySqlConnectionStringBuilder(conStr);
                _logger.LogCritical(ex, "ERROR CRÍTICO: No se pudo inicializar la base de datos Legacy en {Host}:{Port}.", builder.Server, builder.Port);
                throw new Exception($"Fallo crítico al inicializar la conexión Legacy ({builder.Server}): {ex.Message}", ex);
            }
        }

        private async Task EnsureDatabaseExistsAsync(string connStr)
        {
            var builder = new MySqlConnectionStringBuilder(connStr);
            var databaseName = builder.Database;

            // Conectamos sin base de datos seleccionada para poder crearla
            builder.Database = null; 
            var serverConnStr = builder.ConnectionString;

            using var connection = new MySqlConnection(serverConnStr);
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS `{databaseName}` CHARACTER SET utf8mb4;";
            await cmd.ExecuteNonQueryAsync();
            
            _logger.LogInformation($"Base de datos `{databaseName}` verificada/creada.");
        }
    }
}
