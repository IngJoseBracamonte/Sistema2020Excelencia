using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Repositories
{
    public class TurnoMedicoRepository : ITurnoMedicoRepository
    {
        private readonly SatHospitalarioDbContext _context;

        public TurnoMedicoRepository(SatHospitalarioDbContext context)
        {
            _context = context;
        }

        public async Task AgregarAsync(TurnoMedico turno, CancellationToken cancellationToken)
        {
            await _context.TurnosMedicos.AddAsync(turno, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IncidenciaHorario> ObtenerIncidenciaSolaParaHoraAsync(Guid medicoId, DateTime horaTarget, CancellationToken cancellationToken)
        {
            return await _context.IncidenciasHorario
                .FirstOrDefaultAsync(i => i.MedicoId == medicoId 
                                       && i.Inicio <= horaTarget 
                                       && i.Fin >= horaTarget, cancellationToken);
        }

        public async Task<TurnoMedico> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.TurnosMedicos.FindAsync(new object[] { id }, cancellationToken);
        }
    }
}
