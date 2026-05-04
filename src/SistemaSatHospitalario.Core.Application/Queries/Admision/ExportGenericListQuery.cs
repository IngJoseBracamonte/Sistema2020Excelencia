using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public enum ReportType
    {
        Appointments,
        Diagnostics,
        Billing,
        Honorariums,
        Patients
    }

    public class ExportGenericListQuery : IRequest<byte[]>
    {
        public ReportType Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsAuditMode { get; set; }
    }

    public class ExportGenericListQueryHandler : IRequestHandler<ExportGenericListQuery, byte[]>
    {
        private readonly IApplicationDbContext _context;
        private readonly IExcelService _excelService;
        private readonly IIdentityService _identityService;

        public ExportGenericListQueryHandler(IApplicationDbContext context, IExcelService excelService, IIdentityService identityService)
        {
            _context = context;
            _excelService = excelService;
            _identityService = identityService;
        }

        public async Task<byte[]> Handle(ExportGenericListQuery request, CancellationToken cancellationToken)
        {
            DataTable dt = new DataTable();
            string sheetName = "Reporte";

            switch (request.Type)
            {
                case ReportType.Appointments:
                    dt = await GetAppointmentsReport(request, cancellationToken);
                    sheetName = "Control de Citas";
                    break;
                case ReportType.Diagnostics:
                    dt = await GetDiagnosticsReport(request, cancellationToken);
                    sheetName = "Ordenes de Estudios";
                    break;
                case ReportType.Billing:
                    dt = await GetBillingReport(request, cancellationToken);
                    sheetName = "Expediente Facturacion";
                    break;
                case ReportType.Honorariums:
                    dt = await GetHonorariumsReport(request, cancellationToken);
                    sheetName = "Calculo Honorarios";
                    break;
                case ReportType.Patients:
                    dt = await GetPatientsReport(request, cancellationToken);
                    sheetName = "Expedientes Pacientes";
                    break;
            }

            return _excelService.GenerateExcel(dt, sheetName);
        }

        private async Task<DataTable> GetAppointmentsReport(ExportGenericListQuery req, CancellationToken ct)
        {
            var date = req.StartDate?.Date ?? DateTime.Today;
            var query = _context.CitasMedicas
                .Include(c => c.Medico)
                    .ThenInclude(m => m.Especialidad)
                .Include(c => c.CuentaServicio)
                    .ThenInclude(cs => cs.Paciente)
                .Where(c => c.HoraPautada.Date == date);

            var list = await query.OrderBy(c => c.HoraPautada).ToListAsync(ct);
            
            DataTable dt = new DataTable();
            dt.Columns.Add("HORA", typeof(string));
            dt.Columns.Add("PACIENTE", typeof(string));
            dt.Columns.Add("MEDICO", typeof(string));
            dt.Columns.Add("ESPECIALIDAD", typeof(string));
            dt.Columns.Add("ESTADO", typeof(string));
            
            if (req.IsAuditMode)
            {
                dt.Columns.Add("REGISTRADO EL", typeof(DateTime));
                dt.Columns.Add("OBSERVACIONES", typeof(string));
            }

            foreach (var c in list)
            {
                dt.Rows.Add(
                    c.HoraPautada.ToString("HH:mm"),
                    c.CuentaServicio.Paciente.NombreCorto,
                    c.Medico.Nombre,
                    c.Medico.Especialidad.Nombre,
                    c.Estado,
                    req.IsAuditMode ? (object)c.FechaRegistro : null,
                    req.IsAuditMode ? c.Comentario : null
                );
            }
            return dt;
        }

        private async Task<DataTable> GetDiagnosticsReport(ExportGenericListQuery req, CancellationToken ct)
        {
            var start = req.StartDate?.Date ?? DateTime.Today;
            var end = req.EndDate?.Date.AddDays(1).AddTicks(-1) ?? DateTime.MaxValue;

            var list = await (from d in _context.DetallesServicioCuenta.AsNoTracking()
                              join c in _context.CuentasServicios.AsNoTracking() on d.CuentaServicioId equals c.Id
                              join p in _context.PacientesAdmision.AsNoTracking() on c.PacienteId equals p.Id
                              where (d.TipoServicio == "RX" || d.TipoServicio == "TOMOGRAFIA" || d.TipoServicio == "ESTUDIO")
                                 && d.FechaCarga >= start && d.FechaCarga <= end
                              select new { d, p }).OrderByDescending(x => x.d.FechaCarga).ToListAsync(ct);

            DataTable dt = new DataTable();
            dt.Columns.Add("FECHA", typeof(DateTime));
            dt.Columns.Add("TIPO", typeof(string));
            dt.Columns.Add("PACIENTE", typeof(string));
            dt.Columns.Add("ESTUDIO", typeof(string));
            dt.Columns.Add("ESTADO", typeof(string));

            if (req.IsAuditMode)
            {
                dt.Columns.Add("TECNICO", typeof(string));
                dt.Columns.Add("FECHA VALIDACION", typeof(DateTime));
                dt.Columns.Add("USUARIO CARGA", typeof(string));
            }

            foreach (var item in list)
            {
                dt.Rows.Add(
                    item.d.FechaCarga,
                    item.d.TipoServicio,
                    item.p.NombreCorto,
                    item.d.Descripcion,
                    item.d.Realizado ? "PROCESADO" : "PENDIENTE",
                    req.IsAuditMode ? item.d.UsuarioTecnico : null,
                    req.IsAuditMode ? (object?)item.d.FechaRealizacion : null,
                    req.IsAuditMode ? item.d.UsuarioCarga : null
                );
            }
            return dt;
        }

        private async Task<DataTable> GetBillingReport(ExportGenericListQuery req, CancellationToken ct)
        {
            var start = req.StartDate?.Date ?? DateTime.Today;
            var end = req.EndDate?.Date.AddDays(1).AddTicks(-1) ?? DateTime.MaxValue;

            var list = await (from d in _context.DetallesServicioCuenta.AsNoTracking()
                              join c in _context.CuentasServicios.AsNoTracking() on d.CuentaServicioId equals c.Id
                              join p in _context.PacientesAdmision.AsNoTracking() on c.PacienteId equals p.Id
                              where d.FechaCarga >= start && d.FechaCarga <= end
                              select new { d, c, p }).OrderByDescending(x => x.d.FechaCarga).ToListAsync(ct);

            DataTable dt = new DataTable();
            dt.Columns.Add("FECHA", typeof(DateTime));
            dt.Columns.Add("PACIENTE", typeof(string));
            dt.Columns.Add("ESTUDIO/SERVICIO", typeof(string));
            dt.Columns.Add("MONTO USD", typeof(decimal));
            dt.Columns.Add("ESTADO", typeof(string));

            if (req.IsAuditMode)
            {
                dt.Columns.Add("USUARIO CARGA", typeof(string));
            }

            foreach (var item in list)
            {
                dt.Rows.Add(item.d.FechaCarga, item.p.NombreCorto, item.d.Descripcion, item.d.Precio * item.d.Cantidad, item.c.Estado, req.IsAuditMode ? item.d.UsuarioCarga : null);
            }
            return dt;
        }

        private async Task<DataTable> GetHonorariumsReport(ExportGenericListQuery req, CancellationToken ct)
        {
            var start = req.StartDate?.Date ?? DateTime.Today;
            var end = req.EndDate?.Date.AddDays(1).AddTicks(-1) ?? DateTime.MaxValue;

            var summary = await (from cita in _context.CitasMedicas
                                 join cs in _context.CuentasServicios on cita.CuentaServicioId equals cs.Id
                                 join detail in _context.DetallesServicioCuenta on cs.Id equals detail.CuentaServicioId
                                 where cs.Estado == "Facturada"
                                    && cs.FechaCierre >= start 
                                    && cs.FechaCierre <= end
                                    && (detail.TipoServicio == "Medico" || detail.TipoServicio == "CONSULTA")
                                 group detail by new { cita.Medico.Nombre } into g
                                 select new
                                 {
                                     Medico = g.Key.Nombre,
                                     Cantidad = g.Count(),
                                     Total = g.Sum(x => x.Honorario * x.Cantidad)
                                 }).ToListAsync(ct);

            DataTable dt = new DataTable();
            dt.Columns.Add("MEDICO", typeof(string));
            dt.Columns.Add("CANTIDAD SERVICIOS", typeof(int));
            dt.Columns.Add("TOTAL HONORARIOS USD", typeof(decimal));

            foreach (var s in summary)
            {
                dt.Rows.Add(s.Medico, s.Cantidad, s.Total);
            }
            return dt;
        }

        private async Task<DataTable> GetPatientsReport(ExportGenericListQuery req, CancellationToken ct)
        {
            var list = await _context.PacientesAdmision.AsNoTracking().OrderBy(p => p.NombreCorto).ToListAsync(ct);
            
            DataTable dt = new DataTable();
            dt.Columns.Add("PACIENTE", typeof(string));
            dt.Columns.Add("CEDULA/PASAPORTE", typeof(string));
            dt.Columns.Add("TELEFONO", typeof(string));
            
            foreach (var p in list)
            {
                dt.Rows.Add(p.NombreCorto, p.CedulaPasaporte, p.TelefonoContact);
            }
            return dt;
        }
    }
}
