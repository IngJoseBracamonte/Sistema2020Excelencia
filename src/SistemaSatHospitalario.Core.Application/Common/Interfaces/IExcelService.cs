using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace SistemaSatHospitalario.Core.Application.Common.Interfaces
{
    public interface IExcelService
    {
        /// <summary>
        /// Genera un archivo Excel (.xlsx) a partir de un DataTable.
        /// </summary>
        byte[] GenerateExcel(DataTable data, string sheetName = "Reporte");

        /// <summary>
        /// Genera un archivo Excel complejo (Maestro-Detalle) para reportes financieros.
        /// </summary>
        byte[] GenerateFinancialReport(IEnumerable<object> data, bool isAuditMode);

        /// <summary>
        /// Genera un reporte detallado de arqueo y transacciones agrupado por cajero.
        /// </summary>
        byte[] GenerateDetailedCashierReport(IEnumerable<CajeroReportDto> data, decimal grandTotalEsperado, decimal grandTotalRecaudado);
    }

    public class CajeroReportDto
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string EstadoCaja { get; set; } = string.Empty;
        public decimal TotalCobrado { get; set; }
        public decimal TotalIngresado { get; set; }
        public decimal Diferencia { get; set; }
        public List<PagoDetalladoDto> Pagos { get; set; } = new();
        public List<DesgloseMetodoDto> Desglose { get; set; } = new();
    }

    public class PagoDetalladoDto
    {
        public DateTime Fecha { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public string PacienteCedula { get; set; } = string.Empty;
        public string Concepto { get; set; } = string.Empty;
        public string MetodoPago { get; set; } = string.Empty;
        public string Moneda { get; set; } = string.Empty;
        public decimal MontoMonedaOriginal { get; set; }
        public decimal EquivalenteUSD { get; set; }
    }

    public class DesgloseMetodoDto
    {
        public string Metodo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal Esperado { get; set; }
        public decimal Declarado { get; set; }
        public decimal Diferencia { get; set; }
        public bool EsUSD { get; set; }
    }
}
