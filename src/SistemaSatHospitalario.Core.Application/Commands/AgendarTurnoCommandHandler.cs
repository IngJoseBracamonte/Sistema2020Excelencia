using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands
{
    public class AgendarTurnoCommandHandler : IRequestHandler<AgendarTurnoCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public AgendarTurnoCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(AgendarTurnoCommand request, CancellationToken cancellationToken)
        {
            // 1. Validar disponibilidad (Evitar colisión de horario exacto)
            var colision = await _context.CitasMedicas
                .AnyAsync(c => c.MedicoId == request.MedicoId 
                            && c.HoraPautada == request.FechaHoraToma 
                            && c.EstadoAtencion != "Cancelado", 
                            cancellationToken);

            if (colision)
            {
                throw new InvalidOperationException("El horario solicitado ya se encuentra ocupado por otra cita.");
            }

            // 2. Crear Cita Médica
            var cita = new CitaMedica(
                request.MedicoId,
                request.PacienteId,
                request.CuentaServicioId,
                request.FechaHoraToma
            );

            _context.CitasMedicas.Add(cita);

            // 3. Persistir
            await _context.SaveChangesAsync(cancellationToken);

            return cita.Id;
        }
    }
}
