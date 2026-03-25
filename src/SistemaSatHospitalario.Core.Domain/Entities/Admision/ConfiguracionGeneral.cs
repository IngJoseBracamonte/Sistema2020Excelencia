using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class ConfiguracionGeneral
    {
        public Guid Id { get; private set; }
        public string NombreEmpresa { get; private set; }
        public string Rif { get; private set; }
        public decimal Iva { get; private set; }
        public DateTime UltimaActualizacion { get; private set; }

        protected ConfiguracionGeneral() { }

        public ConfiguracionGeneral(string nombreEmpresa, string rif, decimal iva)
        {
            Id = Guid.NewGuid();
            Actualizar(nombreEmpresa, rif, iva);
        }

        public void Actualizar(string nombreEmpresa, string rif, decimal iva)
        {
            NombreEmpresa = nombreEmpresa ?? throw new ArgumentNullException(nameof(nombreEmpresa));
            Rif = rif ?? throw new ArgumentNullException(nameof(rif));
            Iva = iva;
            UltimaActualizacion = DateTime.Now;
        }
    }
}
