using System;
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
    public class ConsultationLoadingStrategy : IServiceLoadingStrategy
    {
        private readonly IBillingRepository _repository;
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public ConsultationLoadingStrategy(IBillingRepository repository, IApplicationDbContext context, IMediator mediator)
        {
            _repository = repository;
            _context = context;
            _mediator = mediator;
        }

        public bool CanHandle(string tipoServicio, ServicioClinico? baseService)
        {
            return EstadoConstants.EsConsulta(tipoServicio) || 
                   (baseService != null && baseService.Category == ServiceCategory.Consultation);
        }

        public async Task ExecuteAsync(
            CargarServicioACuentaCommand request, 
            CuentaServicios cuenta, 
            PacienteAdmision paciente, 
            DetalleServicioCuenta detalle, 
            ServicioClinico? baseService, 
            CancellationToken cancellationToken)
        {
            Guid? citaAreaClinicaId = request.AreaClinicaId;
            if (!string.IsNullOrEmpty(request.OrigenCarga) && request.OrigenCarga.StartsWith("ENFERMERIA", StringComparison.OrdinalIgnoreCase))
            {
                var areaEnf = await _context.AreasClinicas.FirstOrDefaultAsync(a => a.Codigo == "ENFERMERIA", cancellationToken);
                if (areaEnf == null)
                {
                    areaEnf = new AreaClinica(SeedConstants.SedeId_Principal, "ENFERMERIA", "Enfermería");
                    _context.AreasClinicas.Add(areaEnf);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                citaAreaClinicaId = areaEnf.Id;
            }

            // Bifurcación: Facturación (HoraCita programada) vs Enfermería/Demanda (HoraCita == null)
            if (request.HoraCita.HasValue && request.MedicoId.HasValue)
            {
                var horaNormalizada = new DateTime(
                    request.HoraCita.Value.Year, request.HoraCita.Value.Month, request.HoraCita.Value.Day,
                    request.HoraCita.Value.Hour, request.HoraCita.Value.Minute, 0, 
                    DateTimeKind.Unspecified);

                while (await _repository.ExisteCitaSimultaneaAsync(request.MedicoId.Value, horaNormalizada, cancellationToken))
                {
                    horaNormalizada = horaNormalizada.AddMinutes(1);
                }

                var cita = new CitaMedica(request.MedicoId.Value, paciente.Id, cuenta.Id, horaNormalizada, null, citaAreaClinicaId);
                await _repository.AgregarCitaMedicaAsync(cita, cancellationToken);
            }
            else if (request.MedicoId.HasValue)
            {
                // Desde Enfermería o atención interna sin bloque de hora prefijado: se registra CitaMedica activa con hora actual para cola de evaluación médica
                var cita = new CitaMedica(request.MedicoId.Value, paciente.Id, cuenta.Id, DateTime.Now, "Atención Interna / Por Demanda", citaAreaClinicaId);
                await _repository.AgregarCitaMedicaAsync(cita, cancellationToken);
            }
            else if (request.TipoIngreso == EstadoConstants.Particular || request.TipoIngreso == EstadoConstants.Seguro)
            {
                throw new InvalidOperationException("Los servicios de consulta requieren asignación de Médico.");
            }

            // Notificación MediatR desacoplada
            string nombrePaciente = paciente.NombreCompleto ?? paciente.NombreCorto ?? "Paciente Desconocido";
            string areaOrigen = request.OrigenCarga ?? cuenta.TipoIngreso;
            var notification = new ServicioCargadoNotification(
                "CONSULTA",
                request.OrigenCarga ?? request.TipoIngreso,
                paciente.Id,
                nombrePaciente,
                areaOrigen,
                request.Descripcion,
                request.MedicoId,
                request.HoraCita
            );

            await _mediator.Publish(notification, cancellationToken);
        }
    }
}
