using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CargarServicioACuentaCommand : IRequest<Guid>
    {
        // Se cambió de Guid a int para sincronización con Legacy
        public int PacienteId { get; set; }
        public string TipoIngreso { get; set; } = string.Empty; // Particular, Seguro, Hospitalizacion, Emergencia
        // Se cambió de Guid? a int? para sincronización con Legacy
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

    public class CargarServicioACuentaCommandHandler : IRequestHandler<CargarServicioACuentaCommand, Guid>
    {
        private readonly IBillingRepository _repository;
        private readonly IOrdenExternaService _externaService;

        public CargarServicioACuentaCommandHandler(IBillingRepository repository, IOrdenExternaService externaService)
        {
            _repository = repository;
            _externaService = externaService;
        }

        public async Task<Guid> Handle(CargarServicioACuentaCommand request, CancellationToken cancellationToken)
        {
            // 1. Obtener o crear cuenta abierta para el paciente
            var cuenta = await _repository.ObtenerCuentaAbiertaPorPacienteAsync(request.PacienteId, cancellationToken);
            
            if (cuenta == null)
            {
                cuenta = new CuentaServicios(request.PacienteId, request.TipoIngreso, request.ConvenioId);
                await _repository.AgregarCuentaAsync(cuenta, cancellationToken);
            }

            // 2. Validaciones específicas por tipo de servicio
            if (request.TipoServicio == "Medico" || request.TipoServicio == "CONSULTA")
            {
                if (!request.MedicoId.HasValue || !request.HoraCita.HasValue)
                    throw new InvalidOperationException("Para servicios médicos se requiere Médico y Hora de Cita.");

                // Validar solapamiento de cola
                bool existeCita = await _repository.ExisteCitaSimultaneaAsync(request.MedicoId.Value, request.HoraCita.Value, cancellationToken);
                if (existeCita)
                    throw new InvalidOperationException($"El médico ya tiene una cita pautada para las {request.HoraCita.Value:HH:mm}.");

                // Crear Cita Médica (Cola)
                var cita = new CitaMedica(request.MedicoId.Value, request.PacienteId, cuenta.Id, request.HoraCita.Value);
                await _repository.AgregarCitaMedicaAsync(cita, cancellationToken);
            }

            // 3. Agregar el servicio a la cuenta
            cuenta.AgregarServicio(
                request.ServicioId, 
                request.Descripcion, 
                request.Precio, 
                request.Cantidad, 
                request.TipoServicio, 
                request.UsuarioCarga);

            // 4. Triggers para sistemas externos
            if (request.TipoServicio == "RX")
            {
                await _externaService.EnviarOrdenRXAsync(request.Descripcion, "PacienteID:" + request.PacienteId, cancellationToken);
            }
            
            // Integración Legacy (Siempre se envía un cargo si es necesario segun lógica del usuario)
            await _externaService.EnviarOrdenLegacyAsync(request.Precio * request.Cantidad, 0, cancellationToken);

            await _repository.GuardarCambiosAsync(cancellationToken);

            return cuenta.Id;
        }
    }
}
