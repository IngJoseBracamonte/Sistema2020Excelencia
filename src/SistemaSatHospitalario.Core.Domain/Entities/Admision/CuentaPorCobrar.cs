using System;
using System.Collections.Generic;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CuentaPorCobrar
    {
        public Guid Id { get; private set; }
        public Guid CuentaServicioId { get; private set; }
        // Se cambió de int a Guid para el nuevo sistema de identidad (V11.0 Sync Pro)
        public Guid PacienteId { get; private set; }
        public decimal MontoTotalBase { get; private set; }
        public decimal MontoPagadoBase { get; private set; }
        public decimal SaldoPendienteBase => MontoTotalBase - MontoPagadoBase;
        public DateTime FechaCreacion { get; private set; }
        public string Estado { get; private set; } // Pendiente, Parcial, Pagada
        public bool IsAudited { get; private set; }

        /// <summary>
        /// LEGACY (3FN): username en texto plano. Fuente de verdad: <see cref="UsuarioAuditoriaId"/>
        /// (FK lógica a Usuarios, PK Guid). Se mantiene mapeado como alias de compatibilidad
        /// hasta el DROP de columna (delta posterior a validación en producción).
        /// </summary>
        [Obsolete("Usar UsuarioAuditoriaId. Columna legacy pendiente de DROP.")]
        public string? UsuarioAuditoria { get; private set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario que auditó.</summary>
        public Guid? UsuarioAuditoriaId { get; private set; }
        public DateTime? FechaAuditoria { get; private set; }
        public bool CompromisoGenerado { get; private set; }
        public bool GarantiaGenerada { get; private set; }
        
        public string? QuienAutorizo { get; private set; }
        public string? DoctorProcedimiento { get; private set; }
        public string? InformacionAdicional { get; private set; }

        // [V12.8] Colección de ítems de garantía prendaria (1:N)
        public ICollection<GarantiaItem> GarantiasItems { get; private set; } = new List<GarantiaItem>();

        public CuentaServicios Cuenta { get; private set; }

        protected CuentaPorCobrar() { }

        public CuentaPorCobrar(Guid cuentaId, Guid pacienteId, decimal total, decimal pagado)
        {
            Id = Guid.NewGuid();
            CuentaServicioId = cuentaId;
            PacienteId = pacienteId;
            MontoTotalBase = total;
            MontoPagadoBase = pagado;
            FechaCreacion = DateTime.UtcNow;
            Estado = EstadoConstants.Pendiente;
            IsAudited = false;
        }

        public void MarcarComoCobrada()
        {
            Estado = EstadoConstants.Cobrada;
            MontoPagadoBase = MontoTotalBase;
        }

        public void MarcarComoAuditada(string usuario)
        {
            IsAudited = true;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UsuarioAuditoria = usuario ?? throw new ArgumentNullException(nameof(usuario));
#pragma warning restore CS0618
            // 3FN: si el valor es un GUID válido, poblar también la FK
            UsuarioAuditoriaId = Guid.TryParse(usuario, out var parsed) ? parsed : UsuarioAuditoriaId;
            FechaAuditoria = DateTime.UtcNow;
        }

        /// <summary>3FN: variante con FK explícita al usuario de Identity.</summary>
        public void MarcarComoAuditada(Guid usuarioId, string? usuarioNombreAlias = null)
        {
            if (usuarioId == Guid.Empty) throw new ArgumentException("El ID de usuario no puede ser vacío.", nameof(usuarioId));
            IsAudited = true;
            UsuarioAuditoriaId = usuarioId;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            if (!string.IsNullOrWhiteSpace(usuarioNombreAlias))
            {
                UsuarioAuditoria = usuarioNombreAlias;
            }
#pragma warning restore CS0618
            FechaAuditoria = DateTime.UtcNow;
        }

        public void MarcarCompromisoGenerado()
        {
            CompromisoGenerado = true;
        }

        public void MarcarGarantiaGenerada()
        {
            GarantiaGenerada = true;
        }

        public void ActualizarMetadataDocumentos(string? quienAutorizo, string? doctorProcedimiento, string? informacionAdicional)
        {
            QuienAutorizo = quienAutorizo;
            DoctorProcedimiento = doctorProcedimiento;
            InformacionAdicional = informacionAdicional;
        }

        public void CambiarPacienteAdministrativo(Guid nuevoPacienteId)
        {
            if (nuevoPacienteId == Guid.Empty) throw new ArgumentException("El PacienteId no puede ser vacío.");
            PacienteId = nuevoPacienteId;
        }

        public void ActualizarMontoTotalAdministrativo(decimal nuevoTotal)
        {
            MontoTotalBase = nuevoTotal;
            if (MontoPagadoBase >= MontoTotalBase)
            {
                Estado = EstadoConstants.Cobrada;
            }
            else if (MontoPagadoBase > 0)
            {
                Estado = EstadoConstants.Parcial;
            }
            else
            {
                Estado = EstadoConstants.Pendiente;
            }
        }
    }
}
