using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Legacy;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Dapper;
using System.Linq;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Legacy
{
    public class LegacyLabRepository : ILegacyLabRepository
    {
        private readonly Sistema2020LegacyDbContext _context;
        private readonly string _connectionString;

        public LegacyLabRepository(Sistema2020LegacyDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("LegacyConnection") ?? "";
        }

        public async Task<int> GenerarOrdenLaboratorioAsync(
            OrdenLegacy orden, 
            List<PerfilesFacturadosLegacy> perfilesAFacturar, 
            List<ResultadosPacienteLegacy> resultados, 
            CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _context.Orden.AddAsync(orden, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                foreach (var perfil in perfilesAFacturar)
                {
                    perfil.IdOrden = orden.IdOrden;
                }
                await _context.PerfilesFacturados.AddRangeAsync(perfilesAFacturar, cancellationToken);
                
                foreach (var res in resultados)
                {
                    res.IdOrden = orden.IdOrden;
                }
                await _context.ResultadosPaciente.AddRangeAsync(resultados, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return orden.IdOrden;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new InvalidOperationException("Fallo crítico en Facturación Laboratorio MySQL: " + ex.Message, ex);
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
                global::System.Console.WriteLine($"[LEGACY ERROR] GetPatientByCedulaAsync: {ex.Message}");
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
                global::System.Console.WriteLine($"[LEGACY ERROR] SearchPatientsLimitedAsync: {ex.Message}");
                return new List<DatosPersonalesLegacy>();
            }
        }

        public async Task<List<PerfilLegacy>> GetAvailableProfilesAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_connectionString)) return new List<PerfilLegacy>();
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                // El esquema real según Conexion.cs es: Tabla 'Perfil', Columnas 'NombrePerfil', 'Precio', 'Activo'
                const string sql = "SELECT IdPerfil, NombrePerfil AS Descripcion, PrecioDOlar, Activo AS Estado FROM perfil";
                var result = await connection.QueryAsync<PerfilLegacy>(sql);
                return result.ToList();
            }
            catch (global::System.Exception ex)
            {
                global::System.Console.WriteLine($"[LEGACY ERROR] GetAvailableProfilesAsync: {ex.Message}");
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
                global::System.Console.WriteLine($"[LEGACY ERROR] CreatePatientLegacyAsync: {ex.Message}");
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
                global::System.Console.WriteLine($"[LEGACY ERROR] GetLegacyAgreementsIdsAsync: {ex.Message}");
                return new List<int>();
            }
        }
    }
}
