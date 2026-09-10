namespace SistemaSatHospitalario.Core.Domain.Entities
{
    public class ErrorTicket
    {
        public Guid Id { get; set; }
        public string RequestPath { get; set; } = string.Empty;
        public string MetodoHTTP { get; set; } = string.Empty;
        public string MensajeExcepcion { get; set; } = string.Empty;
        public string StackTrace { get; set; } = string.Empty;

        // Auditoría e Identidad (3FN Limpio)
        public Guid? UsuarioAsociadoId { get; set; }
        public string? UsuarioAsociado { get; set; } // Alias legacy opcional
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Resolución del Ticket
        public bool Resuelto { get; set; }
        public string? ComentariosResolucion { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public Guid? ResueltoPorId { get; set; }
        public string? ResueltoPor { get; set; } // Alias legacy opcional

        public ErrorTicket()
        {
            Id = Guid.NewGuid();
            FechaCreacion = DateTime.UtcNow;
            Resuelto = false;
        }
    }
}
