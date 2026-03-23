using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class CatalogItemDto : IPricedItem
    {
        public string Id { get; set; } // Puede ser Guid o IdPersona/IdPerfil string
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; } // Compatibilidad (será Bs)
        public decimal PrecioBs { get; set; }
        public decimal PrecioUsd { get; set; }
        public string Tipo { get; set; } // CONSULTA, RX, LABORATORIO, etc.
        public bool EsLegacy { get; set; }

        public void CalculatePrices(decimal tasa)
        {
            if (tasa <= 0) tasa = 1;
            PrecioBs = PrecioUsd * tasa;
            Precio = PrecioBs;
        }
    }
}
