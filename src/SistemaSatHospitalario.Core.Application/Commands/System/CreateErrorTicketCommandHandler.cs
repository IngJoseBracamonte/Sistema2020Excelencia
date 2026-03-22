using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities;

namespace SistemaSatHospitalario.Core.Application.Commands.System
{
    public class CreateErrorTicketCommandHandler : IRequestHandler<CreateErrorTicketCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPublisher _publisher;

        public CreateErrorTicketCommandHandler(IApplicationDbContext context, IPublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }

        public async Task<Guid> Handle(CreateErrorTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = new ErrorTicket
            {
                Id = Guid.NewGuid(),
                RequestPath = request.RequestPath,
                MetodoHTTP = request.MetodoHTTP,
                MensajeExcepcion = request.MensajeExcepcion,
                StackTrace = request.StackTrace,
                UsuarioAsociado = request.UsuarioAsociado,
                FechaCreacion = DateTime.UtcNow,
                Resuelto = false
            };

            _context.ErrorTickets.Add(ticket);
            await _context.SaveChangesAsync(cancellationToken);

            // Disparar evento para notificación por correo (Fuego y Olvido)
            var ticketEvent = new ErrorTicketCreatedEvent
            {
                TicketId = ticket.Id,
                RequestPath = ticket.RequestPath,
                MensajeExcepcion = ticket.MensajeExcepcion,
                StackTrace = ticket.StackTrace,
                FechaCreacion = ticket.FechaCreacion
            };

            await _publisher.Publish(ticketEvent, cancellationToken);

            return ticket.Id;
        }
    }
}
