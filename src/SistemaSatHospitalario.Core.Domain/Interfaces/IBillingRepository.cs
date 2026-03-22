using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Domain.Interfaces
{
    public interface IBillingRepository
    {
        // Se cambió de Guid a int para sincronización con Legacy
        Task<CuentaServicios?> ObtenerCuentaAbiertaPorPacienteAsync(int pacienteId, CancellationToken cancellationToken);
        Task<CuentaServicios?> ObtenerCuentaPorIdAsync(Guid cuentaId, CancellationToken cancellationToken);
        // Se cambió de Guid a int para sincronización con Legacy
        Task<List<CuentaServicios>> ObtenerCuentasPorPacienteAsync(int pacienteId, CancellationToken cancellationToken);
        Task AgregarCuentaAsync(CuentaServicios cuenta, CancellationToken cancellationToken);
        Task ActualizarCuentaAsync(CuentaServicios cuenta, CancellationToken cancellationToken);
        
        Task AgregarCitaMedicaAsync(CitaMedica cita, CancellationToken cancellationToken);
        Task<bool> ExisteCitaSimultaneaAsync(Guid medicoId, DateTime hora, CancellationToken cancellationToken);

        Task GuardarCambiosAsync(CancellationToken cancellationToken);
    }
}
