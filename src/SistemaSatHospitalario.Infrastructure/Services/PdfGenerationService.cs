using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using System;
using System.Linq;

namespace SistemaSatHospitalario.Infrastructure.Services
{
    public class PdfGenerationService : IPdfService
    {
        public PdfGenerationService()
        {
            // QuestPDF License Requirement
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerarReciboPdf(ReciboPdfDto data)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Verdana));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("COMPROBANTE DE FACTURACIÓN").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                            col.Item().Text($"{data.NumeroRecibo}").FontSize(14);
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text("SISTEMA SAT HOSPITALARIO").FontSize(12).SemiBold();
                            col.Item().Text($"Fecha: {data.FechaEmision:dd/MM/yyyy HH:mm}");
                        });
                    });

                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        // Patient Section
                        column.Item().PaddingBottom(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Row(row =>
                        {
                            row.RelativeItem().Column(c => {
                                c.Item().Text("DATOS DEL PACIENTE").FontSize(9).FontColor(Colors.Grey.Medium);
                                c.Item().Text(data.PacienteNombre).SemiBold();
                                c.Item().Text($"C.I./Pasaporte: {data.PacienteCedula}");
                            });
                            row.RelativeItem().AlignRight().Column(c => {
                                c.Item().Text("TIPO DE INGRESO").FontSize(9).FontColor(Colors.Grey.Medium);
                                c.Item().Text(data.TipoIngreso).SemiBold();
                                c.Item().Text($"Tasa Ref: {data.TasaBcv:N2} Bs/$").FontSize(8);
                            });
                        });

                        // Items Table
                        column.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.RelativeColumn();
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(80);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("CANT");
                                header.Cell().Element(CellStyle).Text("DESCRIPCIÓN");
                                header.Cell().Element(CellStyle).AlignRight().Text("PRECIO UNIT");
                                header.Cell().Element(CellStyle).AlignRight().Text("SUBTOTAL");

                                static IContainer CellStyle(IContainer container) => container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            });

                            foreach (var item in data.Detalles)
                            {
                                table.Cell().Element(ItemStyle).Text(item.Cantidad.ToString());
                                table.Cell().Element(ItemStyle).Text(item.Descripcion);
                                table.Cell().Element(ItemStyle).AlignRight().Text($"$ {item.PrecioUnitario:N2}");
                                table.Cell().Element(ItemStyle).AlignRight().Text($"$ {item.Subtotal:N2}");

                                static IContainer ItemStyle(IContainer container) => container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten3);
                            }
                        });

                        // Payments Section
                        column.Item().PaddingTop(15).Row(row =>
                        {
                            row.RelativeItem().Column(c => {
                                c.Item().PaddingBottom(5).Text("MÉTODOS DE PAGO").FontSize(9).SemiBold().FontColor(Colors.Grey.Medium);
                                foreach (var p in data.Pagos)
                                {
                                    c.Item().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten4).PaddingVertical(2).Row(r => {
                                        r.RelativeItem().Text($"{p.MetodoPago}").FontSize(8);
                                        r.RelativeItem().AlignRight().Text($"{p.MontoOriginal:N2}").FontSize(8).SemiBold();
                                    });
                                }
                            });

                            row.ConstantItem(40); // Spacer

                            row.RelativeItem().Column(c => {
                                c.Item().Row(r => {
                                    r.RelativeItem().Text("TOTAL USD:").SemiBold();
                                    r.ConstantItem(80).AlignRight().Text($"$ {data.TotalUSD:N2}").SemiBold();
                                });
                                c.Item().PaddingVertical(5).BorderTop(1).BorderColor(Colors.Grey.Lighten2).Row(r => {
                                    r.RelativeItem().Text("TOTAL Bs.:").FontSize(14).SemiBold().FontColor(Colors.Blue.Medium);
                                    r.ConstantItem(100).AlignRight().Text($"{data.TotalBS:N2}").FontSize(14).SemiBold().FontColor(Colors.Blue.Medium);
                                });
                                c.Item().AlignRight().Text($"Tasa: {data.TasaBcv:N2} Bs/$").FontSize(7).Italic().FontColor(Colors.Grey.Medium);
                            });
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf();

            return document;
        }
    }
}
