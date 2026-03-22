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
            _connectionString = configuration.GetConnectionString("LegacyConnection") 
                                ?? throw new InvalidOperationException("LegacyConnection string not found.");
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
            using var connection = new MySqlConnection(_connectionString);
            // Mapeo manual a las propiedades del DTO si las columnas difieren
            const string sql = @"SELECT IdPersona, Identificacion, Nombre1, Apellido1, FechaNacimiento, Telefono, Direccion 
                                 FROM datospersonales WHERE Identificacion = @cedula LIMIT 1";
            return await connection.QueryFirstOrDefaultAsync<DatosPersonalesLegacy>(sql, new { cedula });
        }

        public async Task<List<DatosPersonalesLegacy>> SearchPatientsLimitedAsync(string term, CancellationToken cancellationToken)
        {
            using var connection = new MySqlConnection(_connectionString);
            const string sql = @"SELECT IdPersona, Identificacion, Nombre1, Apellido1, FechaNacimiento, Telefono, Direccion 
                                 FROM datospersonales 
                                 WHERE Identificacion LIKE @term OR Nombre1 LIKE @term OR Apellido1 LIKE @term 
                                 LIMIT 20";
            var result = await connection.QueryAsync<DatosPersonalesLegacy>(sql, new { term = $"%{term}%" });
            return result.ToList();
        }

        public async Task<List<PerfilLegacy>> GetAvailableProfilesAsync(CancellationToken cancellationToken)
        {
            using var connection = new MySqlConnection(_connectionString);
            const string sql = "SELECT * FROM perfil WHERE Activo = 1";
            var result = await connection.QueryAsync<PerfilLegacy>(sql);
            return result.ToList();
        }

        public async Task<int> CreatePatientLegacyAsync(DatosPersonalesLegacy patient, CancellationToken cancellationToken)
        {
            using var connection = new MySqlConnection(_connectionString);
            const string sql = @"
                INSERT INTO datospersonales (Identificacion, Nombre1, Apellido1, Telefono, FechaNacimiento, Direccion) 
                VALUES (@Identificacion, @Nombre1, @Apellido1, @Telefono, @FechaNacimiento, @Direccion);
                SELECT LAST_INSERT_ID();";
                
            var id = await connection.ExecuteScalarAsync<int>(sql, patient);
            patient.IdPersona = id;
            return id;
        }

        public async Task<List<int>> GetLegacyAgreementsIdsAsync(CancellationToken cancellationToken)
        {
            using var connection = new MySqlConnection(_connectionString);
            const string sql = "SELECT IDConvenio FROM convenios"; // Basado en el nombre estándar
            var result = await connection.QueryAsync<int>(sql);
            return result.ToList();
        }
    }
}
