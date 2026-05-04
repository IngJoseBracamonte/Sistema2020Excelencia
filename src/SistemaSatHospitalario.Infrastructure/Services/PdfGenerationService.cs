using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using System;
using System.Linq;
using System.Globalization;

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

        public byte[] GenerarCompromisoPagoPdf(CompromisoPagoDto data, string? logoBase64)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            if (!string.IsNullOrEmpty(logoBase64))
                            {
                                try
                                {
                                    var imageBytes = Convert.FromBase64String(logoBase64.Split(',').Last());
                                    col.Item().Height(60).Image(imageBytes);
                                }
                                catch { }
                            }
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text("CENTRO DIAGNOSTICO CLINICO LA EXCELENCIA C.A").FontSize(10).SemiBold();
                            col.Item().Text("RIF: J-41168255-1").FontSize(10);
                        });
                    });

                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        column.Item().AlignCenter().PaddingBottom(20)
                            .Text("COMPROMISO DE PAGO").FontSize(14).SemiBold().Underline();

                        column.Item().Text(text =>
                        {
                            text.Span("Yo ");
                            text.Span($"{data.NombreResponsable}").SemiBold();
                            text.Span($" ({data.RelacionResponsable}), titular de la cédula de identidad ");
                            text.Span($"{data.CedulaResponsable}").SemiBold();
                            text.Span($", domiciliado en {data.DireccionResponsable}, teléfonos {data.TelefonoResponsable}, me comprometo a cancelar la deuda por concepto de ");
                            text.Span($"{data.Conceptos}").SemiBold();
                            text.Span($" realizada a ");
                            text.Span($"{data.NombrePaciente}").SemiBold();
                            text.Span($", edad {data.EdadPaciente}, titular de la cédula de identidad ");
                            text.Span($"{data.CedulaPaciente}").SemiBold();
                            text.Span(".");
                        });

                        column.Item().PaddingTop(10).Text(text =>
                        {
                            text.Span("Reconozco que la deuda que tengo con el CENTRO DIAGNÓSTICO CLÍNICO LA EXCELENCIA C.A RIF a partir del ");
                            text.Span($"{data.FechaCompromiso:dd/MM/yyyy}").SemiBold();
                            text.Span($", asciende a la cantidad de ");
                            text.Span($"${data.MontoTotal:N2}").SemiBold();
                            text.Span($". La misma será liquidada en su totalidad en {data.DiasLiquidar} días continuos a partir de la fecha en que fue adquirido este compromiso dividido en {data.Cuotas} cuotas.");
                        });

                        column.Item().PaddingTop(10).Text(text =>
                        {
                            text.Span($"Queda establecido así en este documento que el día {data.FechaVencimiento.Day} del mes de {data.FechaVencimiento.ToString("MMMM", new CultureInfo("es-ES"))} del presente año ({data.FechaVencimiento.Year}) quedará saldada la deuda hacia con ustedes.");
                        });

                        column.Item().PaddingTop(10).Text(text =>
                        {
                            text.Span("En caso de no cumplir con el pago en la fecha antes mencionada me haré responsable de las medidas legales que considere conveniente. No obstante me comprometo a cumplir al pie de la letra con lo establecido para no llevar esto a consecuencias en las que me haga acreedor de una sanción.");
                        });

                        column.Item().PaddingTop(20).Text("Sin otro particular, quedo a la orden para cualquier asunto");

                        // Firmas
                        column.Item().PaddingTop(60).Row(row =>
                        {
                            row.RelativeItem().AlignCenter().Column(c =>
                            {
                                c.Item().LineHorizontal(1);
                                c.Item().PaddingTop(5).Text("DEUDOR").SemiBold();
                                c.Item().Text($"Nombre y Apellido: {data.NombreResponsable}");
                                c.Item().Text($"C.I: {data.CedulaResponsable}");
                            });

                            row.ConstantItem(40); // Spacer

                            row.RelativeItem().AlignCenter().Column(c =>
                            {
                                c.Item().LineHorizontal(1);
                                c.Item().PaddingTop(5).Text("ACREEDOR DE LA DEUDA").SemiBold();
                                c.Item().Text("Nombre y Apellido: ________________");
                                c.Item().Text("C.I: ________________");
                            });
                        });
                    });
                });
            }).GeneratePdf();

            return document;
        }

        public byte[] GenerarGarantiaPdf(CompromisoPagoDto data, string? logoBase64)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            if (!string.IsNullOrEmpty(logoBase64))
                            {
                                try
                                {
                                    var imageBytes = Convert.FromBase64String(logoBase64.Split(',').Last());
                                    col.Item().Height(60).Image(imageBytes);
                                }
                                catch { }
                            }
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text("CENTRO DIAGNOSTICO CLINICO LA EXCELENCIA C.A").FontSize(10).SemiBold();
                            col.Item().Text("RIF: J-41168255-1").FontSize(10);
                        });
                    });

                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        column.Item().AlignCenter().PaddingBottom(20)
                            .Text("GARANTIA DE PAGO").FontSize(14).SemiBold().Underline();

                        column.Item().Text(text =>
                        {
                            text.Span("En la ciudad de Guanare, Yo ");
                            text.Span($"{data.NombreResponsable}").SemiBold();
                            text.Span($" ({data.RelacionResponsable}), titular de la cédula de identidad ");
                            text.Span($"{data.CedulaResponsable}").SemiBold();
                            text.Span($", en mi condición de Garante, declaro mediante el presente documento que garantizo la totalidad de los gastos médicos y servicios hospitalarios realizados a ");
                            text.Span($"{data.NombrePaciente}").SemiBold();
                            text.Span($", titular de la cédula de identidad ");
                            text.Span($"{data.CedulaPaciente}").SemiBold();
                            text.Span(".");
                        });

                        column.Item().PaddingTop(10).Text(text =>
                        {
                            text.Span("Esta garantía respalda la cuenta del paciente por un monto referencial de ");
                            text.Span($"${data.MontoTotal:N2}").SemiBold();
                            text.Span($", comprometiéndome a cubrir cualquier excedente o diferencia no amparada por la empresa aseguradora o convenio en un lapso no mayor a {data.DiasLiquidar} días. ");
                            
                            if (data.MontoGarantia > 0)
                            {
                                text.Span("Como respaldo físico de esta garantía, se hace entrega/registro de: ");
                                text.Span($"{data.DescripcionGarantia ?? "Bien mueble/inmueble"}").SemiBold();
                                text.Span(" con un valor estimado de ");
                                text.Span($"${data.MontoGarantia:N2}").SemiBold();
                                text.Span(".");
                            }
                        });


                        column.Item().PaddingTop(10).Text(text =>
                        {
                            text.Span("Acepto que este documento sirve como título ejecutivo y respaldo legal de la deuda contraída con el CENTRO DIAGNÓSTICO CLÍNICO LA EXCELENCIA C.A.");
                        });

                        column.Item().PaddingTop(20).Text("Sin otro particular, firmo conforme:");

                        // Firmas
                        column.Item().PaddingTop(60).Row(row =>
                        {
                            row.RelativeItem().AlignCenter().Column(c =>
                            {
                                c.Item().LineHorizontal(1);
                                c.Item().PaddingTop(5).Text("EL GARANTE").SemiBold();
                                c.Item().Text($"Nombre: {data.NombreResponsable}");
                                c.Item().Text($"C.I: {data.CedulaResponsable}");
                            });

                            row.ConstantItem(40); // Spacer

                            row.RelativeItem().AlignCenter().Column(c =>
                            {
                                c.Item().LineHorizontal(1);
                                c.Item().PaddingTop(5).Text("POR LA CLINICA").SemiBold();
                                c.Item().Text("Sello y Firma Autorizada");
                            });
                        });
                    });
                });
            }).GeneratePdf();

            return document;
        }
    }
}
