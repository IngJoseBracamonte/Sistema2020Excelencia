using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CategoriaInsumo
    {
        public Guid Id { get; private set; }
        public string Nombre { get; private set; }
        public string? Codigo { get; private set; }
        public bool Activo { get; private set; }
        public DateTime FechaCreacion { get; private set; }

        private CategoriaInsumo() { }

        public CategoriaInsumo(string nombre, string? codigo = null, Guid? id = null, DateTime? fechaCreacion = null)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre de la categoría no puede estar vacío.", nameof(nombre));
            }

            Id = id ?? Guid.NewGuid();
            Nombre = nombre.Trim();
            Codigo = codigo?.Trim();
            Activo = true;
            FechaCreacion = fechaCreacion ?? DateTime.UtcNow;
        }

        public void ActualizarNombre(string nombre, string? codigo = null)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre de la categoría no puede estar vacío.", nameof(nombre));
            }

            Nombre = nombre.Trim();
            if (codigo != null)
            {
                Codigo = string.IsNullOrWhiteSpace(codigo) ? null : codigo.Trim();
            }
        }

        public void SetEstado(bool activo)
        {
            Activo = activo;
        }
    }
}
