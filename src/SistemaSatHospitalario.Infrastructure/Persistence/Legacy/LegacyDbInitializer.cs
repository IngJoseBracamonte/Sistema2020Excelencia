using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Infrastructure.Persistence.Seeds;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Legacy
{
    public class LegacyDbInitializer : IDatabaseInitializer
    {
        private readonly Sistema2020LegacyDbContext _context;
        private readonly ILogger<LegacyDbInitializer> _logger;

        public LegacyDbInitializer(
            Sistema2020LegacyDbContext context, 
            ILogger<LegacyDbInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Verificando y aplicando migraciones pendientes a Legacy Database...");
                
                // Aplica automáticamente cualquier migración de EF Core pendiente en Sistema2020LegacyDbContext
                await _context.Database.MigrateAsync();
                
                _logger.LogInformation("Legacy Database Inicializada Correctamente.");
            }
            catch (Exception ex)
            {
                // Un error aquí significa que no hay conexión, o el schema está bloqueado
                _logger.LogError(ex, "Ocurrió un error inicializando Legacy Database (Migraciones).");
                throw;
            }
        }
    }
}
