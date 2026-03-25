using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Infrastructure.Identity.Seeds;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Seeds
{
    public class SystemDbInitializer : IDatabaseInitializer
    {
        private readonly SatHospitalarioDbContext _context;
        private readonly ILogger<SystemDbInitializer> _logger;

        public SystemDbInitializer(SatHospitalarioDbContext context, ILogger<SystemDbInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Aplicando migraciones a System Database...");
                await _context.Database.MigrateAsync();

                _logger.LogInformation("Poblando System Database con datos de prueba...");

                await SeedServiciosClinicosAsync();
                await SeedMedicosAsync();
                await SeedPacientesAsync();
                await SeedCajaDiariaAsync();
                await SeedConfiguracionAsync();

                _logger.LogInformation("System Database Inicializada Correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error inicializando System Database.");
                throw;
            }
        }

        private async Task SeedServiciosClinicosAsync()
        {
            if (!await _context.ServiciosClinicos.AnyAsync())
            {
                _context.ServiciosClinicos.AddRange(
                    new ServicioClinico("S001", "Consulta Medica General", 30.00m, "Consulta"),
                    new ServicioClinico("S002", "Radiografía Tórax", 45.00m, "RX")
                );
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedMedicosAsync()
        {
            if (!await _context.Medicos.AnyAsync())
            {
                _context.Medicos.AddRange(
                    new Medico("Gregory House", "Diagnóstico Diferencial"),
                    new Medico("James Wilson", "Oncología")
                );
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedPacientesAsync()
        {
            if (!await _context.PacientesAdmision.AnyAsync())
            {
                _context.PacientesAdmision.AddRange(
                    new PacienteAdmision(1, "0999999999", "John Doe", "0987654321"),
                    new PacienteAdmision(2, "0888888888", "Jane Smith", "0912345678")
                );
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedCajaDiariaAsync()
        {
            if (!await _context.CajasDiarias.AnyAsync())
            {
                // Un cajero genérico asumiendo un ID de de admin
                _context.CajasDiarias.Add(
                    new CajaDiaria(50.00m, 1500.00m, "1", "admin")
                );
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedConfiguracionAsync()
        {
            if (!await _context.ConfiguracionGeneral.AnyAsync())
            {
                _context.ConfiguracionGeneral.Add(
                    new ConfiguracionGeneral("SAT HOSPITALARIO - EXCELENCIA", "J-12345678-9", 16.00m)
                );
                await _context.SaveChangesAsync();
            }
        }
    }
}
