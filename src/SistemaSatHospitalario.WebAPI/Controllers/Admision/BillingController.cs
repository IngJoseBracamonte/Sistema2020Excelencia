using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using SistemaSatHospitalario.Core.Application.Commands;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.WebAPI.Infrastructure.Security;

using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize(Roles = AuthorizationConstants.AdminRoles + "," + AuthorizationConstants.AsistenteParticular + "," + AuthorizationConstants.AsistenteSeguro + "," + AuthorizationConstants.AsistenteDeSeguros + "," + AuthorizationConstants.AsistenteRX + "," + AuthorizationConstants.Medico + "," + AuthorizationConstants.Cajero + "," + AuthorizationConstants.AsistenteHospitalario + "," + AuthorizationConstants.AsistenteEmergencia)]
    [ApiController]
    [Route("api/[controller]")]
    public class BillingController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BillingController> _logger;
 
        public BillingController(IMediator mediator, ILogger<BillingController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("CargarServicio")]
        [Idempotent]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CargarServicio([FromBody] CargarServicioACuentaCommand command)
        {
            try
            {
                // Enriquecimiento de Seguridad (V2.0 Core Extensions)
                command.UsuarioCarga = User.GetUserName();
                command.IsPrivilegedUser = User.IsPrivileged();

                var result = await _mediator.Send(command);
                return Ok(new { 
                    Message = "Servicio cargado exitosamente.", 
                    CuentaId = result.CuentaId,
                    DetalleId = result.DetalleId
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validación de negocio fallida en CargarServicio");
                return BadRequest(new { Error = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al cargar servicio");
                return BadRequest(new { Error = "Error de integridad de datos. Verifique que los IDs de paciente y servicio sean correctos.", Details = ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en CargarServicio");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Ha ocurrido un error inesperado al procesar el servicio." });
            }
        }

        [HttpPost("CargarServiciosMasivo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CargarServiciosMasivo([FromBody] CargarServiciosMasivoCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(new { 
                    Message = "Servicios cargados masivamente con éxito.", 
                    Resultados = result
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validación de negocio fallida en CargarServiciosMasivo");
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en CargarServiciosMasivo");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = ex.Message });
            }
        }

        [HttpPost("AbrirCuenta")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AbrirCuenta([FromBody] AbrirCuentaClinicaCommand command)
        {
            try
            {
                command.UsuarioCarga = User.GetUserName();
                var accountId = await _mediator.Send(command);
                return Ok(new { 
                    Message = "Cuenta clínica abierta exitosamente.", 
                    CuentaId = accountId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
        
        [HttpPost("SincronizarCarrito")]
        [Idempotent]
        [ActionName("SincronizarCarrito")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SincronizarCarrito([FromBody] SyncCarritoCommand command)
        {
            try
            {
                // Enriquecimiento de Seguridad (V2.0 Core Extensions)
                command.UsuarioCarga = User.GetUserName();
                command.IsPrivilegedUser = User.IsPrivileged();

                var result = await _mediator.Send(command);
                return Ok(new { 
                    Message = "Carrito sincronizado exitosamente.", 
                    CuentaId = result.CuentaId,
                    Detalles = result.Detalles
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validación de negocio fallida en SincronizarCarrito");
                return BadRequest(new { Error = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de persistencia masiva en SincronizarCarrito");
                return BadRequest(new { Error = "Error de persistencia masiva/integridad. Detalle: " + ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo crítico en SincronizarCarrito");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Error crítico durante la sincronización masiva." });
            }
        }

        [HttpPost("CloseAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CloseAccount([FromBody] CloseAccountCommand command)
        {
            try
            {
                // Auto-poblar identidad del cajero (V2.0 Core Extensions)
                command.UsuarioId = User.GetUserId();
                command.UsuarioCajero = User.GetCajeroName();

                var result = await _mediator.Send(command);
                return Ok(new { 
                    Message = "Cuenta cerrada y facturada exitosamente.", 
                    ReciboId = result.ReciboId,
                    CuentaPorCobrarId = result.CuentaPorCobrarId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("RemoveServicio")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveServicio([FromQuery] Guid cuentaId, [FromQuery] Guid detalleId, [FromQuery] Guid? medicoId = null, [FromQuery] DateTime? horaCita = null)
        {
            try
            {
                var command = new RemoveServicioDeCuentaCommand 
                { 
                    CuentaId = cuentaId, 
                    DetalleId = detalleId,
                    MedicoId = medicoId,
                    HoraCita = horaCita
                };
                await _mediator.Send(command);
                return Ok(new { Message = "Servicio removido exitosamente de la cuenta." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("CancelAppointment/{appointmentId}")]
        [Authorize(Roles = "Admin,Administrador")]
        public async Task<IActionResult> CancelAppointment(Guid appointmentId)
        {
            try
            {
                var command = new AdminCancelAppointmentCommand { AppointmentId = appointmentId };
                await _mediator.Send(command);
                return Ok(new { Message = "Cita anulada administrativamente exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("Appointments")]
        [Authorize(Roles = "Admin,Administrador")]
        public async Task<IActionResult> GetAppointments([FromQuery] DateTime? fecha, [FromQuery] Guid? medicoId)
        {
            try
            {
                var query = new GetActiveAppointmentsQuery { Fecha = fecha, MedicoId = medicoId };
                var results = await _mediator.Send(query);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("DailyBilledPatients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDailyBilledPatients([FromQuery] DateTime? fecha)
        {
            try
            {
                // Normalización de Fecha (V11.10): Si no se provee, usamos el día actual del servidor.
                // Usamos .Date para asegurar que el query reciba el inicio del día.
                var targetDate = fecha?.Date ?? DateTime.Today; 
                var query = new GetDailyBilledPatientsQuery { Fecha = targetDate };
                var results = await _mediator.Send(query);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pacientes facturados del día");
                return BadRequest(new { Error = "Error al procesar el reporte diario de facturación." });
            }
        }

        [HttpGet("nurse-audit-report")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetNurseAuditReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string? nurseUsername)
        {
            try
            {
                var query = new GetNurseAuditReportQuery 
                { 
                    StartDate = startDate, 
                    EndDate = endDate, 
                    NurseUsername = nurseUsername 
                };
                var results = await _mediator.Send(query);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte de auditoria de enfermeras");
                return BadRequest(new { Error = "Error al procesar el reporte de auditoria de enfermeras: " + ex.Message });
            }
        }

        [HttpPost("ReservarTurno")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReservarTurno([FromBody] ReservarTurnoTemporalCommand command)
        {
            try
            {
                command.UsuarioId = User.GetUserId();
                await _mediator.Send(command);
                return Ok(new { Message = "Turno reservado temporalmente." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Error inesperado: " + ex.Message });
            }
        }

        [HttpDelete("LiberarTurno")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> LiberarTurno([FromQuery] Guid medicoId, [FromQuery] DateTime horaPautada)
        {
            try
            {
                var command = new LiberarTurnoCommand 
                { 
                    MedicoId = medicoId, 
                    HoraPautada = horaPautada,
                    UsuarioId = User.GetUserId()
                };
                await _mediator.Send(command);
                return Ok(new { Message = "Turno liberado." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("BloquearHorario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize] // Permite a cualquier usuario autenticado (Cajero, Asistente, etc.)
        public async Task<IActionResult> BloquearHorario([FromBody] BloquearHorarioCommand command)
        {
            try
            {
                var exito = await _mediator.Send(command);
                if (exito) return Ok(new { Message = "Horario bloqueado administrativamente." });
                return BadRequest(new { Error = "No se pudo bloquear el horario (podría tener una cita activa)." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("ValidarCuenta")]
        [Authorize(Roles = "Admin,Administrador,Supervisor,Asistente Seguro,Asistente de Seguros")]
        public async Task<IActionResult> ValidarCuenta([FromBody] ValidarCuentaCommand command)
        {
            try
            {
                command.UsuarioValidador = User.GetUserName();
                var result = await _mediator.Send(command);
                if (result) return Ok(new { Message = "Cuenta validada exitosamente." });
                return BadRequest(new { Error = "No se pudo validar la cuenta." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("AuditarCuenta")]
        [Authorize(Roles = "Admin,Administrador,Supervisor")]
        public async Task<IActionResult> AuditarCuenta([FromBody] AuditarCuentaCommand command)
        {
            try
            {
                command.UsuarioAuditor = User.GetUserName();
                var result = await _mediator.Send(command);
                if (result) return Ok(new { Message = "Cuenta marcada como auditada." });
                return BadRequest(new { Error = "No se pudo auditar la cuenta." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("UpdateARMetadata")]
        public async Task<IActionResult> UpdateARMetadata([FromBody] UpdateARMetadataCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                if (result) return Ok(new { Message = "Metadata de documentos actualizada exitosamente." });
                return BadRequest(new { Error = "No se pudo actualizar la metadata de documentos." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("OpenAccount/{pacienteId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOpenAccount(Guid pacienteId, [FromQuery] string? tipoIngreso)
        {
            try
            {
                var query = new GetOpenAccountQuery { PacienteId = pacienteId, TipoIngreso = tipoIngreso };
                var result = await _mediator.Send(query);
                if (result == null) return NoContent();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cuenta abierta para Paciente: {PacienteId}", pacienteId);
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("cuentas-administrativas")]
        [Authorize(Roles = "Admin,Administrador,Supervisor,Asistente Particular,Asistente Seguro,Asistente de Seguros,Asistente Hospitalario,Asistente de Emergencia")]
        public async Task<IActionResult> GetCuentasAdministrativas([FromQuery] string? searchTerm, [FromQuery] string? tipoIngreso, [FromQuery] string? estado)
        {
            try
            {
                var query = new GetCuentasAdministrativasQuery { SearchTerm = searchTerm, TipoIngreso = tipoIngreso, Estado = estado };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("update-cuenta-administrativa")]
        [Authorize(Roles = "Admin,Administrador,Supervisor")]
        public async Task<IActionResult> UpdateCuentaAdministrativa([FromBody] UpdateCuentaAdministrativaCommand command)
        {
            try
            {
                command.UsuarioModificacion = User.GetUserName();
                var result = await _mediator.Send(command);
                if (result) return Ok(new { Message = "Cuenta modificada administrativamente con éxito." });
                return BadRequest(new { Error = "No se pudo realizar la modificación de la cuenta." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("cuenta-historial/{cuentaId}")]
        [Authorize(Roles = "Admin,Administrador,Supervisor")]
        public async Task<IActionResult> GetHistorialModificaciones(Guid cuentaId)
        {
            try
            {
                var query = new GetHistorialModificacionesQuery { CuentaServicioId = cuentaId };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
