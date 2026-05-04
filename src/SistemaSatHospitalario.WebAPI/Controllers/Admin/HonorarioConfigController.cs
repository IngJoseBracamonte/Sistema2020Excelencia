using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Commands.Admin;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admin
{
    [Authorize(Roles = AuthorizationConstants.AdminRoles)]
    [ApiController]
    [Route("api/[controller]")]
    public class HonorarioConfigController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IApplicationDbContext _context;

        public HonorarioConfigController(IMediator mediator, IApplicationDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var configs = await _context.HonorariosConfig
                .Include(h => h.MedicoDefault)
                .Select(h => new {
                    h.Id, h.CategoriaServicio,
                    h.MedicoDefaultId,
                    MedicoDefaultNombre = h.MedicoDefault != null ? h.MedicoDefault.Nombre : null,
                    h.UsuarioConfiguro, h.FechaConfiguracion, h.NotasConfig
                }).ToListAsync();
            return Ok(configs);
        }

        [HttpPut("{categoria}")]
        public async Task<IActionResult> SetDefault(string categoria, [FromBody] SetHonorarioDefaultCommand command)
        {
            command.CategoriaServicio = categoria;
            await _mediator.Send(command);
            return Ok(new { message = "Configuración actualizada" });
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        {
            var query = _context.LogsAsignacionHonorario.AsQueryable();
            if (desde.HasValue) query = query.Where(l => l.FechaAccion >= desde.Value.Date);
            if (hasta.HasValue) query = query.Where(l => l.FechaAccion <= hasta.Value.Date.AddDays(1));
            var logs = await query.OrderByDescending(l => l.FechaAccion).Take(200).ToListAsync();
            return Ok(logs);
        }
    }
}
