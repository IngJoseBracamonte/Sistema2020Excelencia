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
        public int UnidadMedidaConsumoId { get; private set; }

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
            UnidadMedidaConsumoId = Constants.UnidadMedidaConstants.FromEnum(unidadMedidaConsumo);
        }

        public void ActualizarReceta(decimal cantidad, UnidadMedida unidadMedidaConsumo)
        {
            Cantidad = cantidad;
            UnidadMedidaConsumoId = Constants.UnidadMedidaConstants.FromEnum(unidadMedidaConsumo);
        }
    }
}
