using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;

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
                EsServicioInforme = request.EsServicioInforme || (request.TipoServicioId == TipoServicioConstants.Informe) || request.Tipo.Equals("INFORME", StringComparison.OrdinalIgnoreCase)
            };

            if (request.TipoServicioId.HasValue)
            {
                item.TipoServicioId = request.TipoServicioId.Value;
            }

            // Validar Invariantes de Dominio (ej: PrecioBase >= HonorarioBase para Informes)
            item.ValidarInvariantes();

            await _context.ServiciosClinicos.AddAsync(item, cancellationToken);
            
            if (request.SugerenciasIds != null && request.SugerenciasIds.Any())
            {
                foreach (var sugeridoId in request.SugerenciasIds)
                {
                    if (Guid.TryParse(sugeridoId, out var parsedId))
                    {
                        var sugerencia = new ServicioSugerencia(item.Id, parsedId);
                        _context.ServiciosSugerencias.Add(sugerencia);
                    }
                }
            }

            // Guardar honorarios específicos por médico
            var honorarios = (request.HonorariosMedicos != null && request.HonorariosMedicos.Any()) ? request.HonorariosMedicos : request.HonorariosEspecificos;
            if (honorarios != null && honorarios.Any())
            {
                foreach (var h in honorarios)
                {
                    if (h.Honorario > 0)
                    {
                        var newHon = new HonorarioMedicoServicio(item.Id, h.MedicoId, h.Honorario, "Admin");
                        _context.HonorariosMedicosServicios.Add(newHon);
                    }
                }
            }

            // Guardar receta BOM de insumos
            var recetaItems = (request.RecetaInsumos != null && request.RecetaInsumos.Any()) ? request.RecetaInsumos : request.Receta;
            if (recetaItems != null && recetaItems.Any())
            {
                foreach (var r in recetaItems)
                {
                    if (Enum.TryParse<SistemaSatHospitalario.Core.Domain.Enums.UnidadMedida>(r.UnidadMedidaConsumo, true, out var uom))
                    {
                        var receta = new ServicioInsumoReceta(item.Id, item.Codigo, r.InsumoId, r.Cantidad, uom);
                        _context.ServiciosInsumoRecetas.Add(receta);
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return item.Id;
        }
    }
}
