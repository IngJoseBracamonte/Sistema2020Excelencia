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
            _connectionString = configuration.GetConnectionString("LegacyConnection") ?? "";
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

            // 1. Datos de la Orden (Header Trace)
            _logger.LogTrace($"[LEGACY-REPO] Generando Orden para Paciente Legacy ID: {orden.IdPersona}, Convenio: {orden.IDConvenio}");
            
            try 
            {
                // Determinar Número de Día (Lógica Legacy)
                int count = await _queryService.GetCurrentDayOrderCountAsync(cancellationToken);
                orden.NumeroDia = count + 1;
                _logger.LogTrace($"[LEGACY-REPO] Número de Orden asignado para el día: {orden.NumeroDia}");

                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    // 1. Cabecera de Orden (EF CORE)
                    orden.Fecha = DateTime.Today; 
                    await _context.Orden.AddAsync(orden, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                    _logger.LogTrace($"[LEGACY-REPO] 'orden' insertada (ID: {orden.IdOrden})");

                    // a. Perfiles Facturados (EF CORE)
                    if (perfilesAFacturar.Any())
                    {
                        foreach (var p in perfilesAFacturar)
                        {
                            p.IdOrden = orden.IdOrden;
                            p.IdPersona = orden.IdPersona;
                            p.Facturado = "S";
                        }
                        await _context.PerfilesFacturados.AddRangeAsync(perfilesAFacturar, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);
                        _logger.LogTrace($"[LEGACY-REPO] {perfilesAFacturar.Count} perfiles insertados via EF Core.");
                    }

                    // b. Busqueda de Análisis vinculados
                    var perfilIds = perfilesAFacturar.Select(p => p.IdPerfil).Distinct().ToList();
                    var analysesList = (await _queryService.GetAnalysesForProfilesAsync(perfilIds, cancellationToken)).ToList();
                    _logger.LogTrace($"[LEGACY-REPO] Se encontraron {analysesList.Count} análisis técnicos vinculados.");

                    // c. Inserción de Resultados del Paciente (Stubs via EF CORE)
                    if (analysesList.Any())
                    {
                        var resultadosParaInsertar = analysesList.Select(a => new ResultadosPacienteLegacy
                        {
                            IdPaciente = orden.IdPersona,
                            IdOrden = orden.IdOrden,
                            IdAnalisis = a.IdAnalisis,
                            IDOrganizador = a.IDOrganizador,
                            IdConvenio = orden.IDConvenio,
                            EstadoDeResultado = 1, // Pendiente
                            FechaIngreso = orden.Fecha,
                            HoraIngreso = orden.HoraIngreso
                        }).ToList();

                        await _context.ResultadosPaciente.AddRangeAsync(resultadosParaInsertar, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);
                        _logger.LogTrace($"[LEGACY-REPO] {resultadosParaInsertar.Count} resultados generados exitosamente.");
                    }

                    await _context.Database.CommitTransactionAsync(cancellationToken);
                    _logger.LogTrace($"[LEGACY-REPO] [SUCCESS] COMMIT - Sincronización Exitosa para Orden {orden.IdOrden}");
                    return orden.IdOrden;
                }
                catch (Exception ex)
                {
                    await _context.Database.RollbackTransactionAsync(cancellationToken);
                    var innerMsg = ex.InnerException?.Message ?? "Sin InnerException";
                    var innerInnerMsg = ex.InnerException?.InnerException?.Message ?? "";
                    _logger.LogError($"[LEGACY-REPO] [TRANSACTION] ROLLBACK | Outer: {ex.Message} | Inner: {innerMsg} | InnerInner: {innerInnerMsg}", ex);
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
                return await connection.QueryFirstOrDefaultAsync<DatosPersonalesLegacy>(sql, new { legacyId });
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
                // El esquema real según Conexion.cs es: Tabla 'Perfil', Columnas 'NombrePerfil', 'Precio', 'PrecioDolar', 'Activo'
                const string sql = "SELECT IdPerfil, NombrePerfil AS Descripcion, Precio, PrecioDolar, Activo AS Estado FROM perfil";
                var result = await connection.QueryAsync<PerfilLegacy>(sql);
                return result.ToList();
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
