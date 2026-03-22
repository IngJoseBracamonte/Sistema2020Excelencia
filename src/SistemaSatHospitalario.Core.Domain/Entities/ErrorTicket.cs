using System;

namespace SistemaSatHospitalario.Core.Domain.Entities
{
    public class ErrorTicket
    {
        public Guid Id { get; set; }
        public string RequestPath { get; set; } = string.Empty;
        public string MetodoHTTP { get; set; } = string.Empty;
        public string MensajeExcepcion { get; set; } = string.Empty;
        public string StackTrace { get; set; } = string.Empty;
        public string? UsuarioAsociado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Resuelto { get; set; }
        public string? ComentariosResolucion { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public string? ResueltoPor { get; set; }
    }
}
