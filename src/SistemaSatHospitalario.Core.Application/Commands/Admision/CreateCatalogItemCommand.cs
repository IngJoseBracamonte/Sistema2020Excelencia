using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CreateCatalogItemCommand : IRequest<Guid>
    {
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public decimal PrecioUsd { get; set; }
        public string Tipo { get; set; } // LABORATORIO, RX, CONSULTA, INFORME, etc.
        public int? TipoServicioId { get; set; }
        public bool Activo { get; set; } = true;
        public decimal HonorarioBase { get; set; }
        public string? HonorariumCategory { get; set; }
        public bool RequiereInventario { get; set; } = true;
        public Guid? ServicioInformeId { get; set; }
        public bool EsServicioInforme { get; set; } = false;
        public List<string> SugerenciasIds { get; set; } = new List<string>();
        public List<DoctorHonorarioInputDto> HonorariosMedicos { get; set; } = new List<DoctorHonorarioInputDto>();
        public List<DoctorHonorarioInputDto> HonorariosEspecificos { get; set; } = new List<DoctorHonorarioInputDto>();
        public List<ServicioInsumoRecetaInputDto> RecetaInsumos { get; set; } = new List<ServicioInsumoRecetaInputDto>();
        public List<ServicioInsumoRecetaInputDto> Receta { get; set; } = new List<ServicioInsumoRecetaInputDto>();
    }

    public class CreateCatalogItemCommandHandler : IRequestHandler<CreateCatalogItemCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateCatalogItemCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateCatalogItemCommand request, CancellationToken cancellationToken)
        {
            var item = new ServicioClinico(request.Codigo, request.Descripcion, request.PrecioUsd, request.Tipo)
            {
                Activo = request.Activo,
                HonorarioBase = request.HonorarioBase,
                HonorariumCategory = request.HonorariumCategory,
                RequiereInventario = request.RequiereInventario,
                ServicioInformeId = request.ServicioInformeId,
                EsServicioInforme = request.EsServicioInforme 
                    || (request.TipoServicioId == TipoServicioConstants.Informe) 
                    || string.Equals(request.Tipo, "INFORME", StringComparison.OrdinalIgnoreCase)
            };

            if (request.TipoServicioId.HasValue)
            {
                item.TipoServicioId = request.TipoServicioId.Value;
            }

            // Validar Invariantes de Dominio
            item.ValidarInvariantes();

            await _context.ServiciosClinicos.AddAsync(item, cancellationToken);

            // 1. Filtrar, parsear y validar existencia de Sugerencias en BD
            if (request.SugerenciasIds != null && request.SugerenciasIds.Any())
            {
                var sugerenciasGuids = request.SugerenciasIds
                    .Select(id => Guid.TryParse(id, out var parsed) ? parsed : (Guid?)null)
                    .Where(g => g.HasValue)
                    .Select(g => g!.Value)
                    .Distinct()
                    .ToList();

                if (sugerenciasGuids.Any())
                {
                    // Consultar únicamente los IDs que realmente existen en la tabla serviciosclinicos
                    var sugerenciasExistentes = await _context.ServiciosClinicos
                        .AsNoTracking()
                        .Where(s => sugerenciasGuids.Contains(s.Id))
                        .Select(s => s.Id)
                        .ToListAsync(cancellationToken);

                    foreach (var sugeridoId in sugerenciasExistentes)
                    {
                        var sugerencia = new ServicioSugerencia(item.Id, sugeridoId);
                        _context.ServiciosSugerencias.Add(sugerencia);
                    }
                }
            }

            // 2. Guardar honorarios específicos por médico
            var honorarios = (request.HonorariosMedicos != null && request.HonorariosMedicos.Any()) 
                ? request.HonorariosMedicos 
                : request.HonorariosEspecificos;

            if (honorarios != null && honorarios.Any())
            {
                foreach (var h in honorarios.Where(h => h.Honorario > 0))
                {
                    var newHon = new HonorarioMedicoServicio(item.Id, h.MedicoId, h.Honorario, "Admin");
                    _context.HonorariosMedicosServicios.Add(newHon);
                }
            }

            // 3. Guardar receta BOM de insumos
            var recetaItems = (request.RecetaInsumos != null && request.RecetaInsumos.Any()) 
                ? request.RecetaInsumos 
                : request.Receta;

            if (recetaItems != null && recetaItems.Any())
            {
                foreach (var r in recetaItems)
                {
                    if (Enum.TryParse<SistemaSatHospitalario.Core.Domain.Enums.UnidadMedida>(r.UnidadMedidaConsumo, true, out var uom))
                    {
                        var receta = new ServicioInsumoReceta(item.Id, r.InsumoId, r.Cantidad, uom);
                        _context.ServiciosInsumoRecetas.Add(receta);
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return item.Id;
        }
    }
}