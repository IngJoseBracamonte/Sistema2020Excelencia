using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class PrincipioActivo
    {
        public Guid Id { get; private set; }
        public string Nombre { get; private set; }
        public bool Activo { get; private set; }

        public virtual ICollection<InsumoPrincipioActivo> Insumos { get; private set; } = new List<InsumoPrincipioActivo>();

        private PrincipioActivo() { }

        public PrincipioActivo(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre del principio activo no puede estar vacío.", nameof(nombre));
            }

            Id = Guid.NewGuid();
            Nombre = nombre.Trim();
            Activo = true;
        }

        public void ActualizarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre del principio activo no puede estar vacío.", nameof(nombre));
            }
            Nombre = nombre.Trim();
        }

        public void SetEstado(bool activo)
        {
            Activo = activo;
        }
    }
}
