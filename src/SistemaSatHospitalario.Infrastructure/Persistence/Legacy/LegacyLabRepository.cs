using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SistemaSatHospitalario.Core.Domain.Entities.Legacy;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Dapper;
using System.Linq;
using SistemaSatHospitalario.Core.Domain.DTOs.Legacy;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Infrastructure.Common.Helpers;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Legacy
{
    public class LegacyLabRepository : ILegacyLabRepository
    {
        private readonly Sistema2020LegacyDbContext _context;
        private readonly ILegacyQueryService _queryService;
        private readonly ILegacyErrorReportingService _logger;
        private readonly string _connectionString;

        public LegacyLabRepository(
            Sistema2020LegacyDbContext context, 
            ILegacyQueryService queryService, 
            IConfiguration configuration,
            ILegacyErrorReportingService logger)
        {
            _context = context;
            _queryService = queryService;
            _logger = logger;
            
            var rawConnStr = configuration.GetConnectionString("LegacyConnection") ?? "";
            _connectionString = ConnectionStringHelper.NormalizeMySqlConnectionString(rawConnStr, forceLowercase: false);
        }

        public async Task<int> GenerarOrdenLaboratorioAsync(
            OrdenLegacy orden, 
            List<PerfilesFacturadosLegacy> perfilesAFacturar, 
            List<ResultadosPacienteLegacy> resultados, 
            CancellationToken cancellationToken)
        {
            // 0. Guard Clause: Verificamos configuración antes de transaccionar
            if (string.IsNullOrEmpty(_connectionString))
            {
                _logger.LogError("[LEGACY-REPO] ABORTADO: No hay cadena de conexión configurada (LegacyConnection).");
                throw new InvalidOperationException("El sistema legacy no está configurado (Falta LegacyConnection).");
            }

            // [DIAGNOSTIC] Check schema of perfilesfacturados to fix "Unknown column" error
            try {
                using var conn = new MySqlConnection(_connectionString);
                await conn.OpenAsync(cancellationToken);
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SHOW COLUMNS FROM perfilesfacturados";
                using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                var columns = new List<string>();
                while (await reader.ReadAsync(cancellationToken)) columns.Add(reader[0].ToString());
                _logger.LogTrace($"[SCHEMA-DIAGNOSTIC] perfilesfacturados columns: {string.Join(", ", columns)}");
            } catch (Exception ex) {
                _logger.LogError($"[SCHEMA-DIAGNOSTIC] Failed to read schema: {ex.Message}");
            }

            // 1. Datos de la Orden (Header Trace)
            _logger.LogTrace($"[LEGACY-REPO] Generando Orden para Paciente Legacy ID: {orden.IdPersona}, Convenio: {orden.IDConvenio}");
            
            try 
            {
                // Determinar Número de Día (Lógica Legacy)
                int count = await _queryService.GetCurrentDayOrderCountAsync(cancellationToken);
                orden.NumeroDia = count + 1;
                _logger.LogTrace($"[LEGACY-REPO] Número de Orden asignado para el día: {orden.NumeroDia}");

                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);
                using var transaction = await connection.BeginTransactionAsync(cancellationToken);
                try
                {
                    // 1. Cabecera de Orden (Raw SQL)
                    const string sqlOrden = @"
                        INSERT INTO ordenes (IdPersona, Fecha, HoraIngreso, NumeroDia, EstadoDeOrden, IDConvenio, IDTasa, IdCierreDeCaja, Muestra, PrecioF, Usuario, VALIDADA) 
                        VALUES (@IdPersona, @Fecha, @HoraIngreso, @NumeroDia, @EstadoDeOrden, @IDConvenio, @IDTasa, @IdCierreDeCaja, @Muestra, @PrecioF, @Usuario, @VALIDADA);
                        SELECT LAST_INSERT_ID();";
                    
                    var idOrden = await connection.ExecuteScalarAsync<int>(sqlOrden, new {
                        orden.IdPersona,
                        Fecha = DateTime.Today,
                        orden.HoraIngreso,
                        orden.NumeroDia,
                        orden.EstadoDeOrden,
                        orden.IDConvenio,
                        orden.IDTasa,
                        orden.IdCierreDeCaja,
                        orden.Muestra,
                        orden.PrecioF,
                        orden.Usuario,
                        orden.VALIDADA
                    }, transaction);

                    _logger.LogTrace($"[LEGACY-REPO] 'orden' insertada via Raw SQL (ID: {idOrden})");

                    // 2. Perfiles Facturados (Raw SQL - Evita errores de PK IdFacturado)
                    if (perfilesAFacturar.Any())
                    {
                        const string sqlPerfil = @"
                            INSERT INTO perfilesfacturados (IdOrden, IdPersona, IdPerfil, PrecioPerfil, Facturado) 
                            VALUES (@IdOrden, @IdPersona, @IdPerfil, @PrecioPerfil, @Facturado)";
                        
                        foreach (var p in perfilesAFacturar)
                        {
                            await connection.ExecuteAsync(sqlPerfil, new {
                                IdOrden = idOrden,
                                p.IdPersona,
                                p.IdPerfil,
                                p.PrecioPerfil,
                                Facturado = "S"
                            }, transaction);
                        }
                        _logger.LogTrace($"[LEGACY-REPO] {perfilesAFacturar.Count} perfiles insertados via Raw SQL.");
                    }

                    // 3. Resultados (Raw SQL)
                    var perfilIds = perfilesAFacturar.Select(p => p.IdPerfil).Distinct().ToList();
                    var analysesList = (await _queryService.GetAnalysesForProfilesAsync(perfilIds, cancellationToken)).ToList();
                    
                    if (analysesList.Any())
                    {
                        const string sqlResultado = @"
                            INSERT INTO resultadospaciente (IdPaciente, IdOrden, IdAnalisis, IDOrganizador, IdConvenio, EstadoDeResultado, FechaIngreso, HoraIngreso, PorEnviar) 
                            VALUES (@IdPaciente, @IdOrden, @IdAnalisis, @IDOrganizador, @IdConvenio, @EstadoDeResultado, @FechaIngreso, @HoraIngreso, 0)";
                        
                        foreach (var a in analysesList)
                        {
                            await connection.ExecuteAsync(sqlResultado, new {
                                IdPaciente = orden.IdPersona,
                                IdOrden = idOrden,
                                a.IdAnalisis,
                                a.IDOrganizador,
                                orden.IDConvenio,
                                EstadoDeResultado = 1,
                                FechaIngreso = DateTime.Today,
                                orden.HoraIngreso
                            }, transaction);
                        }
                    }

                    await transaction.CommitAsync(cancellationToken);
                    return idOrden;
                }
                catch (MySqlException sqlEx) when (sqlEx.Number == 1049 || sqlEx.Message.Contains("Unknown database"))
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError($"[LEGACY-REPO] ERROR CRÍTICO: La base de datos legacy no es accesible.", sqlEx);
                    throw;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    var innerMsg = ex.InnerException?.Message ?? "Sin InnerException";
                    _logger.LogError($"[LEGACY-REPO] [TRANSACTION] ROLLBACK | Outer: {ex.Message} | Inner: {innerMsg}", ex);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[LEGACY-REPO] Error persistente al generar orden", ex);
                throw;
            }
        }

        public async Task<DatosPersonalesLegacy?> GetPatientByCedulaAsync(string cedula, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_connectionString)) return null;
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                const string sql = @"SELECT IdPersona, Cedula, Nombre, Apellidos, Sexo, Fecha, Correo, TipoCorreo, Celular, Telefono, CodigoCelular, CodigoTelefono, Visible
                                     FROM datospersonales WHERE Cedula = @cedula LIMIT 1";
                return await connection.QueryFirstOrDefaultAsync<DatosPersonalesLegacy>(sql, new { cedula });
            }
            catch (global::System.Exception ex)
            {
                // Logueamos pero no rompemos el flujo principal (visto como 500)
                _logger.LogError($"[LEGACY ERROR] GetPatientByCedulaAsync: {ex.Message}", ex);
                return null;
            }
        }

        public async Task<DatosPersonalesLegacy?> GetPatientByIdAsync(string legacyId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_connectionString)) return null;
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                const string sql = @"SELECT IdPersona, Cedula, Nombre, Apellidos, Sexo, Fecha, Correo, TipoCorreo, Celular, Telefono, CodigoCelular, CodigoTelefono, Visible
                                     FROM datospersonales WHERE IdPersona = @legacyId LIMIT 1";
                var res = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { legacyId });
                if (res == null) return null;
                
                return new DatosPersonalesLegacy {
                    IdPersona = (int)res.IdPersona,
                    Cedula = (string)res.Cedula,
                    Nombre = (string)res.Nombre,
                    Apellidos = (string)res.Apellidos,
                    Sexo = (string)res.Sexo,
                    Fecha = (string)res.Fecha,
                    Visible = (int)res.Visible
                };
            }
            catch (global::System.Exception ex)
            {
                _logger.LogError($"[LEGACY ERROR] GetPatientByIdAsync: {ex.Message}", ex);
                return null;
            }
        }

        public async Task<List<DatosPersonalesLegacy>> SearchPatientsLimitedAsync(string term, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_connectionString)) return new List<DatosPersonalesLegacy>();
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                const string sql = @"SELECT IdPersona, Cedula, Nombre, Apellidos, Sexo, Fecha, Correo, TipoCorreo, Celular, Telefono, CodigoCelular, CodigoTelefono, Visible
                                     FROM datospersonales 
                                     WHERE Cedula LIKE @term OR Nombre LIKE @term OR Apellidos LIKE @term 
                                     LIMIT 20";
                var result = await connection.QueryAsync<DatosPersonalesLegacy>(sql, new { term = $"%{term}%" });
                return result.ToList();
            }
            catch (global::System.Exception ex)
            {
                _logger.LogError($"[LEGACY ERROR] SearchPatientsLimitedAsync: {ex.Message}", ex);
                return new List<DatosPersonalesLegacy>();
            }
        }

        public async Task<List<PerfilLegacy>> GetAvailableProfilesAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_connectionString)) return new List<PerfilLegacy>();
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                // El esquema real en la DB reseteada usa nombres legacy: 'NombrePerfil' y 'Activo'
                // V12.6 Senior Alignment Fix: Usamos alias para mapear a la entidad moderna
                const string sql = "SELECT IdPerfil, NombrePerfil AS Descripcion, Precio, PrecioDolar, Activo AS Estado FROM perfil";
                var result = await connection.QueryAsync<PerfilLegacy>(sql);
                var list = result.ToList();
                _logger.LogTrace($"[LEGACY-REPO] GetAvailableProfilesAsync: Se recuperaron {list.Count} perfiles de la base de datos.");
                return list;
            }
            catch (global::System.Exception ex)
            {
                _logger.LogError($"[LEGACY ERROR] GetAvailableProfilesAsync: {ex.Message}", ex);
                return new List<PerfilLegacy>();
            }
        }

        public async Task<int> CreatePatientLegacyAsync(DatosPersonalesLegacy patient, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_connectionString)) return 0;
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                const string sql = @"
                    INSERT INTO datospersonales (Cedula, Nombre, Apellidos, Sexo, Fecha, Correo, TipoCorreo, Celular, Telefono, CodigoCelular, CodigoTelefono) 
                    VALUES (@Cedula, @Nombre, @Apellidos, @Sexo, @Fecha, @Correo, @TipoCorreo, @Celular, @Telefono, @CodigoCelular, @CodigoTelefono);
                    SELECT LAST_INSERT_ID();";
                    
                var id = await connection.ExecuteScalarAsync<int>(sql, patient);
                patient.IdPersona = id;
                return id;
            }
            catch (global::System.Exception ex)
            {
                _logger.LogError($"[LEGACY ERROR] CreatePatientLegacyAsync: {ex.Message}", ex);
                return 0;
            }
        }

        public async Task<List<int>> GetLegacyAgreementsIdsAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_connectionString)) return new List<int>();
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                const string sql = "SELECT IDConvenio FROM convenios"; // Basado en el nombre estándar
                var result = await connection.QueryAsync<int>(sql);
                return result.ToList();
            }
            catch (global::System.Exception ex)
            {
                _logger.LogError($"[LEGACY ERROR] GetLegacyAgreementsIdsAsync: {ex.Message}", ex);
                return new List<int>();
            }
        }
    }
}
