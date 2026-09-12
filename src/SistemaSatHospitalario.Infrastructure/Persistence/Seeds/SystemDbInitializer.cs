using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using System;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Seeds
{
    public class SystemDbInitializer : IDatabaseInitializer
    {
        private readonly SatHospitalarioDbContext _context;
        private readonly ILogger<SystemDbInitializer> _logger;

        public SystemDbInitializer(
            SatHospitalarioDbContext context,
            ILogger<SystemDbInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Aplicando migraciones pendientes...");
                // Aplica automáticamente cualquier cambio del ModelBuilder a la BD física
                await _context.Database.MigrateAsync();

                _logger.LogInformation("Verificando datos iniciales (Seeds)...");
                await SeedConfiguracionAsync();
                await SeedTasaCambioAsync();
                await NormalizeAuditLogsAsync();
                // Invoca aquí solo los seeds que NO tengas en HasData()

                _logger.LogInformation("Base de datos inicializada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error inicializando la base de datos.");
                throw;
            }
        }

        private async Task NormalizeAuditLogsAsync()
        {
            try
            {
                var logsToFix = await _context.AuditLogs
                    .Where(a => a.ActionType == "TRASLADO_AREA" && a.NewValue != null && (a.NewValue.Contains("30000000-") || a.NewValue.Contains("AreaDestino: Cama") || a.NewValue.Contains("AreaDestino: Habitación") || a.NewValue.Contains("AreaDestino: Box")))
                    .ToListAsync();

                if (logsToFix.Any())
                {
                    var camas = await _context.AreasClinicas.Include(c => c.Sede).ToListAsync();
                    foreach (var log in logsToFix)
                    {
                        var text = log.NewValue!;
                        var match = System.Text.RegularExpressions.Regex.Match(text, @"Cama:\s*([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})");
                        if (match.Success && Guid.TryParse(match.Groups[1].Value, out var camaId))
                        {
                            var cama = camas.FirstOrDefault(c => c.Id == camaId);
                            if (cama != null)
                            {
                                var sedeNom = cama.Sede?.Nombre ?? "Sede General";
                                text = System.Text.RegularExpressions.Regex.Replace(text, @"AreaDestino:[^,]+,", $"AreaDestino: {sedeNom},");
                                text = text.Replace(match.Value, $"Cama: {cama.Nombre}");
                                log.NewValue = text;
                            }
                        }
                        else
                        {
                            foreach (var c in camas)
                            {
                                if (!string.IsNullOrWhiteSpace(c.Nombre) && text.Contains($"AreaDestino: {c.Nombre}"))
                                {
                                    var sedeNom = c.Sede?.Nombre ?? "Sede General";
                                    text = text.Replace($"AreaDestino: {c.Nombre}", $"AreaDestino: {sedeNom}");
                                    log.NewValue = text;
                                    break;
                                }
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Normalizados {Count} registros históricos de auditoría TRASLADO_AREA.", logsToFix.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Advertencia: no se pudo normalizar los logs históricos de auditoría.");
            }
        }

        private async Task SeedConfiguracionAsync()
        {
            if (!await _context.ConfiguracionGeneral.AnyAsync())
            {
                _context.ConfiguracionGeneral.Add(
                    new ConfiguracionGeneral("SAT HOSPITALARIO - EXCELENCIA", "J-12345678-9", 16.00m, "1234", false, false)
                );
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedTasaCambioAsync()
        {
            if (!await _context.TasaCambio.AnyAsync())
            {
                _context.TasaCambio.Add(new TasaCambio(36.50m));
                await _context.SaveChangesAsync();
            }
        }
    }
}