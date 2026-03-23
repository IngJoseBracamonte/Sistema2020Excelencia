using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class TasaCambio
    {
        public Guid Id { get; private set; }
        public DateTime Fecha { get; private set; }
        public decimal Monto { get; private set; }
        public bool Activo { get; private set; }

        private TasaCambio() { }

        public TasaCambio(decimal monto)
        {
            Id = Guid.NewGuid();
            Fecha = DateTime.Now;
            Monto = monto;
            Activo = true;
        }
    }
}
