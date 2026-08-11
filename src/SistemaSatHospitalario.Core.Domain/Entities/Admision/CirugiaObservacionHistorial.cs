using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    /// <summary>
    /// Entidad inmutable para el registro de historial de observaciones, motivos de reprogramación e hitos quirúrgicos.
    /// </summary>
    public class CirugiaObservacionHistorial
    {
        public Guid Id { get; private set; }
        public Guid OrdenCirugiaId { get; private set; }
        public string Observacion { get; private set; }
        public string Tipo { get; private set; }
        public DateTime FechaRegistro { get; private set; }
        public string UsuarioRegistro { get; private set; }

        public virtual OrdenCirugia OrdenCirugia { get; private set; }

        protected CirugiaObservacionHistorial() { }

        public CirugiaObservacionHistorial(Guid ordenCirugiaId, string observacion, string tipo, string usuarioRegistro)
        {
            if (ordenCirugiaId == Guid.Empty)
                throw new ArgumentException("El ID de la orden de cirugía es obligatorio.", nameof(ordenCirugiaId));
            if (string.IsNullOrWhiteSpace(observacion))
                throw new ArgumentException("La observación o motivo es obligatorio.", nameof(observacion));

            Id = Guid.NewGuid();
            OrdenCirugiaId = ordenCirugiaId;
            Observacion = observacion.Trim();
            Tipo = string.IsNullOrWhiteSpace(tipo) ? "ObservacionMedica" : tipo.Trim();
            FechaRegistro = DateTime.UtcNow;
            UsuarioRegistro = string.IsNullOrWhiteSpace(usuarioRegistro) ? "Sistema" : usuarioRegistro.Trim();
        }
    }
}
