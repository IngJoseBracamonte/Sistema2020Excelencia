using System;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    /// <summary>
    /// Representa una orden de estudio de imagen (RX o TOMO) pendiente de procesamiento.
    /// (Senior Strategy: Domain Persistence for Real-Time Stations)
    /// </summary>
    public class OrdenImagen
    {
        public int Id { get; set; }
        public Guid CuentaId { get; set; }
        public Guid PacienteId { get; set; }

        /// <summary>
        /// LEGACY (3FN): nombre desnormalizado del paciente. Fuente de verdad:
        /// la navegación <see cref="Paciente"/> vía <see cref="PacienteId"/>. Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar Paciente.NombreCorto vía PacienteId. Columna legacy pendiente de DROP.")]
        public string PacienteNombre { get; set; } = string.Empty;
        public string Estudio { get; set; } = string.Empty;
        public string TipoServicio { get; set; } = string.Empty; // RX o TOMO
        public EstadoOrdenImagen Estado { get; set; } = EstadoOrdenImagen.Pendiente;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public string? ProcesadoPor { get; set; }
        public DateTime? FechaProcesado { get; set; }
        public bool EsDirecta { get; set; } = false;
        public bool RequiereValidacion { get; set; } = false;
        public bool Validada { get; set; } = false;
        public string? ValidadorPor { get; set; }
        public DateTime? FechaValidacion { get; set; }
        public Guid? MedicoSolicitanteId { get; set; }

        /// <summary>
        /// LEGACY (3FN): nombre desnormalizado del médico. Fuente de verdad:
        /// la navegación <see cref="MedicoSolicitante"/> vía <see cref="MedicoSolicitanteId"/>. Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar MedicoSolicitante.Nombre vía MedicoSolicitanteId. Columna legacy pendiente de DROP.")]
        public string? MedicoSolicitanteNombre { get; set; }
        public string? Informe { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(PacienteId))]
        public virtual PacienteAdmision? Paciente { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(MedicoSolicitanteId))]
        public virtual Medico? MedicoSolicitante { get; set; }

        // Nuevos campos para informe de imagenología y trazabilidad técnica
        public string? LinkInforme { get; set; }
        public string? ObservacionesMedico { get; set; }
        public Guid? MedicoInterpreteId { get; set; }
        public bool RequiereInforme { get; set; } = false;

        public OrdenImagen() { }

        public OrdenImagen(Guid cuentaId, Guid pacienteId, string pacienteNombre, string estudio, string tipoServicio)
        {
            CuentaId = cuentaId;
            PacienteId = pacienteId;
            PacienteNombre = pacienteNombre;
            Estudio = estudio;
            TipoServicio = tipoServicio;
            Estado = EstadoOrdenImagen.Pendiente;
            FechaCreacion = DateTime.UtcNow;
        }

        public void MarcarComoProcesado(string usuario)
        {
            Estado = EstadoOrdenImagen.Procesado;
            ProcesadoPor = usuario;
            FechaProcesado = DateTime.UtcNow;
        }
    }
}
