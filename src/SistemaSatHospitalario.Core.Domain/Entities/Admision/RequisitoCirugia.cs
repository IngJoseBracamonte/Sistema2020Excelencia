using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    /// <summary>
    /// Entidad de catálogo maestro DB-Driven para requisitos quirúrgicos (Checklist pre-operatorio).
    /// </summary>
    public class RequisitoCirugia
    {
        public Guid Id { get; private set; }
        public string Nombre { get; private set; }
        public string Descripcion { get; private set; }
        public bool EsActivo { get; private set; }
        public DateTime FechaCreacion { get; private set; }

        private readonly List<OrdenCirugiaRequisito> _ordenesRequisitos = new();
        public virtual IReadOnlyCollection<OrdenCirugiaRequisito> OrdenesRequisitos => _ordenesRequisitos.AsReadOnly();

        protected RequisitoCirugia() { }

        public RequisitoCirugia(string nombre, string descripcion, bool esActivo = true, Guid? id = null, DateTime? fechaCreacion = null)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del requisito es obligatorio.", nameof(nombre));

            Id = id ?? Guid.NewGuid();
            Nombre = nombre.Trim();
            Descripcion = (descripcion ?? string.Empty).Trim();
            EsActivo = esActivo;
            FechaCreacion = fechaCreacion ?? DateTime.UtcNow;
        }

        public void Update(string nombre, string descripcion, bool esActivo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del requisito es obligatorio.", nameof(nombre));

            Nombre = nombre.Trim();
            Descripcion = (descripcion ?? string.Empty).Trim();
            EsActivo = esActivo;
        }
    }
}
