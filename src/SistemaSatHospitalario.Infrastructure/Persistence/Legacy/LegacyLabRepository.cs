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
            var connection = _context.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            // [DIAGNOSTIC] Check schema of perfilesfacturados to fix "Unknown column" error
            try {
                if (_context.Database.IsMySql())
                {
                    using var cmd = connection.CreateCommand();
                    cmd.CommandText = "SHOW COLUMNS FROM perfilesfacturados";
                    using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                    var columns = new List<string>();
                    while (await reader.ReadAsync(cancellationToken)) columns.Add(reader[0].ToString() ?? "");
                    _logger.LogTrace($"[SCHEMA-DIAGNOSTIC] perfilesfacturados columns: {string.Join(", ", columns)}");
                }
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

                using var transaction = await connection.BeginTransactionAsync(cancellationToken);
                try
                {
                    // 1. Cabecera de Orden (Raw SQL)
                    string sqlOrden;
                    if (_context.Database.IsSqlite())
                    {
                        sqlOrden = @"
                            INSERT INTO ordenes (IdPersona, Fecha, HoraIngreso, NumeroDia, EstadoDeOrden, IDConvenio, IDTasa, IdCierreDeCaja, Muestra, PrecioF, Usuario, VALIDADA) 
                            VALUES (@IdPersona, @Fecha, @HoraIngreso, @NumeroDia, @EstadoDeOrden, @IDConvenio, @IDTasa, @IdCierreDeCaja, @Muestra, @PrecioF, @Usuario, @VALIDADA);
                            SELECT last_insert_rowid();";
                    }
                    else
                    {
                        sqlOrden = @"
                            INSERT INTO ordenes (IdPersona, Fecha, HoraIngreso, NumeroDia, EstadoDeOrden, IDConvenio, IDTasa, IdCierreDeCaja, Muestra, PrecioF, Usuario, VALIDADA) 
                            VALUES (@IdPersona, @Fecha, @HoraIngreso, @NumeroDia, @EstadoDeOrden, @IDConvenio, @IDTasa, @IdCierreDeCaja, @Muestra, @PrecioF, @Usuario, @VALIDADA);
                            SELECT LAST_INSERT_ID();";
                    }
                    
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

                    orden.IdOrden = idOrden;
                    _logger.LogTrace($"[LEGACY-REPO] 'orden' insertada via Raw SQL (ID: {idOrden})");

                    // 2. Perfiles Facturados (Raw SQL - Evita errores de PK IdFacturado)
                    if (perfilesAFacturar.Any())
                    {
                        const string sqlPerfil = @"
                            INSERT INTO perfilesfacturados (IdOrden, IdPersona, IdPerfil, PrecioPerfil, Facturado) 
                            VALUES (@IdOrden, @IdPersona, @IdPerfil, @PrecioPerfil, @Facturado)";
                        
                        foreach (var p in perfilesAFacturar)
                        {
                            p.IdOrden = idOrden;
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
                    var analysesList = (await _queryService.GetAnalysesForProfilesAsync(perfilIds, cancellationToken, transaction)).ToList();
                    
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
                                IdAnalisis = a.IdAnalisis,
                                IDOrganizador = a.IDOrganizador,
                                IdConvenio = orden.IDConvenio,
                                EstadoDeResultado = 1,
                                FechaIngreso = DateTime.Today,
                                HoraIngreso = orden.HoraIngreso
                            }, transaction);
                        }
                    }

                    await transaction.CommitAsync(cancellationToken);
                    return idOrden;
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
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync(cancellationToken);

                const string sql = @"SELECT IdPersona, Cedula, Nombre, Apellidos, Sexo, Fecha, Correo, TipoCorreo, Celular, Telefono, CodigoCelular, CodigoTelefono
                                     FROM datospersonales WHERE Cedula = @cedula LIMIT 1";
                return await connection.QueryFirstOrDefaultAsync<DatosPersonalesLegacy>(sql, new { cedula });
            }
            catch (global::System.Exception ex)
            {
                _logger.LogError($"[LEGACY ERROR] GetPatientByCedulaAsync: {ex.Message}", ex);
                return null;
            }
        }

        public async Task<DatosPersonalesLegacy?> GetPatientByIdAsync(string legacyId, CancellationToken cancellationToken)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync(cancellationToken);

                const string sql = @"SELECT IdPersona, Cedula, Nombre, Apellidos, Sexo, Fecha, Correo, TipoCorreo, Celular, Telefono, CodigoCelular, CodigoTelefono
                                     FROM datospersonales WHERE IdPersona = @legacyId LIMIT 1";
                var res = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { legacyId });
                if (res == null) return null;
                
                return new DatosPersonalesLegacy {
                    IdPersona = (int)res.IdPersona,
                    Cedula = (string)res.Cedula,
                    Nombre = (string)res.Nombre,
                    Apellidos = (string)res.Apellidos,
                    Sexo = (string)res.Sexo,
                    Fecha = (string)res.Fecha
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
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync(cancellationToken);

                const string sql = @"SELECT IdPersona, Cedula, Nombre, Apellidos, Sexo, Fecha, Correo, TipoCorreo, Celular, Telefono, CodigoCelular, CodigoTelefono
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
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync(cancellationToken);

                bool hasNombrePerfil = false;
                bool hasActivo = false;

                if (_context.Database.IsMySql())
                {
                    var columns = await connection.QueryAsync<string>(
                        "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = @db AND TABLE_NAME = 'perfil'",
                        new { db = connection.Database });
                    var colList = columns.Select(c => c.ToUpper()).ToList();
                    hasNombrePerfil = colList.Contains("NOMBREPERFIL");
                    hasActivo = colList.Contains("ACTIVO");
                }
                else if (_context.Database.IsSqlite())
                {
                    var columns = await connection.QueryAsync<dynamic>("PRAGMA table_info(perfil)");
                    var colList = columns.Select(c => ((string)c.name).ToUpper()).ToList();
                    hasNombrePerfil = colList.Contains("NOMBREPERFIL");
                    hasActivo = colList.Contains("ACTIVO");
                }

                string descCol = hasNombrePerfil ? "NombrePerfil" : "Descripcion";
                string estadoCol = hasActivo ? "Activo" : "Estado";

                string sql = $"SELECT IdPerfil, {descCol} AS Descripcion, Precio, PrecioDolar, {estadoCol} AS Estado FROM perfil";
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
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync(cancellationToken);

                string sql;
                if (_context.Database.IsSqlite())
                {
                    sql = @"
                        INSERT INTO datospersonales (Cedula, Nombre, Apellidos, Sexo, Fecha, Correo, TipoCorreo, Celular, Telefono, CodigoCelular, CodigoTelefono) 
                        VALUES (@Cedula, @Nombre, @Apellidos, @Sexo, @Fecha, @Correo, @TipoCorreo, @Celular, @Telefono, @CodigoCelular, @CodigoTelefono);
                        SELECT last_insert_rowid();";
                }
                else
                {
                    sql = @"
                        INSERT INTO datospersonales (Cedula, Nombre, Apellidos, Sexo, Fecha, Correo, TipoCorreo, Celular, Telefono, CodigoCelular, CodigoTelefono) 
                        VALUES (@Cedula, @Nombre, @Apellidos, @Sexo, @Fecha, @Correo, @TipoCorreo, @Celular, @Telefono, @CodigoCelular, @CodigoTelefono);
                        SELECT LAST_INSERT_ID();";
                }
                    
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
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync(cancellationToken);

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
 
        public async Task<int?> GetMuestraStatusAsync(int legacyOrderId, CancellationToken cancellationToken)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync(cancellationToken);

                const string sql = "SELECT Muestra FROM ordenes WHERE IdOrden = @legacyOrderId LIMIT 1";
                return await connection.QueryFirstOrDefaultAsync<int?>(sql, new { legacyOrderId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[LEGACY ERROR] GetMuestraStatusAsync (Order: {legacyOrderId}): {ex.Message}", ex);
                return null;
            }
        }
    }
}
