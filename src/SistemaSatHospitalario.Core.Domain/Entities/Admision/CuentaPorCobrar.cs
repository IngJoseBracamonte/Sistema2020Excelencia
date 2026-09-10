using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CuentaPorCobrar
    {
        public Guid Id { get; private set; }
        public Guid CuentaServicioId { get; private set; }
        public Guid PacienteId { get; private set; }
        public decimal MontoTotalBase { get; private set; }
        public decimal MontoPagadoBase { get; private set; }

        // ✅ Propiedad calculada dinámica (Ignorada automáticamente en persistencia)
        public decimal SaldoPendienteBase => MontoTotalBase - MontoPagadoBase;

        public DateTime FechaCreacion { get; private set; }
        public string Estado { get; private set; } // Pendiente, Parcial, Pagada / Cobrada
        public bool IsAudited { get; private set; }

        // Auditoría e Identidad (3FN Limpio)
        public Guid? UsuarioAuditoriaId { get; private set; }
        public string? UsuarioAuditoria { get; private set; } // Alias legacy opcional
        public DateTime? FechaAuditoria { get; private set; }

        public bool CompromisoGenerado { get; private set; }
        public bool GarantiaGenerada { get; private set; }
        
        public string? QuienAutorizo { get; private set; }
        public string? DoctorProcedimiento { get; private set; }
        public string? InformacionAdicional { get; private set; }

        // Colección de ítems de garantía prendaria (1:N)
        public virtual ICollection<GarantiaItem> GarantiasItems { get; private set; } = new List<GarantiaItem>();

        public virtual CuentaServicios Cuenta { get; private set; } = null!;

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

        public void MarcarComoAuditada(Guid usuarioId, string? usuarioNombreAlias = null)
        {
            if (usuarioId == Guid.Empty) throw new ArgumentException("El ID de usuario no puede ser vacío.", nameof(usuarioId));
            IsAudited = true;
            UsuarioAuditoriaId = usuarioId;
            UsuarioAuditoria = usuarioNombreAlias;
            FechaAuditoria = DateTime.UtcNow;
        }

        public void MarcarCompromisoGenerado() => CompromisoGenerado = true;

        public void MarcarGarantiaGenerada() => GarantiaGenerada = true;

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