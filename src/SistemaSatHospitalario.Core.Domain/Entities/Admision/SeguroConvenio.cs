using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class SeguroConvenio
    {
        public Guid Id { get; protected set; }
        public string Nombre { get; protected set; }
        public decimal PorcentajeCobertura { get; protected set; }
        public bool Activo { get; protected set; }

        protected SeguroConvenio() { }

        public SeguroConvenio(string nombre, decimal porcentajeCobertura)
        {
            Id = Guid.NewGuid();
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            PorcentajeCobertura = porcentajeCobertura;
            Activo = true;
        }

        public void Desactivar() => Activo = false;
        public void Activar() => Activo = true;
    }
}
