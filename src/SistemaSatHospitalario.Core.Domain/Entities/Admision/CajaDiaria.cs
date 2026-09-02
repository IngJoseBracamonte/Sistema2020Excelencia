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
        public string Estado { get; protected set; } // "Abierta", "CerradaPorAsistente" o "Cerrada"
        
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
            Estado = EstadoConstants.CajaAbierta;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UsuarioId = usuarioId;
            NombreUsuario = nombreUsuario;
#pragma warning restore CS0618
            // 3FN: poblar la FK si el texto es un GUID válido
            UsuarioIdentityId = Guid.TryParse(usuarioId, out var parsed) ? parsed : (Guid?)null;
        }

        public void CerrarCaja()
        {
            if (Estado == EstadoConstants.CajaCerrada) throw new InvalidOperationException("La caja ya se encuentra cerrada.");
            Estado = EstadoConstants.CajaCerrada;
            FechaCierre = DateTime.UtcNow;
        }

        public void CerrarPorAsistente(decimal totalIngresado, decimal totalCobrado, decimal diferencia)
        {
            if (Estado == EstadoConstants.CajaCerrada || Estado == EstadoConstants.CajaCerradaPorAsistente) 
                throw new InvalidOperationException("La caja ya se encuentra cerrada o en proceso de consolidación.");
            
            Estado = EstadoConstants.CajaCerradaPorAsistente;
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
            if (Estado == EstadoConstants.CajaCerrada) throw new InvalidOperationException("La caja ya se encuentra consolidada.");
            Estado = EstadoConstants.CajaCerrada;
        }
    }
}
