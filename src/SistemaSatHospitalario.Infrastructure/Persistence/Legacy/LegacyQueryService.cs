using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Dapper;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using SistemaSatHospitalario.Infrastructure.Common.Helpers;

using SistemaSatHospitalario.Core.Domain.DTOs.Legacy;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Legacy
{
    public class LegacyQueryService : ILegacyQueryService
    {
        private readonly Sistema2020LegacyDbContext _context;

        public LegacyQueryService(Sistema2020LegacyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AnalysisMappingDto>> GetAnalysesForProfilesAsync(List<int> profileIds, CancellationToken ct)
        {
            if (profileIds == null || profileIds.Count == 0) return Array.Empty<AnalysisMappingDto>();

            var connection = _context.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync(ct);

            const string sqlAnalisis = "SELECT IDOrganizador, IdAnalisis FROM perfilesanalisis WHERE IdPerfil IN @Ids";
            return await connection.QueryAsync<AnalysisMappingDto>(sqlAnalisis, new { Ids = profileIds });
        }

        public async Task<int> GetCurrentDayOrderCountAsync(CancellationToken ct)
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync(ct);

            string sqlNumeroDia;
            if (_context.Database.IsSqlite())
            {
                sqlNumeroDia = "SELECT COUNT(IdOrden) FROM ordenes WHERE date(Fecha) = date('now')";
            }
            else
            {
                sqlNumeroDia = "SELECT COUNT(IdOrden) FROM ordenes WHERE DATE(Fecha) = CURRENT_DATE";
            }
            
            return await connection.ExecuteScalarAsync<int>(sqlNumeroDia);
        }
    }
}
