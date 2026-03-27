using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CargarServicioACuentaCommand : IRequest<CargarServicioResult>
    {
        // Se estandarizó de int a Guid para identidad nativa (V11.1)
        public Guid PacienteId { get; set; }
        public string TipoIngreso { get; set; } = string.Empty; // Particular, Seguro, Hospitalizacion, Emergencia
        // Se mantiene int? para ConvenioId por ahora (referencia Legacy)
        public int? ConvenioId { get; set; }
        public Guid ServicioId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public string TipoServicio { get; set; } = string.Empty; // Medico, RX, Laboratorio, Insumo
        public string UsuarioCarga { get; set; } = string.Empty;

        // Datos para Cita Médica (solo si TipoServicio == "Medico")
        public Guid? MedicoId { get; set; }
        public DateTime? HoraCita { get; set; }
    }

    public record CargarServicioResult(Guid CuentaId, Guid DetalleId);

    public class CargarServicioACuentaCommandHandler : IRequestHandler<CargarServicioACuentaCommand, CargarServicioResult>
    {
        private readonly IBillingRepository _repository;
        private readonly IOrdenExternaService _externaService;
        private readonly IApplicationDbContext _context;

        public CargarServicioACuentaCommandHandler(IBillingRepository repository, IOrdenExternaService externaService, IApplicationDbContext context)
        {
            _repository = repository;
            _externaService = externaService;
            _context = context;
        }

        public async Task<CargarServicioResult> Handle(CargarServicioACuentaCommand request, CancellationToken cancellationToken)
        {
            // 1. Obtención Directa del Paciente (V11.1 Identity Alignment)
            var paciente = await _context.PacientesAdmision.FirstOrDefaultAsync(
                p => p.Id == request.PacienteId, cancellationToken);

            if (paciente == null)
            {
                throw new InvalidOperationException($"No se encontró un paciente con el ID: {request.PacienteId}. Asegúrese de registrarlo primero.");
            }

            // 2. Asegurar cuenta activa usando el GUID local
            var cuenta = await GetOrCreateCuentaAsync(paciente.Id, request, cancellationToken);

            // 3. Procesar lógica específica de Consultas/Citas
            bool esConsulta = EstadoConstants.EsConsulta(request.TipoServicio);
            if (esConsulta)
            {
                await ProcesarCitaMedicaAsync(request, paciente.Id, cuenta.Id, cancellationToken);
            }

            // 4. Persistir el servicio
            var detalle = cuenta.AgregarServicio(
                request.ServicioId, 
                request.Descripcion, 
                request.Precio, 
                request.Cantidad, 
                request.TipoServicio, 
                request.UsuarioCarga);
            
            // 5. Notificaciones e Integraciones Externas
            await NotificarSistemasExternosAsync(request, cancellationToken);

            await _repository.GuardarCambiosAsync(cancellationToken);

            return new CargarServicioResult(cuenta.Id, detalle.Id);
        }

        private async Task<CuentaServicios> GetOrCreateCuentaAsync(Guid pacienteId, CargarServicioACuentaCommand request, CancellationToken ct)
        {
            var cuenta = await _repository.ObtenerCuentaAbiertaPorPacienteAsync(pacienteId, ct);
            if (cuenta == null)
            {
                cuenta = new CuentaServicios(pacienteId, request.UsuarioCarga, request.TipoIngreso, request.ConvenioId);
                await _repository.AgregarCuentaAsync(cuenta, ct);
            }
            return cuenta;
        }

        private async Task ProcesarCitaMedicaAsync(CargarServicioACuentaCommand request, Guid pacienteId, Guid cuentaId, CancellationToken ct)
        {
            if (!request.MedicoId.HasValue || !request.HoraCita.HasValue)
                throw new InvalidOperationException("Los servicios de consulta requieren Médico y Hora de Cita.");

            var horaNormalizada = new DateTime(
                request.HoraCita.Value.Year, request.HoraCita.Value.Month, request.HoraCita.Value.Day,
                request.HoraCita.Value.Hour, request.HoraCita.Value.Minute, 0, 
                DateTimeKind.Unspecified);

            if (await _repository.ExisteCitaSimultaneaAsync(request.MedicoId.Value, horaNormalizada, ct))
                throw new InvalidOperationException($"El médico ya tiene una cita pautada para las {horaNormalizada:HH:mm}.");

            var cita = new CitaMedica(request.MedicoId.Value, pacienteId, cuentaId, horaNormalizada);
            await _repository.AgregarCitaMedicaAsync(cita, ct);
        }

        private async Task NotificarSistemasExternosAsync(CargarServicioACuentaCommand request, CancellationToken ct)
        {
            if (request.TipoServicio.Equals(EstadoConstants.RX, StringComparison.OrdinalIgnoreCase))
            {
                await _externaService.EnviarOrdenRXAsync(request.Descripcion, $"PacienteID:{request.PacienteId}", ct);
            }
            
            await _externaService.EnviarOrdenLegacyAsync(request.Precio * request.Cantidad, 0, ct);
        }
    }
}
