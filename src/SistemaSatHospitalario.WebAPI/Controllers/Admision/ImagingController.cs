using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Infrastructure.Hubs;
using System.Security.Claims;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ImagingController : ControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly IHubContext<DashboardHub> _hubContext;

        public ImagingController(IApplicationDbContext context, IHubContext<DashboardHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingOrders([FromQuery] string type)
        {
            // type debe ser RX o TOMO
            var orders = await _context.OrdenesImagenes
                .Where(o => o.TipoServicio == type && o.Estado == "Pendiente")
                .OrderByDescending(o => o.FechaCreacion)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(
            [FromQuery] string? type,
            [FromQuery] string? status,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? search)
        {
            var query = _context.OrdenesImagenes.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(type) && !type.Equals("ALL", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(o => o.TipoServicio == type);
            }

            if (!string.IsNullOrEmpty(status) && !status.Equals("ALL", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(o => o.Estado == status);
            }

            if (startDate.HasValue || endDate.HasValue)
            {
                var start = startDate?.Date ?? DateTime.MinValue;
                var end = endDate?.Date.AddDays(1).AddTicks(-1) ?? DateTime.MaxValue;
                query = query.Where(o => o.FechaCreacion >= start && o.FechaCreacion <= end);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();
                var pacIds = await _context.PacientesAdmision
                    .Where(p => p.CedulaPasaporte.Contains(searchLower))
                    .Select(p => p.Id)
                    .ToListAsync();

                query = query.Where(o => o.PacienteNombre.ToLower().Contains(searchLower) || 
                                         o.Estudio.ToLower().Contains(searchLower) || 
                                         pacIds.Contains(o.PacienteId));
            }

            var orders = await query
                .OrderByDescending(o => o.FechaCreacion)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            var order = await _context.OrdenesImagenes.FindAsync(id);
            if (order == null) return NotFound(new { Message = "Orden no encontrada." });

            var usuario = User.Identity?.Name ?? "Sistema";
            order.MarcarComoProcesado(usuario);

            await _context.SaveChangesAsync(default);

            // Broadcast real vía SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveTicketUpdate", new {
                orderId = order.Id,
                status = order.Estado,
                patientName = order.PacienteNombre,
                servicioNombre = order.Estudio,
                tipoServicio = order.TipoServicio
            });

            return Ok(new { Message = "Orden marcada como procesada." });
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _context.OrdenesImagenes.FindAsync(id);
            if (order == null) return NotFound(new { Message = "Orden no encontrada." });

            var usuario = User.Identity?.Name ?? "Sistema";
            order.Estado = "Anulado";
            order.ProcesadoPor = usuario;
            order.FechaProcesado = DateTime.UtcNow;

            await _context.SaveChangesAsync(default);

            // Broadcast real vía SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveTicketUpdate", new {
                orderId = order.Id,
                status = order.Estado,
                patientName = order.PacienteNombre,
                servicioNombre = order.Estudio,
                tipoServicio = order.TipoServicio
            });

            return Ok(new { Message = "Orden marcada como anulada." });
        }
    }
}
