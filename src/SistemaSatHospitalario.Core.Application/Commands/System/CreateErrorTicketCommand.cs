using MediatR;
using System;

namespace SistemaSatHospitalario.Core.Application.Commands.System
{
    public class CreateErrorTicketCommand : IRequest<Guid>
    {
        public string RequestPath { get; set; } = string.Empty;
        public string MetodoHTTP { get; set; } = string.Empty;
        public string MensajeExcepcion { get; set; } = string.Empty;
        public string StackTrace { get; set; } = string.Empty;
        public string? UsuarioAsociado { get; set; }
    }
}
