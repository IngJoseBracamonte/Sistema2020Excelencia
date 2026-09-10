using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    /// <summary>
    /// Registro inmutable de auditoría para cada evento operativo dentro del ciclo de vida de una cirugía.
    /// </summary>
    public class CirugiaLog
    {
        public Guid Id { get; private set; }
        public Guid OrdenCirugiaId { get; private set; }

        // Auditoría e Identidad (3FN Limpio)
        public Guid? UsuarioIdentityId { get; private set; }
        public string? UsuarioId { get; private set; } // Alias legacy opcional

        public string Evento { get; private set; } = string.Empty;
        public string Detalle { get; private set; } = string.Empty;
        public DateTime Timestamp { get; private set; }

        // Navegación
        public virtual OrdenCirugia OrdenCirugia { get; private set; } = null!;

        protected CirugiaLog() { }

        public CirugiaLog(
            Guid ordenCirugiaId, 
            Guid? usuarioIdentityId = null,
            string? usuarioId = null, 
            string evento = "EventoQuirurgico", 
            string? detalle = null)
        {
            if (ordenCirugiaId == Guid.Empty)
                throw new ArgumentException("El ID de la orden de cirugía es obligatorio.", nameof(ordenCirugiaId));
            if (string.IsNullOrWhiteSpace(evento))
                throw new ArgumentException("El tipo de evento es obligatorio.", nameof(evento));

            Id = Guid.NewGuid();
            OrdenCirugiaId = ordenCirugiaId;
            
            // 3FN: Asignar ID directo o intentar parsear string si vino el alias
            UsuarioIdentityId = usuarioIdentityId ?? (Guid.TryParse(usuarioId, out var parsed) ? parsed : (Guid?)null);
            UsuarioId = usuarioId;

            Evento = evento.Trim();
            Detalle = detalle ?? string.Empty;
            Timestamp = DateTime.UtcNow;
        }

        public CirugiaLog(Guid ordenCirugiaId, string usuarioId, string evento, string detalle)
            : this(ordenCirugiaId, null, usuarioId, evento, detalle)
        {
        }
    }
}