using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Infrastructure.Hubs;

namespace SistemaSatHospitalario.Infrastructure.Integration
{
    public class OrdenExternaService : IOrdenExternaService
    {
        private readonly ILogger<OrdenExternaService> _logger;
        private readonly IHubContext<DashboardHub> _hubContext;

        public OrdenExternaService(ILogger<OrdenExternaService> logger, IHubContext<DashboardHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task EnviarOrdenRXAsync(string estudio, string paciente, CancellationToken cancellationToken)
        {
            _logger.LogInformation("TRIGGER RX: Enviando orden de {Estudio} para el paciente {Paciente}.", estudio, paciente);
            
            // Broadcast real vía SignalR (V16.0 Imaging Optimization)
            await _hubContext.Clients.All.SendAsync("ReceiveTicketUpdate", new {
                orderId = new Random().Next(1000, 9999),
                status = "Pendiente",
                patientName = paciente,
                servicioNombre = estudio,
                tipoServicio = "RX"
            }, cancellationToken);
        }

        public async Task EnviarOrdenTomoAsync(string estudio, string paciente, CancellationToken cancellationToken)
        {
            _logger.LogInformation("TRIGGER TOMO: Enviando orden de {Estudio} para el paciente {Paciente}.", estudio, paciente);
            
            // Broadcast real vía SignalR (V16.0 Imaging Optimization)
            await _hubContext.Clients.All.SendAsync("ReceiveTicketUpdate", new {
                orderId = new Random().Next(1000, 9999),
                status = "Pendiente",
                patientName = paciente,
                servicioNombre = estudio,
                tipoServicio = "TOMO"
            }, cancellationToken);
        }

        public Task EnviarOrdenLegacyAsync(decimal monto, int idPersona, CancellationToken cancellationToken)
        {
            _logger.LogInformation("INTEGRACION LEGACY: Registrando orden de {Monto} para ID Persona {IdPersona}.", monto, idPersona);
            return Task.CompletedTask;
        }
    }
}
