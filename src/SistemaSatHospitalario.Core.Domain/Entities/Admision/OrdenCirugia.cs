using System;
using System.Collections.Generic;
using System.Linq;
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
        public Guid? SedeQuirofanoId { get; private set; }
        public Guid? AreaClinicaOrigenId { get; private set; }
        public Guid? SedeOrigenId { get; private set; }
        public string DescripcionCirugia { get; private set; }
        public decimal PrecioBaseUsd { get; private set; }
        public decimal PrecioDerechoSalaUsd { get; private set; }
        public Guid MedicoId { get; private set; }
        public DateTime FechaHoraProgramada { get; private set; }
        public string Estado { get; private set; }
        public string? MotivoCancelacion { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public string UsuarioCreacion { get; private set; }

        // Nuevos campos operativos
        public string SalaQuirofano { get; private set; }
        public string ModalidadAnestesia { get; private set; }
        public bool EsAlquilado { get; private set; }

        // Navegación
        public virtual CuentaServicios CuentaServicio { get; private set; }
        public virtual PacienteAdmision Paciente { get; private set; }
        public virtual Medico Medico { get; private set; }
        public virtual AreaClinica? AreaClinica { get; private set; }
        public virtual Sede? SedeQuirofano { get; private set; }
        public virtual AreaClinica? AreaClinicaOrigen { get; private set; }
        public virtual Sede? SedeOrigen { get; private set; }

        private readonly List<CirugiaLog> _logs = new();
        public virtual IReadOnlyCollection<CirugiaLog> Logs => _logs.AsReadOnly();

        private readonly List<OrdenCirugiaRequisito> _requisitos = new();
        public virtual IReadOnlyCollection<OrdenCirugiaRequisito> Requisitos => _requisitos.AsReadOnly();

        private readonly List<CirugiaObservacionHistorial> _historialObservaciones = new();
        public virtual IReadOnlyCollection<CirugiaObservacionHistorial> HistorialObservaciones => _historialObservaciones.AsReadOnly();

        private readonly List<CirugiaMedicoHonorario> _medicosHonorarios = new();
        public virtual IReadOnlyCollection<CirugiaMedicoHonorario> MedicosHonorarios => _medicosHonorarios.AsReadOnly();

        private readonly List<SolicitudInsumoCirugia> _solicitudesInsumos = new();
        public virtual IReadOnlyCollection<SolicitudInsumoCirugia> SolicitudesInsumos => _solicitudesInsumos.AsReadOnly();

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
            Guid? areaClinicaId = null,
            string salaQuirofano = "Quirófano 1",
            string modalidadAnestesia = "General",
            bool esAlquilado = false,
            decimal precioDerechoSalaUsd = 0,
            Guid? sedeQuirofanoId = null)
        {
            if (string.IsNullOrWhiteSpace(descripcionCirugia))
                throw new ArgumentException("La descripción de la cirugía es obligatoria.", nameof(descripcionCirugia));
            if (precioBaseUsd < 0)
                throw new ArgumentException("El precio base no puede ser negativo.", nameof(precioBaseUsd));
            if (precioDerechoSalaUsd < 0)
                throw new ArgumentException("El derecho de sala no puede ser negativo.", nameof(precioDerechoSalaUsd));
            if (medicoId == Guid.Empty)
                throw new ArgumentException("El médico cirujano es obligatorio.", nameof(medicoId));
            if (string.IsNullOrWhiteSpace(usuarioCreacion))
                throw new ArgumentException("El usuario de creación es obligatorio.", nameof(usuarioCreacion));

            Id = Guid.NewGuid();
            CuentaServicioId = cuentaServicioId;
            PacienteId = pacienteId;
            AreaClinicaId = areaClinicaId;
            SedeQuirofanoId = sedeQuirofanoId;
            DescripcionCirugia = descripcionCirugia.Trim();
            PrecioBaseUsd = precioBaseUsd;
            PrecioDerechoSalaUsd = precioDerechoSalaUsd;
            MedicoId = medicoId;
            FechaHoraProgramada = fechaHoraProgramada;
            SalaQuirofano = string.IsNullOrWhiteSpace(salaQuirofano) ? "Quirófano 1" : salaQuirofano.Trim();
            ModalidadAnestesia = string.IsNullOrWhiteSpace(modalidadAnestesia) ? "General" : modalidadAnestesia.Trim();
            EsAlquilado = esAlquilado;
            Estado = EstadoCirugiaConstants.Programada;
            FechaCreacion = DateTime.UtcNow;
            UsuarioCreacion = usuarioCreacion.Trim();
            _currentState = new ProgramadaState();
        }

        // --- Gestión Administrativa y de Precios ---

        public void ActualizarPreciosAdministrativos(decimal derechoSalaUsd, decimal precioBaseUsd, bool esAlquilado, string usuarioId)
        {
            if (derechoSalaUsd < 0) throw new ArgumentException("El derecho de sala no puede ser negativo.", nameof(derechoSalaUsd));
            if (precioBaseUsd < 0) throw new ArgumentException("El precio base no puede ser negativo.", nameof(precioBaseUsd));

            PrecioDerechoSalaUsd = derechoSalaUsd;
            PrecioBaseUsd = precioBaseUsd;
            EsAlquilado = esAlquilado;
        }

        public void AsignarSalaYAnestesia(string sala, string anestesia, string usuarioId)
        {
            if (!string.IsNullOrWhiteSpace(sala)) SalaQuirofano = sala.Trim();
            if (!string.IsNullOrWhiteSpace(anestesia)) ModalidadAnestesia = anestesia.Trim();
            AgregarLog(usuarioId, "AsignacionSalaAnestesia", $"Sala: {SalaQuirofano}, Anestesia: {ModalidadAnestesia}");
        }

        public void GuardarUbicacionOrigen(Guid? areaClinicaId, Guid? sedeId)
        {
            AreaClinicaOrigenId = areaClinicaId;
            SedeOrigenId = sedeId;
        }

        public void LimpiarUbicacionOrigen()
        {
            AreaClinicaOrigenId = null;
            SedeOrigenId = null;
        }

        public CirugiaMedicoHonorario AsignarMedicoHonorario(Guid medicoId, Guid especialidadId, decimal honorarioUsd, bool esPrincipal = false)
        {
            var existente = _medicosHonorarios.FirstOrDefault(m => m.MedicoId == medicoId);
            if (existente != null)
            {
                existente.ActualizarHonorario(honorarioUsd, especialidadId);
                return existente;
            }

            var item = new CirugiaMedicoHonorario(Id, medicoId, especialidadId, honorarioUsd, esPrincipal);
            _medicosHonorarios.Add(item);
            return item;
        }

        public void LimpiarMedicosHonorarios()
        {
            _medicosHonorarios.Clear();
        }

        public SolicitudInsumoCirugia AgregarSolicitudInsumoExtra(Guid insumoId, decimal cantidad, Guid almacenOrigenId, string usuario)
        {
            var sol = new SolicitudInsumoCirugia(Id, insumoId, cantidad, almacenOrigenId, usuario);
            _solicitudesInsumos.Add(sol);
            AgregarLog(usuario, "SolicitudInsumoExtra", $"Solicitud ad-hoc: Insumo {insumoId}, Cantidad {cantidad}");
            return sol;
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
        public const string SolicitudInsumoExtra = "SolicitudInsumoExtra";
    }
}
