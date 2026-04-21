using System;
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
        public bool CompromisoGenerado { get; private set; }

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

        public void MarcarComoAuditada()
        {
            IsAudited = true;
        }

        public void MarcarCompromisoGenerado()
        {
            CompromisoGenerado = true;
        }
    }
}
