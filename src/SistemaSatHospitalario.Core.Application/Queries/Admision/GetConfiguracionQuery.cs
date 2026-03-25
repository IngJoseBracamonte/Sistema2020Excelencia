using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetConfiguracionQuery : IRequest<ConfiguracionGeneralDto?> { }

    public class GetConfiguracionQueryHandler : IRequestHandler<GetConfiguracionQuery, ConfiguracionGeneralDto?>
    {
        private readonly IApplicationDbContext _context;

        public GetConfiguracionQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ConfiguracionGeneralDto> Handle(GetConfiguracionQuery request, CancellationToken cancellationToken)
        {
            var config = await _context.ConfiguracionGeneral
                .OrderByDescending(c => c.UltimaActualizacion)
                .Select(c => new ConfiguracionGeneralDto
                {
                    Id = c.Id,
                    NombreEmpresa = c.NombreEmpresa,
                    Rif = c.Rif,
                    Iva = c.Iva,
                    UltimaActualizacion = c.UltimaActualizacion
                })
                .FirstOrDefaultAsync(cancellationToken);

            return config;
        }
    }
}
