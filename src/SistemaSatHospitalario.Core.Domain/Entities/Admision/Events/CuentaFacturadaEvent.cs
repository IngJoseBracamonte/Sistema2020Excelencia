using System;
using SistemaSatHospitalario.Core.Domain.Common;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision.Events
{
    /// <summary>
    /// [PHASE-5] Event raised when a hospital account is settled (Billed).
    /// </summary>
    public class CuentaFacturadaEvent : DomainEvent
    {
        public Guid CuentaId { get; }
        public DateTime FechaCierre { get; }

        public CuentaFacturadaEvent(Guid cuentaId, DateTime fechaCierre)
        {
            CuentaId = cuentaId;
            FechaCierre = fechaCierre;
        }
    }
}
