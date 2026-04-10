using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class ConfiguracionGeneral
    {
        public Guid Id { get; private set; }
        public string NombreEmpresa { get; private set; }
        public string Rif { get; private set; }
        public decimal Iva { get; private set; }
        public string ClaveSupervisor { get; private set; }
        public DateTime UltimaActualizacion { get; private set; }

        protected ConfiguracionGeneral() { }

        public ConfiguracionGeneral(string nombreEmpresa, string rif, decimal iva, string claveSupervisor = "1234")
        {
            Id = Guid.NewGuid();
            Actualizar(nombreEmpresa, rif, iva, claveSupervisor);
        }

        public void Actualizar(string nombreEmpresa, string rif, decimal iva, string claveSupervisor)
        {
            NombreEmpresa = nombreEmpresa ?? throw new ArgumentNullException(nameof(nombreEmpresa));
            Rif = rif ?? throw new ArgumentNullException(nameof(rif));
            Iva = iva;
            ClaveSupervisor = claveSupervisor ?? "1234";
            UltimaActualizacion = DateTime.Now;
        }
    }
}
