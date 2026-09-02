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

        /// <summary>
        /// LEGACY (3FN): texto del estado. Fuente de verdad: <see cref="EstadoId"/>
        /// (FK a EstadosCaja). Alias de compatibilidad hasta el DROP de columna.
        /// </summary>
        [Obsolete("Usar EstadoId / EstadoNav. Columna legacy pendiente de DROP.")]
        public string Estado { get; protected set; } // "Abierta", "CerradaPorAsistente" o "Cerrada"

        /// <summary>FK al catálogo EstadosCaja (3FN).</summary>
        public int EstadoId { get; protected set; }

        /// <summary>Navegación al catálogo de estados de caja.</summary>
        public virtual EstadoCaja EstadoNav { get; protected set; } = null!;
        
        // Identidad del Responsable (Micro-Ciclo 28)

        /// <summary>
        /// LEGACY (3FN): ID de usuario como texto. Fuente de verdad:
        /// <see cref="UsuarioIdentityId"/> (FK lógica a Usuarios, PK Guid). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar UsuarioIdentityId. Columna legacy pendiente de DROP.")]
        public string UsuarioId { get; protected set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del cajero responsable.</summary>
        public Guid? UsuarioIdentityId { get; protected set; }

        /// <summary>
        /// LEGACY (3FN): nombre desnormalizado del usuario. Se deriva de
        /// <see cref="UsuarioIdentityId"/> vía Identity. Alias hasta el DROP.
        /// </summary>
        [Obsolete("Derivar de UsuarioIdentityId vía Identity. Columna legacy pendiente de DROP.")]
        public string NombreUsuario { get; protected set; }

        // Campos de Auditoría y Cierre en 2 Fases (V13.0)

        /// <summary>
        /// LEGACY (3FN): JSON desnormalizado de declaración de cierre.
        /// Fuente de verdad: <see cref="DeclaracionesPorMetodo"/> (tabla CajasDeclaracionesMetodos).
        /// Se mantiene mapeado solo como fallback de lectura para cajas históricas
        /// hasta el DROP de columna (delta posterior a validación en producción).
        /// </summary>
        [Obsolete("Usar DeclaracionesPorMetodo. Columna legacy pendiente de DROP.")]
        public string? DeclaracionCierreJson { get; protected set; }
        public decimal? TotalIngresado { get; protected set; }
        public decimal? TotalCobrado { get; protected set; }
        public decimal? Diferencia { get; protected set; }
        public virtual ICollection<CajaDeclaracionMetodo> DeclaracionesPorMetodo { get; protected set; } = new List<CajaDeclaracionMetodo>();

        protected CajaDiaria() { }

        public CajaDiaria(decimal montoInicialDivisa, decimal montoInicialBs, string usuarioId, string nombreUsuario)
        {
            Id = Guid.NewGuid();
            FechaApertura = DateTime.UtcNow;
            MontoInicialDivisa = montoInicialDivisa;
            MontoInicialBs = montoInicialBs;
            EstadoId = EstadoCajaConstants.AbiertaId;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            Estado = EstadoConstants.CajaAbierta;
            UsuarioId = usuarioId;
            NombreUsuario = nombreUsuario;
#pragma warning restore CS0618
            // 3FN: poblar la FK si el texto es un GUID válido
            UsuarioIdentityId = Guid.TryParse(usuarioId, out var parsed) ? parsed : (Guid?)null;
        }

        /// <summary>3FN: cambio de estado por FK; sincroniza el alias legacy.</summary>
        private void SetEstado(int estadoId)
        {
            EstadoId = estadoId;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            Estado = EstadoCajaConstants.ToLegacyString(estadoId);
#pragma warning restore CS0618
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
#pragma warning disable CS0618 // limpieza del residuo legacy
            DeclaracionCierreJson = null;
#pragma warning restore CS0618
            TotalIngresado = totalIngresado;
            TotalCobrado = totalCobrado;
            Diferencia = diferencia;
        }

        public void ConsolidarCaja()
        {
            if (EstadoId == EstadoCajaConstants.CerradaId) throw new InvalidOperationException("La caja ya se encuentra consolidada.");
            SetEstado(EstadoCajaConstants.CerradaId);
        }
    }
}
