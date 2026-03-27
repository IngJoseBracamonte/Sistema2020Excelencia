using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;

namespace SistemaSatHospitalario.Core.Application.Commands
{
    public class AgendarTurnoCommandHandler : IRequestHandler<AgendarTurnoCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILegacyLabRepository _legacyRepository;

        public AgendarTurnoCommandHandler(IApplicationDbContext context, ILegacyLabRepository legacyRepository)
        {
            _context = context;
            _legacyRepository = legacyRepository;
        }

        public async Task<Guid> Handle(AgendarTurnoCommand request, CancellationToken cancellationToken)
        {
            // 0. Asegurar Existencia Local del Paciente (V11.0 Sync Pro)
            // Traducimos el ID de la base de datos vieja a la identidad GUID de la nueva
            var paciente = await _context.PacientesAdmision.FirstOrDefaultAsync(
                p => p.IdPacienteLegacy == request.PacienteId, cancellationToken);

            if (paciente == null)
            {
                // Consultamos al legado para obtener datos reales (V11.8)
                var legacyResult = await _legacyRepository.GetPatientByIdAsync(request.PacienteId.ToString(), cancellationToken);

                if (legacyResult != null)
                {
                    var fullName = $"{legacyResult.Nombre} {legacyResult.Apellidos}".Trim();
                    var mainPhone = !string.IsNullOrEmpty(legacyResult.Celular) ? legacyResult.Celular : legacyResult.Telefono;
                    
                    paciente = new PacienteAdmision(legacyResult.Cedula, fullName, mainPhone ?? "", legacyResult.IdPersona);
                    _context.PacientesAdmision.Add(paciente);
                    // No persistimos aún para permitir atomicidad con la Cita
                }
                else
                {
                    // Senior Identity Guard: No permitimos stubs de baja calidad (V11.8)
                    throw new InvalidOperationException($"No se pudo resolver la identidad del paciente (Legacy ID: {request.PacienteId}). El registro original no existe en el sistema base.");
                }
            }

            // Normalización para garantizar paridad con el sistema de Reservas y prevenir errores de índice único
            var targetHora = new DateTime(
                request.FechaHoraToma.Year, request.FechaHoraToma.Month, request.FechaHoraToma.Day, 
                request.FechaHoraToma.Hour, request.FechaHoraToma.Minute, 0, 
                DateTimeKind.Unspecified);

            // 1. Validar disponibilidad (Evitar colisión de horario exacto)
            var colisionCita = await _context.CitasMedicas
                .AnyAsync(c => c.MedicoId == request.MedicoId 
                            && c.HoraPautada == targetHora 
                            && c.Estado != EstadoConstants.Cancelado, 
                            cancellationToken);

            var colisionBloqueo = await _context.BloqueosHorarios
                .AnyAsync(b => b.MedicoId == request.MedicoId && b.HoraPautada == targetHora, cancellationToken);

            if (colisionCita || colisionBloqueo)
            {
                throw new InvalidOperationException("El horario solicitado ya se encuentra ocupado o bloqueado.");
            }

            // 2. Limpiar reservas temporales para este horario al confirmar la cita real
            var reservasAsociadas = await _context.ReservasTemporales
                .Where(r => r.MedicoId == request.MedicoId && r.HoraPautada == targetHora)
                .ToListAsync(cancellationToken);
            
            if (reservasAsociadas.Any())
            {
                _context.ReservasTemporales.RemoveRange(reservasAsociadas);
            }

            // 2. Crear Cita Médica usando el GUID local
            var cita = new CitaMedica(
                request.MedicoId,
                paciente.Id,
                request.CuentaServicioId,
                targetHora,
                request.Comentario
            );

            _context.CitasMedicas.Add(cita);

            // 3. Persistir
            await _context.SaveChangesAsync(cancellationToken);

            return cita.Id;
        }
    }
}
