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
    public class CreateCatalogItemCommand : IRequest<Guid>
    {
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public decimal PrecioUsd { get; set; }
        public string Tipo { get; set; } // LABORATORIO, RX, CONSULTA, etc.
        public bool Activo { get; set; } = true;
        public decimal HonorarioBase { get; set; }
        public List<string> SugerenciasIds { get; set; } = new List<string>();
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
                HonorarioBase = request.HonorarioBase
            };

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

            await _context.SaveChangesAsync(cancellationToken);

            return item.Id;
        }
    }
}
