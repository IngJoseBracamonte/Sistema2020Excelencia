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
                            bool hasPrendaria = data.MontoGarantia > 0 && !string.IsNullOrEmpty(data.DescripcionGarantia);
                            bool hasFiador = !esMismoPaciente;
                            bool noGarantia = !hasPrendaria && !hasFiador;

                            g.Item().PaddingVertical(2).Row(r =>
                            {
                                r.ConstantItem(25).Text(noGarantia ? "[ X ]" : "[   ]").Bold().FontColor(noGarantia ? Colors.Blue.Darken4 : Colors.Grey.Darken1);
                                r.RelativeItem().DefaultTextStyle(x => x.FontSize(9)).Text(t =>
                                {
                                    t.Span("NO DEJA GARANTÍA.");
                                });
                            });

                            g.Item().PaddingVertical(2).Row(r =>
                            {
                                r.ConstantItem(25).Text(hasPrendaria ? "[ X ]" : "[   ]").Bold().FontColor(hasPrendaria ? Colors.Blue.Darken4 : Colors.Grey.Darken1);
                                r.RelativeItem().DefaultTextStyle(x => x.FontSize(9)).Text(t =>
                                {
                                    t.Span("GARANTÍA PRENDARIA: ").Bold();
                                    t.Span("Entrega en resguardo el bien: ");
                                    t.Span(hasPrendaria ? data.DescripcionGarantia! : "__________________________").Bold();
                                    if (hasPrendaria)
                                    {
                                        t.Span(" con un valor estimado de ");
                                        t.Span($"${data.MontoGarantia:N2}").Bold();
                                    }
                                    t.Span(". (Regulado por el Art. 1.837 del Código Civil. El objeto se devolverá inmediatamente al saldar la deuda).");
                                });
                            });

                            g.Item().PaddingVertical(2).Row(r =>
                            {
                                r.ConstantItem(25).Text(hasFiador ? "[ X ]" : "[   ]").Bold().FontColor(hasFiador ? Colors.Blue.Darken4 : Colors.Grey.Darken1);
                                r.RelativeItem().DefaultTextStyle(x => x.FontSize(9)).Text(t =>
                                {
                                    t.Span("FIADOR / GARANTE PERSONAL: ").Bold();
                                    t.Span("Nombre: ");
                                    t.Span(hasFiador ? data.NombreResponsable : "__________________________").Bold();
                                    t.Span(", C.I.: ");
                                    t.Span(hasFiador ? data.CedulaResponsable : "__________________________").Bold();
                                    t.Span(", Telf: ");
                                    t.Span(hasFiador ? data.TelefonoResponsable : "__________________________").Bold();
                                    t.Span(".");
                                });
                            });
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
                                text.Span($"{data.NombreResponsable}").Bold();
                                text.Span(esMismoPaciente ? " (Titular)" : $" ({data.RelacionResponsable})");
                                text.Span(", titular de la cédula de identidad ");
                                text.Span($"{data.CedulaResponsable}").Bold();
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

                            if (data.MontoGarantia > 0)
                            {
                                column.Item().PaddingBottom(12).DefaultTextStyle(x => x.FontSize(9.5f)).Background(Colors.Grey.Lighten5).Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(8).Text(text =>
                                {
                                    text.Justify();
                                    text.Span("RESPALDO FÍSICO DE LA GARANTÍA: ").Bold().FontColor(Colors.Blue.Darken4);
                                    text.Span("Como respaldo físico de esta garantía, se hace entrega/registro de: ");
                                    text.Span($"{data.DescripcionGarantia ?? "Bien mueble/inmueble"}").Bold();
                                    text.Span(" con un valor estimado de ");
                                    text.Span($"${data.MontoGarantia:N2}").Bold();
                                    text.Span(". (Regulado por el Art. 1.837 del Código Civil. El objeto se devolverá inmediatamente al saldar la deuda).");
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
                            text.Span($"{data.NombreResponsable}").Bold();
                            text.Span(esMismoPaciente ? " (Titular)" : $" ({data.RelacionResponsable})");
                            text.Span(", titular de la cédula de identidad ");
                            text.Span($"{data.CedulaResponsable}").Bold();
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

                        if (data.MontoGarantia > 0)
                        {
                            column.Item().PaddingBottom(12).DefaultTextStyle(x => x.FontSize(9.5f)).Background(Colors.Grey.Lighten5).Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(8).Text(text =>
                            {
                                text.Justify();
                                text.Span("RESPALDO FÍSICO DE LA GARANTÍA: ").Bold().FontColor(Colors.Blue.Darken4);
                                text.Span("Como respaldo físico de esta garantía, se hace entrega/registro de: ");
                                text.Span($"{data.DescripcionGarantia ?? "Bien mueble/inmueble"}").Bold();
                                text.Span(" con un valor estimado de ");
                                text.Span($"${data.MontoGarantia:N2}").Bold();
                                text.Span(". (Regulado por el Art. 1.837 del Código Civil. El objeto se devolverá inmediatamente al saldar la deuda).");
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
                    });
                });
            }).GeneratePdf();

            return document;
        }
    }
}
