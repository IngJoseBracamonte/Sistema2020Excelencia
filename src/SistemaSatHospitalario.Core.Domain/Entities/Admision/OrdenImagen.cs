using System;
using SistemaSatHospitalario.Core.Domain.Constants;

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
        public string PacienteNombre { get; set; } = string.Empty;
        public string Estudio { get; set; } = string.Empty;
        public string TipoServicio { get; set; } = string.Empty; // RX o TOMO
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Procesado, Anulado
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public string? ProcesadoPor { get; set; }
        public DateTime? FechaProcesado { get; set; }

        public OrdenImagen() { }

        public OrdenImagen(Guid cuentaId, Guid pacienteId, string pacienteNombre, string estudio, string tipoServicio)
        {
            CuentaId = cuentaId;
            PacienteId = pacienteId;
            PacienteNombre = pacienteNombre;
            Estudio = estudio;
            TipoServicio = tipoServicio;
            Estado = "Pendiente";
            FechaCreacion = DateTime.UtcNow;
        }

        public void MarcarComoProcesado(string usuario)
        {
            Estado = "Procesado";
            ProcesadoPor = usuario;
            FechaProcesado = DateTime.UtcNow;
        }
    }
}
