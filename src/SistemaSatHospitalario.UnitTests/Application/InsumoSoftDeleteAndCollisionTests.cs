using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Xunit;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class InsumoSoftDeleteAndCollisionTests
    {
        private SatHospitalarioDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new SatHospitalarioDbContext(options);
        }

        [Fact]
        public async Task Insumo_SoftDelete_PermiteDetectarEstadoInactivoParaReactivacion()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var codigo = "INS-TEST-001";
            var insumo = new Insumo(codigo, "Gasa Estéril 10x10", 100, UnidadMedida.UNIDAD, 0.50m, false, "Descartable");
            db.Insumos.Add(insumo);
            await db.SaveChangesAsync();

            // Act: Inhabilitar (Soft Delete)
            insumo.SoftDelete();
            await db.SaveChangesAsync();

            // Assert
            var insumoDb = await db.Insumos.FirstOrDefaultAsync(i => i.Codigo == codigo);
            Assert.NotNull(insumoDb);
            Assert.True(insumoDb.IsDeleted);
            Assert.NotNull(insumoDb.FechaInactivacion);
            Assert.True(insumoDb.OcultoEnTraslados);

            // Act 2: Restaurar / Reactivar
            insumoDb.Restaurar();
            await db.SaveChangesAsync();

            // Assert 2
            var insumoRestaurado = await db.Insumos.FirstOrDefaultAsync(i => i.Codigo == codigo);
            Assert.NotNull(insumoRestaurado);
            Assert.False(insumoRestaurado.IsDeleted);
            Assert.Null(insumoRestaurado.FechaInactivacion);
            Assert.False(insumoRestaurado.OcultoEnTraslados);
        }

        [Fact]
        public async Task Medico_Delete_DebeHacerSoftDeleteYNoEliminarRegistroFisico()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var especialidad = new Especialidad("Cirugía General");
            db.Especialidades.Add(especialidad);
            await db.SaveChangesAsync();

            var medico = new Medico("Dr. Alejandro Magno", especialidad.Id, 150m, "04141234567");
            db.Medicos.Add(medico);
            await db.SaveChangesAsync();

            var handler = new DeleteMedicoCommandHandler(db);

            // Act
            var resultado = await handler.Handle(new DeleteMedicoCommand { Id = medico.Id }, CancellationToken.None);

            // Assert
            Assert.True(resultado);
            var medicoDb = await db.Medicos.FindAsync(medico.Id);
            Assert.NotNull(medicoDb); // No fue eliminado físicamente
            Assert.False(medicoDb.Activo); // Soft delete aplicado
        }

        [Fact]
        public async Task Especialidad_Delete_DebeHacerSoftDeleteYNoEliminarRegistroFisico()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var especialidad = new Especialidad("Traumatología");
            db.Especialidades.Add(especialidad);
            await db.SaveChangesAsync();

            var handler = new DeleteEspecialidadCommandHandler(db);

            // Act
            await handler.Handle(new DeleteEspecialidadCommand { Id = especialidad.Id }, CancellationToken.None);

            // Assert
            var espDb = await db.Especialidades.FindAsync(especialidad.Id);
            Assert.NotNull(espDb); // No fue eliminado físicamente
            Assert.False(espDb.Activo); // Soft delete aplicado
        }
    }
}
