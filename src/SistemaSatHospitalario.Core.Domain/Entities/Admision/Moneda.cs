using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class Moneda
    {
        public int Id { get; private set; }
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public string Simbolo { get; private set; }
        public bool EsBaseUsd { get; private set; }

        protected Moneda() { }

        public Moneda(int id, string codigo, string nombre, string simbolo, bool esBaseUsd)
        {
            Id = id;
            Codigo = codigo ?? throw new ArgumentNullException(nameof(codigo));
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Simbolo = simbolo ?? throw new ArgumentNullException(nameof(simbolo));
            EsBaseUsd = esBaseUsd;
        }
    }
}
