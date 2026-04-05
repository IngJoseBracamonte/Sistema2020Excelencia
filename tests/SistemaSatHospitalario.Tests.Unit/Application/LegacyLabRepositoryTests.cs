using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.DTOs.Legacy;
using SistemaSatHospitalario.Core.Domain.Entities.Legacy;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using SistemaSatHospitalario.Infrastructure.Persistence.Legacy;
using Microsoft.Extensions.Configuration;
using SistemaSatHospitalario.Tests.Unit.Common;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    /// <summary>
    /// LegacyLabRepositoryTests (High Standard).
    /// Valida la convergencia de datos entre Perfiles y Análisis en el sistema Legacy.
    /// </summary>
    public class LegacyLabRepositoryTests : IDisposable
    {
        private readonly Mock<ILegacyQueryService> _queryServiceMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly LegacyLabRepository _repository;
        private readonly Sistema2020LegacyDbContext _context;
        private readonly SqliteConnection _sqliteConnection;

        public LegacyLabRepositoryTests()
        {
            _queryServiceMock = new Mock<ILegacyQueryService>();
            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(c => c.GetSection("ConnectionStrings")["LegacyConnection"]).Returns("Server=test;Database=test;");
            
            // SQLite In-Memory for transactions support
            _sqliteConnection = new SqliteConnection("DataSource=:memory:");
            _sqliteConnection.Open();

            var options = new DbContextOptionsBuilder<Sistema2020LegacyDbContext>()
                .UseSqlite(_sqliteConnection)
                .Options;
            
            _context = new Sistema2020LegacyDbContext(options);
            _context.Database.EnsureCreated(); // Essential for SQLite In-Memory

            _repository = new LegacyLabRepository(_context, _queryServiceMock.Object, _configMock.Object);

            // Default mocks for non-targeted calls
            _queryServiceMock.Setup(q => q.GetCurrentDayOrderCountAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);
        }

        public void Dispose()
        {
            _sqliteConnection.Close();
            _context.Dispose();
        }

        [Fact]
        public async Task Should_CorrectlyExpandProfileToMultipleAnalyses_And_CreateResultStubs()
        {
            // Arrange (Escenario: Perfil Química con 3 Análisis)
            var idPersona = 101;
            var idPerfil = 50; // Perfil de Química
            
            var orden = new OrdenLegacy 
            { 
                IdPersona = idPersona, 
                Fecha = DateTime.Now,
                HoraIngreso = "10:00:00",
                Usuario = 1,
                IDConvenio = 1 
            };

            var perfilesFacturados = new List<PerfilesFacturadosLegacy> 
            { 
                new PerfilesFacturadosLegacy { IdPerfil = idPerfil, PrecioPerfil = 150 } 
            };

            // Simulamos la respuesta del QueryService (Lo que antes era Dapper raw SQL)
            // El Perfil 50 tiene 3 análisis asociados
            var analysesMock = new List<AnalysisMappingDto>
            {
                new AnalysisMappingDto { IDOrganizador = 1, IdAnalisis = 10 }, // Glucosa
                new AnalysisMappingDto { IDOrganizador = 1, IdAnalisis = 11 }, // Urea
                new AnalysisMappingDto { IDOrganizador = 1, IdAnalisis = 12 }  // Creatinina
            };

            _queryServiceMock.Setup(q => q.GetAnalysesForProfilesAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(analysesMock);

            _queryServiceMock.Setup(q => q.GetCurrentDayOrderCountAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(5); // Hay 5 órdenes hoy, esta será la #6

            // Act
            var resultId = await _repository.GenerarOrdenLaboratorioAsync(orden, perfilesFacturados, new List<ResultadosPacienteLegacy>(), CancellationToken.None);

            // Assert
            resultId.Should().Be(orden.IdOrden);
            orden.NumeroDia.Should().Be(6);

            // Verificar la explosión a Resultados (Debe haber 3 por los 3 análisis)
            var resultadosGenerados = await _context.ResultadosPaciente.Where(r => r.IdOrden == resultId).ToListAsync();
            
            resultadosGenerados.Should().HaveCount(3);
            resultadosGenerados.All(r => r.IdPaciente == idPersona).Should().BeTrue();
            resultadosGenerados.All(r => r.IdConvenio == 1).Should().BeTrue();
            resultadosGenerados.All(r => r.EstadoDeResultado == 1).Should().BeTrue(); // Pendiente de realizar

            // Verificar IDs específicos de los análisis convergidos
            resultadosGenerados.Select(r => r.IdAnalisis).Should().Contain(new[] { 10, 11, 12 });
            
            // Verificar asignación del IdOrden en la factura detalle
            perfilesFacturados.All(p => p.IdOrden == resultId).Should().BeTrue();
        }

        [Fact]
        public async Task Should_NotCreateResults_WhenProfileHasNoMappedAnalyses()
        {
            // Arrange
            var idPerfil = 99; // Perfil sin mapeos
            var orden = new OrdenLegacy 
            { 
                IdPersona = 1, 
                Fecha = DateTime.Now,
                HoraIngreso = "11:00:00",
                Usuario = 1
            };
            var perfilesFacturados = new List<PerfilesFacturadosLegacy> { new PerfilesFacturadosLegacy { IdPerfil = idPerfil, PrecioPerfil = 100 } };

            // Simulamos respuesta vacía de análisis
            _queryServiceMock.Setup(q => q.GetAnalysesForProfilesAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AnalysisMappingDto>());

            // Act
            await _repository.GenerarOrdenLaboratorioAsync(orden, perfilesFacturados, new List<ResultadosPacienteLegacy>(), CancellationToken.None);

            // Assert
            var resultados = await _context.ResultadosPaciente.ToListAsync();
            resultados.Should().BeEmpty();
        }

        [Fact]
        public async Task Should_HandleEmptyProfiles_Gracefully()
        {
            // Arrange
            var orden = new OrdenLegacy 
            { 
                IdPersona = 1, 
                Fecha = DateTime.Now,
                HoraIngreso = "12:00:00",
                Usuario = 1
            };
            
            // Act
            var resultId = await _repository.GenerarOrdenLaboratorioAsync(orden, new List<PerfilesFacturadosLegacy>(), new List<ResultadosPacienteLegacy>(), CancellationToken.None);

            // Assert
            resultId.Should().BeGreaterThan(0);
            var perfilesInDb = await _context.PerfilesFacturados.ToListAsync();
            perfilesInDb.Should().BeEmpty();
        }
    }
}
