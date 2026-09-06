using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Repositories
{
    public class OrdenRepository : IOrdenRepository
    {
        private readonly SatHospitalarioDbContext _context;

        public OrdenRepository(SatHospitalarioDbContext context)
        {
            _context = context;
        }

        public async Task<OrdenDeServicio?> ObtenerDetalleOrdenAsync(Guid ordenId, CancellationToken cancellationToken)
        {
            return await _context.OrdenesDeServicio.FirstOrDefaultAsync(o => o.Id == ordenId, cancellationToken);
        }

        public async Task ActualizarOrdenAsync(OrdenDeServicio orden, CancellationToken cancellationToken)
        {
            _context.OrdenesDeServicio.Update(orden);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
