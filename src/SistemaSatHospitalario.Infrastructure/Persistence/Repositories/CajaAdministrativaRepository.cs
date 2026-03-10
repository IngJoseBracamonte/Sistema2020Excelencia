using System;
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
    }
}
