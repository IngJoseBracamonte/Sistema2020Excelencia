using System;
using System.Threading;
using System.Threading.Tasks;
using SistemaSatHospitalario.Core.Domain.Entities;

namespace SistemaSatHospitalario.Core.Domain.Interfaces
{
    public interface ITurnoMedicoRepository
    {
        Task<TurnoMedico> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
        Task AgregarAsync(TurnoMedico turno, CancellationToken cancellationToken);
        Task<IncidenciaHorario> ObtenerIncidenciaSolaParaHoraAsync(Guid medicoId, DateTime horaTarget, CancellationToken cancellationToken);
    }
}
