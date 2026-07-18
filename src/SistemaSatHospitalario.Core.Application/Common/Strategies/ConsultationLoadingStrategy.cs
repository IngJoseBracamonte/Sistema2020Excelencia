using System;
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
    public class ConsultationLoadingStrategy : IServiceLoadingStrategy
    {
        private readonly IBillingRepository _repository;
        private readonly IApplicationDbContext _context;

        public ConsultationLoadingStrategy(IBillingRepository repository, IApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
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
            // Regla de Negocio 3: Consulta - Evaluar si requiere o no la asignación de un horario en la agenda médica.
            bool requiresAppointmentTime = request.TipoIngreso == EstadoConstants.Particular || request.TipoIngreso == EstadoConstants.Seguro;
            
            if (requiresAppointmentTime || (request.MedicoId.HasValue && request.HoraCita.HasValue))
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

                if (!request.MedicoId.HasValue || !request.HoraCita.HasValue)
                {
                    throw new InvalidOperationException("Los servicios de consulta requieren Médico y Hora de Cita.");
                }

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
        }
    }
}
