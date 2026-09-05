using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using System;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SistemaSatHospitalario.WebAPI.Controllers.System
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("financial")]
        public async Task<IActionResult> ExportFinancial([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] bool auditMode = false)
        {
            // Solo Administradores pueden ver el modo auditoría
            bool canSeeAudit = User.IsInRole(AuthorizationConstants.Admin) || User.IsInRole(AuthorizationConstants.Administrador);
            bool finalAuditMode = auditMode && canSeeAudit;

            var query = new ExportFinancialReportQuery 
            { 
                StartDate = startDate, 
                EndDate = endDate, 
                IsAuditMode = finalAuditMode 
            };

            var fileBytes = await _mediator.Send(query);
            string fileName = $"Reporte_Financiero_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("cash-closing")]
        public async Task<IActionResult> ExportCashClosing([FromQuery] string? userId, [FromQuery] DateTime? date, [FromQuery] bool auditMode = false)
        {
            // Un cajero solo puede ver su propia caja a menos que sea Admin
            string? targetUserId = userId;
            bool isAdmin = User.IsInRole(AuthorizationConstants.Admin) || User.IsInRole(AuthorizationConstants.Administrador);
            
            if (!isAdmin)
            {
                targetUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                auditMode = false; // Forzar modo personal para no-admins
            }

            var query = new ExportCashierAuditQuery 
            { 
                UserId = targetUserId, 
                Date = date ?? DateTime.Today,
                IsAuditMode = auditMode && isAdmin
            };

            var fileBytes = await _mediator.Send(query);
            string userLabel = string.IsNullOrEmpty(targetUserId) ? "Global" : "Cajero";
            string fileName = $"Cierre_Caja_{userLabel}_{DateTime.Now:yyyyMMdd}.xlsx";

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("appointments")]
        public async Task<IActionResult> ExportAppointments([FromQuery] DateTime? date, [FromQuery] bool auditMode = false)
        {
            return await ExportGeneric(ReportType.Appointments, date, date, auditMode, "Reporte_Citas");
        }

        [HttpGet("diagnostics")]
        public async Task<IActionResult> ExportDiagnostics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] bool auditMode = false)
        {
            return await ExportGeneric(ReportType.Diagnostics, startDate, endDate, auditMode, "Reporte_Estudios");
        }

        [HttpGet("billing-audit")]
        public async Task<IActionResult> ExportBilling([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] bool auditMode = false)
        {
            return await ExportGeneric(ReportType.Billing, startDate, endDate, auditMode, "Reporte_Facturacion");
        }

        [HttpGet("honorariums")]
        public async Task<IActionResult> ExportHonorariums([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] bool auditMode = false)
        {
            return await ExportGeneric(ReportType.Honorariums, startDate, endDate, auditMode, "Reporte_Honorarios");
        }

        [HttpGet("patients")]
        public async Task<IActionResult> ExportPatients([FromQuery] bool auditMode = false)
        {
            return await ExportGeneric(ReportType.Patients, null, null, auditMode, "Reporte_Pacientes");
        }

        private async Task<IActionResult> ExportGeneric(ReportType type, DateTime? start, DateTime? end, bool auditMode, string baseName)
        {
            bool canSeeAudit = User.IsInRole(AuthorizationConstants.Admin) || User.IsInRole(AuthorizationConstants.Administrador);
            var query = new ExportGenericListQuery 
            { 
                Type = type, 
                StartDate = start, 
                EndDate = end, 
                IsAuditMode = auditMode && canSeeAudit 
            };

            var fileBytes = await _mediator.Send(query);
            string fileName = $"{baseName}_{DateTime.Now:yyyyMMdd}.xlsx";

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
