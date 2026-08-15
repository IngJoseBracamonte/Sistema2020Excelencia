using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Notifications;
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
        private readonly IMediator _mediator;

        public ImagingLoadingStrategy(IOrdenExternaService externaService, IApplicationDbContext context, IMediator mediator)
        {
            _externaService = externaService;
            _context = context;
            _mediator = mediator;
        }

        public bool CanHandle(string tipoServicio, ServicioClinico? baseService)
        {
            return (baseService != null && (baseService.Category == ServiceCategory.Radiology || 
                                            baseService.Category == ServiceCategory.Tomography || 
                                            baseService.TipoServicioId == TipoServicioConstants.RayosX || 
                                            baseService.TipoServicioId == TipoServicioConstants.Tomografia)) || 
                   tipoServicio == TipoServicioConstants.RayosXString || 
                   tipoServicio == TipoServicioConstants.TomografiaString;
        }

        public async Task ExecuteAsync(
            CargarServicioACuentaCommand request, 
            CuentaServicios cuenta, 
            PacienteAdmision paciente, 
            DetalleServicioCuenta detalle, 
            ServicioClinico? baseService, 
            CancellationToken cancellationToken)
        {
            // REGLA 1: El estudio base de imagenología se registra SIN médico responsable ni honorario directo al técnico.
            detalle.LimpiarMedicoResponsable();

            // REGLA 2: Determinar si requiere informe (Por Toggle, por informe vinculado en catálogo, por médico asignado o en Seguros)
            bool requiereInforme = request.RequiereInforme || 
                                   request.MedicoInterpreteId.HasValue ||
                                   (baseService != null && (baseService.ServicioInformeId.HasValue || baseService.EsServicioInforme)) ||
                                   (cuenta.TipoIngreso ?? "").StartsWith(EstadoConstants.Seguro, StringComparison.OrdinalIgnoreCase) || 
                                   (request.OrigenCarga ?? "").StartsWith(EstadoConstants.Seguro, StringComparison.OrdinalIgnoreCase);

            if (requiereInforme)
            {
                ServicioClinico? reportService = null;
                if (baseService?.ServicioInforme != null)
                {
                    reportService = baseService.ServicioInforme;
                }
                else if (baseService?.ServicioInformeId.HasValue == true)
                {
                    reportService = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Id == baseService.ServicioInformeId.Value, cancellationToken);
                }

                if (reportService == null)
                {
                    // Fallback a cualquier informe de imagenología activo catalogado
                    reportService = await _context.ServiciosClinicos
                        .FirstOrDefaultAsync(s => s.Activo && (s.EsServicioInforme || s.TipoServicioId == TipoServicioConstants.Informe || s.TipoServicio == TipoServicioConstants.InformeString), cancellationToken);
                }

                if (reportService != null)
                {
                    // Inyectar el detalle del Informe Médico vinculado al estudio base via DetallePadreId
                    var reportDetail = cuenta.AgregarServicio(
                        reportService.Id, 
                        reportService.Descripcion, 
                        reportService.PrecioBase, 
                        reportService.HonorarioBase, 
                        1, 
                        TipoServicioConstants.InformeString, 
                        request.UsuarioCarga, 
                        null, 
                        request.AreaClinicaId);

                    reportDetail.AsignarDetallePadre(detalle.Id);

                    var medicoId = request.MedicoInterpreteId ?? request.MedicoId;
                    if (medicoId.HasValue)
                    {
                        reportDetail.AsignarMedicoResponsable(medicoId.Value, TipoServicioConstants.RadiologiaEspecialidad, reportService.HonorarioBase);
                    }

                    if (_context.DetallesServicioCuenta != null)
                    {
                        _context.DetallesServicioCuenta.Add(reportDetail);
                    }
                }
            }

            bool isClinical = true; // Toda orden de imagenologia genera orden externa RX o TOMO

            bool esRx = request.TipoServicio == TipoServicioConstants.RayosXString || 
                        (baseService != null && baseService.TipoServicioId == TipoServicioConstants.RayosX);

            if (isClinical)
            {
                string nombrePaciente = paciente.NombreCompleto ?? paciente.NombreCorto ?? "Paciente Desconocido";

                var medicoInterpreteId = request.MedicoInterpreteId ?? request.MedicoId;
                if (esRx)
                {
                    await _externaService.EnviarOrdenRXAsync(cuenta.Id, paciente.Id, request.Descripcion, nombrePaciente, cancellationToken, requiereInforme, medicoInterpreteId);
                }
                else
                {
                    await _externaService.EnviarOrdenTomoAsync(cuenta.Id, paciente.Id, request.Descripcion, nombrePaciente, cancellationToken, requiereInforme, medicoInterpreteId);
                }

                var orden = await _context.OrdenesImagenes
                    .Where(o => o.CuentaId == cuenta.Id 
                             && o.PacienteId == paciente.Id 
                             && o.Estudio == request.Descripcion)
                    .OrderByDescending(o => o.FechaCreacion)
                    .FirstOrDefaultAsync(cancellationToken);

                if (orden != null)
                {
                    orden.RequiereInforme = requiereInforme;
                    orden.MedicoInterpreteId = medicoInterpreteId;
                    await _context.SaveChangesAsync(cancellationToken);
                }

                // Verificar si ya se ingresó un informe previo para marcarlo como completado de inmediato.
                var ordenConInforme = await _context.OrdenesImagenes
                    .Where(o => o.PacienteId == paciente.Id 
                             && o.Estudio == request.Descripcion 
                             && !string.IsNullOrEmpty(o.Informe))
                    .OrderByDescending(o => o.FechaCreacion)
                    .FirstOrDefaultAsync(cancellationToken);

                if (ordenConInforme != null && orden != null)
                {
                    orden.MarcarComoProcesado("Sistema");
                    orden.Informe = ordenConInforme.Informe;
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }

            // Emitir evento desacoplado vía MediatR para SignalR bandejas
            string pNombre = paciente.NombreCompleto ?? paciente.NombreCorto ?? "Paciente Desconocido";
            string areaOrigen = request.OrigenCarga ?? cuenta.TipoIngreso;
            var notification = new ServicioCargadoNotification(
                esRx ? TipoServicioConstants.RayosXString : TipoServicioConstants.TomografiaString,
                request.OrigenCarga ?? request.TipoIngreso,
                paciente.Id,
                pNombre,
                areaOrigen,
                request.Descripcion,
                request.MedicoId,
                request.HoraCita
            );

            await _mediator.Publish(notification, cancellationToken);
        }
    }
}
