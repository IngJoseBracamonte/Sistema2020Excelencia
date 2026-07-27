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
        public string UsuarioId { get; private set; }
        public string Evento { get; private set; }
        public string Detalle { get; private set; }
        public DateTime Timestamp { get; private set; }

        // Navegación
        public virtual OrdenCirugia OrdenCirugia { get; private set; }

        protected CirugiaLog() { }

        public CirugiaLog(Guid ordenCirugiaId, string usuarioId, string evento, string detalle)
        {
            if (ordenCirugiaId == Guid.Empty)
                throw new ArgumentException("El ID de la orden de cirugía es obligatorio.", nameof(ordenCirugiaId));
            if (string.IsNullOrWhiteSpace(usuarioId))
                throw new ArgumentException("El ID de usuario es obligatorio.", nameof(usuarioId));
            if (string.IsNullOrWhiteSpace(evento))
                throw new ArgumentException("El tipo de evento es obligatorio.", nameof(evento));

            Id = Guid.NewGuid();
            OrdenCirugiaId = ordenCirugiaId;
            UsuarioId = usuarioId;
            Evento = evento;
            Detalle = detalle ?? string.Empty;
            Timestamp = DateTime.UtcNow;
        }
    }
}
