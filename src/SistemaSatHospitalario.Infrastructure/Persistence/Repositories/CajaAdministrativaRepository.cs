using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Repositories
{
    public class CajaAdministrativaRepository : ICajaAdministrativaRepository
    {
        private readonly SatHospitalarioDbContext _context;

        public CajaAdministrativaRepository(SatHospitalarioDbContext context)
        {
            _context = context;
        }

        public async Task<CajaDiaria?> ObtenerCajaAbiertaAsync(CancellationToken cancellationToken)
        {
            return await _context.CajasDiarias
                                 .FirstOrDefaultAsync(c => c.Estado == "Abierta", cancellationToken);
        }

        public async Task<CajaDiaria?> ObtenerCajaAbiertaNoTrackingAsync(CancellationToken cancellationToken)
        {
            return await _context.CajasDiarias
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.Estado == "Abierta", cancellationToken);
        }

        public async Task<CajaDiaria?> ObtenerCajaAbiertaPorUsuarioAsync(string usuarioId, CancellationToken cancellationToken)
        {
            return await _context.CajasDiarias
                                 .FirstOrDefaultAsync(c => c.Estado == "Abierta" && c.UsuarioId == usuarioId, cancellationToken);
        }

        public async Task AgregarCajaAsync(CajaDiaria caja, CancellationToken cancellationToken)
        {
            await _context.CajasDiarias.AddAsync(caja, cancellationToken);
        }

        public async Task GuardarCambiosAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<CajaDiaria?> ObtenerCajaAbiertaConDetallesAsync(CancellationToken cancellationToken)
        {
            return await _context.CajasDiarias
                                 .FirstOrDefaultAsync(c => c.Estado == "Abierta", cancellationToken);
        }

        public async Task<IEnumerable<CajaDiaria>> ObtenerHistorialCierresAsync(DateTime desde, DateTime hasta, string? usuarioId, CancellationToken cancellationToken)
        {
            var start = desde.Date;
            var end = hasta.Date.AddDays(1).AddTicks(-1);

            var query = _context.CajasDiarias
                                .Where(c => c.Estado == "Cerrada" && c.FechaCierre >= start && c.FechaCierre <= end);

            if (!string.IsNullOrEmpty(usuarioId))
            {
                query = query.Where(c => c.UsuarioId == usuarioId);
            }

            return await query.OrderByDescending(c => c.FechaCierre)
                              .ToListAsync(cancellationToken);
        }
    }
}
