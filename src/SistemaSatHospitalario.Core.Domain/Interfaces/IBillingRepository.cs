using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Domain.Interfaces
{
    public interface IBillingRepository
    {
        // Se revirtió a Guid para el nuevo sistema de identidad (V11.0 Sync Pro)
        Task<CuentaServicios?> ObtenerCuentaAbiertaPorPacienteAsync(Guid pacienteId, CancellationToken cancellationToken);
        Task<CuentaServicios?> ObtenerCuentaPorIdAsync(Guid cuentaId, CancellationToken cancellationToken);
        Task<List<CuentaServicios>> ObtenerCuentasPorPacienteAsync(Guid pacienteId, CancellationToken cancellationToken);
        Task AgregarCuentaAsync(CuentaServicios cuenta, CancellationToken cancellationToken);
        Task ActualizarCuentaAsync(CuentaServicios cuenta, CancellationToken cancellationToken);
        
        // V11.0: Bypass de concurrencia para cierre atómico (V10.9 SQL Direct)
        Task ForzarCierreCuentaAsync(Guid cuentaId, DateTime fechaCierre, CancellationToken cancellationToken);
        
        Task AgregarCitaMedicaAsync(CitaMedica cita, CancellationToken cancellationToken);
        Task<bool> ExisteCitaSimultaneaAsync(Guid medicoId, DateTime hora, CancellationToken cancellationToken);
        Task CancelarCitaMedicaAsync(Guid cuentaId, Guid medicoId, DateTime hora, CancellationToken cancellationToken);
        Task CancelarCitaPorIdAsync(Guid citaId, CancellationToken cancellationToken);

        Task GuardarCambiosAsync(CancellationToken cancellationToken);
    }
}
