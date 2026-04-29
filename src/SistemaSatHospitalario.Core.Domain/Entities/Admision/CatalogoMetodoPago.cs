using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CatalogoMetodoPago
    {
        public Guid Id { get; private set; }
        public string Nombre { get; private set; } // Nombre para UI: EFECTIVO DOLAR ($)
        public string Valor { get; private set; }  // Valor para el sistema: Dolar Efectivo
        public bool EsUSD { get; private set; }
        public bool EsVuelto { get; private set; }
        public bool Activo { get; private set; }
        public int Orden { get; private set; }

        protected CatalogoMetodoPago() { }

        public CatalogoMetodoPago(string nombre, string valor, bool esUSD, bool esVuelto = false, int orden = 0)
        {
            Id = Guid.NewGuid();
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Valor = valor ?? throw new ArgumentNullException(nameof(valor));
            EsUSD = esUSD;
            EsVuelto = esVuelto;
            Activo = true;
            Orden = orden;
        }

        public void SetActivo(bool activo) => Activo = activo;
        public void SetOrden(int orden) => Orden = orden;
    }
}
