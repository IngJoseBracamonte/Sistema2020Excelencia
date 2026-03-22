using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.System
{
    public class ResolveTicketCommandHandler : IRequestHandler<ResolveTicketCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public ResolveTicketCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ResolveTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _context.ErrorTickets.FindAsync(new object[] { request.TicketId }, cancellationToken);

            if (ticket == null)
                return false;

            ticket.Resuelto = true;
            ticket.ComentariosResolucion = request.ComentariosResolucion;
            ticket.ResueltoPor = request.ResueltoPorUsuarioId;
            ticket.FechaResolucion = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
