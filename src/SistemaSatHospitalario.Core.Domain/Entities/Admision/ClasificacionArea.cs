using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    /// <summary>
    /// Clasificación normalizada de un área clínica (Cama, Quirófano, Sala de Parto, etc.).
    /// Permite determinar el TIPO de un área por su <see cref="Id"/> (Guid) y NO por
    /// comparación de cadenas de texto sobre Nombre/Código/Descripción.
    /// </summary>
    public class ClasificacionArea
    {
        public Guid Id { get; private set; }
        public string Codigo { get; private set; }
        public string Descripcion { get; private set; }

        private ClasificacionArea() { }

        public ClasificacionArea(string codigo, string descripcion, Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            Codigo = codigo ?? throw new ArgumentNullException(nameof(codigo));
            Descripcion = descripcion ?? throw new ArgumentNullException(nameof(descripcion));
        }
    }
}