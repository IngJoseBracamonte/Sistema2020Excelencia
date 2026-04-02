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
                // El contexto Legacy NO usa migraciones de EF Core (es una DB pre-existente)
                // Solo verificamos si podemos conectarnos si el provider está configurado
                if (_context.Database.GetDbConnection().ConnectionString != null)
                {
                    _logger.LogInformation("Verificando conectividad con Legacy Database...");
                    await _context.Database.CanConnectAsync();
                }
                
                _logger.LogInformation("Legacy Database Inicializada (Check de Conexión).");
            }
            catch (Exception ex)
            {
                // Un error aquí suele ser por falta de ConnectionString en el entorno
                _logger.LogWarning($"Legacy Database no disponible o no configurada: {ex.Message}");
            }
        }
    }
}
