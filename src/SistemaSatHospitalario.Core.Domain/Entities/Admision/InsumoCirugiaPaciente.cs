using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class InsumoCirugiaPaciente
    {
        public Guid Id { get; private set; }
        public Guid CuentaServicioId { get; private set; }
        public virtual CuentaServicios CuentaServicio { get; private set; }
        public Guid? OrdenCirugiaId { get; private set; }
        public virtual OrdenCirugia? OrdenCirugia { get; private set; }
        public Guid InsumoId { get; private set; }
        public virtual Insumo Insumo { get; private set; }
        public decimal CantidadEntregada { get; private set; }
        public decimal CantidadDevuelta { get; private set; }
        public decimal CantidadConsumida => CantidadEntregada - CantidadDevuelta;

        protected InsumoCirugiaPaciente() { }

        public InsumoCirugiaPaciente(Guid cuentaServicioId, Guid insumoId, decimal cantidadEntregada, Guid? ordenCirugiaId = null)
        {
            if (cantidadEntregada <= 0)
            {
                throw new ArgumentException("La cantidad entregada debe ser mayor a cero.", nameof(cantidadEntregada));
            }
            Id = Guid.NewGuid();
            CuentaServicioId = cuentaServicioId;
            OrdenCirugiaId = ordenCirugiaId;
            InsumoId = insumoId;
            CantidadEntregada = cantidadEntregada;
            CantidadDevuelta = 0;
        }

        public void RegistrarDevolucion(decimal cantidadARestar)
        {
            if (cantidadARestar <= 0)
            {
                throw new ArgumentException("La cantidad a devolver debe ser mayor a cero.", nameof(cantidadARestar));
            }
            if (CantidadDevuelta + cantidadARestar > CantidadEntregada)
            {
                throw new InvalidOperationException("No se pueden devolver más insumos de los entregados originalmente en el kit de cirugía.");
            }

            CantidadDevuelta += cantidadARestar;
        }

        public void AdicionarEntregada(decimal cantidad)
        {
            if (cantidad <= 0)
            {
                throw new ArgumentException("La cantidad a adicionar debe ser mayor a cero.", nameof(cantidad));
            }
            CantidadEntregada += cantidad;
        }
    }
}
