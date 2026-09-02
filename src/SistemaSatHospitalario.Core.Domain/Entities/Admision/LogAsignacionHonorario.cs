using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class LogAsignacionHonorario
    {
        public Guid Id { get; private set; }
        public Guid DetalleServicioId { get; private set; }
        public string NombreServicio { get; private set; }
        public string TipoAccion { get; private set; } // ASIGNACION_MANUAL, ASIGNACION_DEFAULT, REASIGNACION, CONFIG_DEFAULT
        public Guid? MedicoAnteriorId { get; private set; }
        public string? MedicoAnteriorNombre { get; private set; }
        public Guid? MedicoNuevoId { get; private set; }
        public string? MedicoNuevoNombre { get; private set; }

        /// <summary>
        /// LEGACY (3FN): nombre de usuario en texto plano. Fuente de verdad:
        /// <see cref="UsuarioOperadorId"/> (FK lógica a Usuarios, PK Guid). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar UsuarioOperadorId. Columna legacy pendiente de DROP.")]
        public string UsuarioOperador { get; private set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del operador que ejecutó la acción.</summary>
        public Guid? UsuarioOperadorId { get; private set; }
        public DateTime FechaAccion { get; private set; }
        public string? Observaciones { get; private set; }

        public virtual DetalleServicioCuenta DetalleServicio { get; private set; } = null!;
        public virtual Medico? MedicoAnterior { get; private set; }
        public virtual Medico? MedicoNuevo { get; private set; }

        protected LogAsignacionHonorario() { }

        public LogAsignacionHonorario(
            Guid detalleServicioId, string nombreServicio, string tipoAccion,
            Guid? medicoAnteriorId, string? medicoAnteriorNombre,
            Guid? medicoNuevoId, string? medicoNuevoNombre,
            string usuarioOperador, string? observaciones = null)
        {
            Id = Guid.NewGuid();
            DetalleServicioId = detalleServicioId;
            NombreServicio = nombreServicio;
            TipoAccion = tipoAccion;
            MedicoAnteriorId = medicoAnteriorId;
            MedicoAnteriorNombre = medicoAnteriorNombre;
            MedicoNuevoId = medicoNuevoId;
            MedicoNuevoNombre = medicoNuevoNombre;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UsuarioOperador = usuarioOperador;
#pragma warning restore CS0618
            // 3FN: poblar la FK si el texto es un GUID válido
            UsuarioOperadorId = Guid.TryParse(usuarioOperador, out var parsed) ? parsed : (Guid?)null;
            FechaAccion = DateTime.UtcNow;
            Observaciones = observaciones;
        }
    }
}
