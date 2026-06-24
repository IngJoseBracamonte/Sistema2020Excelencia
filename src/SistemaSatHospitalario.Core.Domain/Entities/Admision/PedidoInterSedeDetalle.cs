using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class PedidoInterSedeDetalle
    {
        public Guid Id { get; private set; }
        public Guid PedidoInterSedeId { get; private set; }
        public virtual PedidoInterSede PedidoInterSede { get; private set; }
        public Guid InsumoId { get; private set; }
        public virtual Insumo Insumo { get; private set; }
        public decimal CantidadSolicitada { get; private set; }
        public decimal CantidadDespachada { get; private set; }
        public decimal CantidadRecibida { get; private set; }

        private PedidoInterSedeDetalle() { }

        public PedidoInterSedeDetalle(Guid insumoId, decimal cantidadSolicitada)
        {
            Id = Guid.NewGuid();
            InsumoId = insumoId;
            CantidadSolicitada = cantidadSolicitada;
            CantidadDespachada = 0;
            CantidadRecibida = 0;
        }

        public PedidoInterSedeDetalle(Insumo insumo, decimal cantidadSolicitada)
        {
            Id = Guid.NewGuid();
            InsumoId = insumo?.Id ?? Guid.Empty;
            Insumo = insumo;
            CantidadSolicitada = cantidadSolicitada;
            CantidadDespachada = 0;
            CantidadRecibida = 0;
        }

        public void SetDespachado(decimal cantidad)
        {
            CantidadDespachada = cantidad;
        }

        public void SetRecibido(decimal cantidad)
        {
            CantidadRecibida = cantidad;
        }
    }
}
