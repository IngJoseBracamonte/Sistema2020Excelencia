using System;
using System.Collections.Generic;
using SistemaSatHospitalario.Core.Domain.State;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    /// <summary>
    /// Representa una orden de cirugía programada asociada a una cuenta de paciente.
    /// Gestiona el ciclo de vida completo mediante el Patrón State: agendamiento, ejecución, consumo y cierre.
    /// </summary>
    public class OrdenCirugia
    {
        public Guid Id { get; private set; }
        public Guid CuentaServicioId { get; private set; }
        public Guid PacienteId { get; private set; }
        public Guid? AreaClinicaId { get; private set; }
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
        public virtual AreaClinica? AreaClinica { get; private set; }

        private readonly List<CirugiaLog> _logs = new();
        public virtual IReadOnlyCollection<CirugiaLog> Logs => _logs.AsReadOnly();

        private readonly List<OrdenCirugiaRequisito> _requisitos = new();
        public virtual IReadOnlyCollection<OrdenCirugiaRequisito> Requisitos => _requisitos.AsReadOnly();

        private readonly List<CirugiaObservacionHistorial> _historialObservaciones = new();
        public virtual IReadOnlyCollection<CirugiaObservacionHistorial> HistorialObservaciones => _historialObservaciones.AsReadOnly();

        private ICirugiaState? _currentState;
        public ICirugiaState CurrentState => _currentState ??= CirugiaStateFactory.GetState(Estado);

        protected OrdenCirugia() { }

        public OrdenCirugia(
            Guid cuentaServicioId,
            Guid pacienteId,
            string descripcionCirugia,
            decimal precioBaseUsd,
            Guid medicoId,
            DateTime fechaHoraProgramada,
            string usuarioCreacion,
            Guid? areaClinicaId = null)
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
            AreaClinicaId = areaClinicaId;
            DescripcionCirugia = descripcionCirugia.Trim();
            PrecioBaseUsd = precioBaseUsd;
            MedicoId = medicoId;
            FechaHoraProgramada = fechaHoraProgramada;
            Estado = EstadoCirugiaConstants.Programada;
            FechaCreacion = DateTime.UtcNow;
            UsuarioCreacion = usuarioCreacion.Trim();
            _currentState = new ProgramadaState();
        }

        // --- Métodos de Estado encapsulados (State Pattern) ---

        public void IniciarEspera(string usuarioId)
        {
            CurrentState.IniciarEspera(this, usuarioId);
        }

        public void IniciarCirugia(string usuarioId)
        {
            CurrentState.IniciarCirugia(this, usuarioId);
        }

        public void FinalizarCirugia(string usuarioId)
        {
            CurrentState.FinalizarCirugia(this, usuarioId);
        }

        public void CompletarCirugia(string usuarioId)
        {
            FinalizarCirugia(usuarioId);
        }

        public void Reprogramar(DateTime nuevaFecha, string motivo, string usuarioId)
        {
            CurrentState.Reprogramar(this, nuevaFecha, motivo, usuarioId);
        }

        public void CancelarCirugia(string usuarioId, string motivo)
        {
            CurrentState.Cancelar(this, motivo, usuarioId);
        }

        // --- Helpers Internos invocados por ICirugiaState ---

        internal void SetEstadoInternal(string nuevoEstado, ICirugiaState stateInstance)
        {
            Estado = nuevoEstado;
            _currentState = stateInstance;
        }

        internal void MotivoCancelacionInternal(string motivo)
        {
            MotivoCancelacion = motivo;
        }

        internal void ActualizarFechaYMotivoReprogramacion(DateTime nuevaFecha, string motivo, string usuarioId)
        {
            var fechaAnterior = FechaHoraProgramada;
            FechaHoraProgramada = nuevaFecha;
            
            var detalle = $"Reprogramada de {fechaAnterior:dd/MM/yyyy HH:mm} a {nuevaFecha:dd/MM/yyyy HH:mm}. Motivo: {motivo}";
            AgregarLog(usuarioId, "Reprogramacion", detalle);
            AgregarHistorialObservacion(detalle, Enums.TipoObservacionCirugia.Reprogramacion, usuarioId, usuarioId);
        }

        public CirugiaObservacionHistorial AgregarHistorialObservacion(string observacion, Enums.TipoObservacionCirugia tipo = Enums.TipoObservacionCirugia.ObservacionMedica, string usuarioRegistro = "Sistema", string? usuarioRegistroId = null)
        {
            var item = new CirugiaObservacionHistorial(Id, observacion, tipo, usuarioRegistro, usuarioRegistroId);
            _historialObservaciones.Add(item);
            return item;
        }

        public CirugiaObservacionHistorial AgregarHistorialObservacion(string observacion, string tipo, string usuarioRegistro, string? usuarioRegistroId = null)
        {
            var tipoEnum = tipo?.ToLowerInvariant() switch
            {
                "reprogramacion" => Enums.TipoObservacionCirugia.Reprogramacion,
                "hitoquirurgico" => Enums.TipoObservacionCirugia.HitoQuirurgico,
                "cancelacion" => Enums.TipoObservacionCirugia.Cancelacion,
                _ => Enums.TipoObservacionCirugia.ObservacionMedica
            };
            return AgregarHistorialObservacion(observacion, tipoEnum, usuarioRegistro, usuarioRegistroId);
        }

        public OrdenCirugiaRequisito AgregarRequisito(Guid requisitoCirugiaId, bool cumplido = false)
        {
            var req = new OrdenCirugiaRequisito(Id, requisitoCirugiaId, cumplido);
            _requisitos.Add(req);
            return req;
        }

        public CirugiaLog AgregarLog(string usuarioId, string evento, string detalle)
        {
            var log = new CirugiaLog(Id, usuarioId, evento, detalle);
            _logs.Add(log);
            return log;
        }
    }

    /// <summary>
    /// Constantes de estado para OrdenCirugia.
    /// Contiene los nuevos estados (Programada, EnEspera, EnCirugia, Finalizado) y alias para compatibilidad retroactiva.
    /// </summary>
    public static class EstadoCirugiaConstants
    {
        public const string Programada = "Programada";
        public const string EnEspera = "EnEspera";
        public const string EnCirugia = "EnCirugia";
        public const string Finalizado = "Finalizado";

        // Aliases para compatibilidad con código/data previa
        public const string PendienteEjecucion = "Programada";
        public const string EnProceso = "EnCirugia";
        public const string Completada = "Finalizado";
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
        public const string Reprogramacion = "Reprogramacion";
    }
}
