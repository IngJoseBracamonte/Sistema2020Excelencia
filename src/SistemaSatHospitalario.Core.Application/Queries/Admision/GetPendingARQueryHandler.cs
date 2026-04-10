using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetPendingARQueryHandler : IRequestHandler<GetPendingARQuery, List<PendingARDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetPendingARQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PendingARDto>> Handle(GetPendingARQuery request, CancellationToken cancellationToken)
        {
            var query = from ar in _context.CuentasPorCobrar.AsNoTracking()
                        join pac in _context.PacientesAdmision.AsNoTracking() on ar.PacienteId equals pac.Id
                        join cta in _context.CuentasServicios.AsNoTracking() on ar.CuentaServicioId equals cta.Id
                        // Ajuste de tipos Guid? a int? para el join de Convenios
                        join conv in _context.SegurosConvenios.AsNoTracking() on cta.ConvenioId equals (int?)conv.Id into convJoin
                        from conv in convJoin.DefaultIfEmpty()
                        select new PendingARDto
                        {
                            Id = ar.Id,
                            CuentaId = ar.CuentaServicioId,
                            PacienteNombre = pac.NombreCorto,
                            PacienteCedula = pac.CedulaPasaporte,
                            TipoIngreso = cta.TipoIngreso,
                            SeguroNombre = conv != null ? conv.Nombre : EstadoConstants.Particular,
                            MontoTotal = ar.MontoTotalBase,
                            SaldoPendiente = ar.MontoTotalBase - ar.MontoPagadoBase,
                            FechaEmision = ar.FechaCreacion,
                            Estado = ar.Estado,
                            IsAudited = ar.IsAudited,
                            Conceptos = _context.DetallesServicioCuenta
                                .Where(d => d.CuentaServicioId == ar.CuentaServicioId)
                                .Select(d => new ConceptoFacturadoDto
                                {
                                    Descripcion = d.Descripcion,
                                    MontoBase = d.Precio * d.Cantidad
                                }).ToList(),
                            Pagos = _context.DetallesPago
                                .Where(dp => _context.RecibosFactura
                                    .Where(rf => rf.CuentaServicioId == ar.CuentaServicioId)
                                    .Select(rf => rf.Id)
                                    .Contains(dp.ReciboFacturaId))
                                .Select(dp => new PaymentHistoryDto
                                {
                                    Fecha = _context.RecibosFactura.First(r => r.Id == dp.ReciboFacturaId).FechaEmision,
                                    Metodo = dp.MetodoPago,
                                    Referencia = dp.ReferenciaBancaria,
                                    MontoBase = dp.EquivalenteAbonadoBase,
                                    MontoCambiario = dp.MontoAbonadoMoneda
                                }).ToList()
                        };

            if (!string.IsNullOrEmpty(request.Estado))
            {
                query = query.Where(ar => ar.Estado == request.Estado);
            }

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(ar => ar.PacienteNombre.Contains(request.SearchTerm) || ar.PacienteCedula.Contains(request.SearchTerm));
            }

            if (request.StartDate.HasValue)
            {
                var start = request.StartDate.Value.Date;
                query = query.Where(ar => ar.FechaEmision >= start);
            }

            if (request.EndDate.HasValue)
            {
                var end = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(ar => ar.FechaEmision <= end);
            }

            return await query.OrderByDescending(ar => ar.FechaEmision).ToListAsync(cancellationToken);
        }
    }
}
