using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Infrastructure.Hubs;

namespace SistemaSatHospitalario.Infrastructure.Integration
{
    public class OrdenExternaService : IOrdenExternaService
    {
        private readonly ILogger<OrdenExternaService> _logger;
        private readonly IHubContext<DashboardHub> _hubContext;
        private readonly IApplicationDbContext _context;

        public OrdenExternaService(ILogger<OrdenExternaService> logger, IHubContext<DashboardHub> hubContext, IApplicationDbContext context)
        {
            _logger = logger;
            _hubContext = hubContext;
            _context = context;
        }

        public async Task EnviarOrdenRXAsync(Guid cuentaId, Guid pacienteId, string estudio, string paciente, CancellationToken cancellationToken)
        {
            _logger.LogInformation("TRIGGER RX: Registrando orden de {Estudio} para el paciente {Paciente}.", estudio, paciente);
            
            var orden = new OrdenImagen(cuentaId, pacienteId, paciente, estudio, "RX");
            _context.OrdenesImagenes.Add(orden);
            await _context.SaveChangesAsync(cancellationToken);

            var patientCedula = (await _context.PacientesAdmision.AsNoTracking()
                .Where(p => p.Id == pacienteId)
                .Select(p => p.CedulaPasaporte)
                .FirstOrDefaultAsync(cancellationToken)) ?? "N/A";

            // Broadcast real vía SignalR con ID real de DB
            await _hubContext.Clients.All.SendAsync("ReceiveTicketUpdate", new {
                orderId = orden.Id,
                status = orden.Estado,
                patientName = orden.PacienteNombre,
                patientCedula = patientCedula,
                servicioNombre = orden.Estudio,
                tipoServicio = orden.TipoServicio,
                informe = orden.Informe
            }, cancellationToken);
        }

        public async Task EnviarOrdenTomoAsync(Guid cuentaId, Guid pacienteId, string estudio, string paciente, CancellationToken cancellationToken)
        {
            _logger.LogInformation("TRIGGER TOMO: Registrando orden de {Estudio} para el paciente {Paciente}.", estudio, paciente);
            
            var orden = new OrdenImagen(cuentaId, pacienteId, paciente, estudio, "TOMO");
            _context.OrdenesImagenes.Add(orden);
            await _context.SaveChangesAsync(cancellationToken);

            var patientCedula = (await _context.PacientesAdmision.AsNoTracking()
                .Where(p => p.Id == pacienteId)
                .Select(p => p.CedulaPasaporte)
                .FirstOrDefaultAsync(cancellationToken)) ?? "N/A";

            // Broadcast real vía SignalR con ID real de DB
            await _hubContext.Clients.All.SendAsync("ReceiveTicketUpdate", new {
                orderId = orden.Id,
                status = orden.Estado,
                patientName = orden.PacienteNombre,
                patientCedula = patientCedula,
                servicioNombre = orden.Estudio,
                tipoServicio = orden.TipoServicio,
                informe = orden.Informe
            }, cancellationToken);
        }

        public Task EnviarOrdenLegacyAsync(decimal monto, int idPersona, CancellationToken cancellationToken)
        {
            _logger.LogInformation("INTEGRACION LEGACY: Registrando orden de {Monto} para ID Persona {IdPersona}.", monto, idPersona);
            return Task.CompletedTask;
        }
    }
}
