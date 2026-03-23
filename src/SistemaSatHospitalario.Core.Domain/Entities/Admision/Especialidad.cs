using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class Especialidad
    {
        public Guid Id { get; private set; }
        public string Nombre { get; private set; }
        public bool Activo { get; private set; }

        private Especialidad() { }

        public Especialidad(string nombre)
        {
            Id = Guid.NewGuid();
            Nombre = nombre;
            Activo = true;
        }

        public void Update(string nombre)
        {
            Nombre = nombre;
        }

        public void SetEstado(bool activo)
        {
            Activo = activo;
        }
    }
}
