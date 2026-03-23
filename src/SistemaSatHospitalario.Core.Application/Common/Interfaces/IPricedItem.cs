using System;

namespace SistemaSatHospitalario.Core.Application.Common.Interfaces
{
    public interface IPricedItem
    {
        decimal PrecioUsd { get; set; }
        decimal PrecioBs { get; set; }
        decimal Precio { get; set; } // Compatibilidad legacy (será Bs)
        
        void CalculatePrices(decimal tasa);
    }
}
