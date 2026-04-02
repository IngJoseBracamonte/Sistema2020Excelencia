using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class Medico
    {
        public Guid Id { get; private set; }
        public string Nombre { get; private set; }
        public Guid EspecialidadId { get; private set; }
        public virtual Especialidad Especialidad { get; private set; }
        public bool Activo { get; private set; }

        private Medico() { }

        public Medico(string nombre, Guid especialidadId)
        {
            Id = Guid.NewGuid();
            Nombre = nombre;
            EspecialidadId = especialidadId;
            Activo = true;
        }

        public void Update(string nombre, Guid especialidadId)
        {
            Nombre = nombre;
            EspecialidadId = especialidadId;
        }

        public void SetEstado(bool activo)
        {
            Activo = activo;
        }
    }
}
