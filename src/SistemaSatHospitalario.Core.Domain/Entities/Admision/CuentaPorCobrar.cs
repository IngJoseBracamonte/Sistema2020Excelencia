using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CuentaPorCobrar
    {
        public Guid Id { get; private set; }
        public Guid CuentaServicioId { get; private set; }
        // Se cambió de Guid a int para sincronización con Legacy
        public int PacienteId { get; private set; }
        public decimal MontoTotalBase { get; private set; }
        public decimal MontoPagadoBase { get; private set; }
        public decimal SaldoPendienteBase { get; private set; }
        public DateTime FechaEmision { get; private set; }
        public DateTime FechaCreacion => FechaEmision;
        public string Estado { get; private set; } // Pendiente, Cobrada, Anulada
        public decimal TotalCargadoBase => MontoTotalBase;
        public ICollection<DetallePago> Abonos { get; private set; } = new List<DetallePago>();

        public virtual CuentaServicios Cuenta { get; private set; }

        private CuentaPorCobrar() { }

        public CuentaPorCobrar(Guid cuentaId, int pacienteId, decimal total, decimal pagado)
        {
            if (pagado >= total) throw new ArgumentException("No se puede crear una cuenta por cobrar si el pago cubre el total.");
            
            Id = Guid.NewGuid();
            CuentaServicioId = cuentaId;
            PacienteId = pacienteId;
            MontoTotalBase = total;
            MontoPagadoBase = pagado;
            SaldoPendienteBase = total - pagado;
            FechaEmision = DateTime.UtcNow;
            Estado = "Pendiente";
        }

        public void MarcarComoCobrada()
        {
            Estado = "Cobrada";
            SaldoPendienteBase = 0;
            MontoPagadoBase = MontoTotalBase;
        }
    }
}
