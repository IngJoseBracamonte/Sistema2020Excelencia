using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class BusinessInsightsDto
    {
        public decimal TotalVentasHoy { get; set; }
        public int PacientesAtendidosHoy { get; set; }
        public decimal SaldoPendienteAR { get; set; }
        public int TurnosPautadosHoy { get; set; }
        public List<RevenueBySpecialtyDto> VentasPorEspecialidad { get; set; } = new();
        public List<RevenueByInsuranceDto> VentasPorSeguro { get; set; } = new();

        // Métricas de Rayos X (Micro-Ciclo 22)
        public int TotalOrdenesRxHoy { get; set; }
        public int OrdenesRxProcesadasHoy { get; set; }
        public decimal VentasRxHoy { get; set; }
    }

    public class RevenueBySpecialtyDto
    {
        public string Especialidad { get; set; }
        public decimal Monto { get; set; }
    }

    public class RevenueByInsuranceDto
    {
        public string Seguro { get; set; }
        public decimal Monto { get; set; }
    }
}
