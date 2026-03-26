using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class SeguroConvenio
    {
        public int Id { get; protected set; }
        public string Nombre { get; protected set; }
        public string Rtn { get; protected set; }
        public string Direccion { get; protected set; }
        public string Telefono { get; protected set; }
        public string Email { get; protected set; }
        public bool Activo { get; protected set; }

        protected SeguroConvenio() { }

        public SeguroConvenio(string nombre, string rtn, string direccion, string telefono, string email)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Rtn = rtn;
            Direccion = direccion;
            Telefono = telefono;
            Email = email;
            Activo = true;
        }

        public void Actualizar(string nombre, string rtn, string direccion, string telefono, string email)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Rtn = rtn;
            Direccion = direccion;
            Telefono = telefono;
            Email = email;
        }

        public void Desactivar() => Activo = false;
        public void Activar() => Activo = true;
        public void SetActivo(bool activo) => Activo = activo;
    }
}
