using System;
using System.Collections.Generic;
using System.Linq;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class ReciboFactura
    {
        public Guid Id { get; protected set; }
        public Guid CuentaServicioId { get; protected set; }
        public Guid? CajaDiariaId { get; protected set; }
        public string? NroControlFiscal { get; protected set; }
        public decimal TasaCambioDia { get; protected set; }
        public int EstadoFiscalId { get; protected set; }
        public virtual EstadoFiscal EstadoFiscalNav { get; protected set; } = null!;
        public Guid PacienteId { get; protected set; }
        public string NumeroRecibo { get; protected set; }
        public string? NumeroComprobante { get; protected set; }
        public decimal TotalFacturadoUSD { get; protected set; }
        public decimal MontoVueltoUSD { get; protected set; } // Pachón Pro V11.2 Change Support
        public decimal TasaBcvUsada => TasaCambioDia;
        public DateTime FechaEmision { get; protected set; }

        public Guid? UsuarioEmisionId { get; protected set; }

        public CuentaServicios CuentaServicio { get; protected set; }
        public CajaDiaria CajaDiaria { get; protected set; }

        private readonly List<DetallePago> _detallesPago = new();
        public IReadOnlyCollection<DetallePago> DetallesPago => _detallesPago.AsReadOnly();

        protected ReciboFactura() { }

        public ReciboFactura(Guid cuentaServicioId, Guid pacienteId, Guid? cajaDiariaId, decimal tasaCambioDia, decimal totalFacturadoUSD, decimal montoVueltoUSD = 0, string estadoFiscal = EstadoConstants.Borrador, string? numeroComprobante = null)
        {
            Id = Guid.NewGuid();
            CuentaServicioId = cuentaServicioId;
            PacienteId = pacienteId;
            CajaDiariaId = cajaDiariaId;
            TasaCambioDia = tasaCambioDia;
            TotalFacturadoUSD = totalFacturadoUSD;
            MontoVueltoUSD = montoVueltoUSD;
            EstadoFiscalId = EstadoFiscalConstants.FromLegacyString(estadoFiscal);
            FechaEmision = DateTime.UtcNow;
            NumeroRecibo = $"REC-{DateTime.Now:yyyyMMdd}-{Id.ToString().Substring(0, 8)}";
            NumeroComprobante = numeroComprobante;
        }

        /// <summary>3FN: cambio de estado fiscal por FK; sincroniza el alias legacy.</summary>
        private void SetEstadoFiscal(int estadoFiscalId)
        {
            EstadoFiscalId = estadoFiscalId;
        }

        /// <summary>3FN: indica si el recibo está en borrador (fuente de verdad: EstadoFiscalId).</summary>
        public bool EsBorrador => EstadoFiscalId == EstadoFiscalConstants.BorradorId;

        /// <summary>3FN: indica si el recibo está anulado (fuente de verdad: EstadoFiscalId).</summary>
        public bool EsAnulada => EstadoFiscalId == EstadoFiscalConstants.AnuladaId;

        public void Emitir(string nroControlFiscal, Guid? usuarioEmision)
        {
            if (EstadoFiscalId != EstadoFiscalConstants.BorradorId) throw new InvalidOperationException("Solo los borradores pueden emitirse como facturas fiscales.");
            NroControlFiscal = nroControlFiscal ?? throw new ArgumentNullException(nameof(nroControlFiscal));
            if (usuarioEmision.HasValue)
            {
                UsuarioEmisionId = usuarioEmision.Value;
            }
            SetEstadoFiscal(EstadoFiscalConstants.EmitidaId);
        }

        /// <summary>3FN: variante con FK explícita al usuario de Identity.</summary>
        public void Emitir(string nroControlFiscal, Guid usuarioEmisionId, string? usuarioNombreAlias = null)
        {
            if (EstadoFiscalId != EstadoFiscalConstants.BorradorId) throw new InvalidOperationException("Solo los borradores pueden emitirse como facturas fiscales.");
            if (usuarioEmisionId == Guid.Empty) throw new ArgumentException("El ID de usuario no puede ser vacío.", nameof(usuarioEmisionId));
            NroControlFiscal = nroControlFiscal ?? throw new ArgumentNullException(nameof(nroControlFiscal));
            UsuarioEmisionId = usuarioEmisionId;
            SetEstadoFiscal(EstadoFiscalConstants.EmitidaId);
        }

        public void Anular()
        {
            SetEstadoFiscal(EstadoFiscalConstants.AnuladaId);
        }

        public void AgregarDetallePago(string metodoPago, Guid metodoPagoId, string referencia, decimal montoCambiario, decimal equivalenteBase, decimal tasaCambioAplicada = 1.0m,Guid? usuarioCarga = null)
        {
            if (EstadoFiscalId == EstadoFiscalConstants.AnuladaId) throw new InvalidOperationException("No se pueden agregar pagos a un recibo anulado.");
            if (metodoPagoId == Guid.Empty) throw new ArgumentException("El método de pago es requerido.", nameof(metodoPagoId));
            _detallesPago.Add(new DetallePago(Id, metodoPago, referencia, montoCambiario, equivalenteBase, tasaCambioAplicada, usuarioCarga, metodoPagoId));
        }

        public decimal ObtenerTotalPagadoBase() => _detallesPago.Sum(p => p.EquivalenteAbonadoBase);

        public void CambiarPacienteAdministrativo(Guid nuevoPacienteId)
        {
            if (nuevoPacienteId == Guid.Empty) throw new ArgumentException("El PacienteId no puede ser vacío.");
            PacienteId = nuevoPacienteId;
        }

        public void ActualizarTotalesAdministrativos(decimal nuevoTotalFacturado, decimal nuevoMontoVuelto)
        {
            TotalFacturadoUSD = nuevoTotalFacturado;
            MontoVueltoUSD = nuevoMontoVuelto;
        }
    }
}
