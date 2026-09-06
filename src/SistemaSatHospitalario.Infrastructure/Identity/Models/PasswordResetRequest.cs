using System;

namespace SistemaSatHospitalario.Infrastructure.Identity.Models
{
    public class PasswordResetRequest
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Aprobado, Completado, Rechazado
        public DateTime? FechaProcesado { get; set; }
        public string? ProcesadoPor { get; set; }

        public PasswordResetRequest()
        {
            Id = Guid.NewGuid();
            FechaSolicitud = DateTime.UtcNow;
        }
    }
}
