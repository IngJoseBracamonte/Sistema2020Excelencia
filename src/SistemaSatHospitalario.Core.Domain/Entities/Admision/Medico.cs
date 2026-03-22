using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class Medico
    {
        public Guid Id { get; private set; }
        public string Nombre { get; private set; }
        public string Especialidad { get; private set; }
        public bool Activo { get; private set; }

        private Medico() { }

        public Medico(string nombre, string especialidad)
        {
            Id = Guid.NewGuid();
            Nombre = nombre;
            Especialidad = especialidad;
            Activo = true;
        }
    }
}
