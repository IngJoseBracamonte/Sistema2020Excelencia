using ClosedXML.Excel;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace SistemaSatHospitalario.Infrastructure.Services
{
    public class ExcelService : IExcelService
    {
        public byte[] GenerateExcel(DataTable data, string sheetName = "Reporte")
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(data, sheetName);
                
                // Formato Estándar "Pro"
                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e293b"); // Slate 800
                headerRow.Style.Font.FontColor = XLColor.White;
                
                worksheet.Columns().AdjustToContents();
                
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public byte[] GenerateFinancialReport(IEnumerable<object> data, bool isAuditMode)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Cartera y Auditoría");
                
                int currentRow = 1;

                // Encabezados
                worksheet.Cell(currentRow, 1).Value = "FECHA";
                worksheet.Cell(currentRow, 2).Value = "PACIENTE";
                worksheet.Cell(currentRow, 3).Value = "CEDULA";
                worksheet.Cell(currentRow, 4).Value = "MONTO TOTAL";
                worksheet.Cell(currentRow, 5).Value = "SALDO";
                worksheet.Cell(currentRow, 6).Value = "ESTADO";
                worksheet.Cell(currentRow, 7).Value = "AUDITADO";

                if (isAuditMode)
                {
                    worksheet.Cell(currentRow, 8).Value = "USUARIO INGRESO";
                    worksheet.Cell(currentRow, 9).Value = "FECHA INGRESO";
                    worksheet.Cell(currentRow, 10).Value = "USUARIO AUDITORIA";
                    worksheet.Cell(currentRow, 11).Value = "FECHA AUDITORIA";
                    
                    var auditHeaders = worksheet.Range(currentRow, 8, currentRow, 11);
                    auditHeaders.Style.Fill.BackgroundColor = XLColor.FromHtml("#fef3c7"); // Amber 100
                    auditHeaders.Style.Font.FontColor = XLColor.FromHtml("#92400e"); // Amber 800
                }

                var headerRange = worksheet.Range(currentRow, 1, currentRow, isAuditMode ? 11 : 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0f172a");
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                currentRow++;

                foreach (dynamic item in data)
                {
                    // Fila Maestro (Factura/Cuenta)
                    worksheet.Cell(currentRow, 1).Value = item.FechaEmision;
                    worksheet.Cell(currentRow, 2).Value = item.PacienteNombre;
                    worksheet.Cell(currentRow, 3).Value = item.PacienteCedula;
                    worksheet.Cell(currentRow, 4).Value = item.MontoTotal;
                    worksheet.Cell(currentRow, 5).Value = item.SaldoPendiente;
                    worksheet.Cell(currentRow, 6).Value = item.Estado;
                    worksheet.Cell(currentRow, 7).Value = item.IsAudited ? "SÍ" : "NO";

                    if (isAuditMode)
                    {
                        worksheet.Cell(currentRow, 8).Value = item.UsuarioIngreso ?? "SISTEMA";
                        worksheet.Cell(currentRow, 9).Value = item.FechaCreacion;
                        worksheet.Cell(currentRow, 10).Value = item.UsuarioAuditoria ?? "-";
                        worksheet.Cell(currentRow, 11).Value = item.FechaAuditoria;
                    }

                    // Formato de moneda
                    worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = "$ #,##0.00";
                    worksheet.Cell(currentRow, 5).Style.NumberFormat.Format = "$ #,##0.00";

                    currentRow++;

                    // Filas Detalle (Pagos)
                    if (item.Pagos != null && ((IEnumerable<dynamic>)item.Pagos).Any())
                    {
                        // Sub-encabezado de pagos
                        worksheet.Cell(currentRow, 2).Value = "└─ DETALLE DE PAGOS:";
                        worksheet.Cell(currentRow, 2).Style.Font.Italic = true;
                        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 9;
                        currentRow++;

                        foreach (dynamic pago in item.Pagos)
                        {
                            worksheet.Cell(currentRow, 2).Value = $"   • {pago.Metodo} - Ref: {pago.Referencia}";
                            worksheet.Cell(currentRow, 4).Value = pago.MontoBase;
                            worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = "$ #,##0.00";
                            worksheet.Cell(currentRow, 4).Style.Font.FontColor = XLColor.Gray;
                            worksheet.Cell(currentRow, 4).Style.Font.FontSize = 9;
                            
                            currentRow++;
                        }
                    }
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}
