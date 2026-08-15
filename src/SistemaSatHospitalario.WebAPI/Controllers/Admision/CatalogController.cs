using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IMediator mediator, ILogger<CatalogController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [HttpGet("unified")]
        public async Task<ActionResult<List<CatalogItemDto>>> GetUnifiedCatalog([FromQuery] int? convenioId)
        {
            var query = new GetUnifiedCatalogQuery { ConvenioId = convenioId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CatalogItemDto>> GetById(string id)
        {
            var catalog = await _mediator.Send(new GetUnifiedCatalogQuery());
            var item = catalog.FirstOrDefault(i => i.Id.ToString() == id || string.Equals(i.Codigo, id, StringComparison.OrdinalIgnoreCase));
            if (item == null) return NotFound(new { message = "Servicio no encontrado" });
            return Ok(item);
        }

        [HttpGet("payment-methods")]
        public async Task<ActionResult<List<PaymentMethodDto>>> GetPaymentMethods([FromQuery] bool soloActivos = true)
        {
            var result = await _mediator.Send(new GetPaymentMethodsQuery { SoloActivos = soloActivos });
            return Ok(result);
        }

        [HttpPost("payment-method")]
        [Authorize(Roles = AuthorizationConstants.AdminRoles)]
        public async Task<ActionResult<Guid>> CreatePaymentMethod([FromBody] CreatePaymentMethodCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("payment-method")]
        [Authorize(Roles = AuthorizationConstants.AdminRoles)]
        public async Task<ActionResult<bool>> UpdatePaymentMethod([FromBody] UpdatePaymentMethodCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("payment-method/{id}")]
        [Authorize(Roles = AuthorizationConstants.AdminRoles)]
        public async Task<ActionResult<bool>> DeletePaymentMethod(Guid id)
        {
            var result = await _mediator.Send(new DeletePaymentMethodCommand { Id = id });
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = AuthorizationConstants.AdminRoles)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateCatalogItemCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = AuthorizationConstants.AdminRoles)]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateCatalogItemCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = AuthorizationConstants.AdminRoles)]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            _logger.LogWarning("[CATALOG-API] ATTEMPTING TO DELETE ITEM ID: {Id}", id);
            
            var result = await _mediator.Send(new DeleteCatalogItemCommand { Id = id });
            
            _logger.LogWarning("[CATALOG-API] DELETE RESULT FOR {Id}: {Result}", id, result);
            
            if (!result) return NotFound(new { message = "El servicio no existe o el ID es inválido" });
            
            return Ok(result);
        }

        // --- RECETAS / BOM ENDPOINTS (Compatibilidad con CatalogController) ---

        [HttpGet("recetas")]
        public async Task<IActionResult> GetRecetas([FromServices] SistemaSatHospitalario.Core.Application.Common.Interfaces.IApplicationDbContext context, CancellationToken ct)
        {
            try
            {
                var recetas = await (from r in context.ServiciosInsumoRecetas.AsNoTracking()
                                     join i in context.Insumos.AsNoTracking() on r.InsumoId equals i.Id into ri
                                     from i in ri.DefaultIfEmpty()
                                     select new
                                     {
                                         Id = r.Id,
                                         ServicioClinicoId = r.ServicioClinicoId,
                                         ServicioCodigo = r.ServicioCodigo,
                                         InsumoId = r.InsumoId,
                                         InsumoNombre = i != null ? i.Nombre : "Insumo Desconocido",
                                         InsumoCodigo = i != null ? i.Codigo : "",
                                         Cantidad = r.Cantidad,
                                         UnidadMedidaConsumo = r.UnidadMedidaConsumo.ToString()
                                     }).ToListAsync(ct);

                return Ok(recetas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener recetas en CatalogController.");
                return Ok(new List<object>());
            }
        }

        [HttpPost("recetas")]
        public async Task<IActionResult> CreateReceta([FromServices] SistemaSatHospitalario.Core.Application.Common.Interfaces.IApplicationDbContext context, [FromBody] CreateRecetaCatalogInputDto dto, CancellationToken ct)
        {
            if (dto == null || dto.ServicioId == Guid.Empty || (dto.InsumoId == Guid.Empty && (dto.Insumos == null || !dto.Insumos.Any())))
            {
                return BadRequest(new { Message = "Parámetros inválidos para registrar receta." });
            }

            var servicio = await context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Id == dto.ServicioId, ct);
            if (servicio == null) return NotFound(new { Message = "El servicio clínico no existe." });

            if (dto.Insumos != null && dto.Insumos.Any())
            {
                var existingRecetas = await context.ServiciosInsumoRecetas.Where(r => r.ServicioClinicoId == servicio.Id).ToListAsync(ct);
                context.ServiciosInsumoRecetas.RemoveRange(existingRecetas);

                foreach (var ins in dto.Insumos)
                {
                    Enum.TryParse<SistemaSatHospitalario.Core.Domain.Enums.UnidadMedida>(ins.UnidadMedidaConsumo ?? "UNIDAD", true, out var uom);
                    var receta = new ServicioInsumoReceta(servicio.Id, servicio.Codigo, ins.InsumoId, ins.Cantidad, uom);
                    context.ServiciosInsumoRecetas.Add(receta);
                }
            }
            else if (dto.InsumoId != Guid.Empty)
            {
                Enum.TryParse<SistemaSatHospitalario.Core.Domain.Enums.UnidadMedida>(dto.UnidadMedidaConsumo ?? "UNIDAD", true, out var uom);
                var receta = new ServicioInsumoReceta(servicio.Id, servicio.Codigo, dto.InsumoId, dto.Cantidad, uom);
                context.ServiciosInsumoRecetas.Add(receta);
            }

            await context.SaveChangesAsync(ct);
            return Ok(new { Message = "Receta guardada con éxito." });
        }

        [HttpDelete("recetas/{id}")]
        public async Task<IActionResult> DeleteReceta([FromServices] SistemaSatHospitalario.Core.Application.Common.Interfaces.IApplicationDbContext context, Guid id, CancellationToken ct)
        {
            var receta = await context.ServiciosInsumoRecetas.FirstOrDefaultAsync(r => r.Id == id, ct);
            if (receta == null) return NotFound(new { Message = "Receta no encontrada." });

            context.ServiciosInsumoRecetas.Remove(receta);
            await context.SaveChangesAsync(ct);
            return Ok(new { Message = "Receta eliminada con éxito." });
        }
    }

    public class CreateRecetaCatalogInputDto
    {
        public Guid ServicioId { get; set; }
        public Guid ServicioClinicoId { get; set; }
        public Guid InsumoId { get; set; }
        public decimal Cantidad { get; set; }
        public string? UnidadMedidaConsumo { get; set; }
        public List<ServicioInsumoRecetaItemInputDto>? Insumos { get; set; }
    }

    public class ServicioInsumoRecetaItemInputDto
    {
        public Guid InsumoId { get; set; }
        public decimal Cantidad { get; set; }
        public string? UnidadMedidaConsumo { get; set; }
    }
}
