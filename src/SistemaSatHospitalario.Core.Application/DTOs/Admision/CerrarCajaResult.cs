using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class CerrarCajaResult
    {
        public Guid CajaId { get; set; }
        public decimal TotalIngresosUSD { get; set; }
        public decimal TotalVueltoUSD { get; set; }
        public decimal TotalIngresosBS { get; set; }
        public int ConteoVentas { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public DateTime FechaCierre { get; set; }
    }
}
