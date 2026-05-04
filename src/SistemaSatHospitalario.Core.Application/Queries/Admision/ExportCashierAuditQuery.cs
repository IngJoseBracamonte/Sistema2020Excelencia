using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class ExportCashierAuditQuery : IRequest<byte[]>
    {
        public string? UserId { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        public bool IsAuditMode { get; set; }
    }

    public class ExportCashierAuditQueryHandler : IRequestHandler<ExportCashierAuditQuery, byte[]>
    {
        private readonly IApplicationDbContext _context;
        private readonly IExcelService _excelService;
        private readonly IIdentityService _identityService;

        public ExportCashierAuditQueryHandler(IApplicationDbContext context, IExcelService excelService, IIdentityService identityService)
        {
            _context = context;
            _excelService = excelService;
            _identityService = identityService;
        }

        public async Task<byte[]> Handle(ExportCashierAuditQuery request, CancellationToken cancellationToken)
        {
            var start = request.Date.Date;
            var end = start.AddDays(1).AddTicks(-1);

            var userMap = (await _identityService.GetUsersAsync()).ToDictionary(u => u.Id.ToString(), u => u.FullName);
            string userFilterName = request.UserId != null && userMap.ContainsKey(request.UserId) ? userMap[request.UserId] : "Global";

            // 1. Lo Facturado (Basado en Recibos)
            var recibosQuery = _context.RecibosFactura
                .Include(r => r.DetallesPago)
                .Include(r => r.CuentaServicio)
                    .ThenInclude(cs => cs.Paciente)
                .Where(r => r.FechaEmision >= start && r.FechaEmision <= end && r.EstadoFiscal != EstadoConstants.Anulada);

            if (!string.IsNullOrEmpty(request.UserId))
            {
                // Un poco complejo porque el UsuarioId está en la CajaDiaria vinculada al recibo
                var cajasDelUsuario = await _context.CajasDiarias
                    .Where(c => c.UsuarioId == request.UserId)
                    .Select(c => c.Id)
                    .ToListAsync(cancellationToken);
                
                recibosQuery = recibosQuery.Where(r => r.CajaDiariaId.HasValue && cajasDelUsuario.Contains(r.CajaDiariaId.Value));
            }

            var recibos = await recibosQuery.ToListAsync(cancellationToken);

            // 2. Lo Pendiente (Cargado por el usuario pero no facturado/cobrado)
            var pendientesQuery = from d in _context.DetallesServicioCuenta.AsNoTracking()
                                  join c in _context.CuentasServicios.AsNoTracking() on d.CuentaServicioId equals c.Id
                                  join p in _context.PacientesAdmision.AsNoTracking() on c.PacienteId equals p.Id
                                  where d.FechaCarga >= start && d.FechaCarga <= end
                                     && c.Estado != EstadoConstants.Facturada
                                  select new { d, p };

            if (!string.IsNullOrEmpty(request.UserId))
            {
                pendientesQuery = pendientesQuery.Where(x => x.d.UsuarioCarga == request.UserId);
            }

            var pendientes = await pendientesQuery.ToListAsync(cancellationToken);

            // CONSTRUCCIÓN DEL DATATABLE PARA EXCEL
            DataTable dt = new DataTable();
            dt.TableName = $"Cierre Caja - {userFilterName}";
            dt.Columns.Add("TIPO", typeof(string));
            dt.Columns.Add("FECHA/HORA", typeof(string));
            dt.Columns.Add("PACIENTE", typeof(string));
            dt.Columns.Add("CONCEPTO", typeof(string));
            dt.Columns.Add("MONTO USD", typeof(decimal));
            dt.Columns.Add("ESTADO", typeof(string));

            if (request.IsAuditMode)
            {
                dt.Columns.Add("USUARIO", typeof(string));
                dt.Columns.Add("METODO PAGO", typeof(string));
            }

            // Agregar Facturados
            foreach (var r in recibos)
            {
                dt.Rows.Add(
                    "FACTURADO",
                    r.FechaEmision.ToString("HH:mm"),
                    r.CuentaServicio.Paciente.NombreCorto,
                    $"Recibo: {r.NumeroRecibo}",
                    r.TotalFacturadoUSD,
                    "COBRADO",
                    request.IsAuditMode ? (userMap.ContainsKey(request.UserId ?? "") ? userMap[request.UserId!] : "SISTEMA") : null,
                    request.IsAuditMode ? string.Join(", ", r.DetallesPago.Select(p => p.MetodoPago).Distinct()) : null
                );
            }

            // Agregar Pendientes
            foreach (var p in pendientes)
            {
                dt.Rows.Add(
                    "PENDIENTE",
                    p.d.FechaCarga.ToString("HH:mm"),
                    p.p.NombreCorto,
                    p.d.Descripcion,
                    p.d.Precio * p.d.Cantidad,
                    "POR COBRAR",
                    request.IsAuditMode ? (userMap.ContainsKey(p.d.UsuarioCarga) ? userMap[p.d.UsuarioCarga] : p.d.UsuarioCarga) : null,
                    request.IsAuditMode ? "-" : null
                );
            }

            return _excelService.GenerateExcel(dt, "Auditoria de Caja");
        }
    }
}
