using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Core.Domain.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Common.Strategies
{
    public class ImagingLoadingStrategy : IServiceLoadingStrategy
    {
        private readonly IOrdenExternaService _externaService;
        private readonly IApplicationDbContext _context;

        public ImagingLoadingStrategy(IOrdenExternaService externaService, IApplicationDbContext context)
        {
            _externaService = externaService;
            _context = context;
        }

        public bool CanHandle(string tipoServicio, ServicioClinico? baseService)
        {
            return (baseService != null && (baseService.Category == ServiceCategory.Radiology || baseService.Category == ServiceCategory.Tomography)) || 
                   tipoServicio == EstadoConstants.RX || 
                   tipoServicio == EstadoConstants.TOMO;
        }

        public async Task ExecuteAsync(
            CargarServicioACuentaCommand request, 
            CuentaServicios cuenta, 
            PacienteAdmision paciente, 
            DetalleServicioCuenta detalle, 
            ServicioClinico? baseService, 
            CancellationToken cancellationToken)
        {
            // Autodetección: Se genera si OrigenCarga está definido OR si es flujo clínico (Hospitalización, Emergencia, UCI).
            bool isClinical = !string.IsNullOrEmpty(request.OrigenCarga) || 
                              cuenta.TipoIngreso == EstadoConstants.Hospitalizacion || 
                              cuenta.TipoIngreso == EstadoConstants.Emergencia || 
                              cuenta.TipoIngreso == "UCI";

            if (isClinical)
            {
                string nombrePaciente = paciente.NombreCompleto ?? paciente.NombreCorto ?? "Paciente Desconocido";
                bool esRx = request.TipoServicio == EstadoConstants.RX || 
                            (baseService != null && baseService.Category == ServiceCategory.Radiology);

                if (esRx)
                {
                    await _externaService.EnviarOrdenRXAsync(cuenta.Id, paciente.Id, request.Descripcion, nombrePaciente, cancellationToken);
                }
                else
                {
                    await _externaService.EnviarOrdenTomoAsync(cuenta.Id, paciente.Id, request.Descripcion, nombrePaciente, cancellationToken);
                }

                // Regla de Negocio 2: Rx / Tomografía - Verificar si ya se ingresó un informe previo para marcarlo como completado de inmediato.
                var ordenConInforme = await _context.OrdenesImagenes
                    .Where(o => o.PacienteId == paciente.Id 
                             && o.Estudio == request.Descripcion 
                             && !string.IsNullOrEmpty(o.Informe))
                    .OrderByDescending(o => o.FechaCreacion)
                    .FirstOrDefaultAsync(cancellationToken);

                if (ordenConInforme != null)
                {
                    // Buscar la orden que acabamos de crear y marcarla como completada/procesada
                    var nuevaOrden = await _context.OrdenesImagenes
                        .Where(o => o.CuentaId == cuenta.Id 
                                 && o.PacienteId == paciente.Id 
                                 && o.Estudio == request.Descripcion 
                                 && o.Estado == "Pendiente")
                        .OrderByDescending(o => o.FechaCreacion)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (nuevaOrden != null)
                    {
                        nuevaOrden.MarcarComoProcesado("Sistema");
                        nuevaOrden.Informe = ordenConInforme.Informe;
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                }
            }
        }
    }
}
