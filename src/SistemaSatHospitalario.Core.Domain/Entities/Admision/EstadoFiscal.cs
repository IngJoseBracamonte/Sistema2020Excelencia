using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    /// <summary>
    /// Catálogo maestro de estados fiscales de recibo/factura (3FN).
    /// Reemplaza el texto libre en <see cref="ReciboFactura.EstadoFiscal"/>.
    /// </summary>
    public class EstadoFiscal
    {
        public int Id { get; private set; }
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public bool Activo { get; private set; }

        private EstadoFiscal() { }

        public EstadoFiscal(int id, string codigo, string nombre, bool activo = true)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("El código del estado fiscal es obligatorio.", nameof(codigo));
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del estado fiscal es obligatorio.", nameof(nombre));

            Id = id;
            Codigo = codigo.Trim().ToUpperInvariant();
            Nombre = nombre.Trim();
            Activo = activo;
        }
    }
}
