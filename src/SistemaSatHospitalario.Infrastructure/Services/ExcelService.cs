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
                        worksheet.Cell(currentRow, 11).Value = (DateTime?)item.FechaAuditoria;
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

        public byte[] GenerateDetailedCashierReport(IEnumerable<CajeroReportDto> data, decimal grandTotalEsperado, decimal grandTotalRecaudado)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Cierres de Caja");
                
                int currentRow = 2;

                foreach (var cajero in data)
                {
                    // Título del Cajero
                    worksheet.Cell(currentRow, 1).Value = $"CAJERO: {cajero.FullName.ToUpper()} ({cajero.Username.ToUpper()})";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                    worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.FromHtml("#1e293b"); // Slate 800
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = $"Estado de Caja: {cajero.EstadoCaja} | Total Cobrado: {cajero.TotalCobrado} USD | Total Declarado: {cajero.TotalIngresado} USD";
                    worksheet.Cell(currentRow, 1).Style.Font.Italic = true;
                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 10;
                    worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.FromHtml("#64748b"); // Slate 500
                    currentRow++;
                    
                    // Encabezados de Pagos
                    worksheet.Cell(currentRow, 1).Value = "FECHA/HORA";
                    worksheet.Cell(currentRow, 2).Value = "PACIENTE";
                    worksheet.Cell(currentRow, 3).Value = "CEDULA";
                    worksheet.Cell(currentRow, 4).Value = "CONCEPTO";
                    worksheet.Cell(currentRow, 5).Value = "METODO PAGO";
                    worksheet.Cell(currentRow, 6).Value = "MONTO ORIG.";
                    worksheet.Cell(currentRow, 7).Value = "EQV. USD";
                    worksheet.Cell(currentRow, 8).Value = "INGRESADO POR";
                    worksheet.Cell(currentRow, 9).Value = "VUELTO DADO POR";
                    worksheet.Cell(currentRow, 10).Value = "MONTO VUELTO";
                    worksheet.Cell(currentRow, 11).Value = "TOTAL CUENTA";
                    worksheet.Cell(currentRow, 12).Value = "PENDIENTE CUENTA";

                    var headerRange = worksheet.Range(currentRow, 1, currentRow, 12);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#475569"); // Slate 600
                    headerRange.Style.Font.FontColor = XLColor.White;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    currentRow++;

                    // Listar Pagos de este Cajero
                    if (cajero.Pagos != null && cajero.Pagos.Any())
                    {
                        foreach (var pago in cajero.Pagos)
                        {
                            worksheet.Cell(currentRow, 1).Value = pago.Fecha.ToString("HH:mm");
                            worksheet.Cell(currentRow, 2).Value = pago.PacienteNombre;
                            worksheet.Cell(currentRow, 3).Value = pago.PacienteCedula;
                            worksheet.Cell(currentRow, 4).Value = pago.Concepto;
                            worksheet.Cell(currentRow, 5).Value = pago.MetodoPago;
                            
                            // Formato de Monto Original
                            worksheet.Cell(currentRow, 6).Value = pago.MontoMonedaOriginal;
                            worksheet.Cell(currentRow, 6).Style.NumberFormat.Format = pago.Moneda == "$" ? "$ #,##0.00" : "Bs. #,##0.00";
                            
                            // Formato de Equivalente USD
                            worksheet.Cell(currentRow, 7).Value = pago.EquivalenteUSD;
                            worksheet.Cell(currentRow, 7).Style.NumberFormat.Format = "$ #,##0.00";

                            // Nuevos campos detallados
                            worksheet.Cell(currentRow, 8).Value = pago.IngresadoPor;
                            worksheet.Cell(currentRow, 9).Value = pago.VueltoDadoPor;

                            worksheet.Cell(currentRow, 10).Value = pago.VueltoUSD;
                            worksheet.Cell(currentRow, 10).Style.NumberFormat.Format = "$ #,##0.00";

                            worksheet.Cell(currentRow, 11).Value = pago.TotalCuentaUSD;
                            worksheet.Cell(currentRow, 11).Style.NumberFormat.Format = "$ #,##0.00";

                            worksheet.Cell(currentRow, 12).Value = pago.PendienteCuentaUSD;
                            worksheet.Cell(currentRow, 12).Style.NumberFormat.Format = "$ #,##0.00";

                            // Alineaciones
                            worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            worksheet.Cell(currentRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            worksheet.Cell(currentRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            worksheet.Cell(currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            worksheet.Cell(currentRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            worksheet.Cell(currentRow, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            currentRow++;
                        }
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 1).Value = "Sin transacciones de cobro registradas hoy.";
                        worksheet.Range(currentRow, 1, currentRow, 12).Merge();
                        worksheet.Cell(currentRow, 1).Style.Font.Italic = true;
                        worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Gray;
                        currentRow++;
                    }

                    currentRow++;

                    // Tabla de Arqueo / Cierre por Método de Pago
                    worksheet.Cell(currentRow, 2).Value = "RESUMEN DE ARQUEO / CIERRE";
                    worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Style.Font.FontSize = 10;
                    worksheet.Cell(currentRow, 2).Style.Font.FontColor = XLColor.FromHtml("#0f172a"); // Slate 900
                    currentRow++;

                    worksheet.Cell(currentRow, 2).Value = "METODO";
                    worksheet.Cell(currentRow, 3).Value = "ESPERADO";
                    worksheet.Cell(currentRow, 4).Value = "DECLARADO";
                    worksheet.Cell(currentRow, 5).Value = "DIFERENCIA";

                    var arqueoHeaderRange = worksheet.Range(currentRow, 2, currentRow, 5);
                    arqueoHeaderRange.Style.Font.Bold = true;
                    arqueoHeaderRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#cbd5e1"); // Slate 300
                    arqueoHeaderRange.Style.Font.FontColor = XLColor.FromHtml("#0f172a");
                    arqueoHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    currentRow++;

                    if (cajero.Desglose != null && cajero.Desglose.Any())
                    {
                        foreach (var d in cajero.Desglose)
                        {
                            worksheet.Cell(currentRow, 2).Value = d.Nombre;
                            
                            worksheet.Cell(currentRow, 3).Value = d.Esperado;
                            worksheet.Cell(currentRow, 3).Style.NumberFormat.Format = d.EsUSD ? "$ #,##0.00" : "Bs. #,##0.00";
                            
                            worksheet.Cell(currentRow, 4).Value = d.Declarado;
                            worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = d.EsUSD ? "$ #,##0.00" : "Bs. #,##0.00";
                            
                            worksheet.Cell(currentRow, 5).Value = d.Diferencia;
                            worksheet.Cell(currentRow, 5).Style.NumberFormat.Format = d.EsUSD ? "$ #,##0.00" : "Bs. #,##0.00";
                            if (d.Diferencia < 0) worksheet.Cell(currentRow, 5).Style.Font.FontColor = XLColor.Red;
                            else if (d.Diferencia > 0) worksheet.Cell(currentRow, 5).Style.Font.FontColor = XLColor.Green;

                            currentRow++;
                        }
                    }

                    // Fila de separación
                    currentRow += 3;
                }

                // Gran Total Diario Consolidado
                worksheet.Cell(currentRow, 1).Value = "====================================================================================";
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "TOTAL DIARIO CONSOLIDADO GENERAL";
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                worksheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.FromHtml("#9f1239"); // Rose 800
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "TOTAL ESPERADO GLOBAL:";
                worksheet.Cell(currentRow, 2).Value = grandTotalEsperado;
                worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "$ #,##0.00";
                worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "TOTAL RECAUDADO GLOBAL:";
                worksheet.Cell(currentRow, 2).Value = grandTotalRecaudado;
                worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "$ #,##0.00";
                worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                currentRow++;

                decimal diffGlobal = grandTotalRecaudado - grandTotalEsperado;
                worksheet.Cell(currentRow, 1).Value = "DIFERENCIA NETA GLOBAL:";
                worksheet.Cell(currentRow, 2).Value = diffGlobal;
                worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "$ #,##0.00";
                worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                if (diffGlobal < 0) worksheet.Cell(currentRow, 2).Style.Font.FontColor = XLColor.Red;
                else if (diffGlobal > 0) worksheet.Cell(currentRow, 2).Style.Font.FontColor = XLColor.Green;

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
