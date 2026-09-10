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

        // Unidad de Medida del Registro (3FN)
        public int UnidadMedidaOriginalId { get; private set; }
        public virtual UnidadMedidaCatalogo UnidadMedidaNav { get; private set; } = null!;
        public decimal CantidadOriginal { get; private set; }

        // Auditoría e Identidad (3FN Limpio)
        public Guid? UsuarioIdentityId { get; private set; }
        public string? UsuarioId { get; private set; } // Alias legacy opcional

        public DateTime Fecha { get; private set; }
        public string Motivo { get; private set; } = string.Empty;

        public virtual Insumo Insumo { get; private set; } = null!;
        public virtual Sede Sede { get; private set; } = null!;

        protected MovimientoInsumo() { }

        public MovimientoInsumo(
            Guid insumoId, 
            Guid sedeId, 
            TipoMovimientoInsumo tipoMovimiento, 
            decimal cantidadBase, 
            UnidadMedidaEnum unidadMedidaOriginal, 
            decimal cantidadOriginal, 
            string motivo, 
            Guid? usuarioIdentityId = null,
            string? usuarioId = null)
        {
            if (insumoId == Guid.Empty) throw new ArgumentException("El insumo es obligatorio.", nameof(insumoId));
            if (sedeId == Guid.Empty) throw new ArgumentException("La sede es obligatoria.", nameof(sedeId));

            Id = Guid.NewGuid();
            InsumoId = insumoId;
            SedeId = sedeId;
            TipoMovimiento = tipoMovimiento;
            CantidadBase = cantidadBase;
            UnidadMedidaOriginalId = UnidadMedidaConstants.FromEnum(unidadMedidaOriginal);
            CantidadOriginal = cantidadOriginal;
            
            // 3FN: Asignar ID directo o intentar parsear string si vino el alias
            UsuarioIdentityId = usuarioIdentityId ?? (Guid.TryParse(usuarioId, out var parsed) ? parsed : (Guid?)null);
            UsuarioId = usuarioId;

            Fecha = DateTime.UtcNow;
            Motivo = motivo ?? string.Empty;
        }

        public MovimientoInsumo(
            Guid insumoId, 
            Guid sedeId, 
            string tipoMovimiento, 
            decimal cantidadBase, 
            UnidadMedidaEnum unidadMedidaOriginal, 
            decimal cantidadOriginal, 
            string motivo, 
            Guid? usuarioIdentityId = null,
            string? usuarioId = null)
            : this(insumoId, sedeId, ParseTipoMovimiento(tipoMovimiento), cantidadBase, unidadMedidaOriginal, cantidadOriginal, motivo, usuarioIdentityId, usuarioId)
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