using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Commands.System;
using System.Security.Claims;

namespace SistemaSatHospitalario.WebAPI.Infrastructure
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Unhandled exception captured by GlobalExceptionHandler at {RequestPath}", httpContext.Request.Path);

            var scrubbedMessage = ScrubPii(exception.Message);
            var scrubbedStack = ScrubPii(exception.StackTrace ?? string.Empty);

            var mediator = httpContext.RequestServices.GetRequiredService<IMediator>();
            
            var command = new CreateErrorTicketCommand
            {
                RequestPath = httpContext.Request.Path,
                MetodoHTTP = httpContext.Request.Method,
                MensajeExcepcion = scrubbedMessage,
                StackTrace = scrubbedStack,
                UsuarioAsociado = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonymous"
            };

            // Ejecutamos el guardado de forma segura, si falla la BD no queremos ocultar el log original
            try
            {
                var ticketId = await mediator.Send(command, cancellationToken);
                _logger.LogInformation("Generated Error Ticket {TicketId}", ticketId);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to persist Error Ticket to database.");
            }

            // Devolvemos el error estandarizado al cliente con detalles para debug en producción
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(new 
            { 
                error = "Ha ocurrido un error inesperado en el sistema interno.",
                ticketId = "Verifique sus notificaciones o contacte a soporte si persiste.",
                devError = scrubbedMessage,
                devStack = scrubbedStack
            }, cancellationToken);

            return true;
        }

        private string ScrubPii(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            
            // Mask numeric strings of 7+ digits (potential IDs or Phones)
            var scrubbed = System.Text.RegularExpressions.Regex.Replace(input, @"\d{7,}", "[MASKED_ID]");
            
            // Mask potential PII in common DB error patterns like "entry '...' for key"
            scrubbed = System.Text.RegularExpressions.Regex.Replace(scrubbed, @"entry '([^']+)' for key", "entry '[MASKED_DATA]' for key");
            
            return scrubbed;
        }
    }
}
