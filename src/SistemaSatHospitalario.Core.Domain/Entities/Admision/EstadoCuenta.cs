using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    /// <summary>
    /// Catálogo maestro de estados de cuenta de servicios (3FN).
    /// Reemplaza el texto libre en <see cref="CuentaServicios.Estado"/>.
    /// </summary>
    public class EstadoCuenta
    {
        public int Id { get; private set; }
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public bool Activo { get; private set; }

        private EstadoCuenta() { }

        public EstadoCuenta(int id, string codigo, string nombre, bool activo = true)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("El código del estado es obligatorio.", nameof(codigo));
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del estado es obligatorio.", nameof(nombre));

            Id = id;
            Codigo = codigo.Trim().ToUpperInvariant();
            Nombre = nombre.Trim();
            Activo = activo;
        }
    }
}
