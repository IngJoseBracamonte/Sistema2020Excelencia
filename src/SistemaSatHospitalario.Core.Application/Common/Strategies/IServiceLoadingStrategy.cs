using System.Threading;
using System.Threading.Tasks;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Application.Commands.Admision;

namespace SistemaSatHospitalario.Core.Application.Common.Strategies
{
    public interface IServiceLoadingStrategy
    {
        bool CanHandle(string tipoServicio, ServicioClinico? baseService);
        
        Task ExecuteAsync(
            CargarServicioACuentaCommand request, 
            CuentaServicios cuenta, 
            PacienteAdmision paciente, 
            DetalleServicioCuenta detalle, 
            ServicioClinico? baseService, 
            CancellationToken cancellationToken);
    }
}
