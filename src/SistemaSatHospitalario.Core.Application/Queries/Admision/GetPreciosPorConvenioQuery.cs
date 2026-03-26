using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetPreciosPorConvenioQuery : IRequest<List<ConvenioPerfilPrecioDto>>
    {
        public int ConvenioId { get; set; }

        public GetPreciosPorConvenioQuery(int convenioId)
        {
            ConvenioId = convenioId;
        }
    }

    public class GetPreciosPorConvenioQueryHandler : IRequestHandler<GetPreciosPorConvenioQuery, List<ConvenioPerfilPrecioDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILegacyLabRepository _legacyRepository;

        public GetPreciosPorConvenioQueryHandler(IApplicationDbContext context, ILegacyLabRepository legacyRepository)
        {
            _context = context;
            _legacyRepository = legacyRepository;
        }

        public async Task<List<ConvenioPerfilPrecioDto>> Handle(GetPreciosPorConvenioQuery request, CancellationToken cancellationToken)
        {
            // 1. Obtener todos los perfiles disponibles del sistema legacy
            var perfilesLegacy = await _legacyRepository.GetAvailableProfilesAsync(cancellationToken);

            // 2. Obtener los precios personalizados registrados en el nuevo sistema para este convenio
            var preciosPersonalizados = await _context.ConvenioPerfilPrecios
                .Where(c => c.SeguroConvenioId == request.ConvenioId && c.Activo)
                .ToListAsync(cancellationToken);

            // 3. Cruzar y construir DTOs
            var result = perfilesLegacy.Select(p =>
            {
                var personalizado = preciosPersonalizados.FirstOrDefault(cp => cp.PerfilId == p.IdPerfil);
                
                return new ConvenioPerfilPrecioDto
                {
                    PerfilId = p.IdPerfil,
                    NombrePerfil = p.Descripcion,
                    // De momento asumimos que 'PrecioDOlar' en legacy es el base USD
                    // Si el usuario indicó 'Precio' decimal, ajustaremos en el repositorio si falta.
                    PrecioBaseUSD = p.PrecioDOlar, 
                    PrecioBaseHNL = 0, // El usuario no especificó el campo HNL en legacy aún, por defecto 0 o calculado
                    
                    PrecioHNL = personalizado?.PrecioHNL,
                    PrecioUSD = personalizado?.PrecioUSD
                };
            }).ToList();

            return result;
        }
    }
}
