using System;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class MovimientoInsumo
    {
        public Guid Id { get; private set; }
        public Guid InsumoId { get; private set; }
        public Guid SedeId { get; private set; }
        public string TipoMovimiento { get; private set; } // Ingreso, Descarte, Consumo, AjusteCierre, TransferenciaEntrada, TransferenciaSalida
        public decimal CantidadBase { get; private set; }
        public UnidadMedida UnidadMedidaOriginal { get; private set; }
        public decimal CantidadOriginal { get; private set; }
        public string Usuario { get; private set; }
        public DateTime Fecha { get; private set; }
        public string Motivo { get; private set; }

        public virtual Insumo Insumo { get; private set; }
        public virtual Sede Sede { get; private set; }

        protected MovimientoInsumo() { }

        public MovimientoInsumo(Guid insumoId, Guid sedeId, string tipoMovimiento, decimal cantidadBase, UnidadMedida unidadMedidaOriginal, decimal cantidadOriginal, string usuario, string motivo)
        {
            Id = Guid.NewGuid();
            InsumoId = insumoId;
            SedeId = sedeId;
            TipoMovimiento = tipoMovimiento ?? throw new ArgumentNullException(nameof(tipoMovimiento));
            CantidadBase = cantidadBase;
            UnidadMedidaOriginal = unidadMedidaOriginal;
            CantidadOriginal = cantidadOriginal;
            Usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
            Fecha = DateTime.UtcNow;
            Motivo = motivo ?? string.Empty;
        }
    }
}
