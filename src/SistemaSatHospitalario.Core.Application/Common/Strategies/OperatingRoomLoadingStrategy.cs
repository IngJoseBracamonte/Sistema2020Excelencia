using System;
using System.Threading;
using System.Threading.Tasks;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Entities;

namespace SistemaSatHospitalario.Core.Application.Common.Strategies
{
    public class OperatingRoomLoadingStrategy : IServiceLoadingStrategy
    {
        public bool CanHandle(string tipoServicio, ServicioClinico? baseService)
        {
            return tipoServicio.Equals("Quirofano", StringComparison.OrdinalIgnoreCase) || 
                   tipoServicio.Equals("Cirugia", StringComparison.OrdinalIgnoreCase) || 
                   tipoServicio.Equals("Quirófano", StringComparison.OrdinalIgnoreCase) ||
                   tipoServicio.Equals("Cirugía", StringComparison.OrdinalIgnoreCase);
        }

        public Task ExecuteAsync(
            CargarServicioACuentaCommand request, 
            CuentaServicios cuenta, 
            PacienteAdmision paciente, 
            DetalleServicioCuenta detalle, 
            ServicioClinico? baseService, 
            CancellationToken cancellationToken)
        {
            // Regla de Negocio 5: Quirófano - Actualmente en fase de desarrollo.
            // La estructura queda lista para su fácil extensión en el futuro.
            return Task.CompletedTask;
        }
    }
}
