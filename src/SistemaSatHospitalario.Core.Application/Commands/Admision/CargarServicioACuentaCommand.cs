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
            // 1. Asegurar cuenta activa (SRP: Delegado a lógica de orquestación mínima)
            var cuenta = await GetOrCreateCuentaAsync(request, cancellationToken);

            // 2. Procesar lógica específica de Consultas/Citas
            bool esConsulta = EsTipoConsulta(request.TipoServicio);
            if (esConsulta)
            {
                await ProcesarCitaMedicaAsync(request, cuenta.Id, cancellationToken);
            }

            // 3. Persistir el servicio con la descripción dinámica recibida
            cuenta.AgregarServicio(
                request.ServicioId, 
                request.Descripcion, 
                request.Precio, 
                request.Cantidad, 
                request.TipoServicio, 
                request.UsuarioCarga);

            // 4. Notificaciones e Integraciones Externas
            await NotificarSistemasExternosAsync(request, cancellationToken);

            await _repository.GuardarCambiosAsync(cancellationToken);

            return cuenta.Id;
        }

        private async Task<CuentaServicios> GetOrCreateCuentaAsync(CargarServicioACuentaCommand request, CancellationToken ct)
        {
            var cuenta = await _repository.ObtenerCuentaAbiertaPorPacienteAsync(request.PacienteId, ct);
            if (cuenta == null)
            {
                cuenta = new CuentaServicios(request.PacienteId, request.TipoIngreso, request.ConvenioId);
                await _repository.AgregarCuentaAsync(cuenta, ct);
            }
            return cuenta;
        }

        private async Task ProcesarCitaMedicaAsync(CargarServicioACuentaCommand request, Guid cuentaId, CancellationToken ct)
        {
            if (!request.MedicoId.HasValue || !request.HoraCita.HasValue)
                throw new InvalidOperationException("Los servicios de consulta requieren Médico y Hora de Cita.");

            // Validar disponibilidad (Principio de Fallo Rápido)
            if (await _repository.ExisteCitaSimultaneaAsync(request.MedicoId.Value, request.HoraCita.Value, ct))
                throw new InvalidOperationException($"El médico ya tiene una cita pautada para las {request.HoraCita.Value:HH:mm}.");

            var cita = new CitaMedica(request.MedicoId.Value, request.PacienteId, cuentaId, request.HoraCita.Value);
            await _repository.AgregarCitaMedicaAsync(cita, ct);
        }

        private async Task NotificarSistemasExternosAsync(CargarServicioACuentaCommand request, CancellationToken ct)
        {
            if (request.TipoServicio.Equals("RX", StringComparison.OrdinalIgnoreCase))
            {
                await _externaService.EnviarOrdenRXAsync(request.Descripcion, $"PacienteID:{request.PacienteId}", ct);
            }
            
            // Sincronización con Facturación Legacy
            await _externaService.EnviarOrdenLegacyAsync(request.Precio * request.Cantidad, 0, ct);
        }

        private bool EsTipoConsulta(string tipo)
        {
            if (string.IsNullOrEmpty(tipo)) return false;
            var t = tipo.ToUpper();
            return t.Contains("CONSULTA") || t.Contains("MEDICO") || t.Contains("MÉDICO");
        }
    }
}
