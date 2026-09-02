using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    /// <summary>
    /// Catálogo maestro de estados de cita médica (3FN).
    /// Reemplaza el texto libre en <see cref="CitaMedica.Estado"/>.
    /// </summary>
    public class EstadoCitaMedica
    {
        public int Id { get; private set; }
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public bool Activo { get; private set; }

        private EstadoCitaMedica() { }

        public EstadoCitaMedica(int id, string codigo, string nombre, bool activo = true)
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

        public void Actualizar(string nombre, bool activo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del estado es obligatorio.", nameof(nombre));

            Nombre = nombre.Trim();
            Activo = activo;
        }
    }
}
