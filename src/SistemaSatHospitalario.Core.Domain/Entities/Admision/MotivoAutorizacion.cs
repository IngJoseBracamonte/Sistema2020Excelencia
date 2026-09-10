using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    /// <summary>
    /// Catálogo de motivos de autorización/omisión (3FN).
    /// Reemplaza cadenas repetitivas como "Autorizado por Dirección Médica"
    /// en observaciones de compromisos de pago y omisiones.
    /// </summary>
    public class MotivoAutorizacion
    {
        public int Id { get; private set; }
        public string Nombre { get; private set; }
        public bool Activo { get; private set; }

        private MotivoAutorizacion() { }

        // Constructor para nuevas creaciones
        public MotivoAutorizacion(string nombre, bool activo = true)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del motivo es obligatorio.", nameof(nombre));

            Nombre = nombre.Trim();
            Activo = activo;
        }

        // Constructor con ID (para inicialización / seeds / pruebas)
        public MotivoAutorizacion(int id, string nombre, bool activo = true) : this(nombre, activo)
        {
            Id = id;
        }

        public void Actualizar(string nombre, bool activo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del motivo es obligatorio.", nameof(nombre));

            Nombre = nombre.Trim();
            Activo = activo;
        }
    }
}