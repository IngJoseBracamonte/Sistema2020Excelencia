using System;
using System.Collections.Generic;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CajaDiaria
    {
        public Guid Id { get; protected set; }
        public DateTime FechaApertura { get; protected set; }
        public DateTime? FechaCierre { get; protected set; }
        public decimal MontoInicialDivisa { get; protected set; }
        public decimal MontoInicialBs { get; protected set; }

        /// <summary>FK al catálogo EstadosCaja (3FN).</summary>
        public int EstadoId { get; protected set; }

        /// <summary>Navegación al catálogo de estados de caja.</summary>
        public virtual EstadoCaja EstadoNav { get; protected set; } = null!;
        
        public Guid? UsuarioIdentityId { get; protected set; }

        public virtual ICollection<CajaDeclaracionMetodo> DeclaracionesPorMetodo { get; protected set; } = new List<CajaDeclaracionMetodo>();

        protected CajaDiaria() { }

        public CajaDiaria(decimal montoInicialDivisa, decimal montoInicialBs, string usuarioId, string nombreUsuario)
        {
            Id = Guid.NewGuid();
            FechaApertura = DateTime.UtcNow;
            MontoInicialDivisa = montoInicialDivisa;
            MontoInicialBs = montoInicialBs;
            EstadoId = EstadoCajaConstants.AbiertaId;
            // 3FN: poblar la FK si el texto es un GUID válido
            UsuarioIdentityId = Guid.TryParse(usuarioId, out var parsed) ? parsed : (Guid?)null;
        }

        /// <summary>3FN: cambio de estado por FK; sincroniza el alias legacy.</summary>
        private void SetEstado(int estadoId)
        {
            EstadoId = estadoId;
        }

        /// <summary>3FN: indica si la caja está abierta (fuente de verdad: EstadoId).</summary>
        public bool EstaAbierta => EstadoId == EstadoCajaConstants.AbiertaId;

        public void CerrarCaja()
        {
            if (EstadoId == EstadoCajaConstants.CerradaId) throw new InvalidOperationException("La caja ya se encuentra cerrada.");
            SetEstado(EstadoCajaConstants.CerradaId);
            FechaCierre = DateTime.UtcNow;
        }

        public void CerrarPorAsistente(decimal totalIngresado, decimal totalCobrado, decimal diferencia)
        {
            if (EstadoId == EstadoCajaConstants.CerradaId || EstadoId == EstadoCajaConstants.CerradaPorAsistenteId)
                throw new InvalidOperationException("La caja ya se encuentra cerrada o en proceso de consolidación.");

            SetEstado(EstadoCajaConstants.CerradaPorAsistenteId);
            FechaCierre = DateTime.UtcNow;
        }

        public void ConsolidarCaja()
        {
            if (EstadoId == EstadoCajaConstants.CerradaId) throw new InvalidOperationException("La caja ya se encuentra consolidada.");
            SetEstado(EstadoCajaConstants.CerradaId);
        }
    }
}
