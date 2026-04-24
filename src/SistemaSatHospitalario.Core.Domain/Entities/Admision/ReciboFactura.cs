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
        public string EstadoFiscal { get; protected set; } // Borrador, Emitida, Anulada
        public Guid PacienteId { get; protected set; }
        public string NumeroRecibo { get; protected set; }
        public decimal TotalFacturadoUSD { get; protected set; }
        public decimal MontoVueltoUSD { get; protected set; } // Pachón Pro V11.2 Change Support
        public decimal TasaBcvUsada => TasaCambioDia;
        public DateTime FechaEmision { get; protected set; }
        public string? UsuarioEmision { get; protected set; }
        public string Estado => EstadoFiscal;

        public CuentaServicios CuentaServicio { get; protected set; }
        public CajaDiaria CajaDiaria { get; protected set; }

        private readonly List<DetallePago> _detallesPago = new();
        public IReadOnlyCollection<DetallePago> DetallesPago => _detallesPago.AsReadOnly();

        protected ReciboFactura() { }

        public ReciboFactura(Guid cuentaServicioId, Guid pacienteId, Guid? cajaDiariaId, decimal tasaCambioDia, decimal totalFacturadoUSD, decimal montoVueltoUSD = 0, string estadoFiscal = EstadoConstants.Borrador)
        {
            Id = Guid.NewGuid();
            CuentaServicioId = cuentaServicioId;
            PacienteId = pacienteId;
            CajaDiariaId = cajaDiariaId;
            TasaCambioDia = tasaCambioDia;
            TotalFacturadoUSD = totalFacturadoUSD;
            MontoVueltoUSD = montoVueltoUSD;
            EstadoFiscal = estadoFiscal;
            FechaEmision = DateTime.UtcNow;
            NumeroRecibo = $"REC-{DateTime.Now:yyyyMMdd}-{Id.ToString().Substring(0, 8)}";
        }

        public void Emitir(string nroControlFiscal, string usuarioEmision)
        {
            if (EstadoFiscal != EstadoConstants.Borrador) throw new InvalidOperationException("Solo los borradores pueden emitirse como facturas fiscales.");
            NroControlFiscal = nroControlFiscal ?? throw new ArgumentNullException(nameof(nroControlFiscal));
            UsuarioEmision = usuarioEmision ?? throw new ArgumentNullException(nameof(usuarioEmision));
            EstadoFiscal = EstadoConstants.Emitida;
        }

        public void Anular()
        {
            EstadoFiscal = EstadoConstants.Anulada;
        }

        public void AgregarDetallePago(string metodoPago, string referencia, decimal montoCambiario, decimal equivalenteBase, string usuarioCarga)
        {
            if (EstadoFiscal == EstadoConstants.Anulada) throw new InvalidOperationException("No se pueden agregar pagos a un recibo anulado.");
            _detallesPago.Add(new DetallePago(Id, metodoPago, referencia, montoCambiario, equivalenteBase, usuarioCarga));
        }

        public decimal ObtenerTotalPagadoBase() => _detallesPago.Sum(p => p.EquivalenteAbonadoBase);
    }
}
