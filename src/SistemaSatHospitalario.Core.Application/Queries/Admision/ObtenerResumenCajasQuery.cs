using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    // DTOs para la Vista del Administrador
    public class ResumenCajaGlobalDto
    {
        public Guid CajaId { get; set; }
        public DateTime FechaApertura { get; set; }
        public decimal MontoInicialBase { get; set; }
        public decimal TotalRecaudadoBase { get; set; }
        public decimal GranTotalEnCajaBase { get; set; }
    }

    // Query
    public class ObtenerResumenCajasQuery : IRequest<ResumenCajaGlobalDto>
    {
    }

    // Handler
    public class ObtenerResumenCajasQueryHandler : IRequestHandler<ObtenerResumenCajasQuery, ResumenCajaGlobalDto>
    {
        private readonly ICajaAdministrativaRepository _cajaRepository;

        public ObtenerResumenCajasQueryHandler(ICajaAdministrativaRepository cajaRepository)
        {
            _cajaRepository = cajaRepository;
        }

        public async Task<ResumenCajaGlobalDto> Handle(ObtenerResumenCajasQuery request, CancellationToken cancellationToken)
        {
            var caja = await _cajaRepository.ObtenerCajaAbiertaConDetallesAsync(cancellationToken);

            if (caja == null)
            {
                throw new InvalidOperationException("No hay ninguna Caja Matriz abierta en este momento.");
            }

            // Nota: El repositorio debe ser actualizado para incluir los Recibos asociados directamente a la Caja
            // Por ahora mantenemos la estructura de cálculo base simplificada
            var dto = new ResumenCajaGlobalDto
            {
                CajaId = caja.Id,
                FechaApertura = caja.FechaApertura,
                MontoInicialBase = caja.MontoInicialDivisa,
                TotalRecaudadoBase = 0 // Esto se calculará sumando recibos de la caja en el repositorio
            };

            dto.GranTotalEnCajaBase = dto.MontoInicialBase + dto.TotalRecaudadoBase;
            return dto;
        }
    }
}
