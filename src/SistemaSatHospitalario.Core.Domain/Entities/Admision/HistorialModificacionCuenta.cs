using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class HistorialModificacionCuenta
    {
        public Guid Id { get; private set; }
        public Guid CuentaServicioId { get; private set; }
        public DateTime FechaModificacion { get; private set; }
        public string Usuario { get; private set; }
        
        // Paciente anterior y nuevo
        public Guid? PacienteAnteriorId { get; private set; }

        public Guid? PacienteNuevoId { get; private set; }

        // Tipo de ingreso/Convenio anterior y nuevo
        public string? TipoIngresoAnterior { get; private set; }
        public string? TipoIngresoNuevo { get; private set; }
        public int? ConvenioAnteriorId { get; private set; }

        public int? ConvenioNuevoId { get; private set; }

        // Totales de la cuenta anterior y nuevo
        public decimal TotalAnteriorUSD { get; private set; }
        public decimal TotalNuevoUSD { get; private set; }

        // Totales de recibo (pagado y vuelto) anterior y nuevo
        public decimal ReciboTotalAnteriorUSD { get; private set; }
        public decimal ReciboTotalNuevoUSD { get; private set; }
        public decimal ReciboVueltoAnteriorUSD { get; private set; }
        public decimal ReciboVueltoNuevoUSD { get; private set; }
        public decimal ReciboPagadoUSD { get; private set; } // El monto pagado/ingresado

        // Cuentas por Cobrar saldo anterior y nuevo
        public decimal CxCSaldoAnteriorUSD { get; private set; }
        public decimal CxCSaldoNuevoUSD { get; private set; }

        public virtual ICollection<HistorialModificacionCuentaDetalle> DetallesModificados { get; private set; } = new List<HistorialModificacionCuentaDetalle>();

        protected HistorialModificacionCuenta() { }

        public HistorialModificacionCuenta(
            Guid cuentaServicioId,
            string usuario,
            Guid? pacienteAnteriorId,
            Guid? pacienteNuevoId,
            string? tipoIngresoAnterior,
            string? tipoIngresoNuevo,
            int? convenioAnteriorId,
            int? convenioNuevoId,
            decimal totalAnteriorUSD,
            decimal totalNuevoUSD,
            decimal reciboTotalAnteriorUSD,
            decimal reciboTotalNuevoUSD,
            decimal reciboVueltoAnteriorUSD,
            decimal reciboVueltoNuevoUSD,
            decimal reciboPagadoUSD,
            decimal cxcSaldoAnteriorUSD,
            decimal cxcSaldoNuevoUSD)
        {
            Id = Guid.NewGuid();
            CuentaServicioId = cuentaServicioId;
            FechaModificacion = DateTime.UtcNow;
            Usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
            
            PacienteAnteriorId = pacienteAnteriorId;
            PacienteNuevoId = pacienteNuevoId;

            TipoIngresoAnterior = tipoIngresoAnterior;
            TipoIngresoNuevo = tipoIngresoNuevo;
            ConvenioAnteriorId = convenioAnteriorId;
            ConvenioNuevoId = convenioNuevoId;

            TotalAnteriorUSD = totalAnteriorUSD;
            TotalNuevoUSD = totalNuevoUSD;

            ReciboTotalAnteriorUSD = reciboTotalAnteriorUSD;
            ReciboTotalNuevoUSD = reciboTotalNuevoUSD;
            ReciboVueltoAnteriorUSD = reciboVueltoAnteriorUSD;
            ReciboVueltoNuevoUSD = reciboVueltoNuevoUSD;
            ReciboPagadoUSD = reciboPagadoUSD;

            CxCSaldoAnteriorUSD = cxcSaldoAnteriorUSD;
            CxCSaldoNuevoUSD = cxcSaldoNuevoUSD;

        }
    }
}
