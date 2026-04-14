using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class ValidateTechnicalServiceCommand : IRequest<bool>
    {
        public Guid TargetId { get; set; }
        public bool IsAppointment { get; set; }
        public string UsuarioOperador { get; set; } = string.Empty;
    }

    public class ValidateTechnicalServiceCommandHandler : IRequestHandler<ValidateTechnicalServiceCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ValidateTechnicalServiceCommandHandler> _logger;

        public ValidateTechnicalServiceCommandHandler(
            IApplicationDbContext context, 
            INotificationService notificationService,
            ILogger<ValidateTechnicalServiceCommandHandler> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<bool> Handle(ValidateTechnicalServiceCommand request, CancellationToken ct)
        {
            _logger.LogInformation("[VALIDATION] Iniciando validación para {Type} ID: {Id} por {Usuario}", 
                request.IsAppointment ? "Cita" : "Servicio", request.TargetId, request.UsuarioOperador);

            string alertMessage = "";
            string category = "";

            if (request.IsAppointment)
            {
                var cita = await _context.CitasMedicas.FindAsync(new object[] { request.TargetId }, ct);
                if (cita == null) return false;

                cita.ActualizarComentario((cita.Comentario ?? "") + $" [Validado por {request.UsuarioOperador}]");
                cita.MarcarComoAtendida();
                
                alertMessage = $"Cita Médica validada por el asistente técnico.";
                category = "Cita";
            }
            else
            {
                var detalle = await _context.DetallesServicioCuenta
                    .FirstOrDefaultAsync(d => d.Id == request.TargetId, ct);
                
                if (detalle == null) return false;

                detalle.MarcarRealizado(request.UsuarioOperador);
                
                alertMessage = $"Estudio '{detalle.Descripcion}' marcado como REALIZADO.";
                category = "Imagen";
            }

            await _context.SaveChangesAsync(ct);

            // [PHASE-6] Real-time Push Notification via abstraction
            await _notificationService.SendValidationAlertAsync(
                "Validación Técnica",
                alertMessage,
                category,
                new { request.TargetId, request.UsuarioOperador },
                ct);

            return true;
        }
    }
}
