using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Domain.Interfaces
{
    public interface IBillingRepository
    {
        Task<CuentaServicios?> ObtenerCuentaAbiertaPorPacienteAsync(Guid pacienteId, CancellationToken cancellationToken);
        Task<CuentaServicios?> ObtenerCuentaPorIdAsync(Guid cuentaId, CancellationToken cancellationToken);
        Task AgregarCuentaAsync(CuentaServicios cuenta, CancellationToken cancellationToken);
        Task ActualizarCuentaAsync(CuentaServicios cuenta, CancellationToken cancellationToken);
        
        Task AgregarCitaMedicaAsync(CitaMedica cita, CancellationToken cancellationToken);
        Task<bool> ExisteCitaSimultaneaAsync(Guid medicoId, DateTime hora, CancellationToken cancellationToken);

        Task GuardarCambiosAsync(CancellationToken cancellationToken);
    }
}
