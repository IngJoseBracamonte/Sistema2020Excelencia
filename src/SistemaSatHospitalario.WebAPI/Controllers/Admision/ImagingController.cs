using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using System.Security.Claims;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ImagingController : ControllerBase
    {
        private readonly IApplicationDbContext _context;

        public ImagingController(IApplicationDbContext context)
        {
            _context = context;
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

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            var order = await _context.OrdenesImagenes.FindAsync(id);
            if (order == null) return NotFound(new { Message = "Orden no encontrada." });

            var usuario = User.Identity?.Name ?? "Sistema";
            order.MarcarComoProcesado(usuario);

            await _context.SaveChangesAsync(default);

            return Ok(new { Message = "Orden marcada como procesada." });
        }
    }
}
