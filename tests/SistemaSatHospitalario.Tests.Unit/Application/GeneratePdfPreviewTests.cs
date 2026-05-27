using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using SistemaSatHospitalario.Infrastructure.Services;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class GeneratePdfPreviewTests
    {
        [Fact]
        public void Generate_Compromiso_And_Garantia_Previews()
        {
            var service = new PdfGenerationService();

            var data = new CompromisoPagoDto
            {
                NombreResponsable = "JUAN CARLOS PEREZ GOMEZ",
                RelacionResponsable = "Padre",
                CedulaResponsable = "V-12.345.678",
                DireccionResponsable = "Av. Principal, Urb. Alto Barinas, Barinas",
                TelefonoResponsable = "+58-414-5555555",
                
                Conceptos = "INGRESO EMERGENCIA + EXÁMENES DE LABORATORIO + ECOGRAFÍA",
                
                NombrePaciente = "MARIA ALICIA PEREZ DIAZ",
                EdadPaciente = 8,
                CedulaPaciente = "V-32.999.888",
                
                MontoTotal = 70.00m,
                DiasLiquidar = 21,
                Cuotas = 0,
                MontoGarantia = 150.00m,
                DescripcionGarantia = "TELEFONO INTELIGENTE SAMSUNG GALAXY S21",
                
                QuienAutorizo = "LIC. MARIA BRACAMONTE",
                DoctorProcedimiento = "DRA. HELENA SANCHEZ (PEDIATRA)",
                InformacionAdicional = "El paciente ingresa por cuadro febril. Garantía prendaria dejada por el padre en administración.",
                EsPagoCompletado = false,
                
                FechaCompromiso = new DateTime(2026, 5, 26),
                FechaVencimiento = new DateTime(2026, 5, 26).AddDays(21)
            };

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // 1. Compromiso de Pago
            byte[] compromisoBytes = service.GenerarCompromisoPagoPdf(data, null);
            string compromisoPath = Path.Combine(desktopPath, "compromiso_de_pago_test_preview.pdf");
            File.WriteAllBytes(compromisoPath, compromisoBytes);

            // 2. Garantía de Pago
            byte[] garantiaBytes = service.GenerarGarantiaPdf(data, null);
            string garantiaPath = Path.Combine(desktopPath, "garantia_de_pago_test_preview.pdf");
            File.WriteAllBytes(garantiaPath, garantiaBytes);
        }

        [Fact]
        public void Generate_Recibo_Preview()
        {
            var service = new PdfGenerationService();

            var data = new ReciboPdfDto
            {
                Id = Guid.NewGuid(),
                NumeroRecibo = "REC-2026-000456",
                FechaEmision = new DateTime(2026, 5, 26, 14, 30, 0),
                PacienteNombre = "MARIA ALICIA PEREZ DIAZ",
                PacienteCedula = "V-32.999.888",
                TipoIngreso = "Seguro",
                TotalUSD = 110.00m,
                TotalBS = 4400.00m,
                TasaBcv = 40.00m,
                Detalles = new List<ReciboDetallePdfDto>
                {
                    new ReciboDetallePdfDto
                    {
                        Descripcion = "CONSULTA PEDIÁTRICA",
                        Cantidad = 1,
                        PrecioUnitario = 40.00m,
                        Subtotal = 40.00m
                    },
                    new ReciboDetallePdfDto
                    {
                        Descripcion = "EXÁMENES DE LABORATORIO (PERFIL 20)",
                        Cantidad = 1,
                        PrecioUnitario = 30.00m,
                        Subtotal = 30.00m
                    },
                    new ReciboDetallePdfDto
                    {
                        Descripcion = "RADIOGRAFÍA DE TÓRAX AP/LAT",
                        Cantidad = 1,
                        PrecioUnitario = 40.00m,
                        Subtotal = 40.00m
                    }
                },
                Pagos = new List<PagoDetallePdfDto>
                {
                    new PagoDetallePdfDto
                    {
                        MetodoPago = "Zelle",
                        MontoOriginal = 10.00m,
                        EquivalenteBase = 10.00m,
                        Referencia = "REF12345"
                    },
                    new PagoDetallePdfDto
                    {
                        MetodoPago = "Pago Móvil (Bolívares)",
                        MontoOriginal = 4000.00m,
                        EquivalenteBase = 100.00m,
                        Referencia = "REF67890"
                    }
                }
            };

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            byte[] reciboBytes = service.GenerarReciboPdf(data);
            string reciboPath = Path.Combine(desktopPath, "comprobante_facturacion_test_preview.pdf");
            File.WriteAllBytes(reciboPath, reciboBytes);
        }
    }
}
