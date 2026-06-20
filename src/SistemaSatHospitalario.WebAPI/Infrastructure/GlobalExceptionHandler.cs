using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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

            var command = new CreateErrorTicketCommand
            {
                RequestPath = httpContext.Request.Path,
                MetodoHTTP = httpContext.Request.Method,
                MensajeExcepcion = scrubbedMessage,
                StackTrace = scrubbedStack,
                UsuarioAsociado = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonymous"
            };

            // CRITICAL: Create a new DI scope so the ErrorTicket is persisted with a FRESH DbContext.
            // The original request's DbContext may have dirty/failed tracked entities (e.g. from
            // DbUpdateConcurrencyException), which would cause SaveChangesAsync to fail again if reused.
            try
            {
                using var scope = httpContext.RequestServices.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
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
