using System;
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
            var baseQuery = from ar in _context.CuentasPorCobrar.AsNoTracking()
                            join pac in _context.PacientesAdmision.AsNoTracking() on ar.PacienteId equals pac.Id
                            join cta in _context.CuentasServicios.AsNoTracking() on ar.CuentaServicioId equals cta.Id
                            join conv in _context.SegurosConvenios.AsNoTracking() on cta.ConvenioId equals (int?)conv.Id into convJoin
                            from conv in convJoin.DefaultIfEmpty()
                            join rf in _context.RecibosFactura.AsNoTracking() on cta.Id equals rf.CuentaServicioId into rfJoin
                            from rf in rfJoin.DefaultIfEmpty()
                            select new
                            {
                                ArId = ar.Id,
                                CuentaId = ar.CuentaServicioId,
                                ReciboId = rf != null ? (Guid?)rf.Id : null,
                                PacienteNombre = pac.NombreCorto,
                                PacienteCedula = pac.CedulaPasaporte,
                                TipoIngreso = cta.TipoIngreso,
                                SeguroNombre = conv != null ? conv.Nombre : EstadoConstants.Particular,
                                MontoTotal = ar.MontoTotalBase,
                                MontoPagadoBase = ar.MontoPagadoBase,
                                FechaEmision = ar.FechaCreacion,
                                Estado = ar.Estado,
                                IsAudited = ar.IsAudited || cta.ConvenioId == null,
                                QuienAutorizo = ar.QuienAutorizo,
                                DoctorProcedimiento = ar.DoctorProcedimiento,
                                InformacionAdicional = ar.InformacionAdicional,
                                CompromisoGenerado = ar.CompromisoGenerado,
                                GarantiaGenerada = ar.GarantiaGenerada,
                                FechaNacimiento = pac.FechaNacimiento,
                                TelefonoContact = pac.TelefonoContact,
                                ConvenioId = cta.ConvenioId
                            };

            if (!string.IsNullOrEmpty(request.Estado))
            {
                baseQuery = baseQuery.Where(ar => ar.Estado == request.Estado);
            }

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                baseQuery = baseQuery.Where(ar => ar.PacienteNombre.Contains(request.SearchTerm) || ar.PacienteCedula.Contains(request.SearchTerm));
            }

            if (request.StartDate.HasValue || request.EndDate.HasValue)
            {
                var start = request.StartDate?.Date ?? DateTime.MinValue;
                var end = request.EndDate?.Date.AddDays(1).AddTicks(-1) ?? DateTime.MaxValue;
                baseQuery = baseQuery.Where(ar => ar.FechaEmision >= start && ar.FechaEmision <= end);
            }

            if (request.SoloCompromiso.HasValue && request.SoloCompromiso.Value)
            {
                baseQuery = baseQuery.Where(ar => ar.CompromisoGenerado && ar.SeguroNombre == EstadoConstants.Particular);
            }

            var rawList = await baseQuery.OrderByDescending(ar => ar.FechaEmision).ToListAsync(cancellationToken);

            if (!rawList.Any())
            {
                return new List<PendingARDto>();
            }

            var cuentaIds = rawList.Select(r => r.CuentaId).Distinct().ToList();

            var conceptos = await _context.DetallesServicioCuenta
                .AsNoTracking()
                .Where(d => cuentaIds.Contains(d.CuentaServicioId))
                .Select(d => new
                {
                    d.CuentaServicioId,
                    d.Descripcion,
                    MontoBase = d.Precio * d.Cantidad
                })
                .ToListAsync(cancellationToken);

            var recibos = await _context.RecibosFactura
                .AsNoTracking()
                .Where(r => cuentaIds.Contains(r.CuentaServicioId))
                .Select(r => new { r.Id, r.CuentaServicioId, r.FechaEmision })
                .ToListAsync(cancellationToken);

            var reciboIds = recibos.Select(r => r.Id).Distinct().ToList();

            var pagos = await _context.DetallesPago
                .AsNoTracking()
                .Where(dp => reciboIds.Contains(dp.ReciboFacturaId))
                .Select(dp => new
                {
                    dp.ReciboFacturaId,
                    dp.MetodoPago,
                    dp.ReferenciaBancaria,
                    dp.EquivalenteAbonadoBase,
                    dp.MontoAbonadoMoneda
                })
                .ToListAsync(cancellationToken);

            var result = rawList.Select(ar => new PendingARDto
            {
                Id = ar.ArId,
                CuentaId = ar.CuentaId,
                ReciboId = ar.ReciboId,
                PacienteNombre = ar.PacienteNombre,
                PacienteCedula = ar.PacienteCedula,
                TipoIngreso = ar.TipoIngreso,
                SeguroNombre = ar.SeguroNombre,
                MontoTotal = ar.MontoTotal,
                SaldoPendiente = ar.MontoTotal - ar.MontoPagadoBase,
                FechaEmision = ar.FechaEmision,
                Estado = ar.Estado,
                IsAudited = ar.IsAudited,
                QuienAutorizo = ar.QuienAutorizo,
                DoctorProcedimiento = ar.DoctorProcedimiento,
                InformacionAdicional = ar.InformacionAdicional,
                CompromisoGenerado = ar.CompromisoGenerado,
                GarantiaGenerada = ar.GarantiaGenerada,
                FechaNacimiento = ar.FechaNacimiento,
                TelefonoContact = ar.TelefonoContact,
                Conceptos = conceptos
                    .Where(c => c.CuentaServicioId == ar.CuentaId)
                    .Select(c => new ConceptoFacturadoDto
                    {
                        Descripcion = c.Descripcion,
                        MontoBase = c.MontoBase
                    })
                    .ToList(),
                Pagos = (from dp in pagos
                         join r in recibos on dp.ReciboFacturaId equals r.Id
                         where r.CuentaServicioId == ar.CuentaId
                         select new PaymentHistoryDto
                         {
                             Fecha = r.FechaEmision,
                             Metodo = dp.MetodoPago,
                             Referencia = dp.ReferenciaBancaria,
                             MontoBase = dp.EquivalenteAbonadoBase,
                             MontoCambiario = dp.MontoAbonadoMoneda
                         }).ToList()
            }).ToList();

            return result;
        }
    }
}
