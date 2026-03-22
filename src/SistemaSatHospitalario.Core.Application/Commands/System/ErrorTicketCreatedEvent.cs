using MediatR;
using System;

namespace SistemaSatHospitalario.Core.Application.Commands.System
{
    public class ErrorTicketCreatedEvent : INotification
    {
        public Guid TicketId { get; set; }
        public string RequestPath { get; set; } = string.Empty;
        public string MensajeExcepcion { get; set; } = string.Empty;
        public string StackTrace { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }
}
