using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Concurrent;

namespace SistemaSatHospitalario.WebAPI.Infrastructure.Security
{
    /// <summary>
    /// Filtro de acción para garantizar la idempotencia en peticiones críticas.
    /// Exige la cabecera 'X-Idempotency-Key'.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class IdempotentAttribute : Attribute, IAsyncActionFilter
    {
        private static readonly ConcurrentDictionary<string, object> _cache = new();

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("X-Idempotency-Key", out var key))
            {
                context.Result = new BadRequestObjectResult(new { Error = "Falta la cabecera 'X-Idempotency-Key' requerida para esta operación." });
                return;
            }

            string idempotencyKey = key.ToString();

            // Verificamos si ya existe (Simplificación de auditoría)
            // En Producción real, esto debería usar Redis o una tabla de auditoría con tiempo de expiración.
            if (_cache.ContainsKey(idempotencyKey))
            {
                context.Result = new ConflictObjectResult(new { Error = "Esta petición ya ha sido procesada o está en curso. Por favor, espere o verifique el estado." });
                return;
            }

            // Marcamos como en curso
            _cache.TryAdd(idempotencyKey, new { Status = "Processing", Timestamp = DateTime.UtcNow });

            try
            {
                var resultContext = await next();
                
                // Si hubo error, podríamos querer removerla para reintentos, 
                // pero si fue éxito, la dejamos para evitar duplicados.
            }
            catch
            {
                _cache.TryRemove(idempotencyKey, out _);
                throw;
            }
        }
    }
}
