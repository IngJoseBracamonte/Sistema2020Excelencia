using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Common.Behaviors
{
    /// <summary>
    /// [PHASE-3] Price Auditing Pipeline.
    /// Automatically detects and logs price/honorarium modifications for any command 
    /// implementing IAuditablePriceRequest.
    /// </summary>
    public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IAuditablePriceRequest
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<AuditBehavior<TRequest, TResponse>> _logger;

        public AuditBehavior(IApplicationDbContext context, ILogger<AuditBehavior<TRequest, TResponse>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // 1. Ejecutar el Handler primero para obtener el resultado (Response) 
            // que debe contener el DetalleId para asociar el log.
            var response = await next();

            // 2. Lógica de Auditoría Post-Ejecución
            try 
            {
                if (Guid.TryParse(request.ServicioId, out var serviceGuid))
                {
                    var catalogo = await _context.ServiciosClinicos
                        .AsNoTracking()
                        .FirstOrDefaultAsync(s => s.Id == serviceGuid, cancellationToken);

                    if (catalogo != null && (catalogo.PrecioBase != request.Precio || catalogo.HonorarioBase != request.Honorario))
                    {
                        // Intentamos obtener el DetalleId de la respuesta usando reflexión (Senior Hybrid Pattern)
                        var responseType = response?.GetType();
                        var detalleIdProp = responseType?.GetProperty("DetalleId");
                        var detalleIdValue = (Guid?)(detalleIdProp?.GetValue(response));

                        if (detalleIdValue.HasValue)
                        {
                            var auditLog = new LogAuditoriaPrecio(
                                detalleIdValue.Value,
                                request.Descripcion,
                                catalogo.PrecioBase,
                                request.Precio,
                                catalogo.HonorarioBase,
                                request.Honorario,
                                request.UsuarioCarga,
                                string.IsNullOrEmpty(request.SupervisorKey) ? "Acceso Autorizado" : "Supervisor Key Verificada"
                            );

                            await _context.AuditLogsPrecios.AddAsync(auditLog, cancellationToken);
                            await _context.SaveChangesAsync(cancellationToken);
                            
                            _logger.LogInformation("[PIPELINE-AUDIT] Cambio de precio registrado para Detalle {DetalleId}", detalleIdValue.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[PIPELINE-AUDIT] Fallo no crítico al intentar auditar precio.");
            }

            return response;
        }
    }
}
