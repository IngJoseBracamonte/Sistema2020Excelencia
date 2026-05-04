using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class ExportFinancialReportQuery : IRequest<byte[]>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsAuditMode { get; set; }
    }

    public class ExportFinancialReportQueryHandler : IRequestHandler<ExportFinancialReportQuery, byte[]>
    {
        private readonly IApplicationDbContext _context;
        private readonly IExcelService _excelService;
        private readonly IIdentityService _identityService;

        public ExportFinancialReportQueryHandler(IApplicationDbContext context, IExcelService excelService, IIdentityService identityService)
        {
            _context = context;
            _excelService = excelService;
            _identityService = identityService;
        }

        public async Task<byte[]> Handle(ExportFinancialReportQuery request, CancellationToken cancellationToken)
        {
            var start = request.StartDate?.Date ?? DateTime.MinValue;
            var end = request.EndDate?.Date.AddDays(1).AddTicks(-1) ?? DateTime.MaxValue;

            var userMap = (await _identityService.GetUsersAsync()).ToDictionary(u => u.Id.ToString(), u => u.FullName);

            var query = from ar in _context.CuentasPorCobrar.AsNoTracking()
                        join pac in _context.PacientesAdmision.AsNoTracking() on ar.PacienteId equals pac.Id
                        join cta in _context.CuentasServicios.AsNoTracking() on ar.CuentaServicioId equals cta.Id
                        where ar.FechaCreacion >= start && ar.FechaCreacion <= end
                        select new
                        {
                            ar.Id,
                            ar.FechaCreacion,
                            ar.MontoTotalBase,
                            ar.MontoPagadoBase,
                            ar.Estado,
                            ar.IsAudited,
                            ar.UsuarioAuditoria,
                            ar.FechaAuditoria,
                            PacienteNombre = pac.NombreCorto,
                            PacienteCedula = pac.CedulaPasaporte,
                            CuentaId = ar.CuentaServicioId
                        };

            var data = await query.OrderByDescending(x => x.FechaCreacion).ToListAsync(cancellationToken);

            // Fetch details and payments in memory for complex report
            var accountIds = data.Select(x => x.CuentaId).ToList();
            
            var details = await _context.DetallesServicioCuenta
                .Where(d => accountIds.Contains(d.CuentaServicioId))
                .ToListAsync(cancellationToken);

            var payments = await _context.DetallesPago
                .Include(dp => dp.ReciboFactura)
                .Where(dp => accountIds.Contains(dp.ReciboFactura.CuentaServicioId))
                .ToListAsync(cancellationToken);

            var reportData = data.Select(x => {
                var firstDetail = details.FirstOrDefault(d => d.CuentaServicioId == x.CuentaId);
                
                return new {
                    FechaEmision = x.FechaCreacion,
                    x.PacienteNombre,
                    x.PacienteCedula,
                    MontoTotal = x.MontoTotalBase,
                    SaldoPendiente = x.MontoTotalBase - x.MontoPagadoBase,
                    x.Estado,
                    x.IsAudited,
                    x.FechaCreacion,
                    UsuarioIngreso = firstDetail != null ? (userMap.ContainsKey(firstDetail.UsuarioCarga) ? userMap[firstDetail.UsuarioCarga] : firstDetail.UsuarioCarga) : "SISTEMA",
                    UsuarioAuditoria = !string.IsNullOrEmpty(x.UsuarioAuditoria) && userMap.ContainsKey(x.UsuarioAuditoria) ? userMap[x.UsuarioAuditoria] : x.UsuarioAuditoria,
                    x.FechaAuditoria,
                    Pagos = payments.Where(p => p.ReciboFactura.CuentaServicioId == x.CuentaId)
                                    .Select(p => new {
                                        Metodo = p.MetodoPago,
                                        Referencia = p.ReferenciaBancaria,
                                        MontoBase = p.EquivalenteAbonadoBase
                                    }).ToList()
                };
            }).ToList();

            return _excelService.GenerateFinancialReport(reportData, request.IsAuditMode);
        }
    }
}
