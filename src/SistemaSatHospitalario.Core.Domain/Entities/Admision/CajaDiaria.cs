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
        
        // Identidad del Responsable (Micro-Ciclo 28)
        public string UsuarioId { get; protected set; }
        public string NombreUsuario { get; protected set; }

        protected CajaDiaria() { }

        public CajaDiaria(decimal montoInicialDivisa, decimal montoInicialBs, string usuarioId, string nombreUsuario)
        {
            Id = Guid.NewGuid();
            FechaApertura = DateTime.UtcNow;
            MontoInicialDivisa = montoInicialDivisa;
            MontoInicialBs = montoInicialBs;
            Estado = "Abierta";
            UsuarioId = usuarioId;
            NombreUsuario = nombreUsuario;
        }

        public void CerrarCaja()
        {
            if (Estado == "Cerrada") throw new InvalidOperationException("La caja ya se encuentra cerrada.");
            Estado = "Cerrada";
            FechaCierre = DateTime.UtcNow;
        }
    }
}
