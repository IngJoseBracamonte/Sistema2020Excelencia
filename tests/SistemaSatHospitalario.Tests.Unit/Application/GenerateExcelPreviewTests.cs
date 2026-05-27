using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Xunit;
using SistemaSatHospitalario.Infrastructure.Services;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class MockFinancialReportRow
    {
        public DateTime FechaEmision { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteCedula { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string Estado { get; set; }
        public bool IsAudited { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioIngreso { get; set; }
        public string UsuarioAuditoria { get; set; }
        public DateTime? FechaAuditoria { get; set; }
        public List<MockFinancialPaymentRow> Pagos { get; set; } = new();
    }

    public class MockFinancialPaymentRow
    {
        public string Metodo { get; set; }
        public string Referencia { get; set; }
        public decimal MontoBase { get; set; }
    }

    public class GenerateExcelPreviewTests
    {
        [Fact]
        public void Generate_Financial_Excel_Preview()
        {
            var service = new ExcelService();

            var data = new List<object>
            {
                new MockFinancialReportRow
                {
                    FechaEmision = new DateTime(2026, 5, 26, 8, 15, 0),
                    PacienteNombre = "JUAN CARLOS PEREZ GOMEZ",
                    PacienteCedula = "V-12.345.678",
                    MontoTotal = 150.00m,
                    SaldoPendiente = 50.00m,
                    Estado = "Parcial",
                    IsAudited = true,
                    FechaCreacion = new DateTime(2026, 5, 26, 8, 15, 0),
                    UsuarioIngreso = "LIC. MARIA BRACAMONTE",
                    UsuarioAuditoria = "DR. JORGE ORTEGA",
                    FechaAuditoria = new DateTime(2026, 5, 26, 10, 0, 0),
                    Pagos = new List<MockFinancialPaymentRow>
                    {
                        new MockFinancialPaymentRow
                        {
                            Metodo = "Zelle",
                            Referencia = "Z-9988221",
                            MontoBase = 100.00m
                        }
                    }
                },
                new MockFinancialReportRow
                {
                    FechaEmision = new DateTime(2026, 5, 26, 9, 30, 0),
                    PacienteNombre = "MARIA ALICIA PEREZ DIAZ",
                    PacienteCedula = "V-32.999.888",
                    MontoTotal = 70.00m,
                    SaldoPendiente = 0.00m,
                    Estado = "Pagada",
                    IsAudited = false,
                    FechaCreacion = new DateTime(2026, 5, 26, 9, 30, 0),
                    UsuarioIngreso = "LIC. MARIA BRACAMONTE",
                    UsuarioAuditoria = null,
                    FechaAuditoria = null,
                    Pagos = new List<MockFinancialPaymentRow>
                    {
                        new MockFinancialPaymentRow
                        {
                            Metodo = "Pago Móvil",
                            Referencia = "P-445566",
                            MontoBase = 70.00m
                        }
                    }
                }
            };

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // 1. Financial Report - Standard Mode
            byte[] normalReportBytes = service.GenerateFinancialReport(data, isAuditMode: false);
            string normalReportPath = Path.Combine(desktopPath, "Reporte_Financiero_test_preview.xlsx");
            File.WriteAllBytes(normalReportPath, normalReportBytes);

            // 2. Financial Report - Audit Mode
            byte[] auditReportBytes = service.GenerateFinancialReport(data, isAuditMode: true);
            string auditReportPath = Path.Combine(desktopPath, "Reporte_Financiero_Auditoria_test_preview.xlsx");
            File.WriteAllBytes(auditReportPath, auditReportBytes);
        }

        [Fact]
        public void Generate_Generic_Excel_Preview()
        {
            var service = new ExcelService();

            var table = new DataTable("CitasMedicas");
            table.Columns.Add("FECHA", typeof(string));
            table.Columns.Add("PACIENTE", typeof(string));
            table.Columns.Add("MÉDICO", typeof(string));
            table.Columns.Add("ESPECIALIDAD", typeof(string));
            table.Columns.Add("ESTADO", typeof(string));

            table.Rows.Add("26/05/2026 08:30", "JUAN PEREZ", "DR. JORGE ORTEGA", "PEDIATRÍA", "Confirmada");
            table.Rows.Add("26/05/2026 09:00", "MARIA DIAZ", "DRA. HELENA SANCHEZ", "GINECOLOGÍA", "Pendiente");
            table.Rows.Add("26/05/2026 09:30", "CARLOS GOMEZ", "DR. JORGE ORTEGA", "PEDIATRÍA", "Cancelada");

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            byte[] genericReportBytes = service.GenerateExcel(table, "Listado de Citas");
            string genericReportPath = Path.Combine(desktopPath, "Reporte_Generico_test_preview.xlsx");
            File.WriteAllBytes(genericReportPath, genericReportBytes);
        }
    }
}
