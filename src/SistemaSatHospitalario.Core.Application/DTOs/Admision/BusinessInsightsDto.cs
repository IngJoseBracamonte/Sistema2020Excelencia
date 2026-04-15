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

        // --- Datos para Gráficos (Fase 6) ---
        public List<RevenueTrendDto> TendenciaIngresos { get; set; } = new();
        public List<PatientDistributionDto> DistribucionPacientes { get; set; } = new();

        // Métricas de Rayos X (Micro-Ciclo 22)
        public int TotalOrdenesRxHoy { get; set; }
        public int OrdenesRxProcesadasHoy { get; set; }
        public decimal VentasRxHoy { get; set; }
        public List<InsightAlertDto> PotentialAlerts { get; set; } = new();
    }

    public class InsightAlertDto
    {
        public string Severity { get; set; } = "Warning";
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
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

    public class RevenueTrendDto
    {
        public string Fecha { get; set; }
        public decimal Monto { get; set; }
    }

    public class PatientDistributionDto
    {
        public string Etiqueta { get; set; }
        public int Valor { get; set; }
    }
}
