using System;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class ServicioInsumoReceta
    {
        public Guid Id { get; private set; }
        public Guid ServicioClinicoId { get; private set; }
        public string ServicioCodigo { get; private set; }
        public Guid InsumoId { get; private set; }
        public decimal Cantidad { get; private set; }
        public UnidadMedida UnidadMedidaConsumo { get; private set; }

        public virtual ServicioClinico ServicioClinico { get; private set; }
        public virtual Insumo Insumo { get; private set; }

        protected ServicioInsumoReceta() { }

        public ServicioInsumoReceta(Guid servicioClinicoId, string servicioCodigo, Guid insumoId, decimal cantidad, UnidadMedida unidadMedidaConsumo)
        {
            Id = Guid.NewGuid();
            ServicioClinicoId = servicioClinicoId;
            ServicioCodigo = servicioCodigo ?? throw new ArgumentNullException(nameof(servicioCodigo));
            InsumoId = insumoId;
            Cantidad = cantidad;
            UnidadMedidaConsumo = unidadMedidaConsumo;
        }

        public void ActualizarReceta(decimal cantidad, UnidadMedida unidadMedidaConsumo)
        {
            Cantidad = cantidad;
            UnidadMedidaConsumo = unidadMedidaConsumo;
        }
    }
}
