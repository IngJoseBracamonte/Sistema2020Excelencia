using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class ConfiguracionGeneral
    {
        public Guid Id { get; private set; }
        public string NombreEmpresa { get; private set; } = string.Empty;
        public string Rif { get; private set; } = string.Empty;
        public decimal Iva { get; private set; }

        // ⚠️ Ver recomendación de seguridad abajo para ClaveSupervisor
        public string ClaveSupervisor { get; private set; } = string.Empty;

        public bool FacturarLaboratorio { get; private set; }
        public bool MostrarDetalleFacturacion { get; private set; }
        public string? LogoBase64 { get; private set; }
        public DateTime UltimaActualizacion { get; private set; }

        protected ConfiguracionGeneral() { }

        public ConfiguracionGeneral(
            string nombreEmpresa, 
            string rif, 
            decimal iva, 
            string claveSupervisor = "1234", 
            bool facturarLaboratorio = false, 
            bool mostrarDetalleFacturacion = false, 
            string? logoBase64 = null)
        {
            Id = Guid.NewGuid();
            Actualizar(nombreEmpresa, rif, iva, claveSupervisor, facturarLaboratorio, mostrarDetalleFacturacion, logoBase64);
        }

        public void Actualizar(
            string nombreEmpresa, 
            string rif, 
            decimal iva, 
            string claveSupervisor, 
            bool facturarLaboratorio, 
            bool mostrarDetalleFacturacion, 
            string? logoBase64 = null)
        {
            NombreEmpresa = nombreEmpresa ?? throw new ArgumentNullException(nameof(nombreEmpresa));
            Rif = rif ?? throw new ArgumentNullException(nameof(rif));
            Iva = iva >= 0 ? iva : throw new ArgumentException("El IVA no puede ser negativo.", nameof(iva));
            ClaveSupervisor = string.IsNullOrWhiteSpace(claveSupervisor) ? "1234" : claveSupervisor;
            FacturarLaboratorio = facturarLaboratorio;
            MostrarDetalleFacturacion = mostrarDetalleFacturacion;
            LogoBase64 = logoBase64;
            UltimaActualizacion = DateTime.UtcNow; // ✅ Siempre UTC
        }
    }
}