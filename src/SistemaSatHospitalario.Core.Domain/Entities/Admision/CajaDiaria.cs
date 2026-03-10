using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CajaDiaria
    {
        public Guid Id { get; protected set; }
        public DateTime FechaApertura { get; protected set; }
        public DateTime? FechaCierre { get; protected set; }
        public decimal MontoInicialDivisa { get; protected set; }
        public decimal MontoInicialBs { get; protected set; }
        public string Estado { get; protected set; } // "Abierta" o "Cerrada"

        protected CajaDiaria() { }

        public CajaDiaria(decimal montoInicialDivisa, decimal montoInicialBs)
        {
            Id = Guid.NewGuid();
            FechaApertura = DateTime.UtcNow;
            MontoInicialDivisa = montoInicialDivisa;
            MontoInicialBs = montoInicialBs;
            Estado = "Abierta";
        }

        public void CerrarCaja()
        {
            if (Estado == "Cerrada") throw new InvalidOperationException("La caja ya se encuentra cerrada.");
            Estado = "Cerrada";
            FechaCierre = DateTime.UtcNow;
        }
    }
}
