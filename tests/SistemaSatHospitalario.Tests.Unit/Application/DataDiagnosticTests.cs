using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class DataDiagnosticTests
    {
        private readonly ITestOutputHelper _output;

        public DataDiagnosticTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task Diagnostic_CheckMappeoData()
        {
            // This test is intended to be run against the REAL development database
            // to verify metadata during research phase.
            string connectionString = "Server=localhost;Port=3306;Database=sistemasat;Uid=root;Pwd=Labordono1818;Allow User Variables=True";
            
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;

            using var context = new SatHospitalarioDbContext(options);

            _output.WriteLine("--- ANALIZANDO METADATOS PARA SINCRONIZACIÓN ---");
            
            var p1 = await context.PacientesAdmision.Where(p => p.NombreCorto.Contains("BRUNEQUILDE")).FirstOrDefaultAsync();
            if (p1 != null) {
                _output.WriteLine($"Paciente: {p1.NombreCorto} | IdPacienteLegacy: {p1.IdPacienteLegacy ?? 0}");
            } else {
                _output.WriteLine("BRUNEQUILDE GIL no encontrada.");
            }

            var s1 = await context.ServiciosClinicos.Where(s => s.Descripcion.Contains("PERFIL 20")).FirstOrDefaultAsync();
            if (s1 != null) {
                _output.WriteLine($"Servicio: {s1.Descripcion} | LegacyMappingId: '{s1.LegacyMappingId}' | Tipo: {s1.TipoServicio}");
            } else {
                _output.WriteLine("PERFIL 20 no encontrado.");
            }

            var candidate = await context.PacientesAdmision.Where(p => p.IdPacienteLegacy > 0).Take(1).FirstOrDefaultAsync();
            if (candidate != null) _output.WriteLine($"Sugerencia: Usa a '{candidate.NombreCorto}' (Legacy ID: {candidate.IdPacienteLegacy})");
        }
    }
}
