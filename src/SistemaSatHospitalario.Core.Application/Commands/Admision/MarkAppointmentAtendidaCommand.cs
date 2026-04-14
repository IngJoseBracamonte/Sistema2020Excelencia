using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class MarkAppointmentAtendidaCommand : IRequest<bool>
    {
        public Guid AppointmentId { get; set; }
    }

    public class MarkAppointmentAtendidaCommandHandler : IRequestHandler<MarkAppointmentAtendidaCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public MarkAppointmentAtendidaCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(MarkAppointmentAtendidaCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _context.CitasMedicas
                .FirstOrDefaultAsync(c => c.Id == request.AppointmentId, cancellationToken);

            if (appointment == null) return false;

            appointment.MarcarComoAtendida();
            
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
