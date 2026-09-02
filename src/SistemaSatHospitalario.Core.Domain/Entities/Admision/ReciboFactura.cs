using System;
using System.Collections.Generic;
using System.Linq;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class ReciboFactura
    {
        public Guid Id { get; protected set; }
        public Guid CuentaServicioId { get; protected set; }
        public Guid? CajaDiariaId { get; protected set; }
        public string? NroControlFiscal { get; protected set; }
        public decimal TasaCambioDia { get; protected set; }

        /// <summary>
        /// LEGACY (3FN): texto del estado fiscal. Fuente de verdad: <see cref="EstadoFiscalId"/>
        /// (FK a EstadosFiscales). Alias de compatibilidad hasta el DROP de columna.
        /// </summary>
        [Obsolete("Usar EstadoFiscalId / EstadoFiscalNav. Columna legacy pendiente de DROP.")]
        public string EstadoFiscal { get; protected set; } // Borrador, Emitida, Anulada

        /// <summary>FK al catálogo EstadosFiscales (3FN).</summary>
        public int EstadoFiscalId { get; protected set; }

        /// <summary>Navegación al catálogo de estados fiscales.</summary>
        public virtual EstadoFiscal EstadoFiscalNav { get; protected set; } = null!;
        public Guid PacienteId { get; protected set; }
        public string NumeroRecibo { get; protected set; }
        public string? NumeroComprobante { get; protected set; }
        public decimal TotalFacturadoUSD { get; protected set; }
        public decimal MontoVueltoUSD { get; protected set; } // Pachón Pro V11.2 Change Support
        public decimal TasaBcvUsada => TasaCambioDia;
        public DateTime FechaEmision { get; protected set; }

        /// <summary>
        /// LEGACY (3FN): nombre de usuario en texto plano. Fuente de verdad:
        /// <see cref="UsuarioEmisionId"/> (FK lógica a Usuarios, PK Guid). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar UsuarioEmisionId. Columna legacy pendiente de DROP.")]
        public string? UsuarioEmision { get; protected set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario que emitió el recibo.</summary>
        public Guid? UsuarioEmisionId { get; protected set; }

        /// <summary>Alias de lectura del estado fiscal (texto legacy).</summary>
        public string Estado => EstadoFiscal;

        public CuentaServicios CuentaServicio { get; protected set; }
        public CajaDiaria CajaDiaria { get; protected set; }

        private readonly List<DetallePago> _detallesPago = new();
        public IReadOnlyCollection<DetallePago> DetallesPago => _detallesPago.AsReadOnly();

        protected ReciboFactura() { }

        public ReciboFactura(Guid cuentaServicioId, Guid pacienteId, Guid? cajaDiariaId, decimal tasaCambioDia, decimal totalFacturadoUSD, decimal montoVueltoUSD = 0, string estadoFiscal = EstadoConstants.Borrador, string? numeroComprobante = null)
        {
            Id = Guid.NewGuid();
            CuentaServicioId = cuentaServicioId;
            PacienteId = pacienteId;
            CajaDiariaId = cajaDiariaId;
            TasaCambioDia = tasaCambioDia;
            TotalFacturadoUSD = totalFacturadoUSD;
            MontoVueltoUSD = montoVueltoUSD;
            EstadoFiscalId = EstadoFiscalConstants.FromLegacyString(estadoFiscal);
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            EstadoFiscal = estadoFiscal;
#pragma warning restore CS0618
            FechaEmision = DateTime.UtcNow;
            NumeroRecibo = $"REC-{DateTime.Now:yyyyMMdd}-{Id.ToString().Substring(0, 8)}";
            NumeroComprobante = numeroComprobante;
        }

        /// <summary>3FN: cambio de estado fiscal por FK; sincroniza el alias legacy.</summary>
        private void SetEstadoFiscal(int estadoFiscalId)
        {
            EstadoFiscalId = estadoFiscalId;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            EstadoFiscal = EstadoFiscalConstants.ToLegacyString(estadoFiscalId);
#pragma warning restore CS0618
        }

        /// <summary>3FN: indica si el recibo está en borrador (fuente de verdad: EstadoFiscalId).</summary>
        public bool EsBorrador => EstadoFiscalId == EstadoFiscalConstants.BorradorId;

        /// <summary>3FN: indica si el recibo está anulado (fuente de verdad: EstadoFiscalId).</summary>
        public bool EsAnulada => EstadoFiscalId == EstadoFiscalConstants.AnuladaId;

        public void Emitir(string nroControlFiscal, string usuarioEmision)
        {
            if (EstadoFiscalId != EstadoFiscalConstants.BorradorId) throw new InvalidOperationException("Solo los borradores pueden emitirse como facturas fiscales.");
            NroControlFiscal = nroControlFiscal ?? throw new ArgumentNullException(nameof(nroControlFiscal));
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UsuarioEmision = usuarioEmision ?? throw new ArgumentNullException(nameof(usuarioEmision));
#pragma warning restore CS0618
            // 3FN: poblar la FK si el texto es un GUID válido
            if (Guid.TryParse(usuarioEmision, out var parsed))
            {
                UsuarioEmisionId = parsed;
            }
            SetEstadoFiscal(EstadoFiscalConstants.EmitidaId);
        }

        /// <summary>3FN: variante con FK explícita al usuario de Identity.</summary>
        public void Emitir(string nroControlFiscal, Guid usuarioEmisionId, string? usuarioNombreAlias = null)
        {
            if (EstadoFiscalId != EstadoFiscalConstants.BorradorId) throw new InvalidOperationException("Solo los borradores pueden emitirse como facturas fiscales.");
            if (usuarioEmisionId == Guid.Empty) throw new ArgumentException("El ID de usuario no puede ser vacío.", nameof(usuarioEmisionId));
            NroControlFiscal = nroControlFiscal ?? throw new ArgumentNullException(nameof(nroControlFiscal));
            UsuarioEmisionId = usuarioEmisionId;
#pragma warning disable CS0618
            if (!string.IsNullOrWhiteSpace(usuarioNombreAlias)) UsuarioEmision = usuarioNombreAlias;
#pragma warning restore CS0618
            SetEstadoFiscal(EstadoFiscalConstants.EmitidaId);
        }

        public void Anular()
        {
            SetEstadoFiscal(EstadoFiscalConstants.AnuladaId);
        }

        public void AgregarDetallePago(string metodoPago, Guid metodoPagoId, string referencia, decimal montoCambiario, decimal equivalenteBase, decimal tasaCambioAplicada = 1.0m, string usuarioCarga = "admin")
        {
            if (EstadoFiscalId == EstadoFiscalConstants.AnuladaId) throw new InvalidOperationException("No se pueden agregar pagos a un recibo anulado.");
            if (metodoPagoId == Guid.Empty) throw new ArgumentException("El método de pago es requerido.", nameof(metodoPagoId));
            _detallesPago.Add(new DetallePago(Id, metodoPago, referencia, montoCambiario, equivalenteBase, tasaCambioAplicada, usuarioCarga, metodoPagoId));
        }

        public decimal ObtenerTotalPagadoBase() => _detallesPago.Sum(p => p.EquivalenteAbonadoBase);

        public void CambiarPacienteAdministrativo(Guid nuevoPacienteId)
        {
            if (nuevoPacienteId == Guid.Empty) throw new ArgumentException("El PacienteId no puede ser vacío.");
            PacienteId = nuevoPacienteId;
        }

        public void ActualizarTotalesAdministrativos(decimal nuevoTotalFacturado, decimal nuevoMontoVuelto)
        {
            TotalFacturadoUSD = nuevoTotalFacturado;
            MontoVueltoUSD = nuevoMontoVuelto;
        }
    }
}
