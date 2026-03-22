using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class SeguroConvenio
    {
        // Se cambió de Guid a int para sincronización con Sistema Legacy (IDConvenio)
        public int Id { get; protected set; }
        public string Nombre { get; protected set; }
        public decimal PorcentajeCobertura { get; protected set; }
        public bool Activo { get; protected set; }

        protected SeguroConvenio() { }

        public SeguroConvenio(int id, string nombre, decimal porcentajeCobertura)
        {
            Id = id;
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            PorcentajeCobertura = porcentajeCobertura;
            Activo = true;
        }

        public void Desactivar() => Activo = false;
        public void Activar() => Activo = true;
    }
}
