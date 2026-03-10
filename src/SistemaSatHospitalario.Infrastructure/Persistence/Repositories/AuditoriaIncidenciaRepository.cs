using System.Threading;
using System.Threading.Tasks;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Repositories
{
    public class AuditoriaIncidenciaRepository : IAuditoriaIncidenciaRepository
    {
        private readonly SatHospitalarioDbContext _context;

        public AuditoriaIncidenciaRepository(SatHospitalarioDbContext context)
        {
            _context = context;
        }

        public async Task RegistrarAsync(RegistroAuditoriaIncidencia registro, CancellationToken cancellationToken)
        {
            await _context.RegistrosAuditoriaIncidencia.AddAsync(registro, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
