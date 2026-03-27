using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetReciboPdfQuery : IRequest<ReciboPdfDto>
    {
        public Guid ReciboId { get; set; }
    }

    public class GetReciboPdfQueryHandler : IRequestHandler<GetReciboPdfQuery, ReciboPdfDto>
    {
        private readonly IApplicationDbContext _context;

        public GetReciboPdfQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReciboPdfDto> Handle(GetReciboPdfQuery request, CancellationToken cancellationToken)
        {
            var recibo = await _context.RecibosFactura
                .Include(r => r.DetallesPago)
                .FirstOrDefaultAsync(r => r.Id == request.ReciboId, cancellationToken);

            if (recibo == null) return null;

            var cuenta = await _context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == recibo.CuentaServicioId, cancellationToken);

            var paciente = await _context.PacientesAdmision
                .FirstOrDefaultAsync(p => p.Id == recibo.PacienteId, cancellationToken);

            return new ReciboPdfDto
            {
                Id = recibo.Id,
                NumeroRecibo = recibo.NumeroRecibo,
                FechaEmision = recibo.FechaEmision,
                PacienteNombre = recibo.PacienteId == Guid.Empty ? EstadoConstants.Particular : (paciente?.NombreCorto ?? EstadoConstants.Desconocido),
                PacienteCedula = paciente?.CedulaPasaporte ?? EstadoConstants.Desconocido,
                TipoIngreso = cuenta?.TipoIngreso ?? EstadoConstants.Particular,
                TotalUSD = recibo.TotalFacturadoUSD,
                TasaBcv = recibo.TasaBcvUsada,
                TotalBS = recibo.TotalFacturadoUSD * recibo.TasaBcvUsada,
                Detalles = cuenta?.Detalles.Select(d => new ReciboDetallePdfDto
                {
                    Descripcion = d.Descripcion,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.Precio,
                    Subtotal = d.Precio * d.Cantidad
                }).ToList() ?? new List<ReciboDetallePdfDto>(),
                Pagos = recibo.DetallesPago.Select(p => new PagoDetallePdfDto
                {
                    MetodoPago = p.MetodoPago,
                    MontoOriginal = p.MontoAbonadoMoneda,
                    EquivalenteBase = p.EquivalenteAbonadoBase,
                    Referencia = p.ReferenciaBancaria
                }).ToList()
            };
        }
    }
}
