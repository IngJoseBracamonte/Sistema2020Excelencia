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
        public string? PacienteAnteriorNombre { get; private set; }
        public Guid? PacienteNuevoId { get; private set; }
        public string? PacienteNuevoNombre { get; private set; }

        // Tipo de ingreso/Convenio anterior y nuevo
        public string? TipoIngresoAnterior { get; private set; }
        public string? TipoIngresoNuevo { get; private set; }
        public int? ConvenioAnteriorId { get; private set; }
        public string? ConvenioAnteriorNombre { get; private set; }
        public int? ConvenioNuevoId { get; private set; }
        public string? ConvenioNuevoNombre { get; private set; }

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

        // Detalle en JSON de cambios de precios de servicios
        public string? DetalleServiciosCambiosJson { get; private set; }

        protected HistorialModificacionCuenta() { }

        public HistorialModificacionCuenta(
            Guid cuentaServicioId,
            string usuario,
            Guid? pacienteAnteriorId,
            string? pacienteAnteriorNombre,
            Guid? pacienteNuevoId,
            string? pacienteNuevoNombre,
            string? tipoIngresoAnterior,
            string? tipoIngresoNuevo,
            int? convenioAnteriorId,
            string? convenioAnteriorNombre,
            int? convenioNuevoId,
            string? convenioNuevoNombre,
            decimal totalAnteriorUSD,
            decimal totalNuevoUSD,
            decimal reciboTotalAnteriorUSD,
            decimal reciboTotalNuevoUSD,
            decimal reciboVueltoAnteriorUSD,
            decimal reciboVueltoNuevoUSD,
            decimal reciboPagadoUSD,
            decimal cxcSaldoAnteriorUSD,
            decimal cxcSaldoNuevoUSD,
            string? detalleServiciosCambiosJson)
        {
            Id = Guid.NewGuid();
            CuentaServicioId = cuentaServicioId;
            FechaModificacion = DateTime.UtcNow;
            Usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
            
            PacienteAnteriorId = pacienteAnteriorId;
            PacienteAnteriorNombre = pacienteAnteriorNombre;
            PacienteNuevoId = pacienteNuevoId;
            PacienteNuevoNombre = pacienteNuevoNombre;

            TipoIngresoAnterior = tipoIngresoAnterior;
            TipoIngresoNuevo = tipoIngresoNuevo;
            ConvenioAnteriorId = convenioAnteriorId;
            ConvenioAnteriorNombre = convenioAnteriorNombre;
            ConvenioNuevoId = convenioNuevoId;
            ConvenioNuevoNombre = convenioNuevoNombre;

            TotalAnteriorUSD = totalAnteriorUSD;
            TotalNuevoUSD = totalNuevoUSD;

            ReciboTotalAnteriorUSD = reciboTotalAnteriorUSD;
            ReciboTotalNuevoUSD = reciboTotalNuevoUSD;
            ReciboVueltoAnteriorUSD = reciboVueltoAnteriorUSD;
            ReciboVueltoNuevoUSD = reciboVueltoNuevoUSD;
            ReciboPagadoUSD = reciboPagadoUSD;

            CxCSaldoAnteriorUSD = cxcSaldoAnteriorUSD;
            CxCSaldoNuevoUSD = cxcSaldoNuevoUSD;

            DetalleServiciosCambiosJson = detalleServiciosCambiosJson;
        }
    }
}
