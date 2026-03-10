using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Legacy;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Legacy
{
    public class LegacyLabRepository : ILegacyLabRepository
    {
        private readonly Sistema2020LegacyDbContext _context;

        public LegacyLabRepository(Sistema2020LegacyDbContext context)
        {
            _context = context;
        }

        public async Task<int> GenerarOrdenLaboratorioAsync(
            OrdenLegacy orden, 
            List<PerfilesFacturadosLegacy> perfilesAFacturar, 
            List<ResultadosPacienteLegacy> resultados, 
            CancellationToken cancellationToken)
        {
            // Transacción Explícita (ACID) para inserción masiva cruzada.
            // Asegura que o entran todas las filas (Orden + Pruebas) o se descarta la Factura.
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // 1. Insertamos Orden para obtener su Id Autonumérico (Si la tabla lo permite, por ahora se inserta el DTO tal cual)
                await _context.Orden.AddAsync(orden, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 2. Insertamos la Relación en Sistema de Facturación
                // (Si IdOrden fue autonumérico, Ef Core llenó 'orden.IdOrden' automáticamente, 
                // pero si no, debemos actualizar nuestras listas hijas).
                foreach (var perfil in perfilesAFacturar)
                {
                    perfil.IdOrden = orden.IdOrden;
                }
                await _context.PerfilesFacturados.AddRangeAsync(perfilesAFacturar, cancellationToken);
                
                // 3. Insertamos el desglose Analítico
                foreach (var res in resultados)
                {
                    res.IdOrden = orden.IdOrden;
                }
                await _context.ResultadosPaciente.AddRangeAsync(resultados, cancellationToken);

                // Guardamos cambios finales y hacemos Commit.
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return orden.IdOrden;
            }
            catch (Exception ex)
            {
                // Un error genérico de DB (Timeouts, violaciones FK, Nulls).
                // Revierte cualquier inserción de la Orden u otras tablas automáticamente.
                await transaction.RollbackAsync(cancellationToken);
                throw new InvalidOperationException("Fallo crítico en Facturación Laboratorio MySQL: " + ex.Message, ex);
            }
        }
    }
}
