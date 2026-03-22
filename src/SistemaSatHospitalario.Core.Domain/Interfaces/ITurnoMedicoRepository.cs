using System;
using System.Threading;
using System.Threading.Tasks;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Domain.Interfaces
{
    public interface ITurnoMedicoRepository
    {
        Task<TurnoMedico> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
        Task AgregarAsync(TurnoMedico turno, CancellationToken cancellationToken);
        Task<IncidenciaHorario> ObtenerIncidenciaSolaParaHoraAsync(Guid medicoId, DateTime horaTarget, CancellationToken cancellationToken);
        
        // Consultas para flujo de citas
        Task<List<Medico>> ListBySpecialtyAsync(string specialty, CancellationToken cancellationToken);
        Task<List<TurnoMedico>> GetBusySlotsAsync(Guid medicoId, DateTime date, CancellationToken cancellationToken);
    }
}
