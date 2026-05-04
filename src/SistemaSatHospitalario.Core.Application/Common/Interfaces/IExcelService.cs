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
    }
}
