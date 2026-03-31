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

            var mediator = httpContext.RequestServices.GetRequiredService<IMediator>();
            
            var command = new CreateErrorTicketCommand
            {
                RequestPath = httpContext.Request.Path,
                MetodoHTTP = httpContext.Request.Method,
                MensajeExcepcion = exception.Message,
                StackTrace = exception.StackTrace ?? string.Empty,
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

            // Devolvemos el error estandarizado al cliente
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(new 
            { 
                error = "Ha ocurrido un error inesperado en el sistema interno.",
                ticketId = "Verifique sus notificaciones o contacte a soporte si persiste."
            }, cancellationToken);

            return true;
        }
    }
}
