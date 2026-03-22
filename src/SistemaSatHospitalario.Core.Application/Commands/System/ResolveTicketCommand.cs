using MediatR;
using System;

namespace SistemaSatHospitalario.Core.Application.Commands.System
{
    public class ResolveTicketCommand : IRequest<bool>
    {
        public Guid TicketId { get; set; }
        public string? ComentariosResolucion { get; set; }
        public string? ResueltoPorUsuarioId { get; set; }
    }
}
