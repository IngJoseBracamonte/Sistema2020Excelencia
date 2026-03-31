using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Dapper;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Legacy
{
    public class LegacyQueryService : ILegacyQueryService
    {
        private readonly string _connectionString;

        public LegacyQueryService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("LegacyConnection") ?? "";
        }

        public async Task<IEnumerable<dynamic>> GetAnalysesForProfilesAsync(List<int> profileIds, CancellationToken ct)
        {
            if (profileIds == null || profileIds.Count == 0) return Array.Empty<dynamic>();

            using var connection = new MySqlConnection(_connectionString);
            const string sqlAnalisis = "SELECT IDOrganizador, IdAnalisis FROM perfilesanalisis WHERE IdPerfil IN @Ids";
            return await connection.QueryAsync(sqlAnalisis, new { Ids = profileIds });
        }

        public async Task<int> GetCurrentDayOrderCountAsync(CancellationToken ct)
        {
            using var connection = new MySqlConnection(_connectionString);
            const string sqlNumeroDia = "SELECT COUNT(IdOrden) FROM orden WHERE DATE(Fecha) = CURRENT_DATE";
            return await connection.ExecuteScalarAsync<int>(sqlNumeroDia);
        }
    }
}
