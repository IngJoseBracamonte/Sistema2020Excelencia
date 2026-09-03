using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    /// <summary>
    /// Catálogo maestro de tipos de ingreso (3FN).
    /// Reemplaza el texto libre en <see cref="CuentaServicios.TipoIngreso"/>.
    /// </summary>
    public class TipoIngreso
    {
        public int Id { get; private set; }
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public bool Activo { get; private set; }

        private TipoIngreso() { }

        public TipoIngreso(int id, string codigo, string nombre, bool activo = true)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("El código del tipo de ingreso es obligatorio.", nameof(codigo));
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del tipo de ingreso es obligatorio.", nameof(nombre));

            Id = id;
            Codigo = codigo.Trim().ToUpperInvariant();
            Nombre = nombre.Trim();
            Activo = activo;
        }
    }
}
