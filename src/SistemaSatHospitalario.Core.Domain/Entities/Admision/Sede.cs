using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class Sede
    {
        public Guid Id { get; private set; }
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public bool EsPrincipal { get; private set; }
        public bool Activo { get; private set; }

        public virtual ICollection<AreaClinica> AreasClinicas { get; private set; } = new List<AreaClinica>();

        private Sede() { }

        public Sede(string codigo, string nombre, bool esPrincipal, Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            Codigo = codigo ?? throw new ArgumentNullException(nameof(codigo));
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            EsPrincipal = esPrincipal;
            Activo = true;
        }

        public void Update(string codigo, string nombre, bool esPrincipal)
        {
            Codigo = codigo ?? throw new ArgumentNullException(nameof(codigo));
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            EsPrincipal = esPrincipal;
        }

        public void SetEstado(bool activo)
        {
            Activo = activo;
        }
    }
}
