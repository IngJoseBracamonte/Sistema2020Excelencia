using System.Linq;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Common;
using SistemaSatHospitalario.Core.Domain.Entities.Admision.Events;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CuentaServicios : BaseEntity
    {
        public Guid Id { get; private set; }
        // Se cambió de int a Guid para el nuevo sistema de identidad (V11.0 Sync Pro)
        public Guid PacienteId { get; private set; }
        public Guid? CuentaPrincipalId { get; private set; }
        public Guid? UsuarioCargaId { get; private set; }
        public DateTime FechaCarga { get; private set; }
        public DateTime? FechaCierre { get; private set; }
        public int EstadoId { get; private set; } // Abierta, Facturada, Anulada

        public int TipoIngresoId { get; private set; }
        public int? ConvenioId { get; private set; }
        public int? LegacyOrderId { get; private set; }
        public string? ProcesamientoEstado { get; private set; } // PENDIENTE, PROCESADA
        public Guid? AreaClinicaId { get; private set; }
        public string? SubAreaClinica { get; private set; }
        public Guid? MedicoId { get; private set; }
        public Guid? CamaRetenidaId { get; private set; }
 
        public virtual PacienteAdmision Paciente { get; private set; }
        public virtual SeguroConvenio Convenio { get; private set; }
        public virtual CuentaServicios? CuentaPrincipal { get; private set; }
        public virtual AreaClinica? AreaClinica { get; private set; }
        public virtual AreaClinica? CamaRetenida { get; private set; }
        public virtual Medico? Medico { get; private set; }
        public virtual EstadoCuenta EstadoNav { get; private set; } = null!;
        public virtual TipoIngreso TipoIngresoNav { get; private set; } = null!;
 
        public virtual ICollection<TriageEnfermeria> Triages { get; private set; } = new List<TriageEnfermeria>();
        public virtual ICollection<ValoracionFisica> Valoraciones { get; private set; } = new List<ValoracionFisica>();


        public Guid? UsuarioValidacionId { get; private set; }
        public DateTime? FechaValidacion { get; private set; }

        public Guid? UsuarioAuditoriaId { get; private set; }
        public DateTime? FechaAuditoria { get; private set; }
        public string? DestinoPaciente { get; private set; }
        public string? PersonalRelevo { get; private set; }

        private readonly List<DetalleServicioCuenta> _detalles = new();
        public IReadOnlyCollection<DetalleServicioCuenta> Detalles => _detalles.AsReadOnly();

        protected CuentaServicios() { }

        public CuentaServicios(Guid pacienteId, string usuarioCarga, string tipoIngreso, int? convenioId = null, Guid? areaClinicaId = null, string? subAreaClinica = null, Guid? medicoId = null, Guid? usuarioCargaId = null)
        {
            Id = Guid.NewGuid();
            PacienteId = pacienteId;
            UsuarioCargaId = usuarioCargaId ?? (Guid.TryParse(usuarioCarga, out var parsedCarga) ? parsedCarga : (Guid?)null);
            FechaCarga = DateTime.UtcNow;
            EstadoId = EstadoCuentaConstants.AbiertaId;
            TipoIngresoId = TipoIngresoConstants.FromLegacyString(tipoIngreso);
            ConvenioId = convenioId;
            AreaClinicaId = areaClinicaId;
            SubAreaClinica = subAreaClinica;
            MedicoId = medicoId;
        }
 
        public void AsignarAreaClinica(Guid? areaClinicaId, string? subAreaClinica)
        {
            AreaClinicaId = areaClinicaId;
            SubAreaClinica = subAreaClinica;
        }

        public void AsignarCamaRetenida(Guid? camaRetenidaId)
        {
            CamaRetenidaId = camaRetenidaId;
        }

        public void ModificarFechaCargaParaPruebas(DateTime fecha)
        {
            FechaCarga = fecha;
        }

        public void AsignarMedico(Guid? medicoId)
        {
            MedicoId = medicoId;
        }

        public void VincularCuentaPrincipal(Guid cuentaPrincipalId)
        {
            if (cuentaPrincipalId == Guid.Empty)
                throw new ArgumentException("El ID de la cuenta principal no puede estar vacío.");
            CuentaPrincipalId = cuentaPrincipalId;
        }

        public DetalleServicioCuenta AgregarServicio(Guid servicioId, string descripcion, decimal precio, decimal honorario, decimal cantidad, string tipoServicio, string usuarioCarga, string? legacyMappingId = null, Guid? areaClinicaId = null, int? tipoServicioId = null)
        {
            if (EstadoId != EstadoCajaConstants.AbiertaId)
                throw new InvalidOperationException("No se pueden agregar servicios a una cuenta que no está abierta.");

            int resolvedTipoServicioId = tipoServicioId ?? (tipoServicio?.ToUpperInvariant() switch
            {
                "MEDICO" or "CONSULTA" => Constants.TipoServicioConstants.Medico,
                "LABORATORIO" => Constants.TipoServicioConstants.Laboratorio,
                "RX" or "RAYOSX" => Constants.TipoServicioConstants.RX,
                "TOMO" or "TOMOGRAFIA" => Constants.TipoServicioConstants.Tomo,
                "INFORME" => Constants.TipoServicioConstants.Informe,
                _ => Constants.TipoServicioConstants.Insumo
            });

            var detalle = new DetalleServicioCuenta(Id, servicioId, descripcion, precio, honorario, cantidad, tipoServicio, usuarioCarga, legacyMappingId, areaClinicaId, null, resolvedTipoServicioId);
            _detalles.Add(detalle);
            return detalle;
        }

        public void RemoverServicio(Guid servicioId)
        {
            if (EstadoId != EstadoCajaConstants.AbiertaId)
                throw new InvalidOperationException("No se pueden remover servicios de una cuenta que no está abierta.");

            var detalle = _detalles.FirstOrDefault(d => d.ServicioId == servicioId);
            if (detalle != null)
            {
                _detalles.Remove(detalle);
            }
        }

        public void RemoverServicioPorDetalleId(Guid detalleId)
        {
            if (EstadoId != EstadoCajaConstants.AbiertaId)
                throw new InvalidOperationException("No se pueden remover servicios de una cuenta que no está abierta.");

            var detalle = _detalles.FirstOrDefault(d => d.Id == detalleId);
            if (detalle != null)
            {
                _detalles.Remove(detalle);
            }
        }

        public decimal CalcularTotal() => _detalles.Sum(d => d.Precio * d.Cantidad);

        public void RegistrarDestinoEgreso(string? destino, string? personalRelevo)
        {
            DestinoPaciente = destino;
            PersonalRelevo = personalRelevo;
        }

        /// <summary>3FN: cambio de estado por FK; sincroniza el alias legacy.</summary>
        private void SetEstado(int estadoId)
        {
            EstadoId = estadoId;
        }

        /// <summary>3FN: indica si la cuenta está abierta (fuente de verdad: EstadoId).</summary>
        public bool EstaAbierta => EstadoId == EstadoCuentaConstants.AbiertaId;

        public void Facturar()
        {
            if (EstadoId != EstadoCuentaConstants.AbiertaId)
                throw new InvalidOperationException("Solo se pueden facturar cuentas abiertas.");

            SetEstado(EstadoCuentaConstants.FacturadaId);
            FechaCierre = DateTime.UtcNow;

            // [PHASE-5] Raise Domain Event for downstream processing
            AddDomainEvent(new CuentaFacturadaEvent(Id, FechaCierre.Value));
        }

        public void Reabrir()
        {
            if (EstadoId != EstadoCuentaConstants.FacturadaId)
                throw new InvalidOperationException("Solo se pueden reabrir cuentas que hayan sido facturadas (check-out).");

            SetEstado(EstadoCuentaConstants.AbiertaId);
            FechaCierre = null;
        }

        public void Anular()
        {
            SetEstado(EstadoCuentaConstants.AnuladaId);
            FechaCierre = DateTime.UtcNow;
        }

        public void Validar(string usuario)
        {

            if (Guid.TryParse(usuario, out var parsedValidacion))
            {
                UsuarioValidacionId = parsedValidacion;
            }
            FechaValidacion = DateTime.UtcNow;
            SetEstado(EstadoCuentaConstants.ValidadaId);
        }

        /// <summary>3FN: variante con FK explícita al usuario de Identity.</summary>
        public void Validar(Guid usuarioId, string? usuarioNombreAlias = null)
        {
            if (usuarioId == Guid.Empty) throw new ArgumentException("El ID de usuario no puede ser vacío.", nameof(usuarioId));
            UsuarioValidacionId = usuarioId;
            FechaValidacion = DateTime.UtcNow;
            SetEstado(EstadoCuentaConstants.ValidadaId);
        }

        public void Auditar(string usuario)
        {
            if (Guid.TryParse(usuario, out var parsedAuditoria))
            {
                UsuarioAuditoriaId = parsedAuditoria;
            }
            FechaAuditoria = DateTime.UtcNow;
            // La auditoría puede ser un estado final o una marca sobre una cuenta facturada
        }

        /// <summary>3FN: variante con FK explícita al usuario de Identity.</summary>
        public void Auditar(Guid usuarioId, string? usuarioNombreAlias = null)
        {
            if (usuarioId == Guid.Empty) throw new ArgumentException("El ID de usuario no puede ser vacío.", nameof(usuarioId));
            UsuarioAuditoriaId = usuarioId;
            FechaAuditoria = DateTime.UtcNow;
        }

        public void AsignarLegacyOrder(int legacyOrderId)
        {
            LegacyOrderId = legacyOrderId;
            ProcesamientoEstado = EstadoConstants.ProcesamientoPendiente;
        }

        public void ActualizarProcesamiento(string nuevoEstado)
        {
            ProcesamientoEstado = nuevoEstado;
        }

        public void CambiarTipoIngresoAdministrativo(string tipoIngreso, int? convenioId)
        {
            if (tipoIngreso != EstadoConstants.Particular &&
                tipoIngreso != EstadoConstants.Seguro &&
                tipoIngreso != EstadoConstants.Hospitalizacion &&
                tipoIngreso != EstadoConstants.Emergencia)
            {
                throw new ArgumentException($"Tipo de ingreso inválido: {tipoIngreso}");
            }

            TipoIngresoId = TipoIngresoConstants.FromLegacyString(tipoIngreso);
            ConvenioId = convenioId;
        }

        /// <summary>3FN: variante con FK explícita al catálogo TiposIngreso.</summary>
        public void CambiarTipoIngresoAdministrativo(int tipoIngresoId, int? convenioId)
        {
            if (tipoIngresoId <= 0) throw new ArgumentException("El tipo de ingreso es obligatorio.", nameof(tipoIngresoId));
            TipoIngresoId = tipoIngresoId;
            ConvenioId = convenioId;
        }

        public void CambiarPacienteAdministrativo(Guid nuevoPacienteId)
        {
            if (nuevoPacienteId == Guid.Empty)
            {
                throw new ArgumentException("El ID del paciente no puede estar vacío.");
            }
            PacienteId = nuevoPacienteId;
        }
    }
}
