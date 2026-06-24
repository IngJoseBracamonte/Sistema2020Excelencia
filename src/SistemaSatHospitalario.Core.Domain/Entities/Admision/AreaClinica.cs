using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class AreaClinica
    {
        public Guid Id { get; private set; }
        public Guid SedeId { get; private set; }
        public virtual Sede Sede { get; private set; }
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public bool Activo { get; private set; }

        private AreaClinica() { }

        public AreaClinica(Guid sedeId, string codigo, string nombre)
        {
            Id = Guid.NewGuid();
            SedeId = sedeId;
            Codigo = codigo ?? throw new ArgumentNullException(nameof(codigo));
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Activo = true;
        }

        public void Update(string codigo, string nombre)
        {
            Codigo = codigo ?? throw new ArgumentNullException(nameof(codigo));
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        }

        public void SetEstado(bool activo)
        {
            Activo = activo;
        }
    }
}
