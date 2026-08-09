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

        // --- Datos para Gráficos y Paneles (Fase 6 - Rediseño Dashboard) ---
        public List<RevenueTrendDto> TendenciaIngresos { get; set; } = new();
        public List<PatientDistributionDto> DistribucionPacientes { get; set; } = new();

        // KPIs Superiores Panel Izquierdo
        public int KpiTotalOrdenes { get; set; }
        public int KpiPacientesAtendidos { get; set; }
        public decimal KpiTicketPromedio { get; set; }

        // Panel Inferior Central (Compras e Inventario)
        public decimal TotalComprasInventario { get; set; }
        public List<CategoryPurchaseDto> ComprasPorCategoria { get; set; } = new();

        // Panel Izquierdo (Desglose Multidimensional)
        public List<ServiceBreakdownDto> DesglosePorServicio { get; set; } = new();
        public List<OriginBreakdownDto> DesglosePorOrigen { get; set; } = new();

        // Panel Lateral Derecho (Auditoría / Eventos)
        public List<DashboardAuditEntryDto> HistorialAuditoria { get; set; } = new();

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
        public int Transacciones { get; set; }
    }

    public class PatientDistributionDto
    {
        public string Etiqueta { get; set; }
        public int Valor { get; set; }
    }

    public class CategoryPurchaseDto
    {
        public string Categoria { get; set; } = string.Empty;
        public decimal Monto { get; set; }
    }

    public class ServiceBreakdownDto
    {
        public string Servicio { get; set; } = string.Empty;
        public int Ordenes { get; set; }
        public decimal Monto { get; set; }
        public double Porcentaje { get; set; }
    }

    public class OriginBreakdownDto
    {
        public string Origen { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public double Porcentaje { get; set; }
    }

    public class DashboardAuditEntryDto
    {
        public string Modulo { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string TipoEvento { get; set; } = string.Empty;
    }
}
