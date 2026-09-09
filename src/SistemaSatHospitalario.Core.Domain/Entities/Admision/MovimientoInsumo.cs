using System;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class MovimientoInsumo
    {
        public Guid Id { get; private set; }
        public Guid InsumoId { get; private set; }
        public Guid SedeId { get; private set; }
        public TipoMovimientoInsumo TipoMovimiento { get; private set; }
        public decimal CantidadBase { get; private set; }

        public int UnidadMedidaOriginalId { get; private set; }

        /// <summary>Navegación al catálogo de unidades de medida.</summary>
        public virtual UnidadMedidaCatalogo UnidadMedidaNav { get; private set; } = null!;
        public decimal CantidadOriginal { get; private set; }
        public string? UsuarioId { get; private set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario que registró el movimiento.</summary>
        public Guid? UsuarioIdentityId { get; private set; }
        public DateTime Fecha { get; private set; }
        public string Motivo { get; private set; }

        public virtual Insumo Insumo { get; private set; }
        public virtual Sede Sede { get; private set; }

        protected MovimientoInsumo() { }

        public MovimientoInsumo(Guid insumoId, Guid sedeId, TipoMovimientoInsumo tipoMovimiento, decimal cantidadBase, UnidadMedidaEnum unidadMedidaOriginal, decimal cantidadOriginal, string motivo, string? usuarioId = null)
        {
            Id = Guid.NewGuid();
            InsumoId = insumoId;
            SedeId = sedeId;
            TipoMovimiento = tipoMovimiento;
            CantidadBase = cantidadBase;
            UnidadMedidaOriginalId = UnidadMedidaConstants.FromEnum(unidadMedidaOriginal);
            CantidadOriginal = cantidadOriginal;
            UsuarioIdentityId = Guid.TryParse(usuarioId, out var parsed) ? parsed : (Guid?)null;
            Fecha = DateTime.UtcNow;
            Motivo = motivo ?? string.Empty;
        }

        public MovimientoInsumo(Guid insumoId, Guid sedeId, string tipoMovimiento, decimal cantidadBase, UnidadMedidaEnum unidadMedidaOriginal, decimal cantidadOriginal, string motivo, string? usuarioId = null)
            : this(insumoId, sedeId, ParseTipoMovimiento(tipoMovimiento), cantidadBase, unidadMedidaOriginal, cantidadOriginal, motivo, usuarioId)
        {
        }

        private static TipoMovimientoInsumo ParseTipoMovimiento(string tipo)
        {
            return tipo?.ToLowerInvariant() switch
            {
                "ingreso" => TipoMovimientoInsumo.Ingreso,
                "descarte" => TipoMovimientoInsumo.Descarte,
                "consumo" or "enviosubarea" => TipoMovimientoInsumo.Consumo,
                "ajuste" or "ajustecierre" => TipoMovimientoInsumo.AjusteCierre,
                "transferenciaentrada" => TipoMovimientoInsumo.TransferenciaEntrada,
                "transferenciasalida" => TipoMovimientoInsumo.TransferenciaSalida,
                _ => TipoMovimientoInsumo.Consumo
            };
        }
    }
}
