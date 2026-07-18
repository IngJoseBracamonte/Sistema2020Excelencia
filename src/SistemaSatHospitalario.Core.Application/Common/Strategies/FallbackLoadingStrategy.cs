using System.Threading;
using System.Threading.Tasks;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Entities;

namespace SistemaSatHospitalario.Core.Application.Common.Strategies
{
    public class FallbackLoadingStrategy : IServiceLoadingStrategy
    {
        public bool CanHandle(string tipoServicio, ServicioClinico? baseService)
        {
            // Retorna true como último recurso
            return true;
        }

        public Task ExecuteAsync(
            CargarServicioACuentaCommand request, 
            CuentaServicios cuenta, 
            PacienteAdmision paciente, 
            DetalleServicioCuenta detalle, 
            ServicioClinico? baseService, 
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
