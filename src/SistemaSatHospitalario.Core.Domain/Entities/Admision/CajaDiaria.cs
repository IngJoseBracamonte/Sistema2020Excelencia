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
        public string Estado { get; protected set; } // "Abierta", "CerradaPorAsistente" o "Cerrada"
        
        // Identidad del Responsable (Micro-Ciclo 28)
        public string UsuarioId { get; protected set; }
        public string NombreUsuario { get; protected set; }

        // Campos de Auditoría y Cierre en 2 Fases (V13.0)
        public string? DeclaracionCierreJson { get; protected set; }
        public decimal? TotalIngresado { get; protected set; }
        public decimal? TotalCobrado { get; protected set; }
        public decimal? Diferencia { get; protected set; }

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

        public void CerrarPorAsistente(string declaracionJson, decimal totalIngresado, decimal totalCobrado, decimal diferencia)
        {
            if (Estado == "Cerrada" || Estado == "CerradaPorAsistente") 
                throw new InvalidOperationException("La caja ya se encuentra cerrada o en proceso de consolidación.");
            
            Estado = "CerradaPorAsistente";
            FechaCierre = DateTime.UtcNow;
            DeclaracionCierreJson = declaracionJson;
            TotalIngresado = totalIngresado;
            TotalCobrado = totalCobrado;
            Diferencia = diferencia;
        }

        public void ConsolidarCaja()
        {
            if (Estado == "Cerrada") throw new InvalidOperationException("La caja ya se encuentra consolidada.");
            Estado = "Cerrada";
        }
    }
}
