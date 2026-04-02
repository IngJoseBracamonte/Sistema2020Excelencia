using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using System.Collections.Generic;
using System.Linq;

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

        public async Task<List<Medico>> ListBySpecialtyAsync(string specialty, CancellationToken cancellationToken)
        {
            return await _context.Medicos
                .Where(m => m.Especialidad.Nombre == specialty && m.Activo)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<TurnoMedico>> GetBusySlotsAsync(Guid medicoId, DateTime date, CancellationToken cancellationToken)
        {
            return await _context.TurnosMedicos
                .Where(t => t.MedicoId == medicoId && t.FechaHoraToma.Date == date.Date)
                .ToListAsync(cancellationToken);
        }
    }
}
