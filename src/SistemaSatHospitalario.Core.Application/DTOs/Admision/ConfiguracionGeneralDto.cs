using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class ConfiguracionGeneralDto
    {
        public Guid Id { get; set; }
        public string NombreEmpresa { get; set; }
        public string Rif { get; set; }
        public decimal Iva { get; set; }
        public DateTime UltimaActualizacion { get; set; }
    }
}
