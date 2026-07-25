using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class RegistrarAltaPacienteCommandHandler : IRequestHandler<RegistrarAltaPacienteCommand, RegistrarAltaPacienteResult>
    {
        private readonly IApplicationDbContext _context;

        public RegistrarAltaPacienteCommandHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<RegistrarAltaPacienteResult> Handle(RegistrarAltaPacienteCommand request, CancellationToken cancellationToken)
        {
            // 1. Obtener la cuenta activa del paciente por AdmisionId (CuentaId) o PacienteId
            var cuenta = await _context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => (c.Id == request.AdmisionId || c.PacienteId == request.PacienteId) && c.Estado == EstadoConstants.Abierta, cancellationToken);

            if (cuenta == null)
            {
                // Fallback: Buscar la cuenta más reciente aunque no esté abierta
                cuenta = await _context.CuentasServicios
                    .Include(c => c.Detalles)
                    .FirstOrDefaultAsync(c => c.Id == request.AdmisionId || c.PacienteId == request.PacienteId, cancellationToken);
            }

            if (cuenta == null)
            {
                throw new InvalidOperationException($"No se encontró una cuenta médica activa para el paciente especificado.");
            }

            // 2. Calcular saldo pendiente de la cuenta
            decimal totalCuenta = cuenta.CalcularTotal();
            decimal totalPagado = await _context.RecibosFactura
                .Where(r => r.CuentaServicioId == cuenta.Id && r.EstadoFiscal != EstadoConstants.Anulada)
                .SumAsync(r => (decimal?)r.TotalFacturadoUSD, cancellationToken) ?? 0m;

            decimal saldoPendiente = Math.Max(0m, totalCuenta - totalPagado);

            // 3. Validación de solvencia: Si existe saldo pendiente y no ha sido confirmado explícitamente por Enfermería, rebotar
            if (saldoPendiente > 0 && !request.ConfirmadoPorEnfermeriaSinSolvencia)
            {
                throw new InvalidOperationException($"El paciente registra un saldo pendiente de ${saldoPendiente:F2} USD. Se requiere confirmación explícita de enfermería para procesar el alta.");
            }

            // 4. Liberar cama asignada si existía
            if (cuenta.AreaClinicaId.HasValue)
            {
                var camaOrigen = await _context.AreasClinicas
                    .FirstOrDefaultAsync(a => a.Id == cuenta.AreaClinicaId.Value, cancellationToken);
                if (camaOrigen != null)
                {
                    camaOrigen.Liberar();
                }
            }

            // 5. Registrar destino de egreso y observaciones
            string destinoText = $"ALTA_{request.TipoAlta.ToString().ToUpper()}";
            cuenta.RegistrarDestinoEgreso(destinoText, request.Observaciones);

            // 6. Registrar Auditoría Inmutable (AuditLog)
            var auditLog = new AuditLog
            {
                UserId = string.IsNullOrWhiteSpace(request.UsuarioAlta) ? "Sistema" : request.UsuarioAlta,
                ActionType = "ALTA_MEDICA",
                OldValue = $"Estado: {cuenta.Estado}, Area: {cuenta.AreaClinicaId}, SaldoPendiente: ${saldoPendiente:F2}",
                NewValue = $"TipoAlta: {request.TipoAlta}, SolvenciaConfirmada: {request.ConfirmadoPorEnfermeriaSinSolvencia}, Obs: {request.Observaciones}",
                IpAddress = string.IsNullOrWhiteSpace(request.IpAddress) ? "127.0.0.1" : request.IpAddress,
                Timestamp = DateTime.UtcNow
            };

            await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new RegistrarAltaPacienteResult
            {
                Exitoso = true,
                AdmisionId = cuenta.Id,
                Mensaje = $"Alta médica ({request.TipoAlta}) registrada exitosamente.",
                SaldoPendienteUsd = saldoPendiente,
                TipoAltaDesc = request.TipoAlta.ToString(),
                FechaAlta = DateTime.UtcNow
            };
        }
    }
}
