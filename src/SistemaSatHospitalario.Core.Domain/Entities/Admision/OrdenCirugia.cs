using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    /// <summary>
    /// Representa una orden de cirugía programada asociada a una cuenta de paciente.
    /// Gestiona el ciclo de vida completo: agendamiento, ejecución, consumo y cierre.
    /// </summary>
    public class OrdenCirugia
    {
        public Guid Id { get; private set; }
        public Guid CuentaServicioId { get; private set; }
        public Guid PacienteId { get; private set; }
        public string DescripcionCirugia { get; private set; }
        public decimal PrecioBaseUsd { get; private set; }
        public Guid MedicoId { get; private set; }
        public DateTime FechaHoraProgramada { get; private set; }
        public string Estado { get; private set; }
        public string? MotivoCancelacion { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public string UsuarioCreacion { get; private set; }

        // Navegación
        public virtual CuentaServicios CuentaServicio { get; private set; }
        public virtual PacienteAdmision Paciente { get; private set; }
        public virtual Medico Medico { get; private set; }

        private readonly List<CirugiaLog> _logs = new();
        public IReadOnlyCollection<CirugiaLog> Logs => _logs.AsReadOnly();

        protected OrdenCirugia() { }

        public OrdenCirugia(
            Guid cuentaServicioId,
            Guid pacienteId,
            string descripcionCirugia,
            decimal precioBaseUsd,
            Guid medicoId,
            DateTime fechaHoraProgramada,
            string usuarioCreacion)
        {
            if (string.IsNullOrWhiteSpace(descripcionCirugia))
                throw new ArgumentException("La descripción de la cirugía es obligatoria.", nameof(descripcionCirugia));
            if (precioBaseUsd < 0)
                throw new ArgumentException("El precio base no puede ser negativo.", nameof(precioBaseUsd));
            if (medicoId == Guid.Empty)
                throw new ArgumentException("El médico cirujano es obligatorio.", nameof(medicoId));
            if (string.IsNullOrWhiteSpace(usuarioCreacion))
                throw new ArgumentException("El usuario de creación es obligatorio.", nameof(usuarioCreacion));

            Id = Guid.NewGuid();
            CuentaServicioId = cuentaServicioId;
            PacienteId = pacienteId;
            DescripcionCirugia = descripcionCirugia;
            PrecioBaseUsd = precioBaseUsd;
            MedicoId = medicoId;
            FechaHoraProgramada = fechaHoraProgramada;
            Estado = EstadoCirugiaConstants.PendienteEjecucion;
            FechaCreacion = DateTime.UtcNow;
            UsuarioCreacion = usuarioCreacion;
        }

        /// <summary>
        /// Transiciona la orden al estado En Proceso.
        /// </summary>
        public void IniciarCirugia(string usuarioId)
        {
            if (Estado != EstadoCirugiaConstants.PendienteEjecucion)
                throw new InvalidOperationException("Solo se puede iniciar una cirugía en estado Pendiente de Ejecución.");

            Estado = EstadoCirugiaConstants.EnProceso;
            AgregarLog(usuarioId, CirugiaEventoConstants.TransicionEstado,
                $"Estado cambiado de {EstadoCirugiaConstants.PendienteEjecucion} a {EstadoCirugiaConstants.EnProceso}");
        }

        /// <summary>
        /// Marca la cirugía como completada.
        /// </summary>
        public void CompletarCirugia(string usuarioId)
        {
            if (Estado != EstadoCirugiaConstants.EnProceso)
                throw new InvalidOperationException("Solo se puede completar una cirugía que está En Proceso.");

            Estado = EstadoCirugiaConstants.Completada;
            AgregarLog(usuarioId, CirugiaEventoConstants.TransicionEstado,
                $"Estado cambiado de {EstadoCirugiaConstants.EnProceso} a {EstadoCirugiaConstants.Completada}");
        }

        /// <summary>
        /// Cancela la cirugía con un motivo obligatorio.
        /// </summary>
        public void CancelarCirugia(string usuarioId, string motivo)
        {
            if (Estado == EstadoCirugiaConstants.Completada)
                throw new InvalidOperationException("No se puede cancelar una cirugía ya completada.");
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("El motivo de cancelación es obligatorio.", nameof(motivo));

            var estadoAnterior = Estado;
            Estado = EstadoCirugiaConstants.Cancelada;
            MotivoCancelacion = motivo;
            AgregarLog(usuarioId, CirugiaEventoConstants.TransicionEstado,
                $"Estado cambiado de {estadoAnterior} a {EstadoCirugiaConstants.Cancelada}. Motivo: {motivo}");
        }

        /// <summary>
        /// Agrega un log de auditoría inmutable a la orden de cirugía.
        /// </summary>
        public CirugiaLog AgregarLog(string usuarioId, string evento, string detalle)
        {
            var log = new CirugiaLog(Id, usuarioId, evento, detalle);
            _logs.Add(log);
            return log;
        }
    }

    /// <summary>
    /// Constantes de estado para OrdenCirugia.
    /// </summary>
    public static class EstadoCirugiaConstants
    {
        public const string PendienteEjecucion = "PendienteEjecucion";
        public const string EnProceso = "EnProceso";
        public const string Completada = "Completada";
        public const string Cancelada = "Cancelada";
    }

    /// <summary>
    /// Constantes de eventos para logs de auditoría de cirugía.
    /// </summary>
    public static class CirugiaEventoConstants
    {
        public const string Creacion = "Creacion";
        public const string DespachoInsumos = "DespachoInsumos";
        public const string DevolucionInsumos = "DevolucionInsumos";
        public const string CargoExtra = "CargoExtra";
        public const string TransicionEstado = "TransicionEstado";
    }
}
