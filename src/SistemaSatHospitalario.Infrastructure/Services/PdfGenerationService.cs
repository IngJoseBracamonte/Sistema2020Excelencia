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
                                    r.ConstantItem(70).AlignRight().Text($"$ {data.TotalUSD:N2}").SemiBold();
                                });
                                c.Item().PaddingTop(10).BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(5).Row(r => {
                                    r.RelativeItem().Text("TOTAL Bs.:").FontSize(13).SemiBold().FontColor(Colors.Blue.Medium);
                                    r.ConstantItem(80).AlignRight().Text($"{data.TotalBS:N2}").FontSize(13).SemiBold().FontColor(Colors.Blue.Medium);
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
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial).FontColor(Colors.Grey.Darken4));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            if (!string.IsNullOrEmpty(logoBase64))
                            {
                                try
                                {
                                    var imageBytes = Convert.FromBase64String(logoBase64.Split(',').Last());
                                    col.Item().Height(50).Image(imageBytes);
                                }
                                catch { }
                            }
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text("CENTRO DIAGNÓSTICO CLÍNICO LA EXCELENCIA, C.A.").FontSize(10).Bold().FontColor(Colors.Blue.Darken4);
                            col.Item().Text("RIF: J-41168255-1").FontSize(8).Bold();
                            col.Item().Text("Urb. José Antonio Páez, Casas 74 y 76, al lado de la Comandancia de la Policía, Barinas").FontSize(7).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().PaddingVertical(15).Column(column =>
                    {
                        column.Item().PaddingBottom(15).Column(c =>
                        {
                            c.Item().LineHorizontal(1).LineColor(Colors.Blue.Darken4);
                            c.Item().PaddingVertical(5).Text(text =>
                            {
                                text.AlignCenter();
                                text.Span(data.EsPagoCompletado ? "COMPROBANTE DE PAGO" : "COMPROMISO DE PAGO")
                                    .FontSize(14)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken4);
                            });
                            c.Item().LineHorizontal(1).LineColor(Colors.Blue.Darken4);
                        });

                        bool esMismoPaciente = data.NombreResponsable.Trim().Equals(data.NombrePaciente.Trim(), StringComparison.OrdinalIgnoreCase);

                        column.Item().PaddingBottom(15).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(120);
                                columns.RelativeColumn();
                                columns.ConstantColumn(120);
                                columns.RelativeColumn();
                            });

                            table.Cell().Element(LabelStyle).Text("Nombre Responsable:");
                            table.Cell().Element(ValueStyle).Text(data.NombreResponsable);
                            table.Cell().Element(LabelStyle).Text("C.I. / RIF:");
                            table.Cell().Element(ValueStyle).Text(data.CedulaResponsable);

                            table.Cell().Element(LabelStyle).Text("Teléfono:");
                            table.Cell().Element(ValueStyle).Text(data.TelefonoResponsable);
                            table.Cell().Element(LabelStyle).Text("Dirección Domicilio:");
                            table.Cell().Element(ValueStyle).Text(data.DireccionResponsable);

                            table.Cell().Element(LabelStyle).Text("Nombre Paciente:");
                            table.Cell().Element(ValueStyle).Text(data.NombrePaciente);
                            table.Cell().Element(LabelStyle).Text("C.I. Paciente:");
                            table.Cell().Element(ValueStyle).Text(data.CedulaPaciente);

                            table.Cell().Element(LabelStyle).Text("Carácter:");
                            table.Cell().Element(ValueStyle).Text(esMismoPaciente ? "Titular (El Paciente)" : $"Familiar Responsable ({data.RelacionResponsable})");
                            table.Cell().Element(LabelStyle).Text("Edad / Teléfono Paciente:");
                            table.Cell().Element(ValueStyle).Text($"{data.EdadPaciente} años / {data.TelefonoPaciente ?? "N/A"}");

                            static IContainer LabelStyle(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(5).DefaultTextStyle(x => x.Bold().FontSize(8));
                            static IContainer ValueStyle(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).DefaultTextStyle(x => x.FontSize(8));
                        });

                        column.Item().PaddingBottom(10).DefaultTextStyle(x => x.FontSize(9.5f)).Text(text =>
                        {
                            text.Span("Entre el ");
                            text.Span("CENTRO DIAGNÓSTICO CLÍNICO LA EXCELENCIA, C.A.").Bold();
                            text.Span(" (ya identificado), denominado ");
                            text.Span("LA CLÍNICA").Bold();
                            text.Span("; y por la otra parte, el ciudadano abajo firmante, denominado ");
                            text.Span("EL DEUDOR").Bold();
                            text.Span(", se suscribe el presente compromiso bajo las cláusulas siguientes:");
                        });

                        var vencimiento = data.FechaCompromiso.AddDays(21);

                        column.Item().PaddingBottom(8).DefaultTextStyle(x => x.FontSize(9.5f)).Text(text =>
                        {
                            text.Justify();
                            text.Span("PRIMERO (Deuda y Servicio): ").Bold().FontColor(Colors.Blue.Darken4);
                            text.Span("El Deudor reconoce expresamente al ");
                            text.Span($"{data.FechaCompromiso:dd/MM/yyyy}").Bold();
                            text.Span(" una deuda líquida y exigible con LA CLÍNICA por la cantidad de ");
                            text.Span($"${data.MontoTotal:N2}").Bold();
                            text.Span(", originada por el servicio médico/estudio realizado: ");
                            text.Span($"{data.Conceptos}").Bold();
                            
                            if (!esMismoPaciente)
                            {
                                text.Span(" realizado al paciente ");
                                text.Span($"{data.NombrePaciente}").Bold();
                                text.Span(", portador de la cédula de identidad ");
                                text.Span($"{data.CedulaPaciente}").Bold();
                            }
                            text.Span(".");
                        });

                        if (data.EsPagoCompletado)
                        {
                            column.Item().PaddingBottom(8).DefaultTextStyle(x => x.FontSize(9.5f)).Text(text =>
                            {
                                text.Justify();
                                text.Span("SEGUNDO (Solvencia de Pago): ").Bold().FontColor(Colors.Blue.Darken4);
                                text.Span("Se certifica que a la fecha ");
                                text.Span($"{data.FechaCompromiso:dd/MM/yyyy}").Bold();
                                text.Span(", la cantidad de ");
                                text.Span($"${data.MontoTotal:N2}").Bold();
                                text.Span(" ha sido recibida conforme por los servicios prestados, quedando la cuenta solvente ante esta institución.");
                            });
                        }
                        else
                        {
                            column.Item().PaddingBottom(8).DefaultTextStyle(x => x.FontSize(9.5f)).Text(text =>
                            {
                                text.Justify();
                                text.Span("SEGUNDO (Plazo de Pago): ").Bold().FontColor(Colors.Blue.Darken4);
                                text.Span("El Deudor se compromete a cancelar la totalidad del monto, fijando como fecha tope ");
                                text.Span("21 días continuos").Bold();
                                text.Span(" a partir de la fecha en que fue adquirido este compromiso.");
                            });

                            column.Item().PaddingBottom(8).DefaultTextStyle(x => x.FontSize(9.5f)).Text(text =>
                            {
                                text.Justify();
                                text.Span("Queda establecido así en este documento que el día ");
                                text.Span($"{vencimiento.Day}").Bold();
                                text.Span(" del mes de ");
                                text.Span($"{vencimiento.ToString("MMMM", new CultureInfo("es-ES"))}").Bold();
                                text.Span($" del presente año ({vencimiento.Year}) quedará saldada la deuda hacia con ustedes.");
                            });
                        }

                        column.Item().PaddingBottom(8).DefaultTextStyle(x => x.FontSize(9.5f)).Text(text =>
                        {
                            text.Span("TERCERO (Garantía): ").Bold().FontColor(Colors.Blue.Darken4);
                            text.Span("Para asegurar el cumplimiento de la obligación se establece la siguiente modalidad:");
                        });

                        column.Item().PaddingLeft(15).PaddingBottom(8).Column(g =>
                        {
                            var validItems = data.GarantiasItems?.Where(i => !string.IsNullOrWhiteSpace(i.Descripcion)).ToList() ?? new List<GarantiaItemDto>();
                            bool hasPrendaria = validItems.Count > 0 || (data.MontoGarantia > 0 && !string.IsNullOrEmpty(data.DescripcionGarantia));
                            bool hasFiador = !string.IsNullOrWhiteSpace(data.NombreFiador) || !esMismoPaciente;
                            bool noGarantia = !hasPrendaria && !hasFiador;

                            g.Item().PaddingVertical(2).Row(r =>
                            {
                                r.ConstantItem(25).Text(noGarantia ? "[ X ]" : "[   ]").Bold().FontColor(noGarantia ? Colors.Blue.Darken4 : Colors.Grey.Darken1);
                                r.RelativeItem().DefaultTextStyle(x => x.FontSize(9)).Text(t =>
                                {
                                    t.Span("NO DEJA GARANTÍA.");
                                });
                            });

                            if (hasPrendaria)
                            {
                                g.Item().PaddingVertical(2).Row(r =>
                                {
                                    r.ConstantItem(25).Text("[ X ]").Bold().FontColor(Colors.Blue.Darken4);
                                    r.RelativeItem().Column(pc =>
                                    {
                                        pc.Item().DefaultTextStyle(x => x.FontSize(9)).Text(t =>
                                        {
                                            t.Span("GARANTÍA PRENDARIA: ").Bold();
                                            t.Span("Entrega en resguardo el/los bienes detallados a continuación. ");
                                            t.Span("(Regulado por el Art. 1.837 del Código Civil. El objeto se devolverá inmediatamente al saldar la deuda).");
                                        });

                                        if (validItems.Count > 0)
                                        {
                                            pc.Item().PaddingTop(5).Table(table =>
                                            {
                                                table.ColumnsDefinition(columns =>
                                                {
                                                    columns.ConstantColumn(30);
                                                    columns.RelativeColumn();
                                                    columns.ConstantColumn(100);
                                                });

                                                table.Header(header =>
                                                {
                                                    header.Cell().Element(HeaderStyle).AlignCenter().Text("Nº");
                                                    header.Cell().Element(HeaderStyle).Text("Descripción del Bien");
                                                    header.Cell().Element(HeaderStyle).AlignRight().Text("Valor Estimado");

                                                    static IContainer HeaderStyle(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Medium).Background(Colors.Grey.Lighten3).Padding(3).DefaultTextStyle(x => x.Bold().FontSize(8));
                                                });

                                                int idx = 1;
                                                foreach (var item in validItems)
                                                {
                                                    table.Cell().Element(CellStyle).AlignCenter().Text(idx++.ToString());
                                                    table.Cell().Element(CellStyle).Text(item.Descripcion);
                                                    table.Cell().Element(CellStyle).AlignRight().Text($"$ {item.ValorEstimado:N2}");

                                                    static IContainer CellStyle(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).DefaultTextStyle(x => x.FontSize(8));
                                                }

                                                // Total Row
                                                table.Cell().Element(CellStyleTotal).Text("");
                                                table.Cell().Element(CellStyleTotal).AlignRight().Text("TOTAL:").Bold();
                                                table.Cell().Element(CellStyleTotal).AlignRight().Text($"$ {validItems.Sum(i => i.ValorEstimado):N2}").Bold();

                                                static IContainer CellStyleTotal(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).DefaultTextStyle(x => x.FontSize(8));
                                            });
                                        }
                                        else if (data.MontoGarantia > 0 && !string.IsNullOrEmpty(data.DescripcionGarantia))
                                        {
                                            pc.Item().PaddingTop(2).DefaultTextStyle(x => x.FontSize(9)).Text(t =>
                                            {
                                                t.Span("Bien: ").Bold();
                                                t.Span(data.DescripcionGarantia).Bold();
                                                t.Span(" con un valor estimado de ");
                                                t.Span($"${data.MontoGarantia:N2}").Bold();
                                                t.Span(".");
                                            });
                                        }
                                    });
                                });
                            }

                            if (hasFiador)
                            {
                                g.Item().PaddingVertical(2).Row(r =>
                                {
                                    r.ConstantItem(25).Text("[ X ]").Bold().FontColor(Colors.Blue.Darken4);
                                    r.RelativeItem().DefaultTextStyle(x => x.FontSize(9)).Text(t =>
                                    {
                                        t.Span("FIADOR / GARANTE PERSONAL: ").Bold();
                                        t.Span("Nombre: ");
                                        t.Span(!string.IsNullOrWhiteSpace(data.NombreFiador) ? data.NombreFiador : data.NombreResponsable).Bold();
                                        t.Span(", C.I.: ");
                                        t.Span(!string.IsNullOrWhiteSpace(data.CedulaFiador) ? data.CedulaFiador : data.CedulaResponsable).Bold();
                                        t.Span(", Telf: ");
                                        t.Span(!string.IsNullOrWhiteSpace(data.TelefonoFiador) ? data.TelefonoFiador : data.TelefonoResponsable).Bold();
                                        t.Span(".");
                                    });
                                });
                            }
                        });

                        column.Item().PaddingBottom(12).DefaultTextStyle(x => x.FontSize(9.5f)).Text(text =>
                        {
                            text.Justify();
                            text.Span("CUARTO (Marco Legal e Incumplimiento): ").Bold().FontColor(Colors.Blue.Darken4);
                            text.Span("Este documento constituye un título ejecutivo y ley entre las partes según el ");
                            text.Span("Art. 1.133 del Código Civil de Venezuela").Bold();
                            text.Span(", siendo de obligatorio cumplimiento bajo el ");
                            text.Span("Art. 1.264 ejusdem").Bold();
                            text.Span(". De verificarse mora a partir del ");
                            text.Span($"{vencimiento:dd/MM/yyyy}").Bold();
                            text.Span(", LA CLÍNICA queda plenamente facultada para ejercer acciones legales y de cobro judicial/extrajudicial conforme a los ");
                            text.Span("Arts. 1.271 y 1.277 del Código Civil").Bold();
                            text.Span(", y el ");
                            text.Span("Art. 537 del Código de Comercio").Bold();
                            text.Span(" para la ejecución de la prenda si aplica, corriendo por cuenta del Deudor los daños, intereses moratorios y costas generadas.");
                        });

                        column.Item().PaddingBottom(15).DefaultTextStyle(x => x.FontSize(9.5f)).Text(text =>
                        {
                            text.Span("En señal de conformidad, se firma en la ciudad de Barinas, a los ");
                            text.Span($"{data.FechaCompromiso:dd}").Bold();
                            text.Span(" días del mes de ");
                            text.Span($"{data.FechaCompromiso.ToString("MMMM", new CultureInfo("es-ES"))}").Bold();
                            text.Span($" del año {data.FechaCompromiso:yyyy}.");
                        });

                        if (!string.IsNullOrEmpty(data.QuienAutorizo) || !string.IsNullOrEmpty(data.DoctorProcedimiento) || !string.IsNullOrEmpty(data.InformacionAdicional))
                        {
                            column.Item().PaddingBottom(20).Background(Colors.Grey.Lighten5).Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(8).Column(c =>
                            {
                                c.Item().Row(r =>
                                {
                                    if (!string.IsNullOrEmpty(data.QuienAutorizo))
                                    {
                                        r.RelativeItem().DefaultTextStyle(x => x.FontSize(8)).Text(t =>
                                        {
                                            t.Span("Autorizado por: ").Bold();
                                            t.Span(data.QuienAutorizo);
                                        });
                                    }
                                    if (!string.IsNullOrEmpty(data.DoctorProcedimiento))
                                    {
                                        r.RelativeItem().AlignRight().DefaultTextStyle(x => x.FontSize(8)).Text(t =>
                                        {
                                            t.Span("Médico / Procedimiento: ").Bold();
                                            t.Span(data.DoctorProcedimiento);
                                        });
                                    }
                                });
                                if (!string.IsNullOrEmpty(data.InformacionAdicional))
                                {
                                    c.Item().PaddingTop(4).DefaultTextStyle(x => x.FontSize(8)).Text(t =>
                                    {
                                        t.Span("Notas Adicionales: ").Bold();
                                        t.Span(data.InformacionAdicional);
                                    });
                                }
                            });
                        }

                        column.Item().PaddingTop(30).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().LineHorizontal(1).LineColor(Colors.Grey.Darken1);
                                c.Item().PaddingTop(5).AlignCenter().Text(data.EsPagoCompletado ? "CLIENTE / PAGADOR" : "EL DEUDOR / RESPONSABLE").FontSize(9).Bold();
                                c.Item().AlignCenter().Text($"{data.NombreResponsable}").FontSize(8);
                                c.Item().AlignCenter().Text($"C.I: {data.CedulaResponsable}").FontSize(8);
                                
                                c.Item().PaddingTop(15).Row(r =>
                                {
                                    r.RelativeItem().Column(b =>
                                    {
                                        b.Item().Width(45).Height(45).Border(0.5f).BorderColor(Colors.Grey.Darken1).Background(Colors.Grey.Lighten5);
                                        b.Item().Width(45).AlignCenter().Text("Firma / Inicial").FontSize(6).FontColor(Colors.Grey.Medium);
                                    });
                                    r.RelativeItem().Column(b =>
                                    {
                                        b.Item().Width(45).Height(45).Border(0.5f).BorderColor(Colors.Grey.Darken1).Background(Colors.Grey.Lighten5);
                                        b.Item().Width(45).AlignCenter().Text("Pulgar Derecho").FontSize(6).FontColor(Colors.Grey.Medium);
                                    });
                                });
                            });

                            row.ConstantItem(40);

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().LineHorizontal(1).LineColor(Colors.Grey.Darken1);
                                c.Item().PaddingTop(5).AlignCenter().Text("POR LA CLÍNICA").FontSize(9).Bold();
                                c.Item().AlignCenter().Text("Firma Autorizada y Sello").FontSize(8);
                                c.Item().AlignCenter().Text("CENTRO DIAGNÓSTICO CLÍNICO LA EXCELENCIA").FontSize(8).FontColor(Colors.Grey.Medium);
                            });
                        });

                        column.Item().PaddingTop(15).Text("[   ] Se adjunta copia de la Cédula de Identidad del Deudor.").FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });

                if (data.AnexarGarantia)
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(1.5f, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial).FontColor(Colors.Grey.Darken4));

                        page.Header().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                if (!string.IsNullOrEmpty(logoBase64))
                                {
                                    try
                                    {
                                        var imageBytes = Convert.FromBase64String(logoBase64.Split(',').Last());
                                        col.Item().Height(50).Image(imageBytes);
                                    }
                                    catch { }
                                }
                            });

                            row.RelativeItem().AlignRight().Column(col =>
                            {
                                col.Item().Text("CENTRO DIAGNÓSTICO CLÍNICO LA EXCELENCIA, C.A.").FontSize(10).Bold().FontColor(Colors.Blue.Darken4);
                                col.Item().Text("RIF: J-41168255-1").FontSize(8).Bold();
                                col.Item().Text("Urb. José Antonio Páez, Casas 74 y 76, al lado de la Comandancia de la Policía, Barinas").FontSize(7).FontColor(Colors.Grey.Medium);
                            });
                        });

                        page.Content().PaddingVertical(15).Column(column =>
                        {
                            column.Item().PaddingBottom(15).Column(c =>
                            {
                                c.Item().LineHorizontal(1).LineColor(Colors.Blue.Darken4);
                                c.Item().PaddingVertical(5).Text(text =>
                                {
                                    text.AlignCenter();
                                    text.Span("GARANTÍA DE PAGO")
                                        .FontSize(14)
                                        .Bold()
                                        .FontColor(Colors.Blue.Darken4);
                                });
                                c.Item().LineHorizontal(1).LineColor(Colors.Blue.Darken4);
                            });

                            bool esMismoPaciente = data.NombreResponsable.Trim().Equals(data.NombrePaciente.Trim(), StringComparison.OrdinalIgnoreCase);

                            column.Item().PaddingBottom(15).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(120);
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(120);
                                    columns.RelativeColumn();
                                });

                                table.Cell().Element(LabelStyle).Text("Nombre Garante / Fiador:");
                                table.Cell().Element(ValueStyle).Text(!string.IsNullOrWhiteSpace(data.NombreFiador) ? data.NombreFiador : data.NombreResponsable);
                                table.Cell().Element(LabelStyle).Text("C.I. / RIF:");
                                table.Cell().Element(ValueStyle).Text(!string.IsNullOrWhiteSpace(data.CedulaFiador) ? data.CedulaFiador : data.CedulaResponsable);

                                table.Cell().Element(LabelStyle).Text("Teléfono:");
                                table.Cell().Element(ValueStyle).Text(!string.IsNullOrWhiteSpace(data.TelefonoFiador) ? data.TelefonoFiador : data.TelefonoResponsable);
                                table.Cell().Element(LabelStyle).Text("Dirección Domicilio:");
                                table.Cell().Element(ValueStyle).Text(!string.IsNullOrWhiteSpace(data.DireccionFiador) ? data.DireccionFiador : data.DireccionResponsable);

                                table.Cell().Element(LabelStyle).Text("Nombre Paciente:");
                                table.Cell().Element(ValueStyle).Text(data.NombrePaciente);
                                table.Cell().Element(LabelStyle).Text("C.I. Paciente:");
                                table.Cell().Element(ValueStyle).Text(data.CedulaPaciente);

                                table.Cell().Element(LabelStyle).Text("Relación con Paciente:");
                                table.Cell().Element(ValueStyle).Text(esMismoPaciente ? "Titular (El Paciente)" : data.RelacionResponsable);
                                table.Cell().Element(LabelStyle).Text("Edad / Teléfono Paciente:");
                                table.Cell().Element(ValueStyle).Text($"{data.EdadPaciente} años / {data.TelefonoPaciente ?? "N/A"}");

                                static IContainer LabelStyle(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(5).DefaultTextStyle(x => x.Bold().FontSize(8));
                                static IContainer ValueStyle(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).DefaultTextStyle(x => x.FontSize(8));
                            });

                            var vencimiento = data.FechaCompromiso.AddDays(21);

                            column.Item().PaddingBottom(12).DefaultTextStyle(x => x.FontSize(9.5f)).Text(text =>
                            {
                                text.Justify();
                                text.Span("En la ciudad de Barinas, Yo ");
                                text.Span($"{(!string.IsNullOrWhiteSpace(data.NombreFiador) ? data.NombreFiador : data.NombreResponsable)}").Bold();
                                text.Span(!string.IsNullOrWhiteSpace(data.NombreFiador) ? " (Fiador)" : (esMismoPaciente ? " (Titular)" : $" ({data.RelacionResponsable})"));
                                text.Span(", titular de la cédula de identidad ");
                                text.Span($"{(!string.IsNullOrWhiteSpace(data.CedulaFiador) ? data.CedulaFiador : data.CedulaResponsable)}").Bold();
                                text.Span(", en mi condición de Garante, declaro mediante el presente documento que garantizo la totalidad de los gastos médicos y servicios hospitalarios realizados a ");
                                text.Span($"{data.NombrePaciente}").Bold();
                                text.Span(", titular de la cédula de identidad ");
                                text.Span($"{data.CedulaPaciente}").Bold();
                                text.Span(".");
                            });

                            column.Item().PaddingBottom(12).DefaultTextStyle(x => x.FontSize(9.5f)).Text(text =>
                            {
                                text.Justify();
                                text.Span("Esta garantía respalda la cuenta del paciente por un monto referencial de ");
text.Span($"${data.MontoTotal:N2}").Bold();
                                text.Span(", comprometiéndome a cubrir cualquier excedente o diferencia no amparada por la empresa aseguradora o convenio en un lapso no mayor a ");
                                text.Span("21 días continuos").Bold();
                                text.Span(", fijando como fecha tope el día ");
                                text.Span($"{vencimiento:dd/MM/yyyy}").Bold();
                                text.Span(".");
                            });

                            var validItems = data.GarantiasItems?.Where(i => !string.IsNullOrWhiteSpace(i.Descripcion)).ToList() ?? new List<GarantiaItemDto>();
                            if (validItems.Count > 0 || data.MontoGarantia > 0)
                            {
                                column.Item().PaddingBottom(12).Column(pc =>
                                {
                                    pc.Item().PaddingBottom(5).DefaultTextStyle(x => x.FontSize(9.5f).Bold().FontColor(Colors.Blue.Darken4)).Text("RESPALDO FÍSICO DE LA GARANTÍA:");
                                    pc.Item().PaddingBottom(5).DefaultTextStyle(x => x.FontSize(9)).Text("Como respaldo físico de esta garantía, se hace entrega/registro de los bienes detallados a continuación (Regulado por el Art. 1.837 del Código Civil. El/los objetos se devolverán inmediatamente al saldar la deuda):");

                                    if (validItems.Count > 0)
                                    {
                                        pc.Item().Table(table =>
                                        {
                                            table.ColumnsDefinition(columns =>
                                            {
                                                columns.ConstantColumn(30);
                                                columns.RelativeColumn();
                                                columns.ConstantColumn(100);
                                            });

                                            table.Header(header =>
                                            {
                                                header.Cell().Element(HeaderStyle).AlignCenter().Text("Nº");
                                                header.Cell().Element(HeaderStyle).Text("Descripción del Bien");
                                                header.Cell().Element(HeaderStyle).AlignRight().Text("Valor Estimado");

                                                static IContainer HeaderStyle(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Medium).Background(Colors.Grey.Lighten3).Padding(3).DefaultTextStyle(x => x.Bold().FontSize(8));
                                            });

                                            int idx = 1;
                                            foreach (var item in validItems)
                                            {
                                                table.Cell().Element(CellStyle).AlignCenter().Text(idx++.ToString());
                                                table.Cell().Element(CellStyle).Text(item.Descripcion);
                                                table.Cell().Element(CellStyle).AlignRight().Text($"$ {item.ValorEstimado:N2}");

                                                static IContainer CellStyle(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).DefaultTextStyle(x => x.FontSize(8));
                                            }

                                            table.Cell().Element(CellStyleTotal).Text("");
                                            table.Cell().Element(CellStyleTotal).AlignRight().Text("TOTAL:").Bold();
                                            table.Cell().Element(CellStyleTotal).AlignRight().Text($"$ {validItems.Sum(i => i.ValorEstimado):N2}").Bold();

                                            static IContainer CellStyleTotal(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).DefaultTextStyle(x => x.FontSize(8));
                                        });
                                    }
                                    else
                                    {
                                        pc.Item().Background(Colors.Grey.Lighten5).Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(8).Text(text =>
                                        {
                                            text.Justify();
                                            text.Span("Bien: ").Bold();
                                            text.Span($"{data.DescripcionGarantia ?? "Bien mueble/inmueble"}").Bold();
                                            text.Span(" con un valor estimado de ");
                                            text.Span($"${data.MontoGarantia:N2}").Bold();
                                            text.Span(".");
                                        });
                                    }
                                });
                            }

                            column.Item().PaddingBottom(15).DefaultTextStyle(x => x.FontSize(9.5f)).Text(text =>
                            {
                                text.Justify();
                                text.Span("Acepto que este documento sirve como título ejecutivo y respaldo legal de la deuda contraída con el ");
                                text.Span("CENTRO DIAGNÓSTICO CLÍNICO LA EXCELENCIA, C.A.").Bold();
                                text.Span(".");
                            });

                            column.Item().PaddingBottom(20).Text("Sin otro particular, firmo conforme:").FontSize(9.5f);

                            column.Item().PaddingTop(30).Row(row =>
                            {
                                row.RelativeItem().Column(c =>
                                {
                                    c.Item().LineHorizontal(1).LineColor(Colors.Grey.Darken1);
                                    c.Item().PaddingTop(5).AlignCenter().Text("EL GARANTE").FontSize(9).Bold();
                                    c.Item().AlignCenter().Text($"{(!string.IsNullOrWhiteSpace(data.NombreFiador) ? data.NombreFiador : data.NombreResponsable)}").FontSize(8);
                                    c.Item().AlignCenter().Text($"C.I: {(!string.IsNullOrWhiteSpace(data.CedulaFiador) ? data.CedulaFiador : data.CedulaResponsable)}").FontSize(8);

                                    c.Item().PaddingTop(15).Row(r =>
                                    {
                                        r.RelativeItem().Column(b =>
                                        {
                                            b.Item().Width(45).Height(45).Border(0.5f).BorderColor(Colors.Grey.Darken1).Background(Colors.Grey.Lighten5);
                                            b.Item().Width(45).AlignCenter().Text("Firma / Inicial").FontSize(6).FontColor(Colors.Grey.Medium);
                                        });
                                        r.RelativeItem().Column(b =>
                                        {
                                            b.Item().Width(45).Height(45).Border(0.5f).BorderColor(Colors.Grey.Darken1).Background(Colors.Grey.Lighten5);
                                            b.Item().Width(45).AlignCenter().Text("Pulgar Derecho").FontSize(6).FontColor(Colors.Grey.Medium);
                                        });
                                    });
                                });

                                row.ConstantItem(40);

                                row.RelativeItem().Column(c =>
                                {
                                    c.Item().LineHorizontal(1).LineColor(Colors.Grey.Darken1);
                                    c.Item().PaddingTop(5).AlignCenter().Text("POR LA CLÍNICA").FontSize(9).Bold();
                                    c.Item().AlignCenter().Text("Firma Autorizada y Sello").FontSize(8);
                                    c.Item().AlignCenter().Text("CENTRO DIAGNÓSTICO CLÍNICO LA EXCELENCIA").FontSize(8).FontColor(Colors.Grey.Medium);
                                });
                            });
                        });
                    });
                }
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
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial).FontColor(Colors.Grey.Darken4));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            if (!string.IsNullOrEmpty(logoBase64))
                            {
                                try
                                {
                                    var imageBytes = Convert.FromBase64String(logoBase64.Split(',').Last());
                                    col.Item().Height(50).Image(imageBytes);
                                }
                                catch { }
                            }
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text("CENTRO DIAGNÓSTICO CLÍNICO LA EXCELENCIA, C.A.").FontSize(10).Bold().FontColor(Colors.Blue.Darken4);
                            col.Item().Text("RIF: J-41168255-1").FontSize(8).Bold();
                            col.Item().Text("Urb. José Antonio Páez, Casas 74 y 76, al lado de la Comandancia de la Policía, Barinas").FontSize(7).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().PaddingVertical(15).Column(column =>
                    {
                        column.Item().PaddingBottom(15).Column(c =>
                        {
                            c.Item().LineHorizontal(1).LineColor(Colors.Blue.Darken4);
                            c.Item().PaddingVertical(5).Text(text =>
                            {
                                text.AlignCenter();
                                text.Span("GARANTÍA DE PAGO")
                                    .FontSize(14)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken4);
                            });
                            c.Item().LineHorizontal(1).LineColor(Colors.Blue.Darken4);
                        });

                        bool esMismoPaciente = data.NombreResponsable.Trim().Equals(data.NombrePaciente.Trim(), StringComparison.OrdinalIgnoreCase);

                        column.Item().PaddingBottom(15).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(120);
                                columns.RelativeColumn();
                                columns.ConstantColumn(120);
                                columns.RelativeColumn();
                            });

                            table.Cell().Element(LabelStyle).Text("Nombre Garante / Fiador:");
                            table.Cell().Element(ValueStyle).Text(!string.IsNullOrWhiteSpace(data.NombreFiador) ? data.NombreFiador : data.NombreResponsable);
                            table.Cell().Element(LabelStyle).Text("C.I. / RIF:");
                            table.Cell().Element(ValueStyle).Text(!string.IsNullOrWhiteSpace(data.CedulaFiador) ? data.CedulaFiador : data.CedulaResponsable);

                            table.Cell().Element(LabelStyle).Text("Teléfono:");
                            table.Cell().Element(ValueStyle).Text(!string.IsNullOrWhiteSpace(data.TelefonoFiador) ? data.TelefonoFiador : data.TelefonoResponsable);
                            table.Cell().Element(LabelStyle).Text("Dirección Domicilio:");
                            table.Cell().Element(ValueStyle).Text(!string.IsNullOrWhiteSpace(data.DireccionFiador) ? data.DireccionFiador : data.DireccionResponsable);

                            table.Cell().Element(LabelStyle).Text("Nombre Paciente:");
                            table.Cell().Element(ValueStyle).Text(data.NombrePaciente);
                            table.Cell().Element(LabelStyle).Text("C.I. Paciente:");
                            table.Cell().Element(ValueStyle).Text(data.CedulaPaciente);

                            table.Cell().Element(LabelStyle).Text("Relación con Paciente:");
                            table.Cell().Element(ValueStyle).Text(esMismoPaciente ? "Titular (El Paciente)" : data.RelacionResponsable);
                            table.Cell().Element(LabelStyle).Text("Edad / Teléfono Paciente:");
                            table.Cell().Element(ValueStyle).Text($"{data.EdadPaciente} años / {data.TelefonoPaciente ?? "N/A"}");

                            static IContainer LabelStyle(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(5).DefaultTextStyle(x => x.Bold().FontSize(8));
                            static IContainer ValueStyle(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).DefaultTextStyle(x => x.FontSize(8));
                        });

                        var vencimiento = data.FechaCompromiso.AddDays(21);

                        column.Item().PaddingBottom(12).DefaultTextStyle(x => x.FontSize(9.5f)).Text(text =>
                        {
                            text.Justify();
                            text.Span("En la ciudad de Barinas, Yo ");
                            text.Span($"{(!string.IsNullOrWhiteSpace(data.NombreFiador) ? data.NombreFiador : data.NombreResponsable)}").Bold();
                            text.Span(!string.IsNullOrWhiteSpace(data.NombreFiador) ? " (Fiador)" : (esMismoPaciente ? " (Titular)" : $" ({data.RelacionResponsable})"));
                            text.Span(", titular de la cédula de identidad ");
                            text.Span($"{(!string.IsNullOrWhiteSpace(data.CedulaFiador) ? data.CedulaFiador : data.CedulaResponsable)}").Bold();
                            text.Span(", en mi condición de Garante, declaro mediante el presente documento que garantizo la totalidad de los gastos médicos y servicios hospitalarios realizados a ");
                            text.Span($"{data.NombrePaciente}").Bold();
                            text.Span(", titular de la cédula de identidad ");
                            text.Span($"{data.CedulaPaciente}").Bold();
                            text.Span(".");
                        });

                        column.Item().PaddingBottom(12).DefaultTextStyle(x => x.FontSize(9.5f)).Text(text =>
                        {
                            text.Justify();
                            text.Span("Esta garantía respalda la cuenta del paciente por un monto referencial de ");
                            text.Span($"${data.MontoTotal:N2}").Bold();
                            text.Span(", comprometiéndome a cubrir cualquier excedente o diferencia no amparada por la empresa aseguradora o convenio en un lapso no mayor a ");
                            text.Span("21 días continuos").Bold();
                            text.Span(", fijando como fecha tope el día ");
                            text.Span($"{vencimiento:dd/MM/yyyy}").Bold();
                            text.Span(".");
                        });

                         var validItems = data.GarantiasItems?.Where(i => !string.IsNullOrWhiteSpace(i.Descripcion)).ToList() ?? new List<GarantiaItemDto>();
                         if (validItems.Count > 0 || data.MontoGarantia > 0)
                         {
                             column.Item().PaddingBottom(12).Column(pc =>
                             {
                                 pc.Item().PaddingBottom(5).DefaultTextStyle(x => x.FontSize(9.5f).Bold().FontColor(Colors.Blue.Darken4)).Text("RESPALDO FÍSICO DE LA GARANTÍA:");
                                 pc.Item().PaddingBottom(5).DefaultTextStyle(x => x.FontSize(9)).Text("Como respaldo físico de esta garantía, se hace entrega/registro de los bienes detallados a continuación (Regulado por el Art. 1.837 del Código Civil. El/los objetos se devolverán inmediatamente al saldar la deuda):");

                                 if (validItems.Count > 0)
                                 {
                                     pc.Item().Table(table =>
                                     {
                                         table.ColumnsDefinition(columns =>
                                         {
                                             columns.ConstantColumn(30);
                                             columns.RelativeColumn();
                                             columns.ConstantColumn(100);
                                         });

                                         table.Header(header =>
                                         {
                                             header.Cell().Element(HeaderStyle).AlignCenter().Text("Nº");
                                             header.Cell().Element(HeaderStyle).Text("Descripción del Bien");
                                             header.Cell().Element(HeaderStyle).AlignRight().Text("Valor Estimado");

                                             static IContainer HeaderStyle(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Medium).Background(Colors.Grey.Lighten3).Padding(3).DefaultTextStyle(x => x.Bold().FontSize(8));
                                         });

                                         int idx = 1;
                                         foreach (var item in validItems)
                                         {
                                             table.Cell().Element(CellStyle).AlignCenter().Text(idx++.ToString());
                                             table.Cell().Element(CellStyle).Text(item.Descripcion);
                                             table.Cell().Element(CellStyle).AlignRight().Text($"$ {item.ValorEstimado:N2}");

                                             static IContainer CellStyle(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).DefaultTextStyle(x => x.FontSize(8));
                                         }

                                         // Total Row
                                         table.Cell().Element(CellStyleTotal).Text("");
                                         table.Cell().Element(CellStyleTotal).AlignRight().Text("TOTAL:").Bold();
                                         table.Cell().Element(CellStyleTotal).AlignRight().Text($"$ {validItems.Sum(i => i.ValorEstimado):N2}").Bold();

                                         static IContainer CellStyleTotal(IContainer container) => container.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).DefaultTextStyle(x => x.FontSize(8));
                                     });
                                 }
                                 else
                                 {
                                     pc.Item().Background(Colors.Grey.Lighten5).Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(8).Text(text =>
                                     {
                                         text.Justify();
                                         text.Span("Bien: ").Bold();
                                         text.Span($"{data.DescripcionGarantia ?? "Bien mueble/inmueble"}").Bold();
                                         text.Span(" con un valor estimado de ");
                                         text.Span($"${data.MontoGarantia:N2}").Bold();
                                         text.Span(".");
                                     });
                                 }
                             });
                         }

                        column.Item().PaddingBottom(15).DefaultTextStyle(x => x.FontSize(9.5f)).Text(text =>
                        {
                            text.Justify();
                            text.Span("Acepto que este documento sirve como título ejecutivo y respaldo legal de la deuda contraída con el ");
                            text.Span("CENTRO DIAGNÓSTICO CLÍNICO LA EXCELENCIA, C.A.").Bold();
                            text.Span(".");
                        });

                        column.Item().PaddingBottom(20).Text("Sin otro particular, firmo conforme:").FontSize(9.5f);

                        column.Item().PaddingTop(30).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().LineHorizontal(1).LineColor(Colors.Grey.Darken1);
                                c.Item().PaddingTop(5).AlignCenter().Text("EL GARANTE").FontSize(9).Bold();
                                c.Item().AlignCenter().Text($"{(!string.IsNullOrWhiteSpace(data.NombreFiador) ? data.NombreFiador : data.NombreResponsable)}").FontSize(8);
                                c.Item().AlignCenter().Text($"C.I: {(!string.IsNullOrWhiteSpace(data.CedulaFiador) ? data.CedulaFiador : data.CedulaResponsable)}").FontSize(8);

                                c.Item().PaddingTop(15).Row(r =>
                                {
                                    r.RelativeItem().Column(b =>
                                    {
                                        b.Item().Width(45).Height(45).Border(0.5f).BorderColor(Colors.Grey.Darken1).Background(Colors.Grey.Lighten5);
                                        b.Item().Width(45).AlignCenter().Text("Firma / Inicial").FontSize(6).FontColor(Colors.Grey.Medium);
                                    });
                                    r.RelativeItem().Column(b =>
                                    {
                                        b.Item().Width(45).Height(45).Border(0.5f).BorderColor(Colors.Grey.Darken1).Background(Colors.Grey.Lighten5);
                                        b.Item().Width(45).AlignCenter().Text("Pulgar Derecho").FontSize(6).FontColor(Colors.Grey.Medium);
                                    });
                                });
                            });

                            row.ConstantItem(40);

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().LineHorizontal(1).LineColor(Colors.Grey.Darken1);
                                c.Item().PaddingTop(5).AlignCenter().Text("POR LA CLÍNICA").FontSize(9).Bold();
                                c.Item().AlignCenter().Text("Firma Autorizada y Sello").FontSize(8);
                                c.Item().AlignCenter().Text("CENTRO DIAGNÓSTICO CLÍNICO LA EXCELENCIA").FontSize(8).FontColor(Colors.Grey.Medium);
                            });
                        });
                    });
                });
            }).GeneratePdf();

            return document;
        }

        public byte[] GenerarConformidadServiciosPdf(CompromisoPagoDto data, string? logoBase64)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial).FontColor(Colors.Grey.Darken4).LineHeight(1.5f));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            if (!string.IsNullOrEmpty(logoBase64))
                            {
                                try
                                {
                                    var imageBytes = Convert.FromBase64String(logoBase64.Split(',').Last());
                                    col.Item().Height(55).Image(imageBytes);
                                }
                                catch { }
                            }
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text("CENTRO DIAGNÓSTICO CLÍNICO LA EXCELENCIA, C.A.").FontSize(10).Bold().FontColor(Colors.Blue.Darken4);
                            col.Item().Text("RIF: J-41168255-1").FontSize(8).Bold();
                            col.Item().Text("Urb. José Antonio Páez, Casas 74 y 76, al lado de la Comandancia de la Policía, Barinas").FontSize(7).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().PaddingVertical(25).Column(column =>
                    {
                        column.Item().PaddingBottom(25).Column(c =>
                        {
                            c.Item().LineHorizontal(1).LineColor(Colors.Blue.Darken4);
                            c.Item().PaddingVertical(8).Text(text =>
                            {
                                text.AlignCenter();
                                text.Span("CONFORMIDAD DE SERVICIOS")
                                    .FontSize(15)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken4);
                            });
                            c.Item().LineHorizontal(1).LineColor(Colors.Blue.Darken4);
                        });
                        column.Item().PaddingBottom(20).Text(text =>
                        {
                            text.Justify();
                            text.Span("Por medio de la presente, yo ");
                            text.Span(!string.IsNullOrWhiteSpace(data.NombrePaciente) ? data.NombrePaciente : "______________________________________________________").Bold();
                            text.Span(", titular de la Cédula de Identidad ");
                            text.Span(!string.IsNullOrWhiteSpace(data.CedulaPaciente) ? data.CedulaPaciente : "_____________________").Bold();

                            bool esMismoPaciente = false;
                            if (!string.IsNullOrWhiteSpace(data.NombreResponsable))
                            {
                                esMismoPaciente = string.Equals(data.NombrePaciente?.Trim(), data.NombreResponsable?.Trim(), StringComparison.OrdinalIgnoreCase) 
                                               || string.Equals(data.CedulaPaciente?.Trim(), data.CedulaResponsable?.Trim(), StringComparison.OrdinalIgnoreCase)
                                               || string.Equals(data.NombreResponsable?.Trim(), "Particular", StringComparison.OrdinalIgnoreCase);
                            }
                            else
                            {
                                esMismoPaciente = string.Equals(data.RelacionResponsable?.Trim(), "Titular", StringComparison.OrdinalIgnoreCase);
                            }

                            if (!esMismoPaciente)
                            {
                                text.Span(", beneficiario del trabajador ");
                                text.Span(!string.IsNullOrWhiteSpace(data.NombreResponsable) ? data.NombreResponsable : "_____________________________________").Bold();
                                text.Span(" de Cedula de Identidad ");
                                text.Span(!string.IsNullOrWhiteSpace(data.CedulaResponsable) ? data.CedulaResponsable : "_____________________").Bold();
                            }

                            text.Span(", declaro haber recibido de conformidad la factura original N° ");
                            text.Span(!string.IsNullOrWhiteSpace(data.NroFactura) ? data.NroFactura : "_____________________").Bold();
                            text.Span(", en el cual se realizaron todos los servicios descritos, por un monto de ");
                            
                            if (data.MontoTotal > 0)
                            {
                                text.Span($"$ {data.MontoTotal:N2}").Bold();
                            }
                            else
                            {
                                text.Span("____________________").Bold();
                            }
                            
                            text.Span(", emitida por ");
                            text.Span("Centro Diagnóstico Clínico La Excelencia, C.A.").Bold();
                            text.Span(".");
                        });

                        column.Item().PaddingBottom(20).Text(text =>
                        {
                            text.Justify();
                            text.Span("La presente firma avala únicamente la recepción física del documento fiscal para los fines administrativos que correspondan.");
                        });

                        column.Item().PaddingBottom(35).Text(text =>
                        {
                            text.Justify();
                            text.Span("Declaro expresamente que he recibido a entera satisfacción los servicios médicos, estudios y/o procedimientos descritos en el documento fiscal antes mencionado. Los nombres y números de documento de identidad aquí registrados corresponden fielmente a nuestras identidades legales, asumiendo total responsabilidad por la veracidad de esta información. Asimismo, certifico que ");
                            text.Span("todo lo reflejado en la factura corresponde fielmente a los servicios realizados").Bold();
                            text.Span(".");
                        });

                        column.Item().PaddingTop(50).Row(row =>
                        {
                            row.RelativeItem().Text(t =>
                            {
                                t.Span("Firma: ").Bold();
                                t.Span("__________________________________");
                            });
                            row.RelativeItem().AlignRight().Text(t =>
                            {
                                t.Span("Fecha: ").Bold();
                                t.Span("__________________________________");
                            });
                        });
                    });
                });
            }).GeneratePdf();

            return document;
        }
    }
}
