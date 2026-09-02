using System;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class ServicioInsumoReceta
    {
        public Guid Id { get; private set; }
        public Guid ServicioClinicoId { get; private set; }
        public Guid InsumoId { get; private set; }
        public decimal Cantidad { get; private set; }

        /// <summary>
        /// LEGACY (3FN): unidad de medida como enum persistido en varchar(20).
        /// Fuente de verdad: <see cref="UnidadMedidaConsumoId"/> (FK a UnidadesMedida).
        /// Alias de compatibilidad hasta el DROP de columna.
        /// </summary>
        [Obsolete("Usar UnidadMedidaConsumoId / UnidadMedidaNav. Columna legacy pendiente de DROP.")]
        public UnidadMedida UnidadMedidaConsumo { get; private set; }

        /// <summary>FK al catálogo UnidadesMedida (3FN).</summary>
        public int UnidadMedidaConsumoId { get; private set; }

        /// <summary>Navegación al catálogo de unidades de medida.</summary>
        public virtual UnidadMedidaCatalogo UnidadMedidaNav { get; private set; } = null!;

        public virtual ServicioClinico ServicioClinico { get; private set; }
        public virtual Insumo Insumo { get; set; }

        protected ServicioInsumoReceta() { }

        public ServicioInsumoReceta(Guid servicioClinicoId, Guid insumoId, decimal cantidad, UnidadMedida unidadMedidaConsumo)
        {
            Id = Guid.NewGuid();
            ServicioClinicoId = servicioClinicoId;
            InsumoId = insumoId;
            Cantidad = cantidad;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UnidadMedidaConsumo = unidadMedidaConsumo;
#pragma warning restore CS0618
            UnidadMedidaConsumoId = Constants.UnidadMedidaConstants.FromEnum(unidadMedidaConsumo);
        }

        public void ActualizarReceta(decimal cantidad, UnidadMedida unidadMedidaConsumo)
        {
            Cantidad = cantidad;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UnidadMedidaConsumo = unidadMedidaConsumo;
#pragma warning restore CS0618
            UnidadMedidaConsumoId = Constants.UnidadMedidaConstants.FromEnum(unidadMedidaConsumo);
        }
    }
}
