using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    /// <summary>
    /// Catálogo maestro de unidades de medida (3FN).
    /// Reemplaza el enum <see cref="Enums.UnidadMedida"/> persistido como varchar(20).
    /// </summary>
    public class UnidadMedidaCatalogo
    {
        public int Id { get; private set; }
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public string Simbolo { get; private set; }
        public bool EsFraccionable { get; private set; }
        public bool Activo { get; private set; }

        private UnidadMedidaCatalogo() { }

        public UnidadMedidaCatalogo(int id, string codigo, string nombre, string simbolo, bool esFraccionable = true, bool activo = true)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("El código de la unidad es obligatorio.", nameof(codigo));
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la unidad es obligatorio.", nameof(nombre));

            Id = id;
            Codigo = codigo.Trim().ToUpperInvariant();
            Nombre = nombre.Trim();
            Simbolo = simbolo?.Trim() ?? string.Empty;
            EsFraccionable = esFraccionable;
            Activo = activo;
        }
    }
}
