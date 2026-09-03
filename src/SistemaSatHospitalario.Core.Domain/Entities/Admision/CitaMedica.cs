using System;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CitaMedica
    {
        public Guid Id { get; private set; }
        public Guid MedicoId { get; private set; }
        // Se cambió de int a Guid para el nuevo sistema de identidad (V11.0 Sync Pro)
        public Guid PacienteId { get; private set; }
        public Guid CuentaServicioId { get; private set; }
        public DateTime HoraPautada { get; private set; }

        /// <summary>
        /// LEGACY (3FN): texto del estado. Fuente de verdad: <see cref="EstadoId"/>
        /// (FK a EstadosCitaMedica). Se mantiene mapeado como alias de compatibilidad
        /// hasta el DROP de columna (delta posterior a validación en producción).
        /// </summary>
        [Obsolete("Usar EstadoId / EstadoNav. Columna legacy pendiente de DROP.")]
        public string Estado { get; private set; } // Pendiente, Confirmada, Cancelada, Atendida

        /// <summary>FK al catálogo EstadosCitaMedica (3FN).</summary>
        public int EstadoId { get; private set; }
        public string? Comentario { get; private set; }
        public DateTime FechaRegistro { get; private set; }
        public Guid? AreaClinicaId { get; private set; }

        public Medico Medico { get; private set; }
        public CuentaServicios CuentaServicio { get; private set; }
        public virtual AreaClinica? AreaClinica { get; private set; }
        public virtual EstadoCitaMedica EstadoNav { get; private set; } = null!;

        protected CitaMedica() { }

        public CitaMedica(Guid medicoId, Guid pacienteId, Guid cuentaServicioId, DateTime horaPautada, string? comentario = null, Guid? areaClinicaId = null)
        {
            Id = Guid.NewGuid();
            MedicoId = medicoId;
            PacienteId = pacienteId;
            CuentaServicioId = cuentaServicioId;
            HoraPautada = horaPautada;
            EstadoId = EstadoCitaConstants.PendienteId;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            Estado = EstadoConstants.Pendiente;
#pragma warning restore CS0618
            Comentario = comentario;
            FechaRegistro = DateTime.UtcNow;
            AreaClinicaId = areaClinicaId;
        }

        public void AsignarAreaClinica(Guid areaClinicaId)
        {
            AreaClinicaId = areaClinicaId;
        }

        public void ActualizarComentario(string? comentario) => Comentario = comentario;

        public void Cancelar() => SetEstado(EstadoCitaConstants.CanceladaId);
        public void ActualizarHoraPautada(DateTime nuevaHora) => HoraPautada = nuevaHora;

        public void MarcarComoAtendida() => SetEstado(EstadoCitaConstants.AtendidaId);

        public void Confirmar() => SetEstado(EstadoCitaConstants.ConfirmadaId);

        /// <summary>3FN: cambio de estado por FK; sincroniza el alias legacy.</summary>
        public void SetEstado(int estadoId)
        {
            EstadoId = estadoId;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            Estado = EstadoCitaConstants.ToLegacyString(estadoId);
#pragma warning restore CS0618
        }

        public void CambiarPacienteAdministrativo(Guid nuevoPacienteId)
        {
            if (nuevoPacienteId == Guid.Empty) throw new ArgumentException("El PacienteId no puede ser vacío.");
            PacienteId = nuevoPacienteId;
        }
    }
}
