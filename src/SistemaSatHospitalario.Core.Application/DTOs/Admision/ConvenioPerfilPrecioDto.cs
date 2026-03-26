using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class ConvenioPerfilPrecioDto
    {
        public int PerfilId { get; set; }
        public string NombrePerfil { get; set; }
        public decimal PrecioBaseHNL { get; set; }
        public decimal PrecioBaseUSD { get; set; }
        
        // Valores personalizados (pueden ser nulos si usa el base)
        public decimal? PrecioHNL { get; set; }
        public decimal? PrecioUSD { get; set; }
        
        // Precio final calculado
        public decimal PrecioFinalHNL => PrecioHNL ?? PrecioBaseHNL;
        public decimal PrecioFinalUSD => PrecioUSD ?? PrecioBaseUSD;
        
        public bool TienePrecioPersonalizado => PrecioHNL.HasValue;
    }
}
