using Moq;
using Xunit;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using Microsoft.Extensions.Caching.Memory;
using MockQueryable.Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class HonorariumTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly IMemoryCache _cache;

        public HonorariumTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        [Fact]
        public async Task MapToCategoryAsync_ShouldPrioritizeExplicitCategory()
        {
            // Arrange
            var service = new ServicioClinico("S1", "Desc 1", 10, "Tipo 1") 
            { 
                HonorariumCategory = "RX" 
            };
            var serviceId = service.Id;
            
            var serviceList = new List<ServicioClinico> { service };
            var servicesMock = serviceList.BuildMockDbSet<ServicioClinico>();

            _mockContext.Setup(c => c.ServiciosClinicos).Returns(servicesMock.Object);
            _mockContext.Setup(c => c.HonorariumMappingRules).Returns(new List<HonorariumMappingRule>().BuildMockDbSet<HonorariumMappingRule>().Object);

            var mapperService = new HonorariumMapperService(_mockContext.Object, _cache);

            // Act
            var result = await mapperService.MapToCategoryAsync("OTRO", serviceId);

            // Assert
            Assert.Equal("RX", result);
        }

        [Fact]
        public async Task MapToCategoryAsync_ShouldFallbackToRulesIfNoExplicitCategory()
        {
            // Arrange
            var service = new ServicioClinico("S1", "Desc 1", 10, "RX-Digital") 
            { 
                HonorariumCategory = null // Sin categoría explícita
            };
            var serviceId = service.Id;
            
            var serviceList = new List<ServicioClinico> { service };
            var servicesMock = serviceList.BuildMockDbSet<ServicioClinico>();

            var ruleList = new List<HonorariumMappingRule>
            {
                new HonorariumMappingRule("RX", "RX", MappingRuleType.Contains, 1, "Admin")
            };
            var rulesMock = ruleList.BuildMockDbSet<HonorariumMappingRule>();

            _mockContext.Setup(c => c.ServiciosClinicos).Returns(servicesMock.Object);
            _mockContext.Setup(c => c.HonorariumMappingRules).Returns(rulesMock.Object);

            var mapperService = new HonorariumMapperService(_mockContext.Object, _cache);

            // Act
            var result = await mapperService.MapToCategoryAsync("RX-Digital", serviceId);

            // Assert
            Assert.Equal("RX", result);
        }

        [Fact]
        public async Task MapToCategoryAsync_ShouldReturnOtrosIfNoMatchFound()
        {
            // Arrange
            var emptyServices = new List<ServicioClinico>().BuildMockDbSet<ServicioClinico>();
            var emptyRules = new List<HonorariumMappingRule>().BuildMockDbSet<HonorariumMappingRule>();

            _mockContext.Setup(c => c.ServiciosClinicos).Returns(emptyServices.Object);
            _mockContext.Setup(c => c.HonorariumMappingRules).Returns(emptyRules.Object);

            var mapperService = new HonorariumMapperService(_mockContext.Object, _cache);

            // Act
            var result = await mapperService.MapToCategoryAsync("MISTERIOSO", Guid.NewGuid());

            // Assert
            Assert.Equal(HonorarioConstants.CategoriaOtros, result);
        }

        [Fact]
        public async Task CompareOldVsNewHonorariumQueries()
        {
            // 1. Arrange - Setup Mock Data
            var pacienteId = Guid.NewGuid();
            var cuentaId = Guid.NewGuid();
            
            var medico = new Medico("Dr. Jose Bracamonte", Guid.NewGuid(), 35.00m);
            var medicoId = medico.Id;

            var cita = new CitaMedica(medicoId, pacienteId, cuentaId, DateTime.Today.AddHours(10));
            cita.MarcarComoAtendida();

            // Create account details
            var detailConsulta = new DetalleServicioCuenta(
                cuentaId, Guid.NewGuid(), "Consulta Especializada", 50.00m, 0.00m, 1, "MEDICO", "Cajero");
            
            var detailTomografia = new DetalleServicioCuenta(
                cuentaId, Guid.NewGuid(), "Tomografía Computada", 450.00m, 0.00m, 1, "TOMO", "Cajero");

            var citasMock = new List<CitaMedica> { cita }.BuildMockDbSet<CitaMedica>();
            var detallesMock = new List<DetalleServicioCuenta> { detailConsulta, detailTomografia }.BuildMockDbSet<DetalleServicioCuenta>();
            var medicosMock = new List<Medico> { medico }.BuildMockDbSet<Medico>();

            _mockContext.Setup(c => c.CitasMedicas).Returns(citasMock.Object);
            _mockContext.Setup(c => c.DetallesServicioCuenta).Returns(detallesMock.Object);
            _mockContext.Setup(c => c.Medicos).Returns(medicosMock.Object);

            var start = DateTime.Today;
            var end = DateTime.Today.AddDays(1).AddTicks(-1);

            // 2. Act - Run the OLD logic (simulated in memory)
            var oldCitas = await (
                from c in citasMock.Object
                join d in detallesMock.Object
                    on c.CuentaServicioId equals d.CuentaServicioId
                where c.Estado == EstadoConstants.Atendida
                   && c.HoraPautada >= start
                   && c.HoraPautada <= end
                select new
                {
                    MedicoId = (Guid?)(d.MedicoResponsableId ?? c.MedicoId),
                    d.Honorario,
                    d.Precio,
                    d.Cantidad,
                    d.CategoriaHonorario
                }
            ).ToListAsync();

            var oldAllItems = oldCitas
                .Select(x => new
                {
                    x.MedicoId,
                    Honorario = x.Honorario > 0 ? x.Honorario : x.Precio,
                    x.Cantidad
                })
                .ToList();

            var oldTotal = oldAllItems.Sum(x => x.Honorario * x.Cantidad);

            // 3. Act - Run the NEW logic (simulated in memory)
            var newCitas = await (
                from c in citasMock.Object
                join d in detallesMock.Object
                    on c.CuentaServicioId equals d.CuentaServicioId
                join m in medicosMock.Object
                    on c.MedicoId equals m.Id
                where c.Estado == EstadoConstants.Atendida
                   && c.HoraPautada >= start
                   && c.HoraPautada <= end
                    && (d.MedicoResponsableId == c.MedicoId || 
                        (d.MedicoResponsableId == null && 
                         (d.TipoServicio == "MEDICO" || d.TipoServicio == "Medico" || (d.TipoServicio.Contains("CONS") || (d.TipoServicio.Contains("MEDI") && !d.TipoServicio.Contains("MEDICINA") && !d.TipoServicio.Contains("MEDICAMENTO"))))))
                select new
                {
                    MedicoId = (Guid?)(d.MedicoResponsableId ?? c.MedicoId),
                    d.Honorario,
                    d.Precio,
                    d.Cantidad,
                    d.CategoriaHonorario,
                    MedicoHonorarioBase = m.HonorarioBase
                }
            ).ToListAsync();

            var newAllItems = newCitas
                .Select(x => new
                {
                    x.MedicoId,
                    Honorario = x.Honorario > 0 ? x.Honorario : (x.MedicoHonorarioBase > 0 ? x.MedicoHonorarioBase : x.Precio),
                    x.Cantidad
                })
                .ToList();

            var newTotal = newAllItems.Sum(x => x.Honorario * x.Cantidad);

            // Print the comparison results in a format we can capture
            Console.WriteLine("================ HONORARIUM QUERY COMPARISON ================");
            Console.WriteLine($"[OLD QUERY] Services assigned: {oldAllItems.Count}");
            foreach (var item in oldAllItems)
            {
                Console.WriteLine($"   - MedicoId: {item.MedicoId}, Honorario calculado: ${item.Honorario}");
            }
            Console.WriteLine($"[OLD QUERY] Total Honorarium: ${oldTotal}");

            Console.WriteLine($"[NEW QUERY] Services assigned: {newAllItems.Count}");
            foreach (var item in newAllItems)
            {
                Console.WriteLine($"   - MedicoId: {item.MedicoId}, Honorario calculado: ${item.Honorario}");
            }
            Console.WriteLine($"[NEW QUERY] Total Honorarium: ${newTotal}");
            Console.WriteLine("=============================================================");

            // Assert that old logic incorrectly includes the tomography and falls back to consultation price ($500 total)
            // while the new logic correctly includes only the consultation and falls back to Dr's base honorarium ($35)
            Assert.Equal(500.00m, oldTotal); // $50 (consultation price fallback) + $450 (tomography price fallback) = $500
            Assert.Equal(35.00m, newTotal);  // Dr. Jose Bracamonte's base honorarium of $35
        }
    }
}

