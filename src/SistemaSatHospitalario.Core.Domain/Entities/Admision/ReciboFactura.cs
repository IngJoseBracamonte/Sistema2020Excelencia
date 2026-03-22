using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class ReciboFactura
    {
        public Guid Id { get; protected set; }
        public Guid CuentaServicioId { get; protected set; }
        public Guid? CajaDiariaId { get; protected set; }
        public string NroControlFiscal { get; protected set; }
        public decimal TasaCambioDia { get; protected set; }
        public string EstadoFiscal { get; protected set; } // Borrador, Emitida, Anulada
        public int PacienteId { get; protected set; }
        public string NumeroRecibo { get; protected set; }
        public decimal TotalFacturadoUSD { get; protected set; }
        public decimal TasaBcvUsada => TasaCambioDia;
        public DateTime FechaEmision { get; protected set; }
        public string Estado => EstadoFiscal;

        public CuentaServicios CuentaServicio { get; protected set; }
        public CajaDiaria CajaDiaria { get; protected set; }

        private readonly List<DetallePago> _detallesPago = new();
        public IReadOnlyCollection<DetallePago> DetallesPago => _detallesPago.AsReadOnly();

        protected ReciboFactura() { }

        public ReciboFactura(Guid cuentaServicioId, int pacienteId, Guid? cajaDiariaId, decimal tasaCambioDia, string estadoFiscal = "Borrador")
        {
            Id = Guid.NewGuid();
            CuentaServicioId = cuentaServicioId;
            PacienteId = pacienteId;
            CajaDiariaId = cajaDiariaId;
            TasaCambioDia = tasaCambioDia;
            EstadoFiscal = estadoFiscal;
            FechaEmision = DateTime.UtcNow;
            NumeroRecibo = $"REC-{DateTime.Now:yyyyMMdd}-{Id.ToString().Substring(0, 8)}";
        }

        public void Emitir(string nroControlFiscal)
        {
            if (EstadoFiscal != "Borrador") throw new InvalidOperationException("Solo los borradores pueden emitirse como facturas fiscales.");
            NroControlFiscal = nroControlFiscal ?? throw new ArgumentNullException(nameof(nroControlFiscal));
            EstadoFiscal = "Emitida";
        }

        public void Anular()
        {
            EstadoFiscal = "Anulada";
        }

        public void AgregarDetallePago(string metodoPago, string referencia, decimal montoCambiario, decimal equivalenteBase)
        {
            if (EstadoFiscal != "Borrador") throw new InvalidOperationException("No se pueden agregar pagos a un recibo ya emitido o anulado.");
            _detallesPago.Add(new DetallePago(Id, metodoPago, referencia, montoCambiario, equivalenteBase));
        }

        public decimal ObtenerTotalPagadoBase() => _detallesPago.Sum(p => p.EquivalenteAbonadoBase);
    }
}
