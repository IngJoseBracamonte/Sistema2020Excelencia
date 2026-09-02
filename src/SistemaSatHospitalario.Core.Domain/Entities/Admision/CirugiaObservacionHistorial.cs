using System;
using SistemaSatHospitalario.Core.Domain.Enums;

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
        public TipoObservacionCirugia Tipo { get; private set; }
        public DateTime FechaRegistro { get; private set; }

        /// <summary>
        /// LEGACY (3FN): nombre de usuario en texto plano. Fuente de verdad:
        /// <see cref="UsuarioRegistroId"/> (FK lógica a Usuarios, PK Guid).
        /// Se mantiene mapeado como alias de compatibilidad hasta el DROP de columna
        /// (delta posterior a validación en producción).
        /// </summary>
        [Obsolete("Usar UsuarioRegistroId. Columna legacy pendiente de DROP.")]
        public string UsuarioRegistro { get; private set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid). Nullable para registros del sistema.</summary>
        public Guid? UsuarioRegistroId { get; private set; }

        public virtual OrdenCirugia OrdenCirugia { get; private set; }

        protected CirugiaObservacionHistorial() { }

        public CirugiaObservacionHistorial(Guid ordenCirugiaId, string observacion, TipoObservacionCirugia tipo, string usuarioRegistro, Guid? usuarioRegistroId = null)
        {
            if (ordenCirugiaId == Guid.Empty)
                throw new ArgumentException("El ID de la orden de cirugía es obligatorio.", nameof(ordenCirugiaId));
            if (string.IsNullOrWhiteSpace(observacion))
                throw new ArgumentException("La observación o motivo es obligatorio.", nameof(observacion));

            Id = Guid.NewGuid();
            OrdenCirugiaId = ordenCirugiaId;
            Observacion = observacion.Trim();
            Tipo = tipo;
            FechaRegistro = DateTime.UtcNow;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UsuarioRegistro = string.IsNullOrWhiteSpace(usuarioRegistro) ? "Sistema" : usuarioRegistro.Trim();
#pragma warning restore CS0618
            UsuarioRegistroId = usuarioRegistroId;
        }
    }
}
