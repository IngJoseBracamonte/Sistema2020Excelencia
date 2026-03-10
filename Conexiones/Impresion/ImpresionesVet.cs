using Conexiones.DbConnect;
using Conexiones.Dto;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Conexiones.Impresiones.Impresion
{
    public class ImpresionesVet
    {
        const string facename = "Arial Rounded MT";
        const string Curva = "Bookman Old Style";
        const string Curva2 = "Bookman Old Style";
        XFont Titulo = new XFont(Curva, 18, XFontStyle.Regular);
        readonly string Ruta = ConfigurationManager.ConnectionStrings["Ruta"].ConnectionString;
        List<Usuarios> Bioanalistas = new List<Usuarios>();
        bool primerapagina = false;
        double PosicionP = 110;
        double Posicion = 110;
        private void VerificarRuta(string ruta)
        {
            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }
        }

        public static PdfSharp.Pdf.PdfDocument Presupuesto(PdfSharp.Pdf.PdfDocument document, int IdOrden, string PrecioF)
        {
            Empresa empresa = new Empresa();
            empresa = ConexionVeterinaria.selectEmpresaActiva();

            XFont fontRegular = new XFont(facename, 12, XFontStyle.Regular);
            XFont fontRegular2 = new XFont(facename, 12, XFontStyle.Regular);
            XFont fontRegular3 = new XFont(facename, 8, XFontStyle.Regular);
            XFont Direccion = new XFont(facename, 8, XFontStyle.Regular);
            XFont DireccionCaratula = new XFont(Curva, 10, XFontStyle.Italic);
            XFont TituloCaratula = new XFont(Curva2, 12, XFontStyle.Regular);
            XFont Caratula = new XFont(Curva, 22, XFontStyle.Regular);
            XFont Cursiva = new XFont(Curva2, 10, XFontStyle.Italic);
            PdfSharp.Pdf.PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            string Formato = "";

            string cmd, cmd2, cmd3, cmd4, cmd5, cmd6, cmd7, cmd8;
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen;
            XSize size, length;
            XPen lineRed = new XPen(XColors.Black, 1);
            XRect rect;
            pen = new XPen(color);
            XSize Elipsesize = new XSize(5, 5);
            XPoint point;
            double contador;
            point = new XPoint(3, 3);
            size = new XSize(589, 394);
            rect = new XRect(point, size);
            gfx.DrawRectangle(pen, rect);
            DatosRepresentante Persona = new DatosRepresentante();
            DataSet Tasa = new DataSet();
            double PosicionP = 15;
            double Posicion = 110;
            XBrush brushes;
            double y = 0;
            DateTime FechaImp = new DateTime();
            List<PerfilesAFacturar> perfilesAFacturar = new List<PerfilesAFacturar>();
            perfilesAFacturar = ConexionVeterinaria.selectPerfilesAFacturar(IdOrden);
            PerfilesAFacturar perfilSeleccionado = perfilesAFacturar.FirstOrDefault();
            Persona = perfilSeleccionado.datosRepresentante;
            DataSet TasaDia = new DataSet();
            TasaDia = ConexionVeterinaria.SELECTTasaDia();
            DataSet ds4 = new DataSet();
            ds4 = ConexionVeterinaria.SeleccionarPagos2(IdOrden.ToString());
            PosicionP = 20;
            //Logo
            XRect Margen = new XRect(220, PosicionP, 145, 14);
            XImage Logo = XImage.FromFile(string.Format(@"Logos\{0}", empresa.Ruta));
            Margen = new XRect(105, PosicionP, 460, 120);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(empresa.Nombre.ToString(), Caratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(470, 95, 250, 48);
            tf.Alignment = XParagraphAlignment.Left;
            PosicionP = 60;
            Margen = new XRect(197, PosicionP, 145, 14);
            string Titulo = "PRESUPUESTO";
            gfx.DrawString(Titulo, Caratula, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            PosicionP = PosicionP + 30;
            Margen = new XRect(20, PosicionP, 145, 14);
            cmd = "";
            cmd2 = DateTime.Now.ToString("dd/MM/yyyy");
            cmd3 = "Nombre del Representante: ";
            cmd4 = Persona.NombreRepresentante + " " + Persona.ApellidoRepresentante;
            cmd5 = "C.I o RIF: ";
            cmd6 = Persona.TipoRepresentante + "-" + Persona.RIF;


            gfx.DrawString(cmd, fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);

            length = gfx.MeasureString(cmd, fontRegular2);
            contador = 30 + length.Width;
            Margen = new XRect(contador, PosicionP, 145, 14);
            gfx.DrawString(cmd2, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);

            length = gfx.MeasureString(cmd2, fontRegular2);
            contador = contador + length.Width + 5;
            Margen = new XRect(contador, PosicionP, 145, 14);
            gfx.DrawString(cmd3, fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);

            length = gfx.MeasureString(cmd3, fontRegular2);
            contador = contador + length.Width + 5;
            Margen = new XRect(contador, PosicionP, 145, 14);
            gfx.DrawString(cmd4, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);

            length = gfx.MeasureString(cmd4, fontRegular2);
            contador = contador + length.Width + 5;
            Margen = new XRect(contador, PosicionP, 145, 14);
            gfx.DrawString(cmd5, fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);

            length = gfx.MeasureString(cmd5, fontRegular2);
            contador = contador + length.Width + 5;
            Margen = new XRect(contador, PosicionP, 145, 14);
            gfx.DrawString(cmd6, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);




            PosicionP = PosicionP + 30;
            Margen = new XRect(40, PosicionP, 145, 14);
            gfx.DrawString("Examenes ", Direccion, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(250, PosicionP, 145, 14);
            gfx.DrawString("Cantidad", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(350, PosicionP, 145, 14);
            gfx.DrawString("Precio Unitario", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(450, PosicionP, 145, 14);
            gfx.DrawString("Total", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            bool parte2 = true;
            PosicionP = PosicionP + 15;
            decimal sumaTotal = 0;
            foreach (var items in perfilesAFacturar)
            {

                decimal Dolar = Convert.ToDecimal(TasaDia.Tables[0].Rows[0]["Dolar"].ToString());

                Margen = new XRect(40, PosicionP, 145, 14);
                gfx.DrawString(items.perfil.NombrePerfil, fontRegular3, XBrushes.Black, Margen, XStringFormats.CenterLeft);

                Margen = new XRect(250, PosicionP, 145, 14);
                gfx.DrawString(items.Cantidad, fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);

                decimal TotalDolar = Convert.ToDecimal(items.Precio) / Dolar;
                Margen = new XRect(360, PosicionP, 145, 14);
                gfx.DrawString(items.perfil.PrecioDolar.ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                Margen = new XRect(452, PosicionP, 145, 14);
                gfx.DrawString(TotalDolar.ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                sumaTotal = sumaTotal + Convert.ToDecimal(TotalDolar);
                PosicionP = PosicionP + 15;
            }
            PosicionP = PosicionP + 50;
            Margen = new XRect(589 - 589 / 4 - 30, PosicionP - 10, 140, 14);
            Margen = new XRect(10, PosicionP, 145, 14);
            decimal Dolares = sumaTotal;

            cmd = "Total: ";
            length = gfx.MeasureString(sumaTotal.ToString(), fontRegular2);
            contador = contador;
            Margen = new XRect(contador, PosicionP, 145, 14);
            gfx.DrawString(cmd, fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);

            length = gfx.MeasureString(cmd, fontRegular2);
            contador = contador + length.Width;
            Margen = new XRect(contador, PosicionP, 145, 14);
            gfx.DrawString(Dolares.ToString("C2", new CultureInfo("en-US")), TituloCaratula, XBrushes.Black, Margen, XStringFormats.CenterLeft);

            Formato = string.Format("DIRECCION: {0} TLF: {1} CORREO: {2}", empresa.Direccion, empresa.Telefono, empresa.Correo);
            PosicionP = PosicionP + 15;
            Margen = new XRect(40, 354, 500, 48);
            tf.DrawString(Formato, DireccionCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            gfx.DrawImage(Logo, 40, 20);
            PosicionP = 45;
            return document;

        }
        /// <summary>
        /// Procedimiento para crear el pdf de los examenes 
        /// </summary>
        /// <param name="IdSesion"></param>
        /// <param name="IdAnalisis"></param>
        /// <param name="Metodo"></param>
        /// <returns> Documento en pagina completa en PDF por PDF SHARP</returns>
        /// 
        public PdfSharp.Pdf.PdfDocument DocumentoPorLote(int IdSesion, string Metodo)
        {
            PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
            PdfSharp.Pdf.PdfPage page = document.AddPage();
            Facturas factura = new Facturas();
            factura = ConexionVeterinaria.selectFacturaPorId(IdSesion);
            List<Perfil> PerfilesV = new List<Perfil>();
            PerfilesV = ConexionVeterinaria.selectExamenesGrupales(IdSesion);
            Empresa empresa = new Empresa();
            if (factura.IdConvenio > 3)
            {
                empresa = ConexionVeterinaria.selectEmpresaActiva(factura.IdConvenio);
            }
            else
            {
                empresa = ConexionVeterinaria.selectEmpresaActiva();
            }
            page = PortadaGrupal(page, factura.datosRepresentante, empresa, factura.datoveterinario, factura.finca, factura.Id);
            var Perfiles = PerfilesV.OrderBy(n => n.IdImpresionAgrupado).GroupBy(P => P.IdPerfil);
            foreach (var Perfil in Perfiles)
            {
                switch (Perfil.First().IdImpresionAgrupado)
                {
                    case 1:
                        document = HematologiaGrupal(document, factura, empresa);
                        break;
                    case 2:
                        document = PerfilCompuestoAgrupado(document, factura, empresa, Perfil.First());
                        break;
                    case 3:
                        document = Leptospira(document, factura, empresa, Perfil.First());
                        break;
                    case 4:
                        document = CoprosDirectaYWillys(document, factura, empresa);
                        document = CoproTamisadoYSedi(document, factura, empresa);
                        break;
                    case 5:
                        document = AnalisisSinValoresDeRef(document, factura, empresa);
                        break;
                    case 7:
                        document = Elisas(document, factura, empresa, Perfil.First());
                        break;
                    case 8:
                        document = DVBGRUPAL(document, factura, empresa);
                        break;
                    case 9:
                        document = HemotropicosGrupal(document, factura, empresa);
                        break;
                }
            }
            return document;
        }
        public PdfSharp.Pdf.PdfDocument DocumentoPorLote(int IdSesion, List<Perfil> PerfilesABuscar)
        {
            PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
            PdfSharp.Pdf.PdfPage page = document.AddPage();
            Facturas factura = new Facturas();
            factura = ConexionVeterinaria.selectFacturaPorId(IdSesion);
            List<Perfil> PerfilesV = new List<Perfil>();
            PerfilesV = ConexionVeterinaria.selectExamenesGrupales(PerfilesABuscar);
            Empresa empresa = new Empresa();
            if (factura.IdConvenio > 3)
            {
                empresa = ConexionVeterinaria.selectEmpresaActiva(factura.IdConvenio);
            }
            else
            {
                empresa = ConexionVeterinaria.selectEmpresaActiva();
            }
            page = PortadaGrupal(page, factura.datosRepresentante, empresa, factura.datoveterinario, factura.finca, factura.Id);
            var Perfiles = PerfilesV.OrderBy(n => n.IdImpresionAgrupado).GroupBy(P => P.IdPerfil);
            foreach (var Perfil in Perfiles)
            {
                switch (Perfil.First().IdImpresionAgrupado)
                {
                    case 1:
                        document = HematologiaGrupal(document, factura, empresa);
                        break;
                    case 2:
                        document = PerfilCompuestoAgrupado(document, factura, empresa, Perfil.First());
                        break;
                    case 3:
                        document = Leptospira(document, factura, empresa, Perfil.First());
                        break;
                    case 4:
                        document = CoprosDirectaYWillys(document, factura, empresa);
                        document = CoproTamisadoYSedi(document, factura, empresa);
                        break;
                    case 5:
                        document = AnalisisSinValoresDeRef(document, factura, empresa);
                        break;
                    case 7:
                        document = Elisas(document, factura, empresa, Perfil.First());
                        break;
                    case 8:
                        document = DVBGRUPAL(document, factura, empresa);
                        break;
                    case 9:
                        document = HemotropicosGrupal(document, factura, empresa);
                        break;
                }
            }
            return document;
        }
        public PdfSharp.Pdf.PdfDocument AnalisisSinValoresDeRef(PdfSharp.Pdf.PdfDocument document, Facturas factura, Empresa empresa)
        {
            int IdEspecie = 0, idAnalisis = 0;
            bool HeaderImpreso = false, title = false;
            List<ResultadosPorAnalisisVet> resultados = new List<ResultadosPorAnalisisVet>();
            List<Hemo> hemoparasitos = new List<Hemo>();
            List<int> Especies = new List<int>();
            PdfSharp.Pdf.PdfPage page = document.Pages[document.Pages.Count - 1];
            Especies = ConexionVeterinaria.EspeciesPorSesion(factura.IdSesion);
            XRect Margen = new XRect(5, PosicionP, 250, 14);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);

            XFont fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
            XSolidBrush blueBrush = new XSolidBrush(XColors.Black);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            XPoint point = new XPoint(5, 70);
            XSize size = new XSize(580, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            int i = 0;
            int PosicionY = 75;
            int PosicionR = 10;
            PosicionP = 138;
            resultados = ConexionVeterinaria.SelectExamenesSinValoresDeRef(factura.IdSesion);
            foreach (var resultado in resultados.OrderBy(r => r.IdAnalisis))
            {

                if (!HeaderImpreso)
                {
                    page = document.AddPage();
                    page = HeaderGrup(page, factura.datosRepresentante, factura.finca, empresa, factura.IdSesion, DateTime.Now, factura.NumeroDia);
                    fontRegular2 = new XFont(facename, 7, XFontStyle.Regular);
                    blueBrush = new XSolidBrush(XColors.Black);
                    color = new XColor { R = 105, G = 105, B = 105 };
                    pen = new XPen(color);
                    point = new XPoint(20, 70);
                    size = new XSize(580, 15);
                    Elipsesize = new XSize(5, 5);
                    rect = new XRect(point, size);
                    gfx = XGraphics.FromPdfPage(page);
                    tf = new XTextFormatter(gfx);
                    //for (int x = 1; x < 9 + hemoparasitos.Count; x++)
                    //{
                    //    PosicionP += 15;
                    //    gfx.DrawLine(pen, 10, PosicionP, 575, PosicionP);
                    //}
                    Margen = new XRect(230, PosicionP - 20, 120, 14);
                    gfx.DrawString($"{resultado.analisisLaboratorio.NombreAnalisis}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                    //Valores de Referencia
                    HeaderImpreso = true;
                    idAnalisis = resultado.IdAnalisis;
                }

                // Logica Para el Tamaño del cuadro
                //------------------------------INICIA------------------
                if (resultado.IdAnalisis != idAnalisis)
                {
                    int PosicionX = 120;
                    int Posicionz = 95;
                    Margen = new XRect(230, PosicionP - 20, 120, 14);
                    gfx.DrawString($"{resultado.analisisLaboratorio.NombreAnalisis}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                }


                double stringLength = 0;

                Margen = new XRect(PosicionR, PosicionP, 95, 15);
                gfx.DrawRectangle(pen, Margen);
                gfx.DrawString("IDENTIFICACION ANIMAL", fontRegular2, blueBrush, Margen, XStringFormats.Center);


                Margen = new XRect(PosicionR, PosicionP + 15, 95, 15);
                gfx.DrawRectangle(pen, Margen);
                DatosPacienteVet datosPacienteVet = ConexionVeterinaria.datosPacientePorOrden(resultado.IdOrden);
                gfx.DrawString($"{datosPacienteVet.NombrePaciente}", fontRegular2, blueBrush, Margen, XStringFormats.Center);


                Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                gfx.DrawRectangle(pen, Margen);
                gfx.DrawString("Resultado", fontRegular2, blueBrush, Margen, XStringFormats.Center);


                Margen = new XRect(PosicionR, PosicionP + 15, 95, 15);
                gfx.DrawRectangle(pen, Margen);
                gfx.DrawString($"{resultado.ValorResultado}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                i++;
                PosicionR += 95;
                if (i % 3 == 0)
                {
                    PosicionR = 10;
                    PosicionP += 30;
                }
                if (i == 21 || resultados.Last() == resultado)
                {
                    Margen = new XRect(PosicionR, PosicionP + 15, 95, 15);
                    gfx.DrawRectangle(pen, Margen);
                    gfx.DrawString($"Valores de Referencia: {resultado.ValorMenor - resultado.ValorMayor}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                    Usuarios Bioanalista = new Usuarios();
                    Bioanalista = resultado.bioanalista;
                    Margen = new XRect(300, PosicionP = PosicionP + 40, 250, 14);
                    tf.DrawString(string.Format("Analista"), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                    Margen = new XRect(300, PosicionP = PosicionP + 12, 250, 14);
                    tf.DrawString(string.Format(" {0} CMVB:{1} MPPS:{2}", Bioanalista.NombreUsuario, Bioanalista.CB, Bioanalista.MPPS), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                    XImage Logo = XImage.FromFile(string.Format(@"firma\{0}", Bioanalista.Firma));
                    gfx.DrawImage(Logo, 490, PosicionP - 15);
                    PosicionP = 110;
                    PosicionR = 10;
                    i = 1;
                    gfx.Dispose();
                    HeaderImpreso = false;
                }

            }

            PosicionP = 110;
            gfx.Dispose();
            return document;
        }
        public PdfSharp.Pdf.PdfDocument IBRGRUPAL(PdfSharp.Pdf.PdfDocument document, Facturas factura, Empresa empresa)
        {
            int IdEspecie = 0, idAnalisis = 0;
            bool HeaderImpreso = false, title = false;
            List<ResultadosPorAnalisisVet> resultados = new List<ResultadosPorAnalisisVet>();
            List<Hemo> hemoparasitos = new List<Hemo>();
            List<int> Especies = new List<int>();
            PdfSharp.Pdf.PdfPage page = document.Pages[document.Pages.Count - 1];
            Especies = ConexionVeterinaria.EspeciesPorSesion(factura.IdSesion);
            XRect Margen = new XRect(5, PosicionP, 250, 14);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);

            XFont fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
            XSolidBrush blueBrush = new XSolidBrush(XColors.Black);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            XPoint point = new XPoint(5, 70);
            XSize size = new XSize(580, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            int i = 0;
            int PosicionY = 75;
            int PosicionR = 10;
            PosicionP = 138;
            resultados = ConexionVeterinaria.IBRGRUPAL(factura.IdSesion);
            foreach (var resultado in resultados.OrderBy(r => r.IdAnalisis))
            {

                if (!HeaderImpreso)
                {
                    page = document.AddPage();
                    page = HeaderGrup(page, factura.datosRepresentante, factura.finca, empresa, factura.IdSesion, DateTime.Now, factura.NumeroDia);
                    fontRegular2 = new XFont(facename, 7, XFontStyle.Regular);
                    blueBrush = new XSolidBrush(XColors.Black);
                    color = new XColor { R = 105, G = 105, B = 105 };
                    pen = new XPen(color);
                    point = new XPoint(20, 70);
                    size = new XSize(580, 15);
                    Elipsesize = new XSize(5, 5);
                    rect = new XRect(point, size);
                    gfx = XGraphics.FromPdfPage(page);
                    tf = new XTextFormatter(gfx);
                    //for (int x = 1; x < 9 + hemoparasitos.Count; x++)
                    //{
                    //    PosicionP += 15;
                    //    gfx.DrawLine(pen, 10, PosicionP, 575, PosicionP);
                    //}
                    Margen = new XRect(230, PosicionP - 20, 120, 14);
                    gfx.DrawString($"{resultado.analisisLaboratorio.NombreAnalisis}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                    //Valores de Referencia
                    HeaderImpreso = true;
                    idAnalisis = resultado.IdAnalisis;

                    Margen = new XRect(PosicionR, PosicionP, 95, 15);
                    gfx.DrawRectangle(pen, Margen);
                    gfx.DrawString("IDENTIFICACION ANIMAL", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                    Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                    gfx.DrawRectangle(pen, Margen);
                    gfx.DrawString("Resultado", fontRegular2, blueBrush, Margen, XStringFormats.Center);


                    Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                    gfx.DrawRectangle(pen, Margen);
                    gfx.DrawString("IDENTIFICACION ANIMAL", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                    Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                    gfx.DrawRectangle(pen, Margen);
                    gfx.DrawString("Resultado", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                    Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                    gfx.DrawRectangle(pen, Margen);
                    gfx.DrawString("IDENTIFICACION ANIMAL", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                    Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                    gfx.DrawRectangle(pen, Margen);
                    gfx.DrawString("Resultado", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                    PosicionP += 15;
                    PosicionR = 10;
                }

                // Logica Para el Tamaño del cuadro
                //------------------------------INICIA------------------


                double stringLength = 0;

                Margen = new XRect(PosicionR, PosicionP, 95, 15);
                gfx.DrawRectangle(pen, Margen);
                DatosPacienteVet datosPacienteVet = ConexionVeterinaria.datosPacientePorOrden(resultado.IdOrden);
                gfx.DrawString($"{datosPacienteVet.NombrePaciente}", fontRegular2, blueBrush, Margen, XStringFormats.Center);


                Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                gfx.DrawRectangle(pen, Margen);
                gfx.DrawString($"{resultado.ValorResultado}  {resultado.unidad}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                i++;
                PosicionR += 95;
                if (i % 3 == 0)
                {
                    PosicionR = 10;
                    PosicionP += 15;
                }
                if (i == 78 || resultados.Last() == resultado)
                {
                    Margen = new XRect(50, PosicionP = PosicionP + 30, 95, 15);
                    gfx.DrawString("Valores de Referencia", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                    Margen = new XRect(50, PosicionP + 15, 95, 15);
                    if (resultado.analisisLaboratorio.TipoAnalisis == 5)
                    {
                        gfx.DrawString($"{resultado.ValorMenor} - {resultado.ValorMayor}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                    }
                    else
                    {
                        gfx.DrawString($"{resultado.MultiplesValores}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                    }

                    Usuarios Bioanalista = new Usuarios();
                    Bioanalista = resultado.bioanalista;
                    Margen = new XRect(300, PosicionP = PosicionP + 12, 250, 14);
                    tf.DrawString(string.Format("Analista"), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                    Margen = new XRect(300, PosicionP = PosicionP + 12, 250, 14);
                    tf.DrawString(string.Format(" {0} CMVB:{1} MPPS:{2}", Bioanalista.NombreUsuario, Bioanalista.CB, Bioanalista.MPPS), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                    XImage Logo = XImage.FromFile(string.Format(@"firma\{0}", Bioanalista.Firma));
                    gfx.DrawImage(Logo, 490, PosicionP - 15);
                    PosicionP = 110;
                    PosicionR = 10;
                    i = 0;
                    gfx.Dispose();
                    HeaderImpreso = false;
                }

            }

            PosicionP = 110;
            gfx.Dispose();
            return document;
        }
        public PdfSharp.Pdf.PdfDocument DVBGRUPAL(PdfSharp.Pdf.PdfDocument document, Facturas factura, Empresa empresa)
        {
            int IdEspecie = 0, idAnalisis = 0;
            bool HeaderImpreso = false, title = false;
            List<ResultadosPorAnalisisVet> resultados = new List<ResultadosPorAnalisisVet>();
            List<Hemo> hemoparasitos = new List<Hemo>();
            List<int> Especies = new List<int>();
            PdfSharp.Pdf.PdfPage page = document.Pages[document.Pages.Count - 1];
            Especies = ConexionVeterinaria.EspeciesPorSesion(factura.IdSesion);
            XRect Margen = new XRect(5, PosicionP, 250, 14);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);

            XFont fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
            XSolidBrush blueBrush = new XSolidBrush(XColors.Black);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            XPoint point = new XPoint(5, 70);
            XSize size = new XSize(580, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            int i = 0;
            int PosicionY = 75;
            int PosicionR = 10;
            PosicionP = 138;
            resultados = ConexionVeterinaria.DVBGRUPAL(factura.IdSesion);
            foreach (var resultado in resultados.OrderBy(r => r.IdAnalisis))
            {

                if (!HeaderImpreso)
                {
                    page = document.AddPage();
                    page = HeaderGrup(page, factura.datosRepresentante, factura.finca, empresa, factura.IdSesion, DateTime.Now, factura.NumeroDia);
                    fontRegular2 = new XFont(facename, 7, XFontStyle.Regular);
                    blueBrush = new XSolidBrush(XColors.Black);
                    color = new XColor { R = 105, G = 105, B = 105 };
                    pen = new XPen(color);
                    point = new XPoint(20, 70);
                    size = new XSize(580, 15);
                    Elipsesize = new XSize(5, 5);
                    rect = new XRect(point, size);
                    gfx = XGraphics.FromPdfPage(page);
                    tf = new XTextFormatter(gfx);
                    //for (int x = 1; x < 9 + hemoparasitos.Count; x++)
                    //{
                    //    PosicionP += 15;
                    //    gfx.DrawLine(pen, 10, PosicionP, 575, PosicionP);
                    //}
                    Margen = new XRect(230, PosicionP - 20, 120, 14);
                    gfx.DrawString($"{resultado.analisisLaboratorio.NombreAnalisis}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                    //Valores de Referencia
                    HeaderImpreso = true;
                    idAnalisis = resultado.IdAnalisis;

                    Margen = new XRect(PosicionR, PosicionP, 95, 15);
                    gfx.DrawRectangle(pen, Margen);
                    gfx.DrawString("IDENTIFICACION ANIMAL", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                    Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                    gfx.DrawRectangle(pen, Margen);
                    gfx.DrawString("Resultado", fontRegular2, blueBrush, Margen, XStringFormats.Center);


                    Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                    gfx.DrawRectangle(pen, Margen);
                    gfx.DrawString("IDENTIFICACION ANIMAL", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                    Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                    gfx.DrawRectangle(pen, Margen);
                    gfx.DrawString("Resultado", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                    Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                    gfx.DrawRectangle(pen, Margen);
                    gfx.DrawString("IDENTIFICACION ANIMAL", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                    Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                    gfx.DrawRectangle(pen, Margen);
                    gfx.DrawString("Resultado", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                    PosicionP += 15;
                    PosicionR = 10;
                }

                // Logica Para el Tamaño del cuadro
                //------------------------------INICIA------------------


                double stringLength = 0;

                Margen = new XRect(PosicionR, PosicionP, 95, 15);
                gfx.DrawRectangle(pen, Margen);
                DatosPacienteVet datosPacienteVet = ConexionVeterinaria.datosPacientePorOrden(resultado.IdOrden);
                gfx.DrawString($"{datosPacienteVet.NombrePaciente}", fontRegular2, blueBrush, Margen, XStringFormats.Center);


                Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                gfx.DrawRectangle(pen, Margen);
                gfx.DrawString($"{resultado.ValorResultado} {resultado.unidad}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                i++;
                PosicionR += 95;
                if (i % 3 == 0)
                {
                    PosicionR = 10;
                    PosicionP += 15;
                }
                if (i == 21 || resultados.Last() == resultado)
                {
                    Margen = new XRect(50, PosicionP = PosicionP + 30, 95, 15);
                    gfx.DrawString("Valores de Referencia", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                    Margen = new XRect(50, PosicionP + 15, 95, 15);
                    if (resultado.analisisLaboratorio.TipoAnalisis == 5)
                    {
                        gfx.DrawString($"{resultado.ValorMenor} - {resultado.ValorMayor}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                    }
                    else
                    {
                        gfx.DrawString($"{resultado.MultiplesValores}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                    }

                    Usuarios Bioanalista = new Usuarios();
                    Bioanalista = resultado.bioanalista;
                    Margen = new XRect(300, PosicionP = PosicionP + 12, 250, 14);
                    tf.DrawString(string.Format("Analista"), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                    Margen = new XRect(300, PosicionP = PosicionP + 12, 250, 14);
                    tf.DrawString(string.Format(" {0} CMVB:{1} MPPS:{2}", Bioanalista.NombreUsuario, Bioanalista.CB, Bioanalista.MPPS), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                    XImage Logo = XImage.FromFile(string.Format(@"firma\{0}", Bioanalista.Firma));
                    gfx.DrawImage(Logo, 490, PosicionP - 15);
                    PosicionP = 110;
                    PosicionR = 10;
                    i = 0;
                    gfx.Dispose();
                    HeaderImpreso = false;
                }

            }

            PosicionP = 110;
            gfx.Dispose();
            return document;
        }
        public PdfSharp.Pdf.PdfDocument Elisas(PdfSharp.Pdf.PdfDocument document, Facturas factura, Empresa empresa, Perfil perfil)
        {
            int idAnalisis = 0;
            bool HeaderImpreso = false, title = false;
            List<ResultadosPorAnalisisVet> resultados = new List<ResultadosPorAnalisisVet>();
            List<Hemo> hemoparasitos = new List<Hemo>();
            List<Especies> Especies = new List<Especies>();
            PdfSharp.Pdf.PdfPage page = document.Pages[document.Pages.Count - 1];
            Especies = ConexionVeterinaria.EspeciesModelPorSesion(factura.IdSesion);
            XRect Margen = new XRect(5, PosicionP, 250, 14);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);

            XFont fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
            XSolidBrush blueBrush = new XSolidBrush(XColors.Black);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            XPoint point = new XPoint(5, 70);
            XSize size = new XSize(580, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            int i = 0;
            int PosicionY = 75;
            int PosicionR = 10;
            PosicionP = 128;

            foreach (var especie in Especies)
            {
                resultados = ConexionVeterinaria.Elisas(factura.IdSesion, perfil.IdPerfil, especie.IdEspecie);
                foreach (var resultado in resultados.OrderBy(r => r.IdAnalisis))
                {

                    if (!HeaderImpreso)
                    {
                        page = document.AddPage();
                        page = HeaderGrup(page, factura.datosRepresentante, factura.finca, empresa, factura.IdSesion, DateTime.Now, factura.NumeroDia);
                        fontRegular2 = new XFont(facename, 7, XFontStyle.Regular);
                        blueBrush = new XSolidBrush(XColors.Black);
                        color = new XColor { R = 105, G = 105, B = 105 };
                        pen = new XPen(color);
                        point = new XPoint(20, 70);
                        size = new XSize(580, 15);
                        Elipsesize = new XSize(5, 5);
                        rect = new XRect(point, size);
                        gfx = XGraphics.FromPdfPage(page);
                        tf = new XTextFormatter(gfx);
                        //for (int x = 1; x < 9 + hemoparasitos.Count; x++)
                        //{
                        //    PosicionP += 15;
                        //    gfx.DrawLine(pen, 10, PosicionP, 575, PosicionP);
                        //}
                        Margen = new XRect(230, PosicionP - 30, 120, 14);
                        if (resultado.IdAnalisis != 353 && resultado.IdAnalisis != 349)
                        {
                            gfx.DrawString($"{resultado.analisisLaboratorio.NombreAnalisis} {especie.Descripcion} ", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        }
                        else
                        {
                            gfx.DrawString($"{resultado.analisisLaboratorio.NombreAnalisis} ", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        }

                        //Valores de Referencia
                        HeaderImpreso = true;
                        idAnalisis = resultado.IdAnalisis;

                        Margen = new XRect(PosicionR, PosicionP, 95, 15);
                        gfx.DrawRectangle(pen, Margen);
                        gfx.DrawString("IDENTIFICACION ANIMAL", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                        Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                        gfx.DrawRectangle(pen, Margen);
                        gfx.DrawString("Resultado", fontRegular2, blueBrush, Margen, XStringFormats.Center);


                        Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                        gfx.DrawRectangle(pen, Margen);
                        gfx.DrawString("IDENTIFICACION ANIMAL", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                        Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                        gfx.DrawRectangle(pen, Margen);
                        gfx.DrawString("Resultado", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                        Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                        gfx.DrawRectangle(pen, Margen);
                        gfx.DrawString("IDENTIFICACION ANIMAL", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                        Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                        gfx.DrawRectangle(pen, Margen);
                        gfx.DrawString("Resultado", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        PosicionR = 10;
                    }

                    // Logica Para el Tamaño del cuadro
                    //------------------------------INICIA------------------


                    double stringLength = 0;

                    Margen = new XRect(PosicionR, PosicionP, 95, 15);
                    gfx.DrawRectangle(pen, Margen);
                    DatosPacienteVet datosPacienteVet = ConexionVeterinaria.datosPacientePorOrden(resultado.IdOrden);
                    gfx.DrawString($"{datosPacienteVet.NombrePaciente}", fontRegular2, blueBrush, Margen, XStringFormats.Center);


                    Margen = new XRect(PosicionR += 95, PosicionP, 95, 15);
                    gfx.DrawRectangle(pen, Margen);
                    gfx.DrawString($"{resultado.ValorResultado} {resultado.unidad}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                    i++;
                    PosicionR += 95;
                    if (i % 3 == 0)
                    {
                        PosicionR = 10;
                        PosicionP += 15;
                    }
                    if (i == 68 || resultados.Last() == resultado)
                    {
                        Margen = new XRect(50, PosicionP = PosicionP + 30, 95, 15);
                        gfx.DrawString("Valores de Referencia", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        Margen = new XRect(50, PosicionP + 15, 95, 15);
                        if (resultado.analisisLaboratorio.TipoAnalisis == 5)
                        {
                            gfx.DrawString($"{resultado.ValorMenor} - {resultado.ValorMayor}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        }
                        else
                        {

                            Margen = new XRect(50, PosicionP + 15, 190, resultado.Lineas * 15);
                            tf.DrawString($"{resultado.MultiplesValores}", fontRegular2, blueBrush, Margen, XStringFormats.TopLeft);
                        }

                        Usuarios Bioanalista = new Usuarios();
                        Bioanalista = resultado.bioanalista;
                        Margen = new XRect(300, PosicionP = PosicionP + 12, 250, 14);
                        tf.DrawString(string.Format("Analista"), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(300, PosicionP = PosicionP + 12, 250, 14);
                        tf.DrawString(string.Format(" {0} CMVB:{1} MPPS:{2}", Bioanalista.NombreUsuario, Bioanalista.CB, Bioanalista.MPPS), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        XImage Logo = XImage.FromFile(string.Format(@"firma\{0}", Bioanalista.Firma));
                        gfx.DrawImage(Logo, 490, PosicionP - 15);
                        PosicionP = 110;
                        PosicionR = 10;
                        i = 0;
                        gfx.Dispose();
                        HeaderImpreso = false;
                    }

                }
                PosicionP = 110;
                gfx.Dispose();
            }

            return document;
        }
        public PdfSharp.Pdf.PdfDocument CoprosDirectaYWillys(PdfSharp.Pdf.PdfDocument document, Facturas factura, Empresa empresa)
        {
            int IdEspecie = 0;
            bool HeaderImpreso = false;
            List<CroprologiasGrupal> ordenes = new List<CroprologiasGrupal>();
            List<Hemo> hemoparasitos = new List<Hemo>();
            List<int> Especies = new List<int>();
            PdfSharp.Pdf.PdfPage page = document.Pages[document.Pages.Count - 1];
            Especies = ConexionVeterinaria.EspeciesPorSesion(factura.IdSesion);
            XRect Margen = new XRect(5, PosicionP, 250, 14);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);

            XFont fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
            XSolidBrush blueBrush = new XSolidBrush(XColors.Black);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            XPoint point = new XPoint(5, 70);
            XSize size = new XSize(580, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            int i = 0;
            int PosicionY = 75;
            PosicionP = 110;
            foreach (var item in Especies)
            {
                ordenes = ConexionVeterinaria.SelectCoprosWillysYDirecta(factura.IdSesion, item);
                foreach (var orden in ordenes)
                {

                    if (!HeaderImpreso)
                    {
                        page = document.AddPage();
                        page = HeaderGrup(page, factura.datosRepresentante, factura.finca, empresa, factura.IdSesion, DateTime.Now, factura.NumeroDia);
                        fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
                        blueBrush = new XSolidBrush(XColors.Black);
                        color = new XColor { R = 105, G = 105, B = 105 };
                        pen = new XPen(color);
                        point = new XPoint(20, 70);
                        size = new XSize(580, 15);
                        Elipsesize = new XSize(5, 5);
                        rect = new XRect(point, size);
                        gfx = XGraphics.FromPdfPage(page);
                        tf = new XTextFormatter(gfx);


                        int PosicionX = 120;
                        int Posicionz = 95;
                        Margen = new XRect(230, PosicionP-15, 120, 14);
                        gfx.DrawString($"ANALISIS COPROLOGICO {ordenes.First().datosPaciente.especies.Descripcion}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        Posicionz += 15;
                        double stringLength = 0;
                        Margen = new XRect(10, Posicionz, 120, 28);
                        gfx.DrawRectangle(pen, Margen);
                        gfx.DrawString("IDENTIFICACION ANIMAL", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        Margen = new XRect(130, Posicionz, 150, 28);
                        gfx.DrawRectangle(pen, Margen);
                        gfx.DrawString("TECNICA WILLYS", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        Margen = new XRect(280, Posicionz, 150, 28);
                        gfx.DrawRectangle(pen, Margen);
                        gfx.DrawString("TECNICA DIRECTA Y SALINA", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        Margen = new XRect(430, Posicionz, 150, 28);
                        gfx.DrawRectangle(pen, Margen);
                        gfx.DrawString("Comentario", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                        //for (int x = 1; x < 9 + hemoparasitos.Count; x++)
                        //{
                         PosicionP += 29;
                        //    gfx.DrawLine(pen, 10, PosicionP, 575, PosicionP);
                        //}

                        //Valores de Referencia
                        PosicionX = 120;
                        HeaderImpreso = true;
                    }
                    // Logica Para el Tamaño del cuadro
                    //------------------------------INICIA------------------
                    string text = string.Empty;
                    string Nombre = string.Empty;
                    string Sedimentacion = string.Empty;
                    string MacMaster = string.Empty;
                    string Comentario = string.Empty;
                    Nombre = orden.datosPaciente.NombrePaciente;
                    Sedimentacion = orden.TECNICAWILLYS;
                    MacMaster = orden.TECNICADIRECTA;
                    Comentario = orden.Comentarios;
                    if (Comentario.Length > Sedimentacion.Length && Comentario.Length > MacMaster.Length)
                    {
                        text = Comentario;
                    }
                    if (Comentario.Length < Sedimentacion.Length && Sedimentacion.Length > MacMaster.Length)
                    {
                        text = Sedimentacion;
                    }
                    if (MacMaster.Length > Sedimentacion.Length && Comentario.Length < MacMaster.Length)
                    {
                        text = MacMaster;
                    }
                    var Measure = gfx.MeasureString(text, fontRegular2);
                    double Evaluar1, Evaluar2;
                    Evaluar1 = Math.Round(Measure.Width / 200, 1);
                    Evaluar2 = Regex.Matches(text, "\n").Count + 1;
                    double total, subtotal;

                    if (Evaluar1 >= Evaluar2)
                    {
                        subtotal = Math.Round(Measure.Width / 200, 1);
                    }
                    else
                    {
                        subtotal = Regex.Matches(text, "\n").Count + 1;
                    }
                    total = 28 * subtotal;
                    //------------------------------Termina la logica para agrandar los cuadros ------------------
                    //------------------------------SI PASA DE LOS 320 pixeles Add New page and Header ------------------
                    if (PosicionP + total > 320 || orden == ordenes.Last())
                    {
                        ///Valores a las variables
                        Margen = new XRect(10, PosicionP, 120, total);
                        gfx.DrawRectangle(pen, Margen);
                        Margen = new XRect(15, PosicionP + 2, 115, total);
                        tf.DrawString($"{Nombre}", fontRegular2, blueBrush, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(130, PosicionP, 150, total);
                        gfx.DrawRectangle(pen, Margen);
                        Margen = new XRect(135, PosicionP + 2, 145, total);
                        tf.DrawString($"{Sedimentacion}", fontRegular2, blueBrush, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(280, PosicionP, 150, total);
                        gfx.DrawRectangle(pen, Margen);
                        Margen = new XRect(285, PosicionP + 2, 145, total);
                        tf.DrawString($"{MacMaster}", fontRegular2, blueBrush, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(430, PosicionP, 150, total);
                        gfx.DrawRectangle(pen, Margen);
                        Margen = new XRect(435, PosicionP + 2, 145, total);
                        tf.DrawString($"{Comentario} ", fontRegular2, blueBrush, Margen, XStringFormats.TopLeft);
                        i++;
                        PosicionP += total;


                        Margen = new XRect(460, PosicionP, 120, 14);
                        Usuarios Bioanalista = new Usuarios();
                        Bioanalista = orden.Bioanalista;
                        Margen = new XRect(300, PosicionP = PosicionP + 20, 250, 14);
                        tf.DrawString(string.Format("Analista"), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(300, PosicionP = PosicionP + 12, 250, 14);
                        tf.DrawString(string.Format(" {0} CMVB:{1} MPPS:{2}", Bioanalista.NombreUsuario, Bioanalista.CB, Bioanalista.MPPS), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        XImage Logo = XImage.FromFile(string.Format(@"firma\{0}", Bioanalista.Firma));
                        gfx.DrawImage(Logo, 490, PosicionP - 15);
                        PosicionY = 75;
                        gfx.Dispose();
                        HeaderImpreso = false;

                        if (orden != ordenes.Last())
                        {
                            PosicionP = 138;
                            page = document.AddPage();
                            page = HeaderGrup(page, factura.datosRepresentante, factura.finca, empresa, factura.IdSesion, DateTime.Now, factura.NumeroDia);
                            fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
                            blueBrush = new XSolidBrush(XColors.Black);
                            color = new XColor { R = 105, G = 105, B = 105 };
                            pen = new XPen(color);
                            point = new XPoint(20, 70);
                            size = new XSize(580, 15);
                            Elipsesize = new XSize(5, 5);
                            rect = new XRect(point, size);
                            gfx = XGraphics.FromPdfPage(page);
                            tf = new XTextFormatter(gfx);
                            int PosicionX = 120;
                            int Posicionz = 95;
                            Margen = new XRect(230, Posicionz, 120, 14);
                            gfx.DrawString($"ANALISIS COPROLOGICO {ordenes.First().datosPaciente.especies.Descripcion}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                            Posicionz += 15;
                            double stringLength = 0;
                            Margen = new XRect(10, Posicionz, 120, 28);
                            gfx.DrawRectangle(pen, Margen);
                            gfx.DrawString("IDENTIFICACION ANIMAL", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                            Margen = new XRect(130, Posicionz, 150, 28);
                            gfx.DrawRectangle(pen, Margen);
                            gfx.DrawString("TECNICA WILLYS", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                            Margen = new XRect(280, Posicionz, 150, 28);
                            gfx.DrawRectangle(pen, Margen);
                            gfx.DrawString("TECNICA DIRECTA Y SALINA", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                            Margen = new XRect(430, Posicionz, 150, 28);
                            gfx.DrawRectangle(pen, Margen);
                            gfx.DrawString("Comentario", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        }


                        //for (int x = 1; x < 9 + hemoparasitos.Count; x++)
                        //{
                        //    PosicionP += 15;
                        //    gfx.DrawLine(pen, 10, PosicionP, 575, PosicionP);
                        //}

                        //Valores de Referencia

                    }
                    ///Valores a las variables
                    Margen = new XRect(10, PosicionP, 120, total);
                    gfx.DrawRectangle(pen, Margen);
                    Margen = new XRect(15, PosicionP + 2, 115, total);
                    tf.DrawString($"{Nombre}", fontRegular2, blueBrush, Margen, XStringFormats.TopLeft);
                    Margen = new XRect(130, PosicionP, 150, total);
                    gfx.DrawRectangle(pen, Margen);
                    Margen = new XRect(135, PosicionP + 2, 145, total);
                    tf.DrawString($"{Sedimentacion}", fontRegular2, blueBrush, Margen, XStringFormats.TopLeft);
                    Margen = new XRect(280, PosicionP, 150, total);
                    gfx.DrawRectangle(pen, Margen);
                    Margen = new XRect(285, PosicionP + 2, 145, total);
                    tf.DrawString($"{MacMaster}", fontRegular2, blueBrush, Margen, XStringFormats.TopLeft);
                    Margen = new XRect(430, PosicionP, 150, total);
                    gfx.DrawRectangle(pen, Margen);
                    Margen = new XRect(435, PosicionP + 2, 145, total);
                    tf.DrawString($"{Comentario} ", fontRegular2, blueBrush, Margen, XStringFormats.TopLeft);
                    i++;
                    PosicionP += total;

                }



            }


            PosicionP = 110;
            gfx.Dispose();
            return document;
        }
        public PdfSharp.Pdf.PdfDocument CoproTamisadoYSedi(PdfSharp.Pdf.PdfDocument document, Facturas factura, Empresa empresa)
        {
            int IdEspecie = 0;
            bool HeaderImpreso = false;
            List<CroprologiasGrupal> ordenes = new List<CroprologiasGrupal>();
            List<Hemo> hemoparasitos = new List<Hemo>();
            List<int> Especies = new List<int>();
            PdfSharp.Pdf.PdfPage page = document.Pages[document.Pages.Count - 1];
            Especies = ConexionVeterinaria.EspeciesPorSesion(factura.IdSesion);
            XRect Margen = new XRect(5, PosicionP, 250, 14);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);

            XFont fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
            XSolidBrush blueBrush = new XSolidBrush(XColors.Black);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            XPoint point = new XPoint(5, 70);
            XSize size = new XSize(580, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            int i = 0;
            int PosicionY = 75;
            PosicionP = 138;
            foreach (var item in Especies)
            {
                ordenes = ConexionVeterinaria.SelectCoprosMacYTamisado(factura.IdSesion, item);
                foreach (var orden in ordenes)
                {

                    if (!HeaderImpreso)
                    {
                        page = document.AddPage();
                        page = HeaderGrup(page, factura.datosRepresentante, factura.finca, empresa, factura.IdSesion, DateTime.Now, factura.NumeroDia);
                        fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
                        blueBrush = new XSolidBrush(XColors.Black);
                        color = new XColor { R = 105, G = 105, B = 105 };
                        pen = new XPen(color);
                        point = new XPoint(20, 70);
                        size = new XSize(580, 15);
                        Elipsesize = new XSize(5, 5);
                        rect = new XRect(point, size);
                        gfx = XGraphics.FromPdfPage(page);
                        tf = new XTextFormatter(gfx);


                        int PosicionX = 120;
                        int Posicionz = 95;
                        Margen = new XRect(230, Posicionz, 120, 14);
                        gfx.DrawString($"ANALISIS COPROLOGICO {ordenes.First().datosPaciente.especies.Descripcion}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        Posicionz += 15;
                        double stringLength = 0;
                        Margen = new XRect(10, Posicionz, 120, 28);
                        gfx.DrawRectangle(pen, Margen);
                        gfx.DrawString("IDENTIFICACION ANIMAL", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        Margen = new XRect(130, Posicionz, 150, 28);
                        gfx.DrawRectangle(pen, Margen);
                        gfx.DrawString("Sedimentacion Tamizado", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        Margen = new XRect(280, Posicionz, 150, 28);
                        gfx.DrawRectangle(pen, Margen);
                        gfx.DrawString("Mac Master", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        Margen = new XRect(430, Posicionz, 150, 28);
                        gfx.DrawRectangle(pen, Margen);
                        gfx.DrawString("Comentario", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                        //for (int x = 1; x < 9 + hemoparasitos.Count; x++)
                        //{
                        //    PosicionP += 15;
                        //    gfx.DrawLine(pen, 10, PosicionP, 575, PosicionP);
                        //}

                        //Valores de Referencia
                        PosicionX = 120;
                        HeaderImpreso = true;
                    }
                    // Logica Para el Tamaño del cuadro
                    //------------------------------INICIA------------------
                    string text = string.Empty;
                    string Nombre = string.Empty;
                    string Sedimentacion = string.Empty;
                    string MacMaster = string.Empty;
                    string Comentario = string.Empty;
                    Nombre = orden.datosPaciente.NombrePaciente;
                    Sedimentacion = orden.SEDIMENTACION;
                    MacMaster = orden.TECNICAMAC;
                    Comentario = orden.Comentarios;
                    if (Comentario.Length > Sedimentacion.Length && Comentario.Length > MacMaster.Length)
                    {
                        text = Comentario;
                    }
                    if (Comentario.Length < Sedimentacion.Length && Sedimentacion.Length > MacMaster.Length)
                    {
                        text = Sedimentacion;
                    }
                    if (MacMaster.Length > Sedimentacion.Length && Comentario.Length < MacMaster.Length)
                    {
                        text = MacMaster;
                    }
                    var Measure = gfx.MeasureString(text, fontRegular2);
                    double Evaluar1, Evaluar2;
                    Evaluar1 = Math.Round(Measure.Width / 200, 1);
                    Evaluar2 = Regex.Matches(text, "\n").Count + 1;
                    double total, subtotal;

                    if (Evaluar1 >= Evaluar2)
                    {
                        subtotal = Math.Round(Measure.Width / 200, 1);
                    }
                    else
                    {
                        subtotal = Regex.Matches(text, "\n").Count + 1;
                    }
                    total = 28 * subtotal;
                    ///Valores a las variables
                    Margen = new XRect(10, PosicionP, 120, total);
                    gfx.DrawRectangle(pen, Margen);
                    Margen = new XRect(15, PosicionP + 2, 115, total);
                    tf.DrawString($"{Nombre} ", fontRegular2, blueBrush, Margen, XStringFormats.TopLeft);
                    Margen = new XRect(130, PosicionP, 150, total);
                    gfx.DrawRectangle(pen, Margen);
                    Margen = new XRect(135, PosicionP + 2, 145, total);
                    tf.DrawString($"{Sedimentacion} ", fontRegular2, blueBrush, Margen, XStringFormats.TopLeft);
                    Margen = new XRect(280, PosicionP, 150, total);
                    gfx.DrawRectangle(pen, Margen);
                    Margen = new XRect(285, PosicionP + 2, 145, total);
                    tf.DrawString($"{MacMaster} ", fontRegular2, blueBrush, Margen, XStringFormats.TopLeft);
                    Margen = new XRect(430, PosicionP, 150, total);
                    gfx.DrawRectangle(pen, Margen);
                    Margen = new XRect(435, PosicionP + 2, 145, total);
                    tf.DrawString($"{Comentario} ", fontRegular2, blueBrush, Margen, XStringFormats.TopLeft);
                    i++;
                    PosicionP += total;
                    //------------------------------Termina la logica para agrandar los cuadros ------------------
                    if (PosicionP + total > 400 || orden == ordenes.Last())
                    {

                        if (orden != ordenes.Last())
                        {


                            PosicionP = 138;
                            page = document.AddPage();
                            page = HeaderGrup(page, factura.datosRepresentante, factura.finca, empresa, factura.IdSesion, DateTime.Now, factura.NumeroDia);
                            fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
                            blueBrush = new XSolidBrush(XColors.Black);
                            color = new XColor { R = 105, G = 105, B = 105 };
                            pen = new XPen(color);
                            point = new XPoint(20, 70);
                            size = new XSize(580, 15);
                            Elipsesize = new XSize(5, 5);
                            rect = new XRect(point, size);
                            gfx = XGraphics.FromPdfPage(page);
                            tf = new XTextFormatter(gfx);
                            int PosicionX = 120;
                            int Posicionz = 95;
                            Margen = new XRect(230, Posicionz, 120, 14);
                            gfx.DrawString($"ANALISIS COPROLOGICO {ordenes.First().datosPaciente.especies.Descripcion}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                            Posicionz += 15;
                            double stringLength = 0;
                            Margen = new XRect(10, Posicionz, 120, 28);
                            gfx.DrawRectangle(pen, Margen);
                            gfx.DrawString("IDENTIFICACION ANIMAL", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                            Margen = new XRect(130, Posicionz, 150, 28);
                            gfx.DrawRectangle(pen, Margen);
                            gfx.DrawString("Sedimentacion Tamizado", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                            Margen = new XRect(280, Posicionz, 150, 28);
                            gfx.DrawRectangle(pen, Margen);
                            gfx.DrawString("Mac Master", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                            Margen = new XRect(430, Posicionz, 150, 28);
                            gfx.DrawRectangle(pen, Margen);
                            gfx.DrawString("Comentario", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        }
                        else
                        {
                            Margen = new XRect(460, PosicionP, 120, 14);
                            Usuarios Bioanalista = new Usuarios();
                            Bioanalista = orden.Bioanalista;
                            Margen = new XRect(300, PosicionP = PosicionP + 20, 250, 14);
                            tf.DrawString(string.Format("Analista"), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                            Margen = new XRect(300, PosicionP = PosicionP + 12, 250, 14);
                            tf.DrawString(string.Format(" {0} CMVB:{1} MPPS:{2}", Bioanalista.NombreUsuario, Bioanalista.CB, Bioanalista.MPPS), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                            XImage Logo = XImage.FromFile(string.Format(@"firma\{0}", Bioanalista.Firma));
                            gfx.DrawImage(Logo, 490, PosicionP - 15);
                            PosicionY = 75;
                            gfx.Dispose();
                            HeaderImpreso = true;
                        }

                      


                      

                    }



                }



            }


            PosicionP = 110;
            gfx.Dispose();
            return document;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdOrden"></param>
        /// <param name="IdAnalisis"></param>
        /// <param name="Metodo"></param>
        /// <param name="IdUsuario"></param>
        /// <returns></returns>
        public PdfSharp.Pdf.PdfDocument Documento(int IdOrden, int IdAnalisis, string Metodo, int IdUsuario)
        {

            DataSet dsPrint = new DataSet();
            PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
            List<int> Bio = new List<int>();
            List<int> IdAnalisisBio = new List<int>();
            bool bioencontrado = false;

            PdfSharp.Pdf.PdfPage page = document.AddPage();

            ///
            /// Cargar Empresa
            ///
            Empresa empresa = new Empresa();
            empresa = ConexionVeterinaria.selectEmpresaActiva();
            ///
            /// Seleccionar Orden
            ///

            Ordenes ordenes = new Ordenes();
            if (Metodo == "Seccion")
            {
                DataSet IdAgrupador = new DataSet();
                int Agrupador;
                IdAgrupador = ConexionVeterinaria.SelectAgrupador(IdAnalisis.ToString());
                int.TryParse(IdAgrupador.Tables[0].Rows[0]["IdAgrupador"].ToString(), out Agrupador);
                ordenes = ConexionVeterinaria.selectOrden(IdOrden, IdAnalisis, Agrupador);
            }
            else
            {
                ordenes = ConexionVeterinaria.selectOrden(IdOrden);
            }


            var ultimo = ordenes.ResultadosAnalisis.OrderBy(x => x.IdOrganizador).LastOrDefault();
            ///
            /// Portada
            ///
            page = Portada(page, ordenes.datosRepresentante, ordenes.datosPacienteVet, empresa, ordenes.idOrden);

            foreach (var items in ordenes.ResultadosAnalisis.OrderBy(x => x.IdOrganizador))
            {

                ConexionVeterinaria.ActualizarEstadoDeResultado(3, items.IdOrden, items.IdAnalisis);
                if (items.IdAnalisis == 55)
                {
                    page = document.AddPage();
                    page = Header(page, ordenes.datosRepresentante, ordenes.datosPacienteVet, empresa, ordenes.idOrden, ordenes.Fecha);
                    page = Hematologia(page, ordenes);
                    if (ultimo.IdAnalisis == items.IdAnalisis) return document;
                }

                ///
                /// Uroanaliss
                ///
                if (items.IdAnalisis == 42)
                {
                    page = document.AddPage();
                    page = Header(page, ordenes.datosRepresentante, ordenes.datosPacienteVet, empresa, ordenes.idOrden, ordenes.Fecha);
                    page = Uroanaliss(page, ordenes);
                    if (ultimo.IdAnalisis == items.IdAnalisis) return document;
                }


                ///
                /// Copro
                ///     

                if (items.IdAnalisis == 12)
                {
                    page = document.AddPage();
                    page = Header(page, ordenes.datosRepresentante, ordenes.datosPacienteVet, empresa, ordenes.idOrden, ordenes.Fecha);
                    page = copro(page, ordenes);
                    if (ultimo.IdAnalisis == items.IdAnalisis) return document;
                }

                ///
                /// HematologiaEspecial
                ///
                if (items.IdAnalisis == 203)
                {
                    page = document.AddPage();
                    page = Header(page, ordenes.datosRepresentante, ordenes.datosPacienteVet, empresa, ordenes.idOrden, ordenes.Fecha);
                    page = HematologiaEspecial(page, ordenes);
                    if (ultimo.IdAnalisis == items.IdAnalisis) return document;
                }

                //Todos los demas analisis
                if (items.IdAnalisis != 203 && items.IdAnalisis != 12 && items.IdAnalisis != 42 && items.IdAnalisis != 55 && items.PorImprimir != true)
                {

                    XFont Titulo = new XFont(Curva, 18, XFontStyle.Regular);



                    if (!primerapagina)
                    {
                        page = document.AddPage();
                        Header(page, ordenes.datosRepresentante, ordenes.datosPacienteVet, empresa, ordenes.idOrden, ordenes.Fecha, 1);
                        primerapagina = true;

                    }
                   
                 
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                      XTextFormatter tf = new XTextFormatter(gfx);
                    XRect Margen = new XRect(5, 0, 145, 14);
                    bioencontrado = false;
                    XColor color = new XColor { R = 105, G = 105, B = 105 };
                    XPen pen;
                    XPoint point = new XPoint(5, 70);
                    XSize size;
                    XSize Elipsesize = new XSize(5, 5);
                    XRect rect;
                    pen = new XPen(color);
                    DataSet Bioanalista = new DataSet();
                    XFont fontRegular3;
                    XFont fontRegular4;
                    XFont fontRegular5;
                    XFont fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
                    XSolidBrush blueBrush = new XSolidBrush(XColors.LightGray);
                    XFont fontRegular = new XFont(facename, 11, XFontStyle.Regular);
                    fontRegular = new XFont(facename, 11, XFontStyle.Regular);
                    //Hematologia
                    fontRegular = new XFont(facename, 11, XFontStyle.Regular);
                    string text;

                    tf.Alignment = XParagraphAlignment.Center;
                    if (items.IdEstadoDeResultado > 1)
                    {

                        bioencontrado = false;
                        pen = new XPen(color);
                        if (!primerapagina)
                        {
                            if (Metodo == "Correo")
                            {
                                page.Height = 400;
                            }
                            primerapagina = true;
                            page = Header(page, ordenes.datosRepresentante, ordenes.datosPacienteVet, empresa, ordenes.idOrden, ordenes.Fecha, 1);
                            PosicionP = 95;


                        }
                        fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
                        fontRegular = new XFont(facename, 11, XFontStyle.Regular);
                        DateTime FechaImp = new DateTime();
                        //Hematologia
                        fontRegular = new XFont(facename, 11, XFontStyle.Regular);
                        tf.Alignment = XParagraphAlignment.Center;
                        if (Convert.ToInt32(ordenes.IdConvenio) <= 3)
                        {
                            empresa = ConexionVeterinaria.selectEmpresaActiva();
                        }

                        if (items.IdEstadoDeResultado > 1)
                        {

                            ConexionVeterinaria.ActualizarOrden("IDEstadoDeResultado = 3", Convert.ToInt32(IdOrden), Convert.ToInt32(items.IdAnalisis));

                            if (items.MultiplesValores == "")
                            {
                                if (items.ValorMenor != 0 || items.ValorMayor != 0)
                                {
                                    text = Convert.ToDouble(items.ValorMenor) + " - " + Convert.ToDouble(items.ValorMayor) + " " + items.unidad;
                                }
                                else
                                {
                                    text = "";
                                }
                            }
                            else
                            {
                                text = items.MultiplesValores;
                            }

                            string Analisis = items.analisisLaboratorio.NombreAnalisis;
                            int AnalisisCount = Analisis.Length;
                            string Observaciones = items.Comentario;
                            var Measure = gfx.MeasureString(text, fontRegular);
                            int ObservacionesCount = text.Length;
                            var ObservacionesString = gfx.MeasureString(Observaciones, fontRegular);
                            double Evaluar1, Evaluar2;
                            Evaluar1 = Math.Round(Measure.Width / 200, 1);
                            Evaluar2 = Regex.Matches(text, "\n").Count + 1;
                            double total;

                            if (Evaluar1 >= Evaluar2)
                            {
                                total = Math.Round(Measure.Width / 200, 1);
                            }
                            else
                            {
                                total = Regex.Matches(text, "\n").Count + 1;
                            }
                            double ObservacionesWidth = ObservacionesString.Width / 550;
                            double NumeroFont3 = 210 / (AnalisisCount * 0.59);
                            if (NumeroFont3 > 11) NumeroFont3 = 10;
                            fontRegular3 = new XFont(facename, NumeroFont3, XFontStyle.Regular);
                            fontRegular5 = new XFont(facename, 10, XFontStyle.Regular);
                            if (total < 1)
                            {
                                if (total != 1)
                                {
                                    total = 1;
                                }
                            }

                            if (items.analisisLaboratorio.Titulo == 1)
                            {
                                total = Convert.ToInt32(items.Lineas);
                            }
                            if (ObservacionesWidth < 1)
                            {
                                ObservacionesWidth = 1;
                            }
                            int MargenAncho = 15;
                            XImage Firma;
                            int y = 380;
                            fontRegular4 = new XFont(facename, 8, XFontStyle.Regular);
                            if (Posicion + MargenAncho * total > 390)
                            {
                                if (items == ultimo)
                                {
                                    foreach (int i in IdAnalisisBio)
                                    {
                                        Bioanalista = new DataSet();
                                        Bioanalista = ConexionVeterinaria.Bioanalista(IdOrden, i);
                                        Margen = new XRect(y, 400, 100, 60);
                                        string Bioanalista1 = string.Format("Analista. {0} CMVB:{1} MPPS:{2}", Bioanalista.Tables[0].Rows[0]["NombreUsuario"].ToString(), Bioanalista.Tables[0].Rows[0]["CB"].ToString(), Bioanalista.Tables[0].Rows[0]["MPPS"].ToString());
                                        tf.Alignment = XParagraphAlignment.Center;
                                        tf.DrawString(Bioanalista1, fontRegular4, XBrushes.Black, Margen, XStringFormats.TopLeft);
                                        Firma = XImage.FromFile(string.Format(@"Firma\{1}", Ruta, Bioanalista.Tables[0].Rows[0]["FIRMA"].ToString()));
                                        gfx.DrawImage(Firma, y + 105, 400);
                                        y = y - 160;
                                    }
                                    Bio.Clear();
                                    IdAnalisisBio.Clear();
                                    y = 380;
                                    primerapagina = false;
                                    if (!primerapagina)
                                    {
                                        page = document.AddPage();
                                        page = Header(page, ordenes.datosRepresentante, ordenes.datosPacienteVet, empresa, ordenes.idOrden, ordenes.Fecha, 1);
                                        primerapagina = true;
                                        gfx = XGraphics.FromPdfPage(page);
                                        tf = new XTextFormatter(gfx);
                                    }
                                    foreach (int i in IdAnalisisBio)
                                    {
                                        Bioanalista = new DataSet();
                                        Bioanalista = ConexionVeterinaria.Bioanalista(IdOrden, i);
                                        Margen = new XRect(y, 400, 100, 60);
                                        string Bioanalista1 = string.Format("Analista. {0} CMVB:{1} MPPS:{2}", Bioanalista.Tables[0].Rows[0]["NombreUsuario"].ToString(), Bioanalista.Tables[0].Rows[0]["CB"].ToString(), Bioanalista.Tables[0].Rows[0]["MPPS"].ToString());
                                        tf.Alignment = XParagraphAlignment.Center;
                                        tf.DrawString(Bioanalista1, fontRegular4, XBrushes.Black, Margen, XStringFormats.TopLeft);
                                        Firma = XImage.FromFile(string.Format(@"Firma\{1}", Ruta, Bioanalista.Tables[0].Rows[0]["FIRMA"].ToString()));
                                        gfx.DrawImage(Firma, y + 105, 400);
                                        y = y - 160;
                                    }

                                    Bioanalista = ConexionVeterinaria.Bioanalista(IdOrden, Convert.ToInt32(items.IdAnalisis));
                                    foreach (int i in Bio)
                                    {
                                        if ("" != Bioanalista.Tables[0].Rows[0]["IdUsuario"].ToString())
                                        {
                                            if (i == Convert.ToInt32(Bioanalista.Tables[0].Rows[0]["IdUsuario"].ToString()))
                                            {
                                                bioencontrado = true;
                                            }
                                        }
                                        else
                                        {
                                            bioencontrado = true;
                                        }
                                    }
                                    if (!bioencontrado)
                                    {
                                        IdAnalisisBio.Add(Convert.ToInt32(items.IdAnalisis));
                                        Bio.Add(Convert.ToInt32(Bioanalista.Tables[0].Rows[0]["IdUsuario"].ToString()));
                                    }
                                }
                                else
                                {

                                    foreach (int i in IdAnalisisBio)
                                    {
                                        Bioanalista = new DataSet();
                                        Bioanalista = ConexionVeterinaria.Bioanalista(IdOrden, i);
                                        Margen = new XRect(y, 400, 100, 60);
                                        string Bioanalista1 = string.Format("Analista. {0} CMVB:{1} MPPS:{2}", Bioanalista.Tables[0].Rows[0]["NombreUsuario"].ToString(), Bioanalista.Tables[0].Rows[0]["CB"].ToString(), Bioanalista.Tables[0].Rows[0]["MPPS"].ToString());
                                        tf.Alignment = XParagraphAlignment.Center;
                                        tf.DrawString(Bioanalista1, fontRegular4, XBrushes.Black, Margen, XStringFormats.TopLeft);
                                        Firma = XImage.FromFile(string.Format(@"Firma\{1}", Ruta, Bioanalista.Tables[0].Rows[0]["FIRMA"].ToString()));
                                        gfx.DrawImage(Firma, y + 105, 400);
                                        y = y - 160;
                                    }
                                    y = 360;
                                    primerapagina = false;
                                    if (!primerapagina)
                                    {
                                        page = document.AddPage();
                                        page = Header(page, ordenes.datosRepresentante, ordenes.datosPacienteVet, empresa, ordenes.idOrden, ordenes.Fecha, 1);
                                        primerapagina = true;
                                        gfx = XGraphics.FromPdfPage(page);
                                        tf = new XTextFormatter(gfx);
                                    }

                                    if (Metodo == "Correo")
                                    {
                                        page.Height = 400;
                                    }
                                    Posicion = 110;
                                    Bio.Clear();
                                    IdAnalisisBio.Clear();
                                    Bioanalista = ConexionVeterinaria.Bioanalista(IdOrden, Convert.ToInt32(items.IdAnalisis));
                                    foreach (int i in Bio)
                                    {
                                        if ("" != Bioanalista.Tables[0].Rows[0]["IdUsuario"].ToString())
                                        {
                                            if (i == Convert.ToInt32(Bioanalista.Tables[0].Rows[0]["IdUsuario"].ToString()))
                                            {
                                                bioencontrado = true;
                                            }
                                        }
                                        else
                                        {
                                            bioencontrado = true;
                                        }
                                    }
                                    if (!bioencontrado)
                                    {
                                        IdAnalisisBio.Add(Convert.ToInt32(items.IdAnalisis));
                                        Bio.Add(Convert.ToInt32(Bioanalista.Tables[0].Rows[0]["IdUsuario"].ToString()));
                                    }
                                }
                            }
                            else
                            {

                                Bioanalista = ConexionVeterinaria.Bioanalista(IdOrden, Convert.ToInt32(items.IdAnalisis));
                                foreach (int i in Bio)
                                {

                                    if ("" != Bioanalista.Tables[0].Rows[0]["IdUsuario"].ToString())
                                    {
                                        if (i == Convert.ToInt32(Bioanalista.Tables[0].Rows[0]["IdUsuario"].ToString()))
                                        {
                                            bioencontrado = true;
                                        }
                                    }
                                    else
                                    {
                                        bioencontrado = true;
                                    }
                                }
                                if (!bioencontrado)
                                {
                                    IdAnalisisBio.Add(Convert.ToInt32(items.IdAnalisis));
                                    Bio.Add(Convert.ToInt32(Bioanalista.Tables[0].Rows[0]["IdUsuario"].ToString()));
                                }
                            }

                            if (Evaluar1 >= Evaluar2)
                            {
                                total = Math.Round(Measure.Width / 200, 1);
                            }
                            else
                            {
                                total = Regex.Matches(text, "\n").Count + 1;
                            }

                            if (items == ultimo)
                            {
                                foreach (int i in IdAnalisisBio)
                                {
                                    Bioanalista = new DataSet();
                                    Bioanalista = ConexionVeterinaria.Bioanalista(IdOrden, i);
                                    Margen = new XRect(y, 400, 100, 60);
                                    string Bioanalista1 = string.Format("Analista. {0} CMVB:{1} MPPS:{2}", Bioanalista.Tables[0].Rows[0]["NombreUsuario"].ToString(), Bioanalista.Tables[0].Rows[0]["CB"].ToString(), Bioanalista.Tables[0].Rows[0]["MPPS"].ToString());
                                    tf.Alignment = XParagraphAlignment.Center;
                                    tf.DrawString(Bioanalista1, fontRegular4, XBrushes.Black, Margen, XStringFormats.TopLeft);
                                    Firma = XImage.FromFile(string.Format(@"Firma\{1}", Ruta, Bioanalista.Tables[0].Rows[0]["FIRMA"].ToString()));
                                    gfx.DrawImage(Firma, y + 105, 400);
                                    y = y - 160;
                                }
                                Bio.Clear();
                                IdAnalisisBio.Clear();
                            }
                            double MargenCentrado = Measure.Height * total;
                            double PosicionCentrada = Posicion + 5;


                            //Analisis

                            if (items.analisisLaboratorio.Titulo == 1)
                            {
                                fontRegular3 = new XFont(facename, 10, XFontStyle.Regular);
                                rect = new XRect(30, PosicionCentrada, 225, MargenCentrado);
                                gfx.DrawString(Analisis, fontRegular, XBrushes.Black, rect, XStringFormats.CenterLeft);
                                if (items.IdAnalisis == 357)
                                {
                                    /*
                                      * Cargar Hemoparasitos
                                      */
                                    double PosicionHorizontal = 5;

                                    if (ordenes.datosPacienteVet.especies.hemoParasitos.Count > 0)
                                    {
                                        double CantidadDeHemoParasitos = 580 / ordenes.datosPacienteVet.especies.hemoParasitos.Count;
                                        double ultimoHemoparasito = CantidadDeHemoParasitos * (ordenes.datosPacienteVet.especies.hemoParasitos.Count - 1);
                                        foreach (var hemo in ordenes.datosPacienteVet.especies.hemoParasitos)
                                        {
                                            if (ultimoHemoparasito < PosicionHorizontal)
                                            {
                                                CantidadDeHemoParasitos = 585 - PosicionHorizontal;
                                                PosicionHorizontal = 585 - CantidadDeHemoParasitos;
                                            }
                                            string ResultadoHemo = hemo.Resultado;
                                            Margen = new XRect(PosicionHorizontal, PosicionCentrada + 20, CantidadDeHemoParasitos, 30);
                                            gfx.DrawRectangle(pen, Margen);
                                            rect = new XRect(PosicionHorizontal, PosicionCentrada + 20, CantidadDeHemoParasitos, 20);
                                            tf.Alignment = XParagraphAlignment.Center;
                                            tf.DrawString(hemo.Descripcion, fontRegular, XBrushes.Black, rect, XStringFormats.TopLeft);
                                            rect = new XRect(PosicionHorizontal, PosicionCentrada + 35, CantidadDeHemoParasitos, 20);
                                            tf.DrawString(hemo.Resultado, fontRegular, XBrushes.Black, rect, XStringFormats.TopLeft);
                                            PosicionHorizontal += CantidadDeHemoParasitos;

                                        }
                                    }


                                    Posicion += 40;
                                }
                            }
                            else
                            {
                                if (items.IdAnalisis == 27)
                                {
    
                                    rect = new XRect(10, Posicion + 5, 230, Measure.Height * total);
                                    tf.Alignment = XParagraphAlignment.Left;
                                    gfx.DrawRectangle(pen, rect);
                                    rect = new XRect(12, PosicionCentrada, 225, MargenCentrado);
                                    tf.DrawString(Analisis, fontRegular3, XBrushes.Black, rect, XStringFormats.TopLeft);
                                    rect = new XRect(242, Posicion + 5, 100 + 233, Measure.Height * total);
                                    tf.Alignment = XParagraphAlignment.Left;
                                    gfx.DrawRectangle(pen, rect);
                                    rect = new XRect(285, PosicionCentrada, 190, MargenCentrado);
                                    tf.DrawString(items.ValorResultado + " " + items.unidad, fontRegular5, XBrushes.Black, rect, XStringFormats.TopLeft);
                                }
                                else
                                {
                                    if (Observaciones == "" || Observaciones == " ")
                                    {
                                        if (MargenCentrado > 15)
                                        {
                                            PosicionCentrada = PosicionCentrada + (MargenCentrado / 2 - Measure.Height / 2);
                                        }
                                    }
                                    rect = new XRect(10, Posicion + 5, 230, Measure.Height * total);
                                    tf.Alignment = XParagraphAlignment.Center;
                                    gfx.DrawRectangle(pen, rect);
                                    rect = new XRect(12, PosicionCentrada, 225, MargenCentrado);
                                    gfx.DrawString(Analisis, fontRegular3, XBrushes.Black, rect, XStringFormats.TopLeft);
                                    rect = new XRect(242, Posicion + 5, 100, Measure.Height * total);
                                    tf.Alignment = XParagraphAlignment.Center;
                                    gfx.DrawRectangle(pen, rect);
                                    rect = new XRect(240, PosicionCentrada, 105, MargenCentrado);
                                    tf.DrawString(items.ValorResultado + " " + items.unidad, fontRegular3, XBrushes.Black, rect, XStringFormats.TopLeft);
                                    rect = new XRect(345, Posicion + 5, 230, Measure.Height * total);
                                    tf.Alignment = XParagraphAlignment.Justify;
                                    gfx.DrawRectangle(pen, rect);
                                    rect = new XRect(350, Posicion + 5, 220, Measure.Height * total);
                                    tf.DrawString(text, fontRegular5, XBrushes.Black, rect, XStringFormats.TopLeft);
                                    rect = new XRect(347, Posicion + 5, 180, Measure.Height * total);
                                }

                            }


                            //Respuesta



                            //unidad
                            //rect = new XRect(515, Posicion+5, 60, Measure.Height * total);
                            //tf.Alignment = XParagraphAlignment.Center;
                            //gfx.DrawRectangle(pen, rect);
                            //rect = new XRect(517, Posicion+5, 60, Measure.Height * total);
                            //tf.DrawString(items.unidad, fontRegular2, XBrushes.Black, rect, XStringFormats.TopLeft);

                            // Valores de Referencia


                            if (Observaciones != "" && Observaciones != " ")
                            {
                                if (total > 1)
                                {
                                    ObservacionesWidth = ObservacionesString.Width / 230;
                                    rect = new XRect(10, Posicion = Posicion + 17, 250, MargenAncho * total - 15);
                                    tf.Alignment = XParagraphAlignment.Left;
                                    rect = new XRect(12, Posicion, 230, MargenAncho * total - 15);
                                    if (items.analisisLaboratorio.NombreAnalisis == "OBSERVACIONES")
                                    {
                                        tf.DrawString(string.Format("Observaciones: {0}", Observaciones), fontRegular2, XBrushes.Black, rect, XStringFormats.TopLeft);
                                    }
                                    else
                                    {
                                        tf.DrawString(string.Format(" {0}", Observaciones), fontRegular2, XBrushes.Black, rect, XStringFormats.TopLeft);
                                    }

                                    Posicion = Posicion + Measure.Height * total;
                                }
                                else
                                {
                                    rect = new XRect(10, Posicion = Posicion + MargenAncho + 3, 565, MargenAncho * ObservacionesWidth);
                                    tf.Alignment = XParagraphAlignment.Left;
                                    gfx.DrawRectangle(pen, blueBrush, rect);
                                    rect = new XRect(12, Posicion + 2, 550, MargenAncho * ObservacionesWidth);
                                    if (items.analisisLaboratorio.NombreAnalisis == "OBSERVACIONES")
                                    {
                                        tf.DrawString(string.Format("Observaciones: {0}", Observaciones), fontRegular2, XBrushes.Black, rect, XStringFormats.TopLeft);
                                    }
                                    else
                                    {
                                        tf.DrawString(string.Format(" {0}", Observaciones), fontRegular2, XBrushes.Black, rect, XStringFormats.TopLeft);
                                    }

                                    if (text == "")
                                    {
                                        Posicion = Posicion + MargenAncho * ObservacionesWidth;
                                    }
                                    else
                                    {
                                        Posicion = Posicion + MargenAncho * total;
                                    }
                                }
                            }
                            else
                            {
                                Posicion = Posicion + Convert.ToInt32(Measure.Height) * total + 5;
                            }

                            if (items.analisisLaboratorio.FinalTitulo == 1)
                            {
                                fontRegular3 = new XFont(facename, 10, XFontStyle.Regular);
                                rect = new XRect(12, PosicionCentrada, 225, MargenCentrado);
                                Posicion = Posicion + MargenAncho * total;
                            }

                        }


                    }
                    gfx.Dispose();
                }




            }


            return document;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdSesion"></param>
        /// <returns></returns>
        public PdfSharp.Pdf.PdfDocument PerfilCompuestoAgrupado(PdfSharp.Pdf.PdfDocument document, Facturas factura, Empresa empresa, Perfil perfil)
        {
            int IdEspecie = 0;
            bool HeaderImpreso = false;
            List<Ordenes> ordenes = new List<Ordenes>();
            
            List<int> Especies = new List<int>();
            PdfSharp.Pdf.PdfPage page = document.Pages[document.Pages.Count - 1];
            Especies = ConexionVeterinaria.EspeciesPorSesion(factura.IdSesion);
            XRect Margen = new XRect(5, PosicionP, 250, 14);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);

            XFont fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
            XSolidBrush blueBrush = new XSolidBrush(XColors.Black);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            XPoint point = new XPoint(5, 70);
            XSize size = new XSize(580, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            int i = 0;
            int PosicionY = 75;
            foreach (var item in Especies)
            {
                ordenes = ConexionVeterinaria.SelectOrdenesPorPerfil(factura.IdSesion, perfil.IdPerfil,item);
                if (ordenes.Count > 0)
                {

                
                foreach (var orden in ordenes)
                {
                    perfil.resultados.Clear();
                    foreach (var ResultadosEncontrados in perfil.analisisLaboratorios.Where(a => a.Titulo == 0))
                    {
                        //  
                        var valores = orden.ResultadosAnalisis.Where(a => a.IdAnalisis == ResultadosEncontrados.IdAnalisis).ToList();
                        foreach (var valor in valores)
                        {
                            perfil.resultados.Add(valor);
                        }

                    }


                    if (!HeaderImpreso)
                    {
                        page = document.AddPage();
                        page = HeaderGrup(page, factura.datosRepresentante, factura.finca, empresa, factura.IdSesion, DateTime.Now, factura.NumeroDia);
                        fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
                        blueBrush = new XSolidBrush(XColors.Black);
                        color = new XColor { R = 105, G = 105, B = 105 };
                        pen = new XPen(color);
                        point = new XPoint(20, 70);
                        size = new XSize(580, 15);
                        Elipsesize = new XSize(5, 5);
                        rect = new XRect(point, size);
                        gfx = XGraphics.FromPdfPage(page);
                        tf = new XTextFormatter(gfx);

                        int PosicionX = 120;
                        double PosicionP = 95;
                        Margen = new XRect(230, PosicionP, 120, 14);
                        gfx.DrawString($"{perfil.NombrePerfil} {ordenes.First().datosPacienteVet.especies.Descripcion}", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("Identificación Animal", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                        foreach (var analisis in perfil.analisisLaboratorios.Where(a => a.Titulo == 0))
                        {
                            PosicionP += 15;
                            double TamañoNombre = 0;
                            var ObservacionesString = gfx.MeasureString(analisis.NombreAnalisis, fontRegular2);

                            TamañoNombre = ObservacionesString.Width / 120;
                            if (TamañoNombre > 1)
                            {
                                string inicales = "";
                                bool espacio = true;

                                foreach (char caracter in analisis.NombreAnalisis)
                                {

                                    if (espacio)
                                    {
                                        inicales += caracter.ToString();
                                        espacio = false;
                                    }

                                    if (char.IsWhiteSpace(caracter))
                                    {

                                        espacio = true;

                                    }

                                }
                                analisis.NombreAnalisis = inicales;
                            }
                            Margen = new XRect(13, PosicionP, 120, 14);

                            tf.DrawString($"{analisis.NombreAnalisis}", fontRegular2, blueBrush, Margen, XStringFormats.TopLeft);
                            gfx.DrawLine(pen, 10, PosicionP, 575, PosicionP);
                        }

                        point = new XPoint(10, 110);
                        size = new XSize(565, PosicionP - 95);
                        //gfx.DrawLine(pen, PosicionX, 110, PosicionX, PosicionP + 15);
                        rect = new XRect(point, size);
                        gfx.DrawRectangle(pen, rect);

                        Usuarios Bioanalista = new Usuarios();
                        Bioanalista = ConexionVeterinaria.selectUsuarioPorId(orden.ResultadosAnalisis.FirstOrDefault().IdUsuario);
                        Margen = new XRect(200, PosicionP += 20, 250, 14);
                        tf.DrawString(string.Format("Analista"), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(200, PosicionP += 12, 250, 14);
                        tf.DrawString(string.Format(" {0} CMVB:{1} MPPS:{2}", Bioanalista.NombreUsuario, Bioanalista.CB, Bioanalista.MPPS), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        XImage Logo = XImage.FromFile(string.Format(@"firma\{0}", Bioanalista.Firma));
                        gfx.DrawImage(Logo, 390, PosicionP - 12);
                        //Valores de Referencia
                        PosicionX = 120;
                        PosicionP = 140;
                        HeaderImpreso = true;
                    }


                    PosicionP = 110;
                    Margen = new XRect(PosicionY + 15, PosicionP, 120, 14);
                    gfx.DrawString($"{orden.datosPacienteVet.NombrePaciente}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                    foreach (var ValoresDeReferencia in perfil.resultados)
                    {
                        PosicionP += 15;
                        Margen = new XRect(PosicionY + 15, PosicionP, 120, 14);
                        gfx.DrawString($"{ValoresDeReferencia.ValorResultado}", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                    }
                    gfx.DrawLine(pen, PosicionY + 50, 110, PosicionY + 50, PosicionP + 15);

                    if (i % 5 == 0 && i > 0 || ordenes.Last() == orden)
                    {
                    
                    }
 


                    PosicionP = 110;
                    PosicionY += 56;
                    //Limitar 6 pacientes o ultima orden
                    if (i % 5 == 0 && i > 0 || ordenes.Last() == orden)
                    {
                        gfx.DrawLine(pen, PosicionY + 50, 110, PosicionY + 50, PosicionP + 15 * (perfil.resultados.Count + 1));
                        if (i < 5)
                        {
                            gfx.DrawLine(pen, 450, 110, 450, PosicionP + 15 * (perfil.resultados.Count + 1));
                        }
                        PosicionP = 110;
                        Margen = new XRect(460, PosicionP, 120, 14);
                        gfx.DrawString("VALORES DE REFERENCIA", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        perfil.resultados = ConexionVeterinaria.Grupales(factura.IdSesion, perfil.IdPerfil, IdEspecie);
                        foreach (var ValoresDeReferencia in perfil.resultados)
                        {
                            PosicionP += 15;
                            Margen = new XRect(470, PosicionP, 90, 14);
                            gfx.DrawString($"{ValoresDeReferencia.ValorMenor} - {ValoresDeReferencia.ValorMayor} {ValoresDeReferencia.unidad}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        }
                        PosicionY = 75;
                        HeaderImpreso = false;
                        gfx.Dispose();

                    }

                    i++;

                }
                }


            }



            gfx.Dispose();
            return document;
        }
        public PdfSharp.Pdf.PdfDocument Leptospira(PdfSharp.Pdf.PdfDocument document, Facturas factura, Empresa empresa, Perfil perfil)
        {
            int IdEspecie = 0;
            bool HeaderImpreso = false;
            List<Ordenes> ordenes = new List<Ordenes>();
            ordenes = factura.Ordenes;
            List<int> Especies = new List<int>();
            PdfSharp.Pdf.PdfPage page = document.Pages[document.Pages.Count - 1];
            Especies = ConexionVeterinaria.EspeciesPorSesion(factura.IdSesion);
            int PosicionP = 95;
            XRect Margen = new XRect();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            XFont fontRegular3 = new XFont(facename, 7, XFontStyle.Regular);
            XFont fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
            XSolidBrush blueBrush = new XSolidBrush(XColors.Black);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            XPoint point = new XPoint(5, 70);
            XSize length = new XSize();
            XSize size = new XSize(580, 15);    
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            List<double> lineasVerticales = new List<double>();

            int i = 0;
            int PosicionY = 60;
            lineasVerticales.Add(55);

            foreach (var item in Especies)
            {
                ordenes = ConexionVeterinaria.SelectOrdenesPorPerfil(factura.IdSesion, perfil.IdPerfil, item);
                foreach (var orden in ordenes)
                {
                    List<ResultadosPorAnalisisVet> resultados = new List<ResultadosPorAnalisisVet>();
                    perfil.resultados = resultados;
                    foreach (var ResultadosEncontrados in perfil.analisisLaboratorios.Where(a => a.Titulo == 0))
                    {
                        ResultadosPorAnalisisVet resultado = new ResultadosPorAnalisisVet();
                        resultado = orden.ResultadosAnalisis.Where(a => a.IdAnalisis == ResultadosEncontrados.IdAnalisis).FirstOrDefault();
                        if (resultado != null)
                        {
                            perfil.resultados.Add(resultado);
                        }

                    }
                    if (perfil.resultados.Count > 0)
                    {
                        if (!HeaderImpreso)
                        {
                            page = document.AddPage();
                            page = HeaderGrup(page, factura.datosRepresentante, factura.finca, empresa, factura.IdSesion, DateTime.Now, factura.NumeroDia);
                            fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
                            blueBrush = new XSolidBrush(XColors.Black);
                            color = new XColor { R = 105, G = 105, B = 105 };
                            pen = new XPen(color);
                            point = new XPoint(20, 70);
                            size = new XSize(580, 15);
                            Elipsesize = new XSize(5, 5);
                            rect = new XRect(point, size);
                            gfx = XGraphics.FromPdfPage(page);
                            tf = new XTextFormatter(gfx);



                            int PosicionX = 120;
                            PosicionP = 95;
                            Margen = new XRect(230, PosicionP, 120, 14);
                            // AGREGANDO NOMBRE DE PERFIL
                            gfx.DrawString($"{perfil.NombrePerfil} {ordenes.First().datosPacienteVet.especies.Descripcion}", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                            PosicionP += 15;
                            Margen = new XRect(15, PosicionP, 40, 40);
                            gfx.DrawString("Id. Animal", fontRegular2, blueBrush, Margen, XStringFormats.CenterLeft);

                            // AGREGANDO NOMBRE DE ANALISIS
                            PosicionY = 60;
                            foreach (var analisis in perfil.analisisLaboratorios.Where(a => a.Titulo == 0))
                            {

                                Margen = new XRect(PosicionY, PosicionP, 40, 40);
                                gfx.DrawString($"{analisis.NombreAnalisis.Replace("LEPTOSPIRA", "")}", fontRegular2, blueBrush, Margen, XStringFormats.CenterLeft);
                                length = gfx.MeasureString($"{analisis.NombreAnalisis.Replace("LEPTOSPIRA", "")}", fontRegular2);
                                double contador = length.Width;
                                Margen = new XRect(PosicionY += Convert.ToInt32(contador) + 10, PosicionP, 40, 40);
                                if (analisis.IdAnalisis < perfil.analisisLaboratorios[6].IdAnalisis)
                                {
                                    lineasVerticales.Add(PosicionY);
                                }

                            }
                            gfx.DrawLine(pen, 10, PosicionP + 25, 575, PosicionP + 25);

                            //Valores de Referencia
                            PosicionX = 120;
                            PosicionP = 135;
                            HeaderImpreso = true;
                        }

                        Margen = new XRect(13, PosicionP += 12, 40, 14);
                        gfx.DrawString($"{orden.datosPacienteVet.NombrePaciente}", fontRegular3, blueBrush, Margen, XStringFormats.Center);
                        PosicionY = 20;
                        int C = 1;
                        // AGREGANDO RESULTADOS
                        foreach (var ValoresDeReferencia in perfil.resultados)
                        {
                            switch (C)
                            {
                                case 1:
                                    Margen = new XRect(PosicionY += 70, PosicionP, 40, 40);
                                    break;
                                case 2:
                                    Margen = new XRect(PosicionY += 90, PosicionP, 40, 40);
                                    break;
                                case 3:
                                    Margen = new XRect(PosicionY += 48, PosicionP, 40, 40);
                                    break;
                                case 4:
                                    Margen = new XRect(PosicionY += 65, PosicionP, 40, 40);
                                    break;
                                case 5:
                                    Margen = new XRect(PosicionY += 80, PosicionP, 40, 40);
                                    break;
                                case 6:
                                    Margen = new XRect(PosicionY += 80, PosicionP, 40, 40);
                                    break;
                                case 7:
                                    Margen = new XRect(PosicionY += 70, PosicionP, 40, 40);
                                    break;
                            }

                            gfx.DrawString($"{ValoresDeReferencia.ValorResultado}", fontRegular3, blueBrush, Margen, XStringFormats.TopLeft);
                            if (ValoresDeReferencia.IdAnalisis < perfil.resultados[6].IdAnalisis)
                            {
                                gfx.DrawLine(pen, 10, PosicionP + 10, 575, PosicionP + 10);
                            }
                            C++;
                        }

                        PosicionP += 10;
                        //Limitar 6 pacientes o ultima orden


                        i++;
                    }
                    if (i % 12 == 0 && i > 0 || ordenes.Last() == orden)
                    {
                        foreach (var linea in lineasVerticales)
                        {
                            gfx.DrawLine(pen, linea, 110, linea, PosicionP);
                        }
                        point = new XPoint(10, 110);
                        size = new XSize(565, PosicionP - 110);
                        rect = new XRect(point, size);
                        gfx.DrawRectangle(pen, rect);
                        Margen = new XRect(460, PosicionP, 120, 14);
                        Usuarios Bioanalista = new Usuarios();
                        Bioanalista = orden.ResultadosAnalisis.First().bioanalista;
                        Margen = new XRect(300, PosicionP = PosicionP + 20, 250, 14);
                        tf.DrawString(string.Format("Analista"), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(300, PosicionP = PosicionP + 12, 250, 14);
                        tf.DrawString(string.Format(" {0} CMVB:{1} MPPS:{2}", Bioanalista.NombreUsuario, Bioanalista.CB, Bioanalista.MPPS), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        XImage Logo = XImage.FromFile(string.Format(@"firma\{0}", Bioanalista.Firma));
                        gfx.DrawImage(Logo, 490, PosicionP - 15);
                        PosicionY = 75;
                        gfx.Dispose();
                        HeaderImpreso = false;


                    }
                    else
                    {


                    }

                }



            }



            gfx.Dispose();
            return document;
        }
        public PdfSharp.Pdf.PdfDocument DocumentoGrupal(PdfSharp.Pdf.PdfDocument document, Perfil perfil, Facturas factura, Empresa empresa)
        {
            int IdEspecie = 0;
            bool HeaderImpreso = false;
            List<Ordenes> ordenes = new List<Ordenes>();
            List<Hemo> hemoparasitos = new List<Hemo>();
            List<int> Especies = new List<int>();
            PdfSharp.Pdf.PdfPage page = document.Pages[document.Pages.Count - 1];
            Especies = ConexionVeterinaria.EspeciesPorSesion(factura.IdSesion);
            XRect Margen = new XRect(5, PosicionP, 250, 14);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);

            XFont fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
            XSolidBrush blueBrush = new XSolidBrush(XColors.Black);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            XPoint point = new XPoint(5, 70);
            XSize size = new XSize(580, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            int i = 0;
            int PosicionY = 75;
            foreach (var item in Especies)
            {
                ordenes = ConexionVeterinaria.SelectHematologiasOrdenPorSesion(factura.IdSesion, item);
                foreach (var orden in ordenes)
                {
                    if (!HeaderImpreso)
                    {
                        page = document.AddPage();
                        page = HeaderGrup(page, factura.datosRepresentante, factura.finca, empresa, factura.IdSesion, DateTime.Now, factura.NumeroDia);

                        hemoparasitos = ConexionVeterinaria.selectHemoparasitosPorÉspecie(item);
                        fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
                        blueBrush = new XSolidBrush(XColors.Black);
                        color = new XColor { R = 105, G = 105, B = 105 };
                        pen = new XPen(color);
                        point = new XPoint(20, 70);
                        size = new XSize(580, 15);
                        Elipsesize = new XSize(5, 5);
                        rect = new XRect(point, size);
                        gfx = XGraphics.FromPdfPage(page);
                        tf = new XTextFormatter(gfx);


                        int PosicionX = 120;
                        int PosicionP = 95;
                        Margen = new XRect(230, PosicionP, 120, 14);
                        gfx.DrawString($"{perfil.NombrePerfil} {ordenes.First().datosPacienteVet.especies.Descripcion}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("IDENTIFICACION ANIMAL", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("HEMOGLOBINA", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("HEMATOCRITO", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("CUENTA LEUCOCITARIA", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("NEUTROFILOS", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("LINFOCITOS", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("MONOCITOS", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("EOSINOFILOS", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("DET. PLAQUETAS", fontRegular2, blueBrush, Margen, XStringFormats.Center);



                        for (int z = 0; z <= hemoparasitos.Count - 1; z++)
                        {
                            PosicionP += 15;
                            Margen = new XRect(5, PosicionP, 120, 14);
                            Hemo hemo = hemoparasitos.ElementAt(z);
                            gfx.DrawString(hemo.Descripcion, fontRegular2, blueBrush, Margen, XStringFormats.Center);

                        }
                        point = new XPoint(10, 110);
                        size = new XSize(565, PosicionP - 95);
                        gfx.DrawLine(pen, PosicionX, 110, PosicionX, PosicionP + 15);
                        rect = new XRect(point, size);
                        gfx.DrawRectangle(pen, rect);


                        //gfx.DrawLine(pen, PosicionX, 110, PosicionX, 320);

                        PosicionP = 110;
                        for (int x = 1; x < 9 + hemoparasitos.Count; x++)
                        {
                            PosicionP += 15;
                            gfx.DrawLine(pen, 10, PosicionP, 575, PosicionP);
                        }

                        //Valores de Referencia
                        PosicionX = 120;
                        PosicionP = 140;
                        HeaderImpreso = true;
                    }


                    PosicionP = 110;
                    Margen = new XRect(PosicionY + 15, PosicionP, 120, 14);
                    gfx.DrawString($"{orden.datosPacienteVet.NombrePaciente}", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                    foreach (var ValoresDeReferencia in orden.ResultadosAnalisis.Where(x => x.IdAnalisis == 256 || x.IdAnalisis == 257 || x.IdAnalisis == 259 || x.IdAnalisis == 261 || x.IdAnalisis == 263 || x.IdAnalisis == 267 || x.IdAnalisis == 269 || x.IdAnalisis == 273).OrderByDescending(n => n.IdAnalisis))
                    {
                        PosicionP += 15;
                        switch (ValoresDeReferencia.IdAnalisis)
                        {
                            case 269:
                                Margen = new XRect(PosicionY + 15, 125, 120, 14);
                                break;
                            case 267:
                                Margen = new XRect(PosicionY + 15, 140, 120, 14);
                                break;
                            case 256:
                                Margen = new XRect(PosicionY + 15, 155, 120, 14);
                                break;
                            case 257:
                                Margen = new XRect(PosicionY + 15, 170, 120, 14);
                                break;
                            case 259:
                                Margen = new XRect(PosicionY + 15, 185, 120, 14);
                                break;
                            case 261:
                                Margen = new XRect(PosicionY + 15, 200, 120, 14);
                                break;
                            case 263:
                                Margen = new XRect(PosicionY + 15, 215, 120, 14);
                                break;
                            case 273:
                                Margen = new XRect(PosicionY + 15, 230, 120, 14);
                                break;
                        }
                        gfx.DrawString($"{ValoresDeReferencia.ValorResultado}", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                    }
                    foreach (var hemo in orden.datosPacienteVet.especies.hemoParasitos)
                    {
                        PosicionP += 15;
                        Margen = new XRect(PosicionY + 15, PosicionP, 120, 14);
                        gfx.DrawString($"{hemo.Resultado}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                    }

                    PosicionY += 56;
                    gfx.DrawLine(pen, PosicionY + 50, 110, PosicionY + 50, PosicionP + 15);
                    if (i % 5 == 0 && i > 0 || ordenes.Last() == orden)
                    {
                        if (i % 5 > 0 || i % 5 < 0)
                        {
                            gfx.DrawLine(pen, 450, 110, 450, PosicionP + 15);
                        }
                        PosicionP = 110;
                        Margen = new XRect(460, PosicionP, 120, 14);
                        gfx.DrawString("VALORES DE REFERENCIA", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                        foreach (var ValoresDeReferencia in orden.ResultadosAnalisis.Where(x => x.IdAnalisis == 256 || x.IdAnalisis == 257 || x.IdAnalisis == 259 || x.IdAnalisis == 261 || x.IdAnalisis == 263 || x.IdAnalisis == 267 || x.IdAnalisis == 269 || x.IdAnalisis == 273).OrderByDescending(n => n.IdAnalisis))
                        {
                            PosicionP += 15;
                            switch (ValoresDeReferencia.IdAnalisis)
                            {
                                case 269:
                                    Margen = new XRect(470, PosicionP, 90, 14);
                                    break;
                                case 267:
                                    Margen = new XRect(470, PosicionP, 90, 14);
                                    break;
                                case 256:
                                    Margen = new XRect(470, PosicionP, 90, 14);
                                    break;
                                case 257:
                                    Margen = new XRect(470, PosicionP, 90, 14);
                                    break;
                                case 259:
                                    Margen = new XRect(470, PosicionP, 90, 14);
                                    break;
                                case 261:
                                    Margen = new XRect(470, PosicionP, 90, 14);
                                    break;
                                case 263:
                                    Margen = new XRect(470, PosicionP, 90, 14);
                                    break;
                                case 273:
                                    Margen = new XRect(470, PosicionP, 90, 14);
                                    break;
                            }
                            gfx.DrawString($"{ValoresDeReferencia.ValorMenor} - {ValoresDeReferencia.ValorMayor} {ValoresDeReferencia.unidad}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        }
                        Usuarios Bioanalista = new Usuarios();
                        Bioanalista = orden.usuarios;
                        Margen = new XRect(302, PosicionP = PosicionP + 70, 250, 14);
                        gfx.DrawString(string.Format("Analista"), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(300, PosicionP = PosicionP + 12, 250, 14);
                        gfx.DrawString(string.Format(" {0} CMVB:{1} MPPS:{2}", Bioanalista.NombreUsuario, Bioanalista.CB, Bioanalista.MPPS), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        XImage Logo = XImage.FromFile(string.Format(@"firma\{0}", Bioanalista.Firma));
                        gfx.DrawImage(Logo, PosicionP + 165, 300);
                        PosicionY = 75;
                        gfx.Dispose();
                        HeaderImpreso = false;


                    }

                    i++;
                }
            }
            return document;
        }
        /// <summary>
        /// Cambiar a private
        /// </summary>
        /// <returns></returns>
        /// 
        public PdfSharp.Pdf.PdfDocument HematologiaGrupal(PdfSharp.Pdf.PdfDocument document, Facturas factura, Empresa empresa)
        {
            int IdEspecie = 0;
            bool HeaderImpreso = false;
            List<Ordenes> ordenes = new List<Ordenes>();
            List<Hemo> hemoparasitos = new List<Hemo>();
            List<int> Especies = new List<int>();
            PdfSharp.Pdf.PdfPage page = document.Pages[document.Pages.Count - 1];
            Especies = ConexionVeterinaria.EspeciesPorSesion(factura.IdSesion);
            XRect Margen = new XRect(5, PosicionP, 250, 14);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);

            XFont fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
            XSolidBrush blueBrush = new XSolidBrush(XColors.Black);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            XPoint point = new XPoint(5, 70);
            XSize size = new XSize(580, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            int i = 0;
            int PosicionY = 75;
            string Observaciones = string.Empty;
            foreach (var item in Especies)
            {
                ordenes = ConexionVeterinaria.SelectHematologiasOrdenPorSesion(factura.IdSesion, item);
                foreach (var orden in ordenes)
                {

                    if (!HeaderImpreso)
                    {
                        page = document.AddPage();
                        page = HeaderGrup(page, factura.datosRepresentante, factura.finca, empresa, factura.IdSesion, DateTime.Now, factura.NumeroDia);

                        hemoparasitos = ConexionVeterinaria.selectHemoparasitosPorÉspecie(item);
                        fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
                        blueBrush = new XSolidBrush(XColors.Black);
                        color = new XColor { R = 105, G = 105, B = 105 };
                        pen = new XPen(color);
                        point = new XPoint(20, 70);
                        size = new XSize(580, 15);
                        Elipsesize = new XSize(5, 5);
                        rect = new XRect(point, size);
                        gfx = XGraphics.FromPdfPage(page);
                        tf = new XTextFormatter(gfx);

                        int PosicionX = 120;
                        int PosicionP = 95;
                        Margen = new XRect(230, PosicionP, 120, 14);
                        gfx.DrawString($"HEMATOLOGIA COMPLETA {ordenes.First().datosPacienteVet.especies.Descripcion}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("IDENTIFICACION ANIMAL", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("HEMOGLOBINA", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("HEMATOCRITO", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("CUENTA LEUCOCITARIA", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("NEUTROFILOS", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("LINFOCITOS", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("MONOCITOS", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("EOSINOFILOS", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("DET. PLAQUETAS", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                        for (int z = 0; z <= hemoparasitos.Count - 1; z++)
                        {
                            PosicionP += 15;
                            Margen = new XRect(5, PosicionP, 120, 14);
                            Hemo hemo = hemoparasitos.ElementAt(z);
                            gfx.DrawString(hemo.Descripcion, fontRegular2, blueBrush, Margen, XStringFormats.Center);

                        }
                        point = new XPoint(10, 110);
                        size = new XSize(565, PosicionP - 95);
                        gfx.DrawLine(pen, PosicionX, 110, PosicionX, PosicionP + 15);
                        rect = new XRect(point, size);
                        gfx.DrawRectangle(pen, rect);


                        //gfx.DrawLine(pen, PosicionX, 110, PosicionX, 320);

                        PosicionP = 110;
                        for (int x = 1; x < 9 + hemoparasitos.Count; x++)
                        {
                            PosicionP += 15;
                            gfx.DrawLine(pen, 10, PosicionP, 575, PosicionP);
                        }

                        //Valores de Referencia
                        PosicionX = 120;
                        PosicionP = 140;
                        HeaderImpreso = true;
                    }


                    PosicionP = 110;
                    Margen = new XRect(PosicionY + 15, PosicionP, 120, 14);
                    gfx.DrawString($"{orden.datosPacienteVet.NombrePaciente}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                    if (!string.IsNullOrEmpty(orden.ResultadosAnalisis.Where(x => x.IdAnalisis == 55).FirstOrDefault().Comentario))
                    {
                        Observaciones += $" Paciente: {orden.datosPacienteVet.NombrePaciente} {orden.ResultadosAnalisis.Where(x => x.IdAnalisis == 55).FirstOrDefault().Comentario} \n";
                    }
                    foreach (var ValoresDeReferencia in orden.ResultadosAnalisis.Where(x => x.IdAnalisis == 256 || x.IdAnalisis == 257 || x.IdAnalisis == 259 || x.IdAnalisis == 261 || x.IdAnalisis == 263 || x.IdAnalisis == 267 || x.IdAnalisis == 269 || x.IdAnalisis == 273).OrderByDescending(n => n.IdAnalisis))
                    {
                        PosicionP += 15;
                        switch (ValoresDeReferencia.IdAnalisis)
                        {
                            case 269:
                                Margen = new XRect(PosicionY + 15, 125, 120, 14);
                                break;
                            case 267:
                                Margen = new XRect(PosicionY + 15, 140, 120, 14);
                                break;
                            case 256:
                                Margen = new XRect(PosicionY + 15, 155, 120, 14);
                                break;
                            case 257:
                                Margen = new XRect(PosicionY + 15, 170, 120, 14);
                                break;
                            case 259:
                                Margen = new XRect(PosicionY + 15, 185, 120, 14);
                                break;
                            case 261:
                                Margen = new XRect(PosicionY + 15, 200, 120, 14);
                                break;
                            case 263:
                                Margen = new XRect(PosicionY + 15, 215, 120, 14);
                                break;
                            case 273:
                                Margen = new XRect(PosicionY + 15, 230, 120, 14);
                                break;
                        }
                        gfx.DrawString($"{ValoresDeReferencia.ValorResultado}", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                    }
                    foreach (var hemo in orden.datosPacienteVet.especies.hemoParasitos)
                    {
                        PosicionP += 15;
                        Margen = new XRect(PosicionY + 15, PosicionP, 120, 14);
                        gfx.DrawString($"{hemo.Resultado}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                    }

                    PosicionY += 56;
                    gfx.DrawLine(pen, PosicionY + 50, 110, PosicionY + 50, PosicionP + 15);
                    if (i % 5 == 0 && i > 0 || ordenes.Last() == orden)
                    {
                        if (i % 5 > 0 || i % 5 < 0)
                        {
                            gfx.DrawLine(pen, 455, 110, 455, PosicionP + 15);
                        }
                        if (i < 5)
                        {
                            gfx.DrawLine(pen, 455, 110, 455, PosicionP + 15);
                        }
                        PosicionP = 110;
                        Margen = new XRect(460, PosicionP, 120, 14);

                        gfx.DrawString("VALORES DE REFERENCIA", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                        foreach (var ValoresDeReferencia in orden.ResultadosAnalisis.Where(x => x.IdAnalisis == 256 || x.IdAnalisis == 257 || x.IdAnalisis == 259 || x.IdAnalisis == 261 || x.IdAnalisis == 263 || x.IdAnalisis == 267 || x.IdAnalisis == 269 || x.IdAnalisis == 273).OrderByDescending(n => n.IdAnalisis))
                        {
                            PosicionP += 15;
                            switch (ValoresDeReferencia.IdAnalisis)
                            {
                                case 269:
                                    Margen = new XRect(470, 125, 90, 14);
                                    break;
                                case 267:
                                    Margen = new XRect(470, 140, 90, 14);
                                    break;
                                case 256:
                                    Margen = new XRect(470, 155, 90, 14);
                                    break;
                                case 257:
                                    Margen = new XRect(470, 170, 90, 14);
                                    break;
                                case 259:
                                    Margen = new XRect(470, 185, 90, 14);
                                    break;
                                case 261:
                                    Margen = new XRect(470, 200, 90, 14);
                                    break;
                                case 263:
                                    Margen = new XRect(470, 215, 90, 14);
                                    break;
                                case 273:
                                    Margen = new XRect(470, 230, 90, 14);
                                    break;
                            }
                            gfx.DrawString($"{ValoresDeReferencia.ValorMenor} - {ValoresDeReferencia.ValorMayor} {ValoresDeReferencia.unidad}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        }
                        Margen = new XRect(15, PosicionP = PosicionP + 90, 250, 90);
                        tf.DrawString(Observaciones, fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Usuarios Bioanalista = new Usuarios();
                        Bioanalista = orden.usuarios;
                        Margen = new XRect(302, PosicionP + 24, 280, 14);
                        gfx.DrawString(string.Format("Analista"), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(300, PosicionP = PosicionP + 36, 250, 14);
                        gfx.DrawString(string.Format(" {0} CMVB:{1} MPPS:{2}", Bioanalista.NombreUsuario, Bioanalista.CB, Bioanalista.MPPS), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        XImage Logo = XImage.FromFile(string.Format(@"firma\{0}", Bioanalista.Firma));
                        gfx.DrawImage(Logo, PosicionP + 150, 330);
                        PosicionY = 75;
                        gfx.Dispose();
                        HeaderImpreso = false;


                    }

                    i++;

                }



            }




            return document;
        }
        public PdfSharp.Pdf.PdfDocument HemotropicosGrupal(PdfSharp.Pdf.PdfDocument document, Facturas factura, Empresa empresa)
        {
            int IdEspecie = 0;
            bool HeaderImpreso = false;
            List<Ordenes> ordenes = new List<Ordenes>();
            List<Hemo> hemoparasitos = new List<Hemo>();
            List<int> Especies = new List<int>();
            PdfSharp.Pdf.PdfPage page = document.Pages[document.Pages.Count - 1];
            Especies = ConexionVeterinaria.EspeciesPorSesion(factura.IdSesion);
            XRect Margen = new XRect(5, PosicionP, 250, 14);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            XFont fontRegular3 = new XFont(facename, 7, XFontStyle.Regular);
            XFont fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
            XSolidBrush blueBrush = new XSolidBrush(XColors.Black);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            XPoint point = new XPoint(5, 70);
            XSize size = new XSize(580, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            int i = 0;
            int PosicionY = 75;
            string Observaciones = string.Empty;
            foreach (var item in Especies)
            {
                ordenes = ConexionVeterinaria.SelectHemotropicosOrdenPorSesion(factura.IdSesion, item);
                foreach (var orden in ordenes)
                {

                    if (!HeaderImpreso)
                    {
                        page = document.AddPage();
                        page = HeaderGrup(page, factura.datosRepresentante, factura.finca, empresa, factura.IdSesion, DateTime.Now, factura.NumeroDia);

                        hemoparasitos = ConexionVeterinaria.selectHemoparasitosPorÉspecie(item);
                        fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
                        blueBrush = new XSolidBrush(XColors.Black);
                        color = new XColor { R = 105, G = 105, B = 105 };
                        pen = new XPen(color);
                        point = new XPoint(20, 70);
                        size = new XSize(580, 15);
                        Elipsesize = new XSize(5, 5);
                        rect = new XRect(point, size);
                        gfx = XGraphics.FromPdfPage(page);
                        tf = new XTextFormatter(gfx);

                        int PosicionX = 120;
                        int PosicionP = 95;
                        Margen = new XRect(230, PosicionP, 120, 14);
                        gfx.DrawString($"DESCARTE DE HEMOTROPICOS {ordenes.First().datosPacienteVet.especies.Descripcion}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("IDENTIFICACION ANIMAL", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("HEMOGLOBINA", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        PosicionP += 15;
                        Margen = new XRect(5, PosicionP, 120, 14);
                        gfx.DrawString("HEMATOCRITO", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        for (int z = 0; z <= hemoparasitos.Count - 1; z++)
                        {
                            PosicionP += 15;
                            Margen = new XRect(5, PosicionP, 120, 14);
                            Hemo hemo = hemoparasitos.ElementAt(z);
                            gfx.DrawString(hemo.Descripcion, fontRegular2, blueBrush, Margen, XStringFormats.Center);

                        }
                        point = new XPoint(10, 110);
                        size = new XSize(565, PosicionP - 95);
                        gfx.DrawLine(pen, PosicionX, 110, PosicionX, PosicionP + 15);
                        rect = new XRect(point, size);
                        gfx.DrawRectangle(pen, rect);


                        //gfx.DrawLine(pen, PosicionX, 110, PosicionX, 320);

                        PosicionP = 110;
                        for (int x = 1; x < 3 + hemoparasitos.Count; x++)
                        {
                            PosicionP += 15;
                            gfx.DrawLine(pen, 10, PosicionP, 575, PosicionP);
                        }

                        //Valores de Referencia
                        PosicionX = 120;
                        PosicionP = 140;
                        HeaderImpreso = true;
                    }


                    PosicionP = 110;
                    Margen = new XRect(PosicionY + 15, PosicionP, 120, 14);
                    gfx.DrawString($"{orden.datosPacienteVet.NombrePaciente}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                    if (!string.IsNullOrEmpty(orden.ResultadosAnalisis.Where(x => x.IdAnalisis == 357).FirstOrDefault().Comentario))
                    {
                        Observaciones += $" Paciente: {orden.datosPacienteVet.NombrePaciente} {orden.ResultadosAnalisis.Where(x => x.IdAnalisis == 357).FirstOrDefault().Comentario} \n";
                    }
                    foreach (var ValoresDeReferencia in orden.ResultadosAnalisis.Where(x => x.IdAnalisis == 256 || x.IdAnalisis == 257 || x.IdAnalisis == 259 || x.IdAnalisis == 261 || x.IdAnalisis == 263 || x.IdAnalisis == 267 || x.IdAnalisis == 269 || x.IdAnalisis == 273).OrderByDescending(n => n.IdAnalisis))
                    {
                        PosicionP += 15;
                        switch (ValoresDeReferencia.IdAnalisis)
                        {
                            case 269:
                                Margen = new XRect(PosicionY + 15, 125, 120, 14);
                                break;
                            case 267:
                                Margen = new XRect(PosicionY + 15, 140, 120, 14);
                                break;
                        }
                        gfx.DrawString($"{ValoresDeReferencia.ValorResultado}", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                    }
                    foreach (var hemo in orden.datosPacienteVet.especies.hemoParasitos)
                    {
                        PosicionP += 15;
                        Margen = new XRect(PosicionY + 18, PosicionP, 120, 14);
                        gfx.DrawString($"{hemo.Resultado}", fontRegular3, blueBrush, Margen, XStringFormats.Center);
                    }

                    PosicionY += 56;
                    gfx.DrawLine(pen, PosicionY + 50, 110, PosicionY + 50, PosicionP + 15);
                    if (i % 5 == 0 && i > 0 || ordenes.Last() == orden)
                    {
                        if (i % 5 > 0 || i % 5 < 0)
                        {
                            gfx.DrawLine(pen, 455, 110, 455, PosicionP + 15);
                        }
                        if (i < 5)
                        {
                            gfx.DrawLine(pen, 455, 110, 455, PosicionP + 15);
                        }
                        PosicionP = 110;
                        Margen = new XRect(460, PosicionP, 120, 14);

                        gfx.DrawString("VALORES DE REFERENCIA", fontRegular2, blueBrush, Margen, XStringFormats.Center);

                        foreach (var ValoresDeReferencia in orden.ResultadosAnalisis.Where(x => x.IdAnalisis == 256 || x.IdAnalisis == 257 || x.IdAnalisis == 259 || x.IdAnalisis == 261 || x.IdAnalisis == 263 || x.IdAnalisis == 267 || x.IdAnalisis == 269 || x.IdAnalisis == 273).OrderByDescending(n => n.IdAnalisis))
                        {
                            PosicionP += 15;
                            switch (ValoresDeReferencia.IdAnalisis)
                            {
                                case 269:
                                    Margen = new XRect(470, 125, 90, 14);
                                    break;
                                case 267:
                                    Margen = new XRect(470, 140, 90, 14);
                                    break;
                                case 256:
                                    Margen = new XRect(470, 155, 90, 14);
                                    break;
                                case 257:
                                    Margen = new XRect(470, 170, 90, 14);
                                    break;
                                case 259:
                                    Margen = new XRect(470, 185, 90, 14);
                                    break;
                                case 261:
                                    Margen = new XRect(470, 200, 90, 14);
                                    break;
                                case 263:
                                    Margen = new XRect(470, 215, 90, 14);
                                    break;
                                case 273:
                                    Margen = new XRect(470, 230, 90, 14);
                                    break;
                            }
                            gfx.DrawString($"{ValoresDeReferencia.ValorMenor} - {ValoresDeReferencia.ValorMayor} {ValoresDeReferencia.unidad}", fontRegular2, blueBrush, Margen, XStringFormats.Center);
                        }
                        Margen = new XRect(15, PosicionP += 70, 250, 90);
                        tf.DrawString(Observaciones, fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Usuarios Bioanalista = new Usuarios();
                        Bioanalista = orden.usuarios;
                        Margen = new XRect(302, PosicionP, 280, 14);
                        gfx.DrawString(string.Format("Analista"), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(300, PosicionP + 8, 250, 14);
                        gfx.DrawString(string.Format(" {0} CMVB:{1} MPPS:{2}", Bioanalista.NombreUsuario, Bioanalista.CB, Bioanalista.MPPS), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        XImage Logo = XImage.FromFile(string.Format(@"firma\{0}", Bioanalista.Firma));
                        gfx.DrawImage(Logo, 470, PosicionP );
                        PosicionY = 75;
                        gfx.Dispose();
                        HeaderImpreso = false;


                    }

                    i++;

                }



            }




            return document;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="datosRepresentante"></param>
        /// <param name="datosPacienteVet"></param>
        /// <param name="empresa"></param>
        /// <param name="IdOrden"></param>
        /// <returns></returns>
        private PdfSharp.Pdf.PdfPage Portada(PdfSharp.Pdf.PdfPage page, DatosRepresentante datosRepresentante, DatosPacienteVet datosPacienteVet, Empresa empresa, int IdOrden)
        {

            const string facename = "Arial Rounded MT";
            double y = 0;
            DateTime FechaImp = new DateTime();
            int MargenAncho = 15;
            double PosicionP = 90;
            double Posicion = 110;
            XBrush brushes;
            XRect Margen = new XRect(5, 0, 145, 14);


            XFont fontRegular = new XFont(facename, 11, XFontStyle.Regular);
            XFont Titulo = new XFont(Curva, 18, XFontStyle.Regular);
            XFont fontRegular2 = new XFont(facename, 9, XFontStyle.Regular);
            XFont Direccion = new XFont(facename, 8, XFontStyle.Regular);
            XFont DireccionCaratula = new XFont(Curva, 10, XFontStyle.Italic);
            XFont TituloCaratula = new XFont(Curva2, 12, XFontStyle.Regular);
            XFont Caratula = new XFont(Curva, 22, XFontStyle.Regular);
            XFont Cursiva = new XFont(Curva2, 10, XFontStyle.Italic);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            string Formato = "";

            XPoint point = new XPoint(50, 70);
            XSize size = new XSize(580, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            PosicionP = 90;
            //Logo
            XImage Logo = XImage.FromFile(string.Format(@"Logos\{0}", empresa.Ruta));
            gfx.DrawImage(Logo, 45, 65);
            //Linea
            point = new XPoint(10, 50);
            size = new XSize(580, 350);
            rect = new XRect(point, size);
            point = new XPoint(5, 10);
            size = new XSize(620, 350);
            rect = new XRect(point, size);
            XPoint Inicio = new XPoint(5, 40);
            XPoint Final = new XPoint(5, 40);
            gfx.DrawLine(pen, rect.X, 110, 585, 110);
            gfx.DrawLine(pen, rect.X, 250, 585, 250);
            gfx.DrawLine(pen, rect.X, 285, 585, 285);
            MargenAncho = 15;
            Posicion = 90;
            int PosicionY = 120;
            Margen = new XRect(50, PosicionY, 400, 60);
            Formato = string.Format("NUMERO DE ORDEN:");
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(210, PosicionY, 400, 60);
            Formato = string.Format("{0}", IdOrden);
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            PosicionY = PosicionY + 25;
            Margen = new XRect(50, PosicionY, 400, 60);
            Formato = string.Format("REPRESENTANTE: ");
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(210, PosicionY, 400, 60);
            Formato = string.Format("{0} {1}", datosRepresentante.NombreRepresentante, datosRepresentante.ApellidoRepresentante);
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            PosicionY = PosicionY + 25;
            Margen = new XRect(50, PosicionY, 400, 60);
            Formato = string.Format("RIF: ");
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(210, PosicionY, 400, 60);
            Formato = string.Format("{0}-{1}", datosRepresentante.TipoRepresentante, datosRepresentante.RIF);
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            PosicionY = PosicionY + 25;
            Margen = new XRect(50, PosicionY, 400, 60);
            Formato = string.Format("PACIENTE:");
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Formato = string.Format("{0}", datosPacienteVet.NombrePaciente);
            Margen = new XRect(210, PosicionY, 400, 60);
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            PosicionY = PosicionY + 25;
            Margen = new XRect(50, PosicionY, 400, 60);
            FechaImp = DateTime.Now;
            Formato = string.Format("FECHA:", FechaImp.ToString("dd/MM/yyyy"));
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(210, PosicionY, 400, 60);
            FechaImp = DateTime.Now;
            Formato = string.Format("{0}", FechaImp.ToString("dd/MM/yyyy"));
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(105, 75, 460, 120);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(empresa.Nombre.ToString(), Caratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(470, 95, 250, 48);
            tf.Alignment = XParagraphAlignment.Left;
            Formato = string.Format("RIF: {0}", empresa.Rif);
            tf.DrawString(Formato, Cursiva, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Formato = string.Format("DIRECCION: {0} TLF: {1} CORREO: {2}", empresa.Direccion, empresa.Telefono, empresa.Correo);
            Margen = new XRect(40, 255, 500, 48);
            tf.DrawString(Formato, DireccionCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            //gfx.DrawImage(Logo, 5, 10);

            return page;
        }
        private PdfSharp.Pdf.PdfPage PortadaGrupal(PdfSharp.Pdf.PdfPage page, DatosRepresentante datosRepresentante, Empresa empresa, Veterinario veterinario, Finca finca, int IdOrden)
        {

            const string facename = "Arial Rounded MT";
            double y = 0;
            DateTime FechaImp = new DateTime();
            int MargenAncho = 15;
            double PosicionP = 90;
            double Posicion = 110;
            XBrush brushes;
            XRect Margen = new XRect(5, 0, 145, 14);


            XFont fontRegular = new XFont(facename, 11, XFontStyle.Regular);
            XFont Titulo = new XFont(Curva, 18, XFontStyle.Regular);
            XFont fontRegular2 = new XFont(facename, 9, XFontStyle.Regular);
            XFont Direccion = new XFont(facename, 8, XFontStyle.Regular);
            XFont DireccionCaratula = new XFont(Curva, 10, XFontStyle.Italic);
            XFont TituloCaratula = new XFont(Curva2, 12, XFontStyle.Regular);
            XFont Caratula = new XFont(Curva, 22, XFontStyle.Regular);
            XFont Cursiva = new XFont(Curva2, 10, XFontStyle.Italic);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            string Formato = "";

            XPoint point = new XPoint(50, 70);
            XSize size = new XSize(580, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            PosicionP = 90;
            //Logo
            try
            {
                XImage Logo = XImage.FromFile(string.Format(@"Logos\{0}", empresa.Ruta));
                gfx.DrawImage(Logo, 45, 65);
            }
            catch (Exception)
            {

            }

            //Linea
            point = new XPoint(10, 50);
            size = new XSize(580, 350);
            rect = new XRect(point, size);
            point = new XPoint(5, 10);
            size = new XSize(620, 350);
            rect = new XRect(point, size);
            XPoint Inicio = new XPoint(5, 40);
            XPoint Final = new XPoint(5, 40);
            gfx.DrawLine(pen, rect.X, 110, 585, 110);

            MargenAncho = 15;
            Posicion = 90;
            int PosicionY = 120;
            Margen = new XRect(50, PosicionY, 400, 60);
            Formato = string.Format("NUMERO DE FACTURA:");
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(210, PosicionY, 400, 60);
            Formato = string.Format("{0}", IdOrden);
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            PosicionY = PosicionY + 20;


            Margen = new XRect(50, PosicionY, 400, 60);
            Formato = string.Format("REPRESENTANTE: ");
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(210, PosicionY, 400, 60);
            Formato = string.Format("{0} {1}", datosRepresentante.NombreRepresentante, datosRepresentante.ApellidoRepresentante);
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            PosicionY = PosicionY + 20;


            Margen = new XRect(50, PosicionY, 400, 60);
            Formato = string.Format("RIF: ");
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(210, PosicionY, 400, 60);
            Formato = string.Format("{0}-{1}", datosRepresentante.TipoRepresentante, datosRepresentante.RIF);
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            PosicionY = PosicionY + 20;


            Margen = new XRect(50, PosicionY, 400, 60);
            Formato = string.Format("FECHA:", FechaImp.ToString("dd/MM/yyyy"));
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(210, PosicionY, 400, 60);
            FechaImp = DateTime.Now;
            Formato = string.Format("{0}", FechaImp.ToString("dd/MM/yyyy"));
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            PosicionY = PosicionY + 20;

            Margen = new XRect(50, PosicionY, 400, 60);
            Formato = string.Format("Veterinario: ");
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(210, PosicionY, 400, 60);
            Formato = $"{veterinario.NombreVeterinario}";
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            PosicionY = PosicionY + 20;


            Margen = new XRect(50, PosicionY, 400, 60);
            Formato = string.Format("Finca: ");
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(210, PosicionY, 400, 60);
            Formato = $"{finca.NombreFinca}";
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            PosicionY = PosicionY + 20;


            Margen = new XRect(50, PosicionY, 400, 60);
            Formato = string.Format("Direccion: ");
            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(210, PosicionY, 400, 60);
            Formato = $"{finca.ciudad.ciudad} Edo. {finca.estado.estado} {finca.municipio.municipio} {finca.parroquia.parroquia}";
            PosicionY = PosicionY + 25;
            gfx.DrawLine(pen, rect.X, PosicionY, 585, PosicionY);



            tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(105, 75, 460, 120);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(empresa.Nombre, Caratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(470, 95, 250, 48);
            tf.Alignment = XParagraphAlignment.Left;



            Formato = string.Format("RIF: {0}", empresa.Rif);
            tf.DrawString(Formato, Cursiva, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Formato = string.Format("DIRECCION: {0} TLF: {1} CORREO: {2}", empresa.Direccion, empresa.Telefono, empresa.Correo);
            Margen = new XRect(40, PosicionY + 5, 500, 48);
            tf.DrawString(Formato, DireccionCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
            PosicionY = PosicionY + 30;
            gfx.DrawLine(pen, rect.X, PosicionY, 585, PosicionY);
            //gfx.DrawImage(Logo, 5, 10);
            gfx.Dispose();

            return page;
        }
        private PdfSharp.Pdf.PdfPage Header(PdfSharp.Pdf.PdfPage page, DatosRepresentante datosRepresentante, DatosPacienteVet datosPacienteVet, Empresa empresa, int IdOrden, DateTime Fecha)
        {
            const string facename = "Arial Rounded MT";
            double y = 0;
            DateTime FechaImp = new DateTime();
            int MargenAncho = 15;
            double PosicionP = 90;
            double Posicion = 110;
            XBrush brushes;
            XRect Margen = new XRect(5, 0, 145, 14);
            string Formato = "";
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            XPoint point = new XPoint(10, 70);
            XSize size = new XSize(570, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            PosicionP = 90;
            XFont fontRegular = new XFont(facename, 9, XFontStyle.Regular);
            XFont Titulo = new XFont(Curva, 18, XFontStyle.Regular);

            XFont Direccion = new XFont(facename, 8, XFontStyle.Regular);
            fontRegular = new XFont(facename, 10, XFontStyle.Regular);
            PosicionP = 90;
            Margen = new XRect(15, 12, 145, 14);

            tf.Alignment = XParagraphAlignment.Center;
            gfx.DrawString(empresa.Nombre, Titulo, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(15, 30, 250, 48);
            tf.Alignment = XParagraphAlignment.Left;
            Formato = string.Format("RIF: {0}\nDireccion:{1} TLF:{2} Correo:{3}", empresa.Rif, empresa.Direccion, empresa.Telefono, empresa.Correo);
            tf.DrawString(Formato, Direccion, XBrushes.Black, Margen, XStringFormats.TopLeft);
            //gfx.DrawImage(Logo, 5, 10);
            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
            point = new XPoint(500, 20);
            size = new XSize(80, 15);
            rect = new XRect(point, size);
            FechaImp = Convert.ToDateTime(Fecha);
            gfx.DrawString(string.Format("{0}", FechaImp.ToString("dd/MM/yyyy")), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
            point = new XPoint(500, 40);
            size = new XSize(80, 15);
            rect = new XRect(point, size);
            gfx.DrawString(IdOrden.ToString(), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
            MargenAncho = 15;
            Posicion = 110;
            PosicionP = 70;
            Margen = new XRect(15, PosicionP, 145, 14);
            gfx.DrawString(string.Format("Paciente:{0} Especie:{1}  Representante:{2} {3} RIF: {4}-{5}  ", datosPacienteVet.NombrePaciente, datosPacienteVet.especies.Descripcion, datosRepresentante.NombreRepresentante, datosRepresentante.ApellidoRepresentante, datosRepresentante.TipoRepresentante, datosRepresentante.RIF), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            gfx.Dispose();
            return page;
        }
        /// <summary>
        /// Header para Examenes Individuales como Hematologia,Copro,Orina,entre otros
        /// </summary>
        /// <param name="page">PDFSHARP page</param>
        /// <param name="datosRepresentante"> Clase DatosRepresentante</param>
        /// <param name="datosPacienteVet"> Clase Datos Paciente</param>
        /// <param name="empresa">Clase Empresa</param>
        /// <param name="IdOrden">Int Numero de Orden</param>
        /// <param name="Fecha">DateTime Fecha del examen</param>
        /// <returns> PDF Page con los datos del Representante Orden y Empresa y Paciente</returns>
        private PdfSharp.Pdf.PdfPage HeaderGrupal(PdfSharp.Pdf.PdfPage page, DatosRepresentante datosRepresentante, Empresa empresa, int IdOrden, DateTime Fecha)
        {
            const string facename = "Arial Rounded MT";
            double y = 0;
            DateTime FechaImp = new DateTime();
            int MargenAncho = 15;
            double PosicionP = 90;
            double Posicion = 110;
            XBrush brushes;
            XRect Margen = new XRect(5, 0, 145, 14);
            string Formato = "";
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            XPoint point = new XPoint(10, 70);
            XSize size = new XSize(570, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            PosicionP = 90;
            XFont fontRegular = new XFont(facename, 9, XFontStyle.Regular);
            XFont Titulo = new XFont(Curva, 18, XFontStyle.Regular);

            XFont Direccion = new XFont(facename, 8, XFontStyle.Regular);
            fontRegular = new XFont(facename, 10, XFontStyle.Regular);
            PosicionP = 90;
            Margen = new XRect(15, 12, 145, 14);

            tf.Alignment = XParagraphAlignment.Center;
            gfx.DrawString(empresa.Nombre, Titulo, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(15, 30, 250, 48);
            tf.Alignment = XParagraphAlignment.Left;
            Formato = string.Format("RIF: {0}\nDireccion:{1} TLF:{2} Correo:{3}", empresa.Rif, empresa.Direccion, empresa.Telefono, empresa.Correo);
            tf.DrawString(Formato, Direccion, XBrushes.Black, Margen, XStringFormats.TopLeft);
            //gfx.DrawImage(Logo, 5, 10);
            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
            point = new XPoint(500, 20);
            size = new XSize(80, 15);
            rect = new XRect(point, size);
            FechaImp = Convert.ToDateTime(Fecha);
            gfx.DrawString(string.Format("{0}", FechaImp.ToString("dd/MM/yyyy")), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
            point = new XPoint(500, 40);
            size = new XSize(80, 15);
            rect = new XRect(point, size);
            gfx.DrawString(IdOrden.ToString(), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
            MargenAncho = 15;
            Posicion = 110;
            PosicionP = 70;
            Margen = new XRect(15, PosicionP, 145, 14);
            gfx.DrawString(string.Format($"Representante:{datosRepresentante.NombreRepresentante} {datosRepresentante.ApellidoRepresentante} RIF: {datosRepresentante.TipoRepresentante}-{datosRepresentante.RIF} "), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            gfx.Dispose();
            return page;
        }
        /// <summary>
        /// Header Para Examenes Por Lote
        /// </summary>
        /// <param name="page">PDFSHARP page</param>
        /// <param name="datosRepresentante"> Clase DatosRepresentante</param>
        /// <param name="empresa">Clase Empresa</param>
        /// <param name="IdSesion">Int Numero de Sesion</param>
        /// <param name="Fecha">DateTime Fecha de la Sesion</param>
        /// <returns> PDFSHARP PAGE con el encabezado para LOTES AGRUPADOS</returns>
        private PdfSharp.Pdf.PdfPage HeaderGrup(PdfSharp.Pdf.PdfPage page, DatosRepresentante datosRepresentante, Finca finca, Empresa empresa, int IdSesion, DateTime Fecha, int NumeroDia)
        {
            const string facename = "Arial Rounded MT";
            double y = 0;
            DateTime FechaImp = new DateTime();
            int MargenAncho = 15;
            double PosicionP = 90;
            double Posicion = 110;
            XBrush brushes;
            XRect Margen = new XRect(5, 0, 145, 14);
            string Formato = "";
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            XPoint point = new XPoint(10, 70);
            XSize size = new XSize(570, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            PosicionP = 90;
            XFont fontRegular = new XFont(facename, 9, XFontStyle.Regular);
            XFont Titulo = new XFont(Curva, 18, XFontStyle.Regular);

            XFont Direccion = new XFont(facename, 8, XFontStyle.Regular);
            fontRegular = new XFont(facename, 10, XFontStyle.Regular);
            PosicionP = 90;
            Margen = new XRect(15, 12, 145, 14);

            tf.Alignment = XParagraphAlignment.Center;
            gfx.DrawString(empresa.Nombre, Titulo, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(15, 30, 250, 48);
            tf.Alignment = XParagraphAlignment.Left;
            Formato = string.Format("RIF: {0}\nDireccion:{1} TLF:{2} Correo:{3}", empresa.Rif, empresa.Direccion, empresa.Telefono, empresa.Correo);
            tf.DrawString(Formato, Direccion, XBrushes.Black, Margen, XStringFormats.TopLeft);
            //gfx.DrawImage(Logo, 5, 10);
            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
            point = new XPoint(500, 20);
            size = new XSize(80, 15);
            rect = new XRect(point, size);
            FechaImp = Convert.ToDateTime(Fecha);
            gfx.DrawString(string.Format("{0}", FechaImp.ToString("dd/MM/yyyy")), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
            point = new XPoint(500, 40);
            size = new XSize(80, 15);
            rect = new XRect(point, size);
            gfx.DrawString($"{FechaImp.ToString("ddMMyyyy")} - {NumeroDia.ToString()}", fontRegular, XBrushes.Black, rect, XStringFormats.Center);
            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
            MargenAncho = 15;
            Posicion = 110;
            PosicionP = 70;
            Margen = new XRect(15, PosicionP, 145, 14);
            string direccionFinca = $"Finca:{finca.NombreFinca} Ubicacion: {finca.ciudad.ciudad}, Edo. {finca.estado.estado}";
            gfx.DrawString(string.Format("Representante:{0} {1}   {2} ", datosRepresentante.NombreRepresentante, datosRepresentante.ApellidoRepresentante, direccionFinca), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            PosicionP += 15;
            Margen = new XRect(15, PosicionP, 145, 14);
            gfx.Dispose();
            return page;
        }
        /// <summary>
        /// Header Para Examenes de quimica
        /// </summary>
        /// <param name="page"></param>
        /// <param name="datosRepresentante"></param>
        /// <param name="datosPacienteVet"></param>
        /// <param name="empresa"></param>
        /// <param name="IdOrden"></param>
        /// <param name="Fecha"></param>
        /// <param name="Tipe"></param>
        /// <returns> PDFSHARP PAGE con todos los datos referentes al encabezado pero agrega al final las columnas Analisis, Resultado, Valores de Referencia</returns>
        private PdfSharp.Pdf.PdfPage Header(PdfSharp.Pdf.PdfPage page, DatosRepresentante datosRepresentante, DatosPacienteVet datosPacienteVet, Empresa empresa, int IdOrden, DateTime Fecha, int Tipe)
        {
            const string facename = "Arial Rounded MT";
            double y = 0;
            DateTime FechaImp = new DateTime();
            int MargenAncho = 15;
            double PosicionP = 90;
            double Posicion = 110;
            XBrush brushes;
            XRect Margen = new XRect(5, 0, 145, 14);
            string Formato = "";
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            XPoint point = new XPoint(10, 70);
            XSize size = new XSize(570, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            PosicionP = 90;
            XFont fontRegular = new XFont(facename, 9, XFontStyle.Regular);
            XFont Titulo = new XFont(Curva, 18, XFontStyle.Regular);

            XFont Direccion = new XFont(facename, 8, XFontStyle.Regular);
            fontRegular = new XFont(facename, 10, XFontStyle.Regular);
            PosicionP = 90;
            Margen = new XRect(15, 12, 145, 14);

            tf.Alignment = XParagraphAlignment.Center;
            gfx.DrawString(empresa.Nombre, Titulo, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(15, 30, 250, 48);
            tf.Alignment = XParagraphAlignment.Left;
            Formato = string.Format("RIF: {0}\nDireccion:{1} TLF:{2} Correo:{3}", empresa.Rif, empresa.Direccion, empresa.Telefono, empresa.Correo);
            tf.DrawString(Formato, Direccion, XBrushes.Black, Margen, XStringFormats.TopLeft);
            //gfx.DrawImage(Logo, 5, 10);
            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
            point = new XPoint(500, 20);
            size = new XSize(80, 15);
            rect = new XRect(point, size);
            FechaImp = Convert.ToDateTime(Fecha);
            gfx.DrawString(string.Format("{0}", FechaImp.ToString("dd/MM/yyyy")), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
            point = new XPoint(500, 40);
            size = new XSize(80, 15);
            rect = new XRect(point, size);
            gfx.DrawString(IdOrden.ToString(), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
            MargenAncho = 15;
            Posicion = 110;
            PosicionP = 70;
            Margen = new XRect(15, PosicionP, 145, 14);
            gfx.DrawString(string.Format("Paciente:{0} Especie:{1}  Representante:{2} {3} RIF: {4}-{5}  ", datosPacienteVet.NombrePaciente, datosPacienteVet.especies.Descripcion, datosRepresentante.NombreRepresentante, datosRepresentante.ApellidoRepresentante, datosRepresentante.TipoRepresentante, datosRepresentante.RIF), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            PosicionP = 95;
            Margen = new XRect(50, PosicionP, 100, 14);
            gfx.DrawString("Analisis", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
            PosicionP = 95;
            double YPalabras = 240;
            Margen = new XRect(YPalabras, PosicionP, 100, 14);
            gfx.DrawString("Resultados", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
            YPalabras = 400;
            Margen = new XRect(YPalabras, PosicionP, 135, 14);
            gfx.DrawString("Valores Normales", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
            YPalabras = 480;
            gfx.Dispose();
            return page;
        }

        private PdfSharp.Pdf.PdfPage Footer(PdfSharp.Pdf.PdfPage page)
        {
            const string facename = "Arial Rounded MT";
            double y = 0;
            y = 380;
            DateTime FechaImp = new DateTime();
            int MargenAncho = 15;
            double PosicionP = 90;
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            double Posicion = 400;
            XBrush brushes;
            XRect Margen = new XRect(5, 0, 145, 14);
            string Ruta = ConfigurationManager.ConnectionStrings["Probando"].ConnectionString;

            XFont fontRegular = new XFont(facename, 11, XFontStyle.Regular);
            XFont Titulo = new XFont(facename, 18, XFontStyle.Regular);
            XFont fontRegular4 = new XFont(facename, 9, XFontStyle.Regular);
            XFont Direccion = new XFont(facename, 8, XFontStyle.Regular);
            XFont DireccionCaratula = new XFont(facename, 10, XFontStyle.Italic);
            XFont TituloCaratula = new XFont(facename, 12, XFontStyle.Regular);
            XFont Caratula = new XFont(facename, 22, XFontStyle.Regular);
            XFont Cursiva = new XFont(facename, 10, XFontStyle.Italic);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            gfx.DrawLine(pen, new XPoint(5, 340), new XPoint(570, 340));
            foreach (var i in Bioanalistas)
            {

                Margen = new XRect(y, 345, 100, 60);
                string Bioanalista1 = string.Format("Analista. {0} CMVB:{1} MPPS:{2}", i.NombreUsuario, i.CB, i.MPPS);
                tf.Alignment = XParagraphAlignment.Center;
                tf.DrawString(Bioanalista1, fontRegular4, XBrushes.Black, Margen, XStringFormats.TopLeft);
                XImage Firma = XImage.FromFile(string.Format(@"Firma\{1}", Ruta, i.Firma));
                gfx.DrawImage(Firma, y + 105, 345);
                y = y - 160;
            }
            gfx.Dispose();
            Bioanalistas.Clear();
            return page;
        }
        private PdfSharp.Pdf.PdfPage Hematologia(PdfSharp.Pdf.PdfPage page, Ordenes ordenes)
        {
            const string facename = "Arial Rounded MT";
            double y = 0;
            DateTime FechaImp = new DateTime();
            int MargenAncho = 15;
            double PosicionP = 90;
            double Posicion = 110;
            XBrush brushes;
            XRect Margen = new XRect(5, 0, 145, 14);
            string Ruta = ConfigurationManager.ConnectionStrings["Probando"].ConnectionString;
            Usuarios Bioanalista = new Usuarios();
            XFont fontRegular = new XFont(facename, 10, XFontStyle.Regular);
            XFont Titulo = new XFont(Curva, 18, XFontStyle.Regular);
            XFont fontRegular2 = new XFont(facename, 9, XFontStyle.Regular);
            XFont Direccion = new XFont(facename, 8, XFontStyle.Regular);
            XFont DireccionCaratula = new XFont(Curva, 10, XFontStyle.Italic);
            XFont TituloCaratula = new XFont(Curva2, 12, XFontStyle.Regular);
            XFont Caratula = new XFont(Curva, 22, XFontStyle.Regular);
            XFont Cursiva = new XFont(Curva2, 10, XFontStyle.Italic);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            string Formato = "";
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            XPoint point = new XPoint(5, 70);
            XSize size = new XSize(580, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            point = new XPoint(5, 110);
            size = new XSize(580, 210);
            rect = new XRect(point, size);
            gfx.DrawRectangle(pen, rect);
            MargenAncho = 15;
            Posicion = 110;
            MargenAncho = 15;
            Posicion = 110;

            /*
             * Aqui se genera cada linea del marco interno
             en los impares marca la linea y en los pares el espaciado. por supuesto se puede colocar.
            */
            for (int i = 1; i < 8; i++)
            {
                if (i == 2)
                {
                    gfx.DrawLine(pen, 5, Posicion = Posicion + MargenAncho * 4, 585, Posicion);
                }
                else if (i == 4)
                {
                    gfx.DrawLine(pen, 5, Posicion = Posicion + MargenAncho * 4, 585, Posicion);
                }
                else
                {
                    gfx.DrawLine(pen, 5, Posicion = Posicion + MargenAncho, 585, Posicion);
                }

            }
            PosicionP = 90;
            Margen = new XRect(5, PosicionP, 250, 14);
            gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
            gfx.DrawString(string.Format("Hematologia Completa {0}", ordenes.datosPacienteVet.especies.Descripcion), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);

            Margen = new XRect(5, PosicionP, 145, 14);
            //ANALISIS
            PosicionP = 110;
            Margen = new XRect(5, PosicionP, 145, 14);
            gfx.DrawString("Analisis", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(10, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString("Leucocitos", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(10, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString("Hematíes", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(10, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString("Hemoglobina", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(10, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString("Hematocritos", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(10, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString("Diferencial Leucocitario", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(10, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString("Neutrófilos#", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(10, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString("Linfocitos#", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(10, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString("Monocitos#", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(10, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString("Eosinofilos#", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(10, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString("Plaquetas", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);



            ResultadosPorAnalisisVet Hematologia = new ResultadosPorAnalisisVet(),
              leucocitos = new ResultadosPorAnalisisVet(),
              neuunidad = new ResultadosPorAnalisisVet(),
              neuPor = new ResultadosPorAnalisisVet(),
              linunidad = new ResultadosPorAnalisisVet(),
               linPor = new ResultadosPorAnalisisVet(),
               Monounidad = new ResultadosPorAnalisisVet(),
               MonoPor = new ResultadosPorAnalisisVet(),
               Eosunidad = new ResultadosPorAnalisisVet(),
               EosPor = new ResultadosPorAnalisisVet(),
               Basounidad = new ResultadosPorAnalisisVet(),
               BasoPor = new ResultadosPorAnalisisVet(),
               hematocrito = new ResultadosPorAnalisisVet(),
               Hematies = new ResultadosPorAnalisisVet(),
               hemoglobina = new ResultadosPorAnalisisVet(),
               VCM = new ResultadosPorAnalisisVet(),
               CHCM = new ResultadosPorAnalisisVet(),
               HCM = new ResultadosPorAnalisisVet(),
               plaquetas = new ResultadosPorAnalisisVet();
            foreach (var Analisis in ordenes.ResultadosAnalisis)
            {

                switch (Analisis.IdAnalisis)
                {
                    case 55:
                        Hematologia = Analisis;
                        Analisis.PorImprimir = true;
                        Bioanalista = Analisis.bioanalista;
                        break;
                    case 256:
                        leucocitos = Analisis;
                        Analisis.PorImprimir = true;
                        break;
                    case 257:
                        neuunidad = Analisis;
                        Analisis.PorImprimir = true;
                        break;
                    case 258:
                        neuPor = Analisis;
                        Analisis.PorImprimir = true;
                        break;
                    case 259:
                        linunidad = Analisis;
                        Analisis.PorImprimir = true;
                        break;
                    case 260:
                        linPor = Analisis;
                        Analisis.PorImprimir = true;
                        break;
                    case 261:
                        Monounidad = Analisis;
                        Analisis.PorImprimir = true;
                        break;
                    case 262:
                        MonoPor = Analisis;
                        Analisis.PorImprimir = true;
                        break;
                    case 263:
                        Eosunidad = Analisis;
                        Analisis.PorImprimir = true;
                        break;
                    case 264:
                        EosPor = Analisis;
                        Analisis.PorImprimir = true;
                        break;
                    case 265:
                        Basounidad = Analisis;
                        Analisis.PorImprimir = true;
                        break;
                    case 266:
                        BasoPor = Analisis;
                        Analisis.PorImprimir = true;
                        break;
                    case 267:
                        hematocrito = Analisis;
                        Analisis.PorImprimir = true;
                        break;
                    case 268:
                        Hematies = Analisis;
                        Analisis.PorImprimir = true;
                        break;
                    case 269:
                        hemoglobina = Analisis;
                        Analisis.PorImprimir = true;
                        break;
                    case 270:
                        VCM = Analisis;
                        Analisis.PorImprimir = true;
                        break;
                    case 271:
                        HCM = Analisis;
                        Analisis.PorImprimir = true;
                        break;
                    case 272:
                        CHCM = Analisis;
                        Analisis.PorImprimir = true;
                        break;
                    case 273:
                        plaquetas = Analisis;
                        Analisis.PorImprimir = true;
                        break;

                }
            }


            PosicionP = 110;
            double YPalabras = 180;
            //Resultados
            Margen = new XRect(YPalabras, PosicionP, 145, 14);
            gfx.DrawString("Resultados", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString(leucocitos.ValorResultado, fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString(Hematies.ValorResultado, fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString(hemoglobina.ValorResultado, fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString(hematocrito.ValorResultado, fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString("Resultados", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);

            if (!string.IsNullOrEmpty(neuunidad.ValorResultado))
            {
                gfx.DrawString(neuunidad.ValorResultado, fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            }
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            if (!string.IsNullOrEmpty(linunidad.ValorResultado))
            {
                gfx.DrawString(linunidad.ValorResultado, fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            }
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            if (!string.IsNullOrEmpty(Monounidad.ValorResultado))
            {
                gfx.DrawString(Monounidad.ValorResultado, fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            }

            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            if (!string.IsNullOrEmpty(Eosunidad.ValorResultado))
            {
                gfx.DrawString(Eosunidad.ValorResultado, fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            }
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            if (!string.IsNullOrEmpty(plaquetas.ValorResultado))
            {
                gfx.DrawString($"   {plaquetas.ValorResultado}", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            }

            PosicionP = 185;
            YPalabras = 95;
            //Resultados Porcentaje
            Margen = new XRect(YPalabras, PosicionP, 145, 14);
            gfx.DrawString("%", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);

            if (!string.IsNullOrEmpty(neuPor.ValorResultado))
            {
                gfx.DrawString(string.Format("{0}%", neuPor.ValorResultado), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            }
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            if (!string.IsNullOrEmpty(linunidad.ValorResultado))
            {
                gfx.DrawString(string.Format("{0}%", Convert.ToDouble(linPor.ValorResultado), linunidad.unidad, linPor.ValorResultado), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            }
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            if (!string.IsNullOrEmpty(Monounidad.ValorResultado))
            {
                gfx.DrawString(string.Format("{0}%", Convert.ToDouble(MonoPor.ValorResultado), Monounidad.unidad, MonoPor.ValorResultado), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            }

            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            if (!string.IsNullOrEmpty(Eosunidad.ValorResultado))
            {
                gfx.DrawString(string.Format("{0}%", Convert.ToDouble(EosPor.ValorResultado), Eosunidad.unidad, EosPor.ValorResultado), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            }

            PosicionP = 110;
            YPalabras = 320;
            //unidades
            Margen = new XRect(YPalabras, PosicionP, 135, 14);
            //Blancos
            gfx.DrawString("Unidades", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString(leucocitos.unidad, fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString(Hematies.unidad, fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString(hemoglobina.unidad, fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString(hematocrito.unidad, fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            //Rojos
            gfx.DrawString("Unidades", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString(neuunidad.unidad, fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString(linunidad.unidad, fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString(Monounidad.unidad, fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString(Eosunidad.unidad, fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString(plaquetas.unidad, fontRegular, XBrushes.Black, Margen, XStringFormats.Center);

            PosicionP = 110;
            YPalabras = 450;
            //Valores Normales
            Margen = new XRect(YPalabras, PosicionP, 135, 14);
            gfx.DrawString("Valores Normales", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString($"{leucocitos.ValorMenor} - {leucocitos.ValorMayor}", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString($"{Hematies.ValorMenor} – {Hematies.ValorMayor}", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString($"{hemoglobina.ValorMenor} – {hemoglobina.ValorMayor}", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString($"{hematocrito.ValorMenor} – {hematocrito.ValorMayor}", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString("Valores Normales", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString($"{neuunidad.ValorMenor} – {neuunidad.ValorMayor} ", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString($"{linunidad.ValorMenor} – {linunidad.ValorMayor}", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString($"{Monounidad.ValorMenor} – {Monounidad.ValorMayor} ", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString($"{Eosunidad.ValorMenor} – {Eosunidad.ValorMayor} ", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString($"{plaquetas.ValorMenor}  – {plaquetas.ValorMayor}", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(220, PosicionP = PosicionP + 15, 135, 14);
            gfx.DrawString("Descarte de Hemotropicos", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            /*
             * Cargar Hemoparasitos
             */
            double PosicionHorizontal = 5;
            if (ordenes.datosPacienteVet.especies.hemoParasitos.Count > 0)
            {
                double CantidadDeHemoParasitos = 580 / ordenes.datosPacienteVet.especies.hemoParasitos.Count;
                double ultimoHemoparasito = CantidadDeHemoParasitos * (ordenes.datosPacienteVet.especies.hemoParasitos.Count - 1);
                foreach (var hemo in ordenes.datosPacienteVet.especies.hemoParasitos)
                {
                    if (ultimoHemoparasito < PosicionHorizontal)
                    {
                        CantidadDeHemoParasitos = 585 - PosicionHorizontal;
                        PosicionHorizontal = 585 - CantidadDeHemoParasitos;
                    }
                    string ResultadoHemo = hemo.Resultado;
                    Margen = new XRect(PosicionHorizontal, PosicionP + 15, CantidadDeHemoParasitos, 30);
                    gfx.DrawRectangle(pen, Margen);
                    rect = new XRect(PosicionHorizontal, PosicionP + 15, CantidadDeHemoParasitos, 20);
                    tf.Alignment = XParagraphAlignment.Center;
                    tf.DrawString(hemo.Descripcion, fontRegular, XBrushes.Black, rect, XStringFormats.TopLeft);
                    rect = new XRect(PosicionHorizontal, PosicionP + 30, CantidadDeHemoParasitos, 20);
                    tf.DrawString(hemo.Resultado, fontRegular, XBrushes.Black, rect, XStringFormats.TopLeft);
                    PosicionHorizontal += CantidadDeHemoParasitos;

                }
            }


            PosicionP = 320;
            Margen = new XRect(5, PosicionP, 380, 60);
            gfx.DrawRectangle(pen, Margen);
            Margen = new XRect(10, PosicionP + 5, 320, 60);
            fontRegular2 = new XFont(facename, 10, XFontStyle.Regular);
            rect = new XRect(10, PosicionP, 375, 60);
            tf.Alignment = XParagraphAlignment.Left;
            string text1 = "Observaciones: " + Hematologia.Comentario;
            tf.DrawString(text1, fontRegular2, XBrushes.Black, rect, XStringFormats.TopLeft);
            Margen = new XRect(385, PosicionP, 200, 60);
            gfx.DrawRectangle(pen, Margen);
            Margen = new XRect(385, PosicionP + 10, 50, 60);
            string text = string.Format("Analista \n {0} CMVB:{1} MPPS:{2}", Bioanalista.NombreUsuario, Bioanalista.CB, Bioanalista.MPPS);
            rect = new XRect(385, PosicionP + 5, 120, 40);
            tf.Alignment = XParagraphAlignment.Center;
            tf.DrawString(text, fontRegular, XBrushes.Black, rect, XStringFormats.TopLeft);
            XImage Firma = XImage.FromFile(string.Format(@"Firma\{1}", Ruta, Bioanalista.Firma));
            gfx.DrawImage(Firma, 520, PosicionP + 5);
            //point = new XPoint(Convert.ToDouble(textBox3.Text), Convert.ToDouble(textBox4.Text));
            //size = new XSize(Convert.ToDouble(textBox1.Text), Convert.ToDouble(textBox2.Text));
            // rect = new XRect(point, size);
            //gfx.DrawRectangle(pen, rect);

            return page;
        }
        private PdfSharp.Pdf.PdfPage Uroanaliss(PdfSharp.Pdf.PdfPage page, Ordenes ordenes)
        {
            Usuarios Bioanalista = new Usuarios();
            UroAnalisis Orina = new UroAnalisis();
            foreach (var Analisis in ordenes.ResultadosAnalisis)
            {
                switch (Analisis.IdAnalisis)
                {
                    case 42:
                        Orina.Comentario = Analisis.Comentario;
                        Analisis.PorImprimir = true;
                        Bioanalista = Analisis.bioanalista;
                        break;
                    case 275:
                        Orina.Color = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 276:
                        Orina.Aspecto = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 277:
                        Orina.TiraReactiva = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 278:
                        Orina.Glucosa = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 279:
                        Orina.Bilirrubina = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 280:
                        Orina.Nitritos = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 281:
                        Orina.Leucocitos = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 282:
                        Orina.Cetonas = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 283:
                        Orina.Olor = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 284:
                        Orina.Hemoglobina = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 285:
                        Orina.Urobilinogeno = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 286:
                        Orina.Benedict = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 287:
                        Orina.Proteinas = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 288:
                        Orina.Robert = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 289:
                        Orina.Bacterias = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 290:
                        Orina.LeucocitosMicro = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 291:
                        Orina.Hematies = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 292:
                        Orina.Mucina = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 293:
                        Orina.Ceplanas = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 294:
                        Orina.Cetransicion = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 295:
                        Orina.Credondas = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 296:
                        Orina.Blastoconidias = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 297:
                        Orina.Cristales = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 298:
                        Orina.Cilindros = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 299:
                        Orina.Ph = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                    case 300:
                        Orina.Densidad = Analisis.ValorResultado;
                        Analisis.PorImprimir = true;
                        break;
                }
            }
            const string facename = "Arial Rounded MT";
            double y = 0;
            DateTime FechaImp = new DateTime();
            int MargenAncho = 15;
            double PosicionP = 90;
            double Posicion = 110;
            XBrush brushes;
            XRect Margen = new XRect(5, 0, 145, 14);
            string Formato = "";
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            XPoint point = new XPoint(5, 70);
            XSize size = new XSize(580, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            PosicionP = 90;
            XFont fontRegular = new XFont(facename, 11, XFontStyle.Regular);
            XFont Titulo = new XFont(Curva, 18, XFontStyle.Regular);
            XFont fontRegular2 = new XFont(facename, 9, XFontStyle.Regular);
            XFont Direccion = new XFont(facename, 8, XFontStyle.Regular);
            XFont DireccionCaratula = new XFont(Curva, 10, XFontStyle.Italic);
            XFont TituloCaratula = new XFont(Curva2, 12, XFontStyle.Regular);
            XFont Caratula = new XFont(Curva, 22, XFontStyle.Regular);
            XFont Cursiva = new XFont(Curva2, 10, XFontStyle.Italic);
            fontRegular = new XFont(facename, 11, XFontStyle.Regular);
            PosicionP = 90;
            int MargenInicial = 15;
            int MargenDetalles = 0;
            Margen = new XRect(MargenInicial, PosicionP, 60, 15);
            gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
            gfx.DrawString("UroAnalisis", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(70 + MargenInicial, PosicionP, 117, 15);
            gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
            Margen = new XRect(72 + MargenInicial, PosicionP, 115, 15);
            gfx.DrawString(string.Format("Olor: {0}", Orina.Olor), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(192 + MargenInicial, PosicionP, 131, 15);
            gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
            Margen = new XRect(194 + MargenInicial, PosicionP, 125, 15);
            gfx.DrawString(string.Format("Color: {0}", Orina.Color), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(328 + MargenInicial, PosicionP, 80, 15);
            gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
            Margen = new XRect(330 + MargenInicial, PosicionP, 75, 15);
            gfx.DrawString(string.Format("Densidad: {0}", Orina.Densidad), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(413 + MargenInicial, PosicionP, 40, 15);
            gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
            Margen = new XRect(415 + MargenInicial, PosicionP, 35, 15);
            gfx.DrawString(string.Format("Ph: {0}", Orina.Ph), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(455 + MargenInicial, PosicionP, 115, 15);
            gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
            Margen = new XRect(460 + MargenInicial, PosicionP, 105, 15);
            gfx.DrawString(string.Format("Aspecto: {0}", Orina.Aspecto), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            PosicionP = 110;
            point = new XPoint(500, 20);
            size = new XSize(80, 15);
            rect = new XRect(point, size);
            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
            Margen = new XRect(10, 110, 150, 15);
            gfx.DrawString("Características Químicas", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(298 + MargenInicial, 110, 150, 15);
            gfx.DrawString("Examen Microscópico", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            point = new XPoint(5, 130);
            size = new XSize(287, 225);
            rect = new XRect(point, size);
            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
            point = new XPoint(298, 130);
            size = new XSize(287, 225);
            rect = new XRect(point, size);
            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
            MargenAncho = 15;
            Posicion = 130;
            for (int i = 1; i < 5; i++)
            {
                if (i == 2)
                {
                    gfx.DrawLine(pen, 5, Posicion = Posicion + MargenAncho * 9, 292, Posicion);
                }
                else if (i == 4)
                {
                    gfx.DrawLine(pen, 5, Posicion = Posicion + MargenAncho * 4, 292, Posicion);
                }
                else
                {
                    gfx.DrawLine(pen, 5, Posicion = Posicion + MargenAncho, 292, Posicion);
                }

            }
            Posicion = 130;
            Margen = new XRect(MargenInicial, Posicion, 95, 15);
            gfx.DrawString("Análisis", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(MargenInicial, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Proteínas", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(MargenInicial, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Hemoglobina", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(MargenInicial, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("C. Cetónicos", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(MargenInicial, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Bilirrubina", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(MargenInicial, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Leucocitos", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(MargenInicial, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Urobilinogeno", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(MargenInicial, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Nitritos", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(MargenInicial, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Robert", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(MargenInicial, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Glucosa", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(MargenInicial, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Observaciones", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            string Observaciones = "";
            string bastoconidias = Orina.Blastoconidias;
            string glucosa = Orina.Benedict;
            if (!(bastoconidias == " " || bastoconidias == ""))
            {
                Observaciones = "Blastoconidias: " + Orina.Blastoconidias + ";";
            }
            if (!(glucosa == " " || glucosa == ""))
            {
                Observaciones += " Benedict: " + Orina.Benedict + ";";
            }
            Observaciones += "  " + Orina.Comentario;
            Margen = new XRect(10, Posicion = Posicion + 15, 280, 60);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(Observaciones, fontRegular, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Posicion = 130;
            Margen = new XRect(100, Posicion, 95, 15);
            gfx.DrawString("Resultado", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(100, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString(Orina.Proteinas, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(100, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString(Orina.Hemoglobina, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(100, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString(Orina.Cetonas, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(100, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString(Orina.Bilirrubina, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(100, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString(Orina.Leucocitos, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(100, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString(Orina.Urobilinogeno, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(100, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString(Orina.Nitritos, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(100, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString(Orina.Robert, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(100, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString(Orina.Glucosa, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Posicion = 130;
            Margen = new XRect(175, 130, 95, 15);
            gfx.DrawString("Valores de Referencia", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(175, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Negativo", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(175, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Negativo", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(175, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Negativo", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(175, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Negativo", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(175, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Negativo", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(175, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("0.2 - 1.0", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Posicion = 130;
            int PosicionY = 450;
            Margen = new XRect(PosicionY, Posicion, 95, 15);
            gfx.DrawString("Resultado", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(PosicionY, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString(Orina.Cetransicion, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(PosicionY, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString(Orina.Ceplanas, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(PosicionY, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString(Orina.Credondas, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(PosicionY, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString(Orina.LeucocitosMicro, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(PosicionY, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString(Orina.Hematies, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(PosicionY, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString(Orina.Bacterias, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(PosicionY, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString(Orina.Mucina, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(PosicionY, Posicion = Posicion + 15, 95, 15);
            Posicion = 130;
            Margen = new XRect(500, Posicion, 95, 15);
            gfx.DrawString("Referencia", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(500, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("X CPO", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(500, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("X CPO", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(500, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("X CPO", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(500, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("X CPO", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(500, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("X CPO", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(500, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("X CPO", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
            Margen = new XRect(500, Posicion = Posicion + 15, 95, 15);

            Posicion = 130;
            Margen = new XRect(303, Posicion, 95, 15);
            gfx.DrawString("Análisis", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(303, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("C. Epiteliales Transición", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(303, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("C. Epiteliales Planas", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(303, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("C. Epiteliales Redondas", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(303, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Leucocitos", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(303, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Hematíes", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(303, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Bacterias", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(303, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Mucina", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(303, Posicion = Posicion + 15, 95, 15);
            gfx.DrawString("Cristales", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(303, Posicion = Posicion + 15, 280, 45);
            string Cristales = Orina.Cristales;
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(Cristales, fontRegular, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(303, Posicion = Posicion + 45, 95, 15);
            gfx.DrawString("Cilindros", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(303, Posicion = Posicion + 15, 280, 45);
            string Cilindros = Orina.Cilindros;
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(Cilindros, fontRegular, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(333, Posicion = Posicion + 30, 150, 45);
            string text = string.Format("Analista. {0} CMVB:{1} MPPS:{2}", Bioanalista.NombreUsuario, Bioanalista.CB, Bioanalista.MPPS);
            tf.Alignment = XParagraphAlignment.Center;
            tf.DrawString(text, fontRegular, XBrushes.Black, Margen, XStringFormats.TopLeft);
            XImage Firma = XImage.FromFile(string.Format(@"Firma\{1}", Ruta, Bioanalista.Firma));
            gfx.DrawImage(Firma, 500, Posicion + 5);
            MargenAncho = 15;
            Posicion = 130;
            for (int i = 1; i < 7; i++)
            {
                if (i == 2)
                {
                    gfx.DrawLine(pen, 298, Posicion = Posicion + MargenAncho * 7, 585, Posicion);
                }
                else if (i == 4)
                {
                    gfx.DrawLine(pen, 298, Posicion = Posicion + MargenAncho * 3, 585, Posicion);
                }
                else if (i == 6)
                {
                    gfx.DrawLine(pen, 298, Posicion = Posicion + MargenAncho * 2, 585, Posicion);
                }
                else
                {
                    gfx.DrawLine(pen, 298, Posicion = Posicion + MargenAncho, 585, Posicion);
                }

            }
            gfx.Dispose();
            return page;
        }
        private PdfSharp.Pdf.PdfPage copro(PdfSharp.Pdf.PdfPage page, Ordenes ordenes)
        {
            ResultadosPorAnalisisVet Willys = new ResultadosPorAnalisisVet();
            ResultadosPorAnalisisVet Directa = new ResultadosPorAnalisisVet();
            ResultadosPorAnalisisVet MacMaster = new ResultadosPorAnalisisVet();
            ResultadosPorAnalisisVet Sedi = new ResultadosPorAnalisisVet();
            Usuarios Bioanalista = new Usuarios();
            foreach (var items in ordenes.ResultadosAnalisis.OrderBy(x => x.IdOrganizador))
            {
                Bioanalista = items.bioanalista;
                switch (items.IdAnalisis)
                {
                    case 303:
                        Willys = items;
                        items.PorImprimir = true;
                        break;
                    case 304:
                        Directa = items;
                        items.PorImprimir = true;
                        break;
                    case 305:
                        MacMaster = items;
                        items.PorImprimir = true;
                        break;
                    case 306:
                        Sedi = items;
                        items.PorImprimir = true;
                        break;
                }

            }
            const string facename = "Arial Rounded MT";
            double y = 0;
            DateTime FechaImp = new DateTime();
            int MargenAncho = 15;
            double PosicionP = 90;
            double Posicion = 110;
            XBrush brushes;
            XRect Margen = new XRect(10, 0, 145, 14);
            string Ruta = ConfigurationManager.ConnectionStrings["Probando"].ConnectionString;

            XFont fontRegular = new XFont(facename, 11, XFontStyle.Regular);
            XFont Titulo = new XFont(Curva, 18, XFontStyle.Regular);
            XFont fontRegular2 = new XFont(facename, 9, XFontStyle.Regular);
            XFont Direccion = new XFont(facename, 8, XFontStyle.Regular);
            XFont DireccionCaratula = new XFont(Curva, 10, XFontStyle.Italic);
            XFont TituloCaratula = new XFont(Curva2, 12, XFontStyle.Regular);
            XFont Caratula = new XFont(Curva, 22, XFontStyle.Regular);
            XFont Cursiva = new XFont(Curva2, 10, XFontStyle.Italic);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            string Formato = "";
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            XPoint point = new XPoint(10, 70);
            XSize size = new XSize(570, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            point = new XPoint(500, 40);
            size = new XSize(80, 15);
            rect = new XRect(point, size);
            gfx.DrawRectangle(pen, rect);
            point = new XPoint(10, 110);
            size = new XSize(570, 225);
            rect = new XRect(point, size);
            gfx.DrawRectangle(pen, rect);
            MargenAncho = 15;
            Posicion = 110;
            MargenAncho = 15;

            //Nombre de Analisis
            PosicionP = 90;
            Margen = new XRect(10, PosicionP, 250, 14);
            gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
            gfx.DrawString(string.Format("Coproanalisis {0}", ordenes.datosPacienteVet.especies.Descripcion), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);


            /*
             * Aqui se genera cada linea del marco interno
             en los impares marca la linea y en los pares el espaciado. por supuesto se puede colocar.
            */
            Posicion = 110;
            for (int i = 1; i < 10; i++)
            {
                if (!(i % 2 == 0))
                {
                    gfx.DrawLine(pen, 10, Posicion = Posicion + MargenAncho, 575, Posicion);
                }
                if (i == 2)
                {
                    gfx.DrawLine(pen, 10, Posicion = Posicion + MargenAncho * 3, 575, Posicion);
                }
                else if (i == 4)
                {
                    gfx.DrawLine(pen, 10, Posicion = Posicion + MargenAncho, 575, Posicion);
                }

                else if (i == 6)
                {
                    gfx.DrawLine(pen, 10, Posicion = Posicion + MargenAncho * 3, 575, Posicion);
                }
                else if (i == 8)
                {
                    gfx.DrawLine(pen, 10, Posicion = Posicion + MargenAncho, 575, Posicion);
                }


            }
            PosicionP = 110;
            // Analisis y Valores
            // 
            ///

            if (ordenes.datosPacienteVet.especies.IdEspecie == 1 || ordenes.datosPacienteVet.especies.IdEspecie == 2 || ordenes.datosPacienteVet.especies.IdEspecie == 10 || ordenes.datosPacienteVet.especies.IdEspecie == 12)
            {

                //tenica de willys
                Margen = new XRect(170, PosicionP, 250, 14);

                tf = new XTextFormatter(gfx);
                gfx.DrawString(string.Format("TECNICA DE WILLYS EN {0}", ordenes.datosPacienteVet.especies.Descripcion), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                Margen = new XRect(20, PosicionP = PosicionP + MargenAncho, 540, 45);
                tf.DrawString(string.Format("{0}", Willys.ValorResultado), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                Margen = new XRect(170, PosicionP = PosicionP + MargenAncho * 3, 250, 14);
                gfx.DrawString(string.Format("OBSERVACIONES DE LA TECNICA", Willys.ValorResultado), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                Margen = new XRect(20, PosicionP = PosicionP + MargenAncho + 2, 540, 15);
                tf.DrawString(string.Format("{0}", Willys.Comentario), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);

                //tecnica directa y salina
                Margen = new XRect(170, PosicionP = PosicionP + MargenAncho, 250, 14);
                gfx.DrawString(string.Format("TECNICA DIRECTA Y SALINA EN {0}", ordenes.datosPacienteVet.especies.Descripcion), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                Margen = new XRect(20, PosicionP = PosicionP + MargenAncho, 540, 45);
                tf.DrawString(string.Format("{0}", Directa.ValorResultado), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                Margen = new XRect(170, PosicionP = PosicionP + MargenAncho * 3, 250, 14);
                gfx.DrawString(string.Format("OBSERVACIONES DE LA TECNICA", Willys.ValorResultado), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                Margen = new XRect(20, PosicionP = PosicionP + MargenAncho, 540, 15);
                tf.DrawString(string.Format("{0}", Directa.Comentario), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);

            }
            else
            {

                //tenica de willys
                Margen = new XRect(170, PosicionP, 250, 14);
                gfx.DrawString(string.Format("TECNICA DE MAC MASTER {0}", ordenes.datosPacienteVet.especies.Descripcion), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                Margen = new XRect(20, PosicionP = PosicionP + MargenAncho, 540, 45);
                tf.DrawString(string.Format("{0}", MacMaster.ValorResultado), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                Margen = new XRect(170, PosicionP = PosicionP + MargenAncho * 3, 250, 14);
                gfx.DrawString(string.Format("OBSERVACIONES DE LA TECNICA EN {0}", ordenes.datosPacienteVet.especies.Descripcion), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                Margen = new XRect(20, PosicionP = PosicionP + MargenAncho + 2, 540, 15);
                tf.DrawString(string.Format("{0}", MacMaster.Comentario), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);

                //tecnica directa y salina
                Margen = new XRect(150, PosicionP = PosicionP + MargenAncho, 250, 14);
                gfx.DrawString(string.Format("TECNICA DE SEDIMENTACION Y MATIZADO {0}", ordenes.datosPacienteVet.especies.Descripcion), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                Margen = new XRect(20, PosicionP = PosicionP + MargenAncho, 540, 45);
                tf.DrawString(string.Format("{0}", Sedi.ValorResultado), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                Margen = new XRect(170, PosicionP = PosicionP + MargenAncho * 3, 250, 14);
                gfx.DrawString(string.Format("OBSERVACIONES DE LA TECNICA EN {0}", ordenes.datosPacienteVet.especies.Descripcion), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                Margen = new XRect(20, PosicionP = PosicionP + MargenAncho, 540, 15);
                tf.DrawString(string.Format("{0}", Sedi.Comentario), fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
            }
            PosicionP = 275;
            Margen = new XRect(20, PosicionP = PosicionP + MargenAncho, 250, 14);
            gfx.DrawString(string.Format("Analista"), fontRegular, XBrushes.Black, Margen, XStringFormats.TopLeft);
            Margen = new XRect(20, PosicionP = PosicionP + 20, 250, 14);
            gfx.DrawString(string.Format(" {0} CMVB:{1} MPPS:{2}", Bioanalista.NombreUsuario, Bioanalista.CB, Bioanalista.MPPS), fontRegular, XBrushes.Black, Margen, XStringFormats.TopLeft);
            XImage Logo = XImage.FromFile(string.Format(@"firma\{0}", Bioanalista.Firma));
            gfx.DrawImage(Logo, PosicionP + 150, 307);
            gfx.Dispose();
            return page;
        }

        public static PdfSharp.Pdf.PdfPage HematologiaEspecial(PdfSharp.Pdf.PdfPage page, Ordenes ordenes)
        {
            return page;
        }
        private void CorreoConvenio(PdfSharp.Pdf.PdfDocument document, int IdOrden, int userId)
        {

            string path = @"C:\TemporalesPDF\";
            VerificarRuta(path);
            string filename;
            DataSet Empresa = new DataSet();
            DataSet ds1 = new DataSet();
            DataSet Paciente = ConexionVeterinaria.PacienteAImprimir(IdOrden);
            filename = string.Format(path + "{0} {1} {2}.pdf", "Jose ", "Bracamonte", DateTime.Now.ToString("ddMMyyyyhhmmss"));
            document.Save(filename);
            Empresa = ConexionVeterinaria.CorreoEmpresa();
            ds1 = ConexionVeterinaria.SELECTConvenioEmail(Paciente.Tables[0].Rows[0]["IdConvenio"].ToString());
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(Empresa.Tables[0].Rows[0]["Correo"].ToString());
            SmtpClient smtp = new SmtpClient();
            smtp.Port = Convert.ToInt32(Empresa.Tables[0].Rows[0]["Puerto"].ToString());
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(Empresa.Tables[0].Rows[0]["Correo"].ToString(), Empresa.Tables[0].Rows[0]["Clave"].ToString(), "");
            smtp.Host = "smtp-mail.outlook.com";

            //recipient
            mail.To.Add(new MailAddress(ds1.Tables[0].Rows[0]["CorreoSede"].ToString()));
            mail.Attachments.Add(new Attachment(filename));
            mail.IsBodyHtml = true;
            string st = "Examenes de Laboratorio";
            mail.Body = st;
            string UserState = "";
            smtp.Send(mail);
            string EnviadoPorCorreo = ConexionVeterinaria.EnviadoPorCorreo(IdOrden, userId);


        }
        private void CorreoPaciente(PdfSharp.Pdf.PdfDocument document, int IdOrden, int userId)
        {
            try
            {
                string path = @"C:\TemporalesPDF\";
                VerificarRuta(path);


                string filename;
                DataSet Empresa = new DataSet();
                DataSet ds1 = new DataSet();

                DataSet Paciente = ConexionVeterinaria.PacienteAImprimir(IdOrden);
                Empresa = ConexionVeterinaria.CorreoEmpresa();
                filename = string.Format(path + "{0} {1} {2}.pdf", Paciente.Tables[0].Rows[0]["NombreRepresentante"].ToString(), Paciente.Tables[0].Rows[0]["ApellidoRepresentante"].ToString(), DateTime.Now.ToString("ddMMyyyyhhmmss"));
                document.Save(filename);
                ds1 = ConexionVeterinaria.SELECTPersonaEmail(IdOrden.ToString());
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(Empresa.Tables[0].Rows[0]["Correo"].ToString());
                SmtpClient smtp = new SmtpClient();
                smtp.Port = Convert.ToInt32(Empresa.Tables[0].Rows[0]["Puerto"].ToString());
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(Empresa.Tables[0].Rows[0]["Correo"].ToString(), Empresa.Tables[0].Rows[0]["Clave"].ToString(), "");
                smtp.Host = "smtp-mail.outlook.com";
                string Correo = string.Format("{0}{1}", Paciente.Tables[0].Rows[0]["Correo"].ToString(), Paciente.Tables[0].Rows[0]["TipoCorreo"].ToString());
                //recipient
                mail.To.Add(new MailAddress(Correo));
                mail.Attachments.Add(new Attachment(filename));
                mail.IsBodyHtml = true;
                string st = "Examenes de Laboratorio";
                mail.Body = st;
                string UserState = "";
                smtp.Send(mail);
                string EnviadoPorCorreo = ConexionVeterinaria.EnviadoPorCorreo(Convert.ToInt32(IdOrden), userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


        }

        public void SendEmail(int IdOrden, int IdAnalisis, string Metodo, int userId)
        {
            ImpresionesVet impresiones = new ImpresionesVet();
            PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
            document = impresiones.Documento(IdOrden, IdAnalisis, Metodo, userId);
            try
            {
                if (Metodo == "Convenio")
                {

                    CorreoConvenio(document, IdOrden, userId);
                }

                else
                {
                    CorreoPaciente(document, IdOrden, userId);
                }
            }
            catch (Exception e)
            {
                // TODO: handle exception
            }


        }

        public void SendWhatsapp(int IdOrden, int IdAnalisis, string Metodo, int userId)
        {
            PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
            if (Metodo != "Grupo")
            {
                document = Documento(IdOrden, IdAnalisis, Metodo, userId);
            }
            else
            {
                Ordenes orden = ConexionVeterinaria.selectOrdenPorId(IdOrden);
                document = DocumentoPorLote(orden.IDSesion, "Correo");
            }
            string path = @"C:\Whatsapp\";
            VerificarRuta(path);
            string filename;
            DataSet Paciente = ConexionVeterinaria.PacienteAImprimir(IdOrden);
            filename = string.Format(path + $"{Paciente.Tables[0].Rows[0]["NombreRepresentante"].ToString()} {Paciente.Tables[0].Rows[0]["ApellidoRepresentante"].ToString()}.pdf");
            document.Save(filename);
            DataSet Empresa = new DataSet();
            Empresa = ConexionVeterinaria.SelectEmpresaActiva();
            string NombrePaciente = $"{Paciente.Tables[0].Rows[0]["NombreRepresentante"].ToString()} {Paciente.Tables[0].Rows[0]["ApellidoRepresentante"].ToString()}";
            string Telefono = "+58" + Paciente.Tables[0].Rows[0]["TipoCelular"].ToString() + Paciente.Tables[0].Rows[0]["Celular"].ToString();
            if (Telefono != "" && Telefono != " " && Telefono != null)
            {
                ProcessStartInfo SendWhatsapp = new ProcessStartInfo($"https://web.whatsapp.com/send?phone={Telefono}&text=Saludos%20cordiales%20{Paciente.Tables[0].Rows[0]["NombreRepresentante"].ToString()}%20{Paciente.Tables[0].Rows[0]["ApellidoRepresentante"].ToString()}%20de%20parte%20de%20{Empresa.Tables[0].Rows[0]["Nombre"].ToString()}%20Próximamente%20se%20les%20enviara%20sus%20resultados%20de%20laboratorio,%20si%20desea%20comunicarse%20con%20nosotros%20por%20favor%20llame%20a%20el%20siguiente%20número%20Tlf:%20{Empresa.Tables[0].Rows[0]["Telefono"].ToString()}");
                Process.Start(SendWhatsapp);
            }
        }
        public void SendWhatsapp(int IdSesion,List<Perfil> PerfilesABuscar)
        {
            PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();

            document = DocumentoPorLote( IdSesion,  PerfilesABuscar);
            string path = @"C:\Whatsapp\";
            VerificarRuta(path);
            string filename;
            DataSet Paciente = ConexionVeterinaria.PacienteAImprimirPorSesion(IdSesion);
            filename = string.Format(path + $"{Paciente.Tables[0].Rows[0]["NombreRepresentante"].ToString()} {Paciente.Tables[0].Rows[0]["ApellidoRepresentante"].ToString()}.pdf");
            document.Save(filename);
            DataSet Empresa = new DataSet();
            Empresa = ConexionVeterinaria.SelectEmpresaActiva();
            string NombrePaciente = $"{Paciente.Tables[0].Rows[0]["NombreRepresentante"].ToString()} {Paciente.Tables[0].Rows[0]["ApellidoRepresentante"].ToString()}";
            string Telefono = "+58" + Paciente.Tables[0].Rows[0]["TipoCelular"].ToString() + Paciente.Tables[0].Rows[0]["Celular"].ToString();
            if (Telefono != "" && Telefono != " " && Telefono != null)
            {
                ProcessStartInfo SendWhatsapp = new ProcessStartInfo($"https://web.whatsapp.com/send?phone={Telefono}&text=Saludos%20cordiales%20{Paciente.Tables[0].Rows[0]["NombreRepresentante"].ToString()}%20{Paciente.Tables[0].Rows[0]["ApellidoRepresentante"].ToString()}%20de%20parte%20de%20{Empresa.Tables[0].Rows[0]["Nombre"].ToString()}%20Próximamente%20se%20les%20enviara%20sus%20resultados%20de%20laboratorio,%20si%20desea%20comunicarse%20con%20nosotros%20por%20favor%20llame%20a%20el%20siguiente%20número%20Tlf:%20{Empresa.Tables[0].Rows[0]["Telefono"].ToString()}");
                Process.Start(SendWhatsapp);
            }
        }
    }
}