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
        public decimal HonorarioBase { get; private set; }
        public int IntervaloTurnoMinutos { get; private set; } = 30;
        public string? Telefono { get; private set; }

        private Medico() { }

        public Medico(string nombre, Guid especialidadId, decimal honorarioBase = 0, string? telefono = null)
        {
            Id = Guid.NewGuid();
            Nombre = nombre;
            EspecialidadId = especialidadId;
            Activo = true;
            HonorarioBase = honorarioBase;
            IntervaloTurnoMinutos = 30;
            Telefono = telefono;
        }

        public void SetIntervalo(int minutos)
        {
            if (minutos <= 0) throw new ArgumentException("El intervalo debe ser mayor a 0.");
            IntervaloTurnoMinutos = minutos;
        }

        public void Update(string nombre, Guid especialidadId, decimal honorarioBase, string? telefono = null)
        {
            Nombre = nombre;
            EspecialidadId = especialidadId;
            HonorarioBase = honorarioBase;
            Telefono = telefono;
        }

        public void SetEstado(bool activo)
        {
            Activo = activo;
        }
    }
}
