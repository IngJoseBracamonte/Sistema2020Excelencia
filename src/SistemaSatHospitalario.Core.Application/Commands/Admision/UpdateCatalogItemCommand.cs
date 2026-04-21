using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class UpdateCatalogItemCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public decimal PrecioUsd { get; set; }
        public string Tipo { get; set; }
        public bool Activo { get; set; }
        public decimal HonorarioBase { get; set; }
        public List<string> SugerenciasIds { get; set; } = new List<string>();
    }

    public class UpdateCatalogItemCommandHandler : IRequestHandler<UpdateCatalogItemCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCatalogItemCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateCatalogItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _context.ServiciosClinicos
                .Include(s => s.Sugerencias)
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
                
            if (item == null) return false;

            item.Descripcion = request.Descripcion;
            item.PrecioBase = request.PrecioUsd;
            item.HonorarioBase = request.HonorarioBase;
            item.TipoServicio = request.Tipo;
            item.Codigo = request.Codigo;
            item.Activo = request.Activo;

            // Actualizar sugerencias
            _context.ServiciosSugerencias.RemoveRange(item.Sugerencias);
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

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
