using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetCajaSummariesQuery : IRequest<CajaSummaryDto>
    {
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public string? UsuarioId { get; set; }
    }

    public class CajaSummaryDto
    {
        public decimal GranTotalDivisa { get; set; }
        public decimal GranTotalBs { get; set; }
        public List<CajaDetailDto> Cierres { get; set; } = new();
    }

    public class CajaDetailDto
    {
        public Guid Id { get; set; }
        public string Usuario { get; set; }
        public DateTime Apertura { get; set; }
        public DateTime? Cierre { get; set; }
        public decimal MontoInicialDivisa { get; set; }
        public decimal MontoInicialBs { get; set; }
        public string Estado { get; set; }
    }

    public class GetCajaSummariesQueryHandler : IRequestHandler<GetCajaSummariesQuery, CajaSummaryDto>
    {
        private readonly ICajaAdministrativaRepository _repository;

        public GetCajaSummariesQueryHandler(ICajaAdministrativaRepository repository)
        {
            _repository = repository;
        }

        public async Task<CajaSummaryDto> Handle(GetCajaSummariesQuery request, CancellationToken cancellationToken)
        {
            var cierres = await _repository.ObtenerHistorialCierresAsync(request.Desde, request.Hasta, request.UsuarioId, cancellationToken);
            
            var list = cierres.Select(c => new CajaDetailDto
            {
                Id = c.Id,
                Usuario = c.NombreUsuario,
                Apertura = c.FechaApertura,
                Cierre = c.FechaCierre,
                MontoInicialDivisa = c.MontoInicialDivisa,
                MontoInicialBs = c.MontoInicialBs,
                Estado = c.Estado
            }).ToList();

            return new CajaSummaryDto
            {
                Cierres = list,
                GranTotalDivisa = list.Sum(x => x.MontoInicialDivisa), // Placeholder: En realidad sumaríamos los recibos asociados
                GranTotalBs = list.Sum(x => x.MontoInicialBs)
            };
        }
    }
}
