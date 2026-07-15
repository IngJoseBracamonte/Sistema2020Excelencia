using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class TipoServicio
    {
        public int Id { get; private set; }
        public string Nombre { get; private set; }
        public string Codigo { get; private set; }

        private TipoServicio() { }

        public TipoServicio(int id, string nombre, string codigo)
        {
            Id = id;
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Codigo = codigo ?? throw new ArgumentNullException(nameof(codigo));
        }
    }
}
