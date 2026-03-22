using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class DailyClosingDto
    {
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
        public int TotalOrdenes { get; set; }
        public decimal TotalVendidoUSD { get; set; }
        public decimal TotalRecaudadoBase { get; set; } // En USD equivalentes
        public List<PaymentMethodSummaryDto> DesgloseMetodos { get; set; } = new();
    }

    public class PaymentMethodSummaryDto
    {
        public string Metodo { get; set; }
        public decimal MontoMonedaOriginal { get; set; }
        public decimal MontoEquivalenteBase { get; set; }
        public int Conteo { get; set; }
    }
}
