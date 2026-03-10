using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using Conexiones.DbConnect;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Conexiones.Impresion
{
    public class Impresiones
    {
        const string Curva = "Arial";
        const string Curva2 = "Arial";
        public static PdfSharp.Pdf.PdfDocument Documento(int IdOrden, int IdAnalisis, string Metodo)
        {
            string Ruta = ConfigurationManager.ConnectionStrings["Probando"].ConnectionString;
            bool impresion, primerapagina = false, cerrar;
            DataSet Paciente = new DataSet();
            DataSet dsPrint = new DataSet();
            PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
            try
            {
                List<int> Bio = new List<int>();
                List<int> IdAnalisisBio = new List<int>();
                bool bioencontrado = false, Portada = false;
                const string facename = "Arial";
                XFont fontRegular = new XFont(facename, 11, XFontStyle.Regular);
                XFont Titulo = new XFont(Curva, 18, XFontStyle.Regular);
                XFont fontRegular2 = new XFont(facename, 9, XFontStyle.Regular);
                XFont ObservacionesHeces = new XFont(facename, 7, XFontStyle.Regular);
                XFont Direccion = new XFont(facename, 8, XFontStyle.Regular);
                XFont DireccionCaratula = new XFont(Curva, 10, XFontStyle.Italic);
                XFont TituloCaratula = new XFont(Curva2, 12, XFontStyle.Regular);
                XFont Caratula = new XFont(Curva, 22, XFontStyle.Regular);
                XFont Cursiva = new XFont(Curva2, 10, XFontStyle.Italic);
                double PosicionP = 90;
                double Posicion = 110;
                XBrush brushes;
                double y = 0;
                DateTime FechaImp = new DateTime();
                int MargenAncho = 15;
                XRect Margen = new XRect(5, 0, 145, 14);
                PdfSharp.Pdf.PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XTextFormatter tf = new XTextFormatter(gfx);
                DataSet Empresa = new DataSet();
                string x = "1";
                Empresa = Conexion.SelectEmpresaActiva();
                if (Empresa.Tables[0].Rows[0]["IdEmpresa"].ToString() == "5")
                {
                    x = "0";
                }
                if (Metodo == "Correo" || Metodo == "Convenio" || Metodo == "Guardar")
                {
                    x = "1";
                }

                if (Metodo == "Seccion")
                {
                    DataSet dsPrint1 = new DataSet();
                    dsPrint1 = Conexion.SELECTAGRUPADOR(IdOrden.ToString(), IdAnalisis.ToString());
                    if (dsPrint1.Tables[0].Rows[0]["IdAgrupador"].ToString() != "")
                    {
                        dsPrint = Conexion.SELECTIMPRIMIRSECCIONAGRUPADOR(IdOrden.ToString(), dsPrint1.Tables[0].Rows[0]["IdAgrupador"].ToString());
                    }
                    else
                    {
                        dsPrint = Conexion.SELECTIMPRIMIRSECCION(IdOrden.ToString(), IdAnalisis.ToString());
                    }
                }
                else
                {
                    dsPrint = Conexion.SELECTIMPRIMIRTOTAL(IdOrden.ToString());
                }

                Paciente = Conexion.PacienteAImprimir(IdOrden);
                Empresa = new DataSet();
                if (Convert.ToInt32(Paciente.Tables[0].Rows[0]["IdConvenio"].ToString()) > 3 && Convert.ToInt32(Paciente.Tables[0].Rows[0]["IdConvenio"].ToString()) < 11)
                {
                    Empresa = Conexion.SelectEmpresaConvenio(Paciente.Tables[0].Rows[0]["IdConvenio"].ToString());
                }
                else
                {
                    Empresa = Conexion.SelectEmpresaActiva();
                }

                //XImage Logo = XImage.FromFile(string.Format(@"" + Ruta + @"\Logos\{0}", Empresa.Tables[0].Rows[0]["Ruta"].ToString()));
                foreach (DataRow r in dsPrint.Tables[0].Rows)
                {
                    if (!Portada)
                    {
                        XImage Logo = XImage.FromFile(string.Format(@"Logos\{0}", Empresa.Tables[0].Rows[0]["Ruta"].ToString()));
                        string Formato = "";
                        gfx.DrawImage(Logo, 5, 25);
                        XPoint point = new XPoint(50, 70);
                        XSize size = new XSize(580, 15);
                        XSize Elipsesize = new XSize(5, 5);
                        XRect rect = new XRect(point, size);
                        XColor color = new XColor { R = 105, G = 105, B = 105 };
                        XPen pen = new XPen(color);
                        PosicionP = 90;

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
                        Formato = string.Format("PACIENTE: ");
                        tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(210, PosicionY, 400, 60);
                        Formato = string.Format("{0} {1}", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString());
                        tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        PosicionY = PosicionY + 25;
                        Margen = new XRect(50, PosicionY, 400, 60);
                        Formato = string.Format("CEDULA: ");
                        tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(210, PosicionY, 400, 60);
                        Formato = string.Format("{0}", Paciente.Tables[0].Rows[0]["Cedula"].ToString());
                        tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        PosicionY = PosicionY + 25;
                        Margen = new XRect(50, PosicionY, 400, 60);
                        DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                        nacimiento = DateTime.Parse(dsPrint.Tables[0].Rows[0]["Fecha"].ToString());
                        string edad = Conexion.Fecha(nacimiento);
                        Formato = string.Format("EDAD:");
                        tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Formato = string.Format("{0}", edad);
                        Margen = new XRect(210, PosicionY, 400, 60);
                        tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        PosicionY = PosicionY + 25;
                        Margen = new XRect(50, PosicionY, 400, 60);
                        FechaImp = Convert.ToDateTime(dsPrint.Tables[0].Rows[0]["FechaImp"].ToString());
                        Formato = string.Format("FECHA:", FechaImp.ToString("dd/MM/yyyy"));
                        tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(210, PosicionY, 400, 60);
                        FechaImp = Convert.ToDateTime(dsPrint.Tables[0].Rows[0]["FechaImp"].ToString());
                        Formato = string.Format("{0}", FechaImp.ToString("dd/MM/yyyy"));
                        tf.DrawString(Formato, TituloCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(105, 75, 460, 120);
                        tf.Alignment = XParagraphAlignment.Left;
                        //tf.DrawString(Empresa.Tables[0].Rows[0]["Nombre"].ToString(), Caratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(470, 95, 250, 48);
                        tf.Alignment = XParagraphAlignment.Left;
                        Formato = string.Format("RIF: {0}", Empresa.Tables[0].Rows[0]["RIF"].ToString());
                        tf.DrawString(Formato, Cursiva, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Formato = string.Format("DIRECCION: {0} TLF: {1} CORREO: {2}", Empresa.Tables[0].Rows[0]["Direccion"].ToString(), Empresa.Tables[0].Rows[0]["Telefono"].ToString(), Empresa.Tables[0].Rows[0]["Correo"].ToString());
                        Margen = new XRect(40, 255, 500, 48);
                        tf.DrawString(Formato, DireccionCaratula, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        //gfx.DrawImage(Logo, 5, 10);

                        Portada = true;

                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        tf = new XTextFormatter(gfx);
                        Posicion = 110;

                    }
                    if (r["IdAnalisis"].ToString() == "55" && r["EstadoDeResultado"].ToString() != "" && r["EstadoDeResultado"].ToString() != " " && r["EstadoDeResultado"].ToString() != x)
                    {

                        //Hematologia
                        fontRegular = new XFont(facename, 11, XFontStyle.Regular);
                        PosicionP = 90;
                        Margen = new XRect(15, 12, 145, 14);
                        if (Metodo == "Correo")
                        {
                            page.Height = 400;
                        }
                        tf.Alignment = XParagraphAlignment.Center;
                        gfx.DrawString(Empresa.Tables[0].Rows[0]["Nombre"].ToString(), Titulo, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(15, 30, 250, 48);
                        tf.Alignment = XParagraphAlignment.Left;
                        string Formato = string.Format("RIF: {0}\nDireccion:{1} TLF:{2} Correo:{3}", Empresa.Tables[0].Rows[0]["RIF"].ToString(), Empresa.Tables[0].Rows[0]["Direccion"].ToString(), Empresa.Tables[0].Rows[0]["Telefono"].ToString(), Empresa.Tables[0].Rows[0]["Correo"].ToString());
                        tf.DrawString(Formato, Direccion, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        //gfx.DrawImage(Logo, 5, 10);
                        XColor color = new XColor { R = 105, G = 105, B = 105 };
                        XPen pen = new XPen(color);
                        XPoint point = new XPoint(5, 70);
                        XSize size = new XSize(580, 15);
                        XSize Elipsesize = new XSize(5, 5);
                        XRect rect = new XRect(point, size);
                        gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                        point = new XPoint(500, 20);
                        size = new XSize(80, 15);
                        rect = new XRect(point, size);
                        FechaImp = Convert.ToDateTime(dsPrint.Tables[0].Rows[0]["FechaImp"].ToString());
                        gfx.DrawString(string.Format("{0}", FechaImp.ToString("dd/MM/yyyy")), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
                        gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                        point = new XPoint(500, 40);
                        size = new XSize(80, 15);
                        rect = new XRect(point, size);
                        gfx.DrawString(IdOrden.ToString(), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
                        gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                        point = new XPoint(5, 110);
                        size = new XSize(580, 225);
                        rect = new XRect(point, size);
                        gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                        MargenAncho = 15;
                        Posicion = 110;

                        for (int i = 1; i < 6; i++)
                        {
                            if (i == 2)
                            {
                                gfx.DrawLine(pen, 5, Posicion = Posicion + MargenAncho * 4, 585, Posicion);
                            }
                            else if (i == 4)
                            {
                                gfx.DrawLine(pen, 5, Posicion = Posicion + MargenAncho * 8, 585, Posicion);
                            }
                            else
                            {
                                gfx.DrawLine(pen, 5, Posicion = Posicion + MargenAncho, 585, Posicion);
                            }

                        }

                        PosicionP = 70;
                        Margen = new XRect(15, PosicionP, 145, 14);
                        DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                        nacimiento = DateTime.Parse(dsPrint.Tables[0].Rows[0]["Fecha"].ToString());
                        gfx.DrawString(string.Format("Paciente:{0} {1}       C.I: {2}        Edad: {3}", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString(), Paciente.Tables[0].Rows[0]["Cedula"].ToString(), Conexion.Fecha(nacimiento)), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        PosicionP = 90;
                        Margen = new XRect(5, PosicionP, 145, 14);
                        gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
                        gfx.DrawString("Hematologia Completa", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        PosicionP = 110;
                        Margen = new XRect(5, PosicionP, 145, 14);
                        //ANALISIS

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
                        gfx.DrawString("Neutrófilos%", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("Linfocitos%", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("Monocitos%", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("Eosinofilos%", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
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

                        PosicionP = 110;
                        double YPalabras = 180;
                        //Resultados
                        Hematologia hematologia = new Hematologia();
                        hematologia = Conexion.Hematologia(Convert.ToInt32(IdOrden.ToString()), 55);
                        Margen = new XRect(YPalabras, PosicionP, 145, 14);
                        gfx.DrawString("Resultados", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString(string.Format("{0:0.00}", Convert.ToDouble(hematologia.leucocitos)), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        if (!string.IsNullOrEmpty(hematologia.Hematies))
                        {
                            gfx.DrawString(string.Format("{0:0.00}", Convert.ToDouble(hematologia.Hematies)), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        }
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString(string.Format("{0:0.00}", Convert.ToDouble(hematologia.Hemoglobina)), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString(string.Format("{0:0.00}", Convert.ToDouble(hematologia.Hematocritos)), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("Resultados", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        if (hematologia.Neutrofilos != "" && hematologia.Neutrofilos != " ")
                        {
                            gfx.DrawString(string.Format("{0:0.00}%", Convert.ToDouble(hematologia.Neutrofilos)), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        }
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        if (hematologia.linfocitos != "" && hematologia.linfocitos != " ")
                        {
                            gfx.DrawString(string.Format("{0:0.00}%", Convert.ToDouble(hematologia.linfocitos)), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        }
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        if (hematologia.Monocitos != "" && hematologia.Monocitos != " ")
                        {
                            gfx.DrawString(string.Format("{0:0.00}%", Convert.ToDouble(hematologia.Monocitos)), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        }
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        if (hematologia.Eosinofilos != "" && hematologia.Eosinofilos != " ")
                        {
                            gfx.DrawString(string.Format("{0:0.00}%", Convert.ToDouble(hematologia.Eosinofilos)), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        }
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        if (hematologia.Neutrofilos2 != "" && hematologia.Neutrofilos2 != " ")
                        {
                            gfx.DrawString(string.Format("{0:0.00}  10^3/ul", Convert.ToDouble(hematologia.Neutrofilos2)), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        }
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        if (hematologia.Linfocitos2 != "" && hematologia.Linfocitos2 != " ")
                        {
                            gfx.DrawString(string.Format("{0:0.00}  10^3/ul", Convert.ToDouble(hematologia.Linfocitos2)), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        }
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        if (hematologia.Monocitos2 != "" && hematologia.Monocitos2 != " ")
                        {
                            gfx.DrawString(string.Format("{0:0.00}  10^3/ul", Convert.ToDouble(hematologia.Monocitos2)), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        }

                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        if (hematologia.Eosinofilos2 != "" && hematologia.Eosinofilos2 != " ")
                        {
                            gfx.DrawString(string.Format("{0:0.00}  10^3/ul", Convert.ToDouble(hematologia.Eosinofilos2)), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        }
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        if (hematologia.Plaquetas != "" && hematologia.Plaquetas != " ")
                        {
                            gfx.DrawString(string.Format("{0:0.00}  10^3/ul", Convert.ToDouble(hematologia.Plaquetas)), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        }

                        PosicionP = 110;
                        YPalabras = 320;
                        //Unidades
                        Margen = new XRect(YPalabras, PosicionP, 135, 14);
                        gfx.DrawString("Unidades", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("10^6/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("gr/dL", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("%", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("Unidades", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("%", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("%", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("%", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("%", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);

                        PosicionP = 110;
                        YPalabras = 460;
                        //Valores Normales
                        Margen = new XRect(YPalabras, PosicionP, 135, 14);
                        gfx.DrawString("Valores Normales", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("4.0 – 10.0 10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("3.5 – 5.5 10^6/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("11.0 – 16.0 gr/dL", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("35.0 – 54.0 %", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("Valores Normales", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("40.0% – 70.0 %", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("18.0% – 48.0 %", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("3.0% 0 – 12.0%", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("1.0% 0 – 7.0 %", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("2.0 – 7.0 10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("1.1 – 2.9 10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("0.12 – 1.2 10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("0.02 – 0.50  10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                        gfx.DrawString("150 – 450 10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        PosicionP = 335;
                        Margen = new XRect(5, PosicionP, 320, 60);
                        gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
                        Margen = new XRect(10, PosicionP + 5, 320, 60);
                        fontRegular2 = new XFont(facename, 10, XFontStyle.Regular);
                        gfx.DrawString("Observaciones", fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        rect = new XRect(10, PosicionP + 15, 300, 40);
                        tf.Alignment = XParagraphAlignment.Left;
                        string text1 = hematologia.Comentario;
                        tf.DrawString(text1, fontRegular2, XBrushes.Black, rect, XStringFormats.TopLeft);
                        //Seleccionar BIoanalista
                        DataSet Bioanalista = new DataSet();
                        Bioanalista = Conexion.Bioanalista(IdOrden, 55);
                        string text = string.Format("LCD. {0} CB:{1} MPPS:{2}", Bioanalista.Tables[0].Rows[0]["NombreUsuario"].ToString(), Bioanalista.Tables[0].Rows[0]["CB"].ToString(), Bioanalista.Tables[0].Rows[0]["MPPS"].ToString());
                        //rectangulo de Comentario
                        Margen = new XRect(325, PosicionP, 260, 60);
                        gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
                        Margen = new XRect(330, PosicionP + 10, 50, 60);
                        rect = new XRect(330, PosicionP + 5, 150, 40);
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString(text, fontRegular, XBrushes.Black, rect, XStringFormats.TopLeft);
                        XImage Firma = XImage.FromFile(string.Format(@"Firma\{1}", Ruta, Bioanalista.Tables[0].Rows[0]["FIRMA"].ToString()));
                        gfx.DrawImage(Firma, 500, PosicionP + 5);
                        //point = new XPoint(Convert.ToDouble(textBox3.Text), Convert.ToDouble(textBox4.Text));
                        //size = new XSize(Convert.ToDouble(textBox1.Text), Convert.ToDouble(textBox2.Text));
                        // rect = new XRect(point, size);
                        //gfx.DrawRectangle(pen, rect);
                        if (r["EstadoDeResultado"].ToString() != "1" && Metodo != "Correo" && Metodo != "Convenio")
                        {
                            Conexion.ActualizarOrden("EstadoDeResultado = 3", Convert.ToInt32(IdOrden), Convert.ToInt32(r["IdAnalisis"].ToString()));
                        }

                        if (r != dsPrint.Tables[0].Rows[dsPrint.Tables[0].Rows.Count - 1])
                        {
                            page = document.AddPage();
                            gfx = XGraphics.FromPdfPage(page);
                            tf = new XTextFormatter(gfx);
                            Posicion = 110;
                        }
                        else
                        {
                            return document;
                        }
                    }
                    else if (r["IdAnalisis"].ToString() == "42" && r["EstadoDeResultado"].ToString() != "" && r["EstadoDeResultado"].ToString() != " " && r["EstadoDeResultado"].ToString() != x)
                    {
                        DataSet Print1 = new DataSet();
                        Print1 = Conexion.Orina(Convert.ToInt32(IdOrden.ToString()), 42);
                        if (Metodo == "Correo")
                        {
                            page.Height = 400;
                        }
                        fontRegular = new XFont(facename, 10, XFontStyle.Regular);
                        PosicionP = 90;
                        Margen = new XRect(15, 12, 145, 14);
                        tf.Alignment = XParagraphAlignment.Center;
                        Empresa = new DataSet();
                        if (Convert.ToInt32(Paciente.Tables[0].Rows[0]["IdConvenio"].ToString()) > 3 && Convert.ToInt32(Paciente.Tables[0].Rows[0]["IdConvenio"].ToString()) < 11)
                        {
                            Empresa = Conexion.SelectEmpresaConvenio(Paciente.Tables[0].Rows[0]["IdConvenio"].ToString());

                        }
                        else
                        {
                            Empresa = Conexion.SelectEmpresaActiva();
                        }
                        // Logo = XImage.FromFile(string.Format(@"" + Ruta + @"\Logos\{0}", Empresa.Tables[0].Rows[0]["Ruta"].ToString()));
                        //gfx.DrawImage(Logo, 5, 1);
                        gfx.DrawString(Empresa.Tables[0].Rows[0]["Nombre"].ToString(), Titulo, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(15, 30, 250, 48);
                        tf.Alignment = XParagraphAlignment.Left;
                        string Formato = string.Format("RIF: {0}\nDireccion:{1} TLF:{2} Correo:{3}", Empresa.Tables[0].Rows[0]["RIF"].ToString(), Empresa.Tables[0].Rows[0]["Direccion"].ToString(), Empresa.Tables[0].Rows[0]["Telefono"].ToString(), Empresa.Tables[0].Rows[0]["Correo"].ToString());
                        tf.DrawString(Formato, Direccion, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        XColor color = new XColor { R = 105, G = 105, B = 105 };
                        XPen pen = new XPen(color);
                        XPoint point = new XPoint(5, 70);
                        XSize size = new XSize(580, 15);
                        XSize Elipsesize = new XSize(5, 5);
                        XRect rect = new XRect(point, size);
                        DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                        Margen = new XRect(5, PosicionP, 60, 15);
                        rect = new XRect(point, size);
                        nacimiento = DateTime.Parse(Paciente.Tables[0].Rows[0]["Fecha"].ToString());
                        gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                        point = new XPoint(15, 70);
                        rect = new XRect(point, size);
                        gfx.DrawString(string.Format("Paciente:{0} {1}       C.I: {2}        Edad: {3}", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString(), Paciente.Tables[0].Rows[0]["Cedula"].ToString(), Conexion.Fecha(nacimiento)), fontRegular, XBrushes.Black, rect, XStringFormats.CenterLeft);
                        Margen = new XRect(5, PosicionP, 60, 15);
                        gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
                        gfx.DrawString("UroAnalisis", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(70, PosicionP, 117, 15);
                        gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
                        Margen = new XRect(72, PosicionP, 115, 15);
                        gfx.DrawString(string.Format("Olor: {0}", Print1.Tables[0].Rows[0]["Olor"].ToString()), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(192, PosicionP, 131, 15);
                        gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
                        Margen = new XRect(194, PosicionP, 125, 15);
                        gfx.DrawString(string.Format("Color: {0}", Print1.Tables[0].Rows[0]["Color"].ToString()), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(328, PosicionP, 75, 15);
                        gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
                        Margen = new XRect(330, PosicionP, 75, 15);
                        gfx.DrawString(string.Format("Densidad: {0}", Print1.Tables[0].Rows[0]["Densidad"].ToString()), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(408, PosicionP, 40, 15);
                        gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
                        Margen = new XRect(410, PosicionP, 35, 15);
                        gfx.DrawString(string.Format("Ph: {0}", Print1.Tables[0].Rows[0]["Ph"].ToString()), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(453, PosicionP, 108, 15);
                        gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
                        Margen = new XRect(455, PosicionP, 105, 15);
                        gfx.DrawString(string.Format("Aspecto: {0}", Print1.Tables[0].Rows[0]["Aspecto"].ToString()), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        PosicionP = 110;
                        point = new XPoint(500, 20);
                        size = new XSize(80, 15);
                        rect = new XRect(point, size);
                        FechaImp = Convert.ToDateTime(dsPrint.Tables[0].Rows[0]["FechaImp"].ToString());
                        gfx.DrawString(string.Format("{0}", FechaImp.ToString("dd/MM/yyyy")), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
                        gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                        point = new XPoint(500, 40);
                        size = new XSize(80, 15);
                        rect = new XRect(point, size);
                        gfx.DrawString(IdOrden.ToString(), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
                        gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                        Margen = new XRect(5, 110, 150, 15);
                        gfx.DrawString("Características Químicas", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(298, 110, 150, 15);
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
                        Margen = new XRect(10, Posicion, 95, 15);
                        gfx.DrawString("Análisis", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Proteínas", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Hemoglobina", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("C. Cetónicos", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Bilirrubina", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Leucocitos", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Urobilinogeno", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Nitritos", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Robert", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Glucosa", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Observaciones", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        string Observaciones = "";
                        string bastoconidias = Print1.Tables[0].Rows[0]["BLASTOCONIDAS"].ToString();
                        string glucosa = Print1.Tables[0].Rows[0]["Benedict"].ToString(); ;
                        if (!(bastoconidias == " " || bastoconidias == ""))
                        {
                            Observaciones = "Blastoconidias: " + Print1.Tables[0].Rows[0]["BLASTOCONIDAS"].ToString() + ";";
                        }
                        if (!(glucosa == " " || glucosa == ""))
                        {
                            Observaciones += " Benedict: " + Print1.Tables[0].Rows[0]["Benedict"].ToString() + ";";
                        }
                        Observaciones += "  " + Print1.Tables[0].Rows[0]["COMENTARIO"].ToString();
                        Margen = new XRect(10, Posicion = Posicion + 15, 280, 60);
                        tf.Alignment = XParagraphAlignment.Left;
                        tf.DrawString(Observaciones, fontRegular, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Posicion = 130;
                        Margen = new XRect(100, Posicion, 95, 15);
                        gfx.DrawString("Resultado", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(100, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["PROTEINAS"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(100, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Hemoglobina"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(100, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["cetonas"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(100, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Bilirrubina"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(100, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Leucocitos"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(100, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Urobilinogeno"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(100, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Nitritos"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(100, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Robert"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(100, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["glucosa"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
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
                        Margen = new XRect(420, Posicion, 95, 15);
                        gfx.DrawString("Resultado", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(420, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["CETRANSICION"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(420, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["CEPLANAS"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(420, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["CREDONDAS"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(420, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["LEUCOCITOSMICRO"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(420, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["HEMATIES"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(420, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["BACTERIAS"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(420, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["MUCINA"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(420, Posicion = Posicion + 15, 95, 15);
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
                        string Cristales = Print1.Tables[0].Rows[0]["Cristales"].ToString();
                        tf.Alignment = XParagraphAlignment.Left;
                        tf.DrawString(Cristales, fontRegular, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(303, Posicion = Posicion + 45, 95, 15);
                        gfx.DrawString("Cilindros", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(303, Posicion = Posicion + 15, 280, 45);
                        string Cilindros = Print1.Tables[0].Rows[0]["CILINDROS"].ToString();
                        DataSet Bioanalista = new DataSet();
                        Bioanalista = Conexion.Bioanalista(IdOrden, 42);
                        tf.Alignment = XParagraphAlignment.Left;
                        tf.DrawString(Cilindros, fontRegular, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(333, Posicion = Posicion + 30, 150, 45);
                        string text = string.Format("LCD. {0} CB:{1} MPPS:{2}", Bioanalista.Tables[0].Rows[0]["NombreUsuario"].ToString(), Bioanalista.Tables[0].Rows[0]["CB"].ToString(), Bioanalista.Tables[0].Rows[0]["MPPS"].ToString());
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString(text, fontRegular, XBrushes.Black, Margen, XStringFormats.TopLeft);

                        XImage Firma = XImage.FromFile(string.Format(@"Firma\{1}", Ruta, Bioanalista.Tables[0].Rows[0]["FIRMA"].ToString()));
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
                        if (r["EstadoDeResultado"].ToString() != "1" && Metodo != "Correo" && Metodo != "Convenio")
                        {
                            Conexion.ActualizarOrden("EstadoDeResultado = 3", Convert.ToInt32(IdOrden), Convert.ToInt32(r["IdAnalisis"].ToString()));
                        }

                        if (r != dsPrint.Tables[0].Rows[dsPrint.Tables[0].Rows.Count - 1])
                        {
                            int index = dsPrint.Tables[0].Rows.IndexOf(r) + 1;
                            if (dsPrint.Tables[0].Rows[index]["EstadoDeResultado"].ToString() != "" && dsPrint.Tables[0].Rows[index]["EstadoDeResultado"].ToString() != "1")
                            {
                                page = document.AddPage();
                                gfx = XGraphics.FromPdfPage(page);
                                tf = new XTextFormatter(gfx);
                                Posicion = 110;
                            }
                            else
                            {
                                return document;
                            }
                        }
                    }
                    else if (r["IdAnalisis"].ToString() == "12" && r["EstadoDeResultado"].ToString() != "" && r["EstadoDeResultado"].ToString() != " " && r["EstadoDeResultado"].ToString() != x)
                    {
                        DataSet Print1 = new DataSet();
                        Print1 = Conexion.Heces(Convert.ToInt32(IdOrden.ToString()), 12);
                        if (Metodo == "Correo")
                        {
                            page.Height = 400;
                        }
                        fontRegular = new XFont(facename, 10, XFontStyle.Regular);
                        fontRegular2 = new XFont(facename, 9, XFontStyle.Regular);
                        PosicionP = 90;
                        Margen = new XRect(15, 12, 145, 14);
                        tf.Alignment = XParagraphAlignment.Center;
                        Empresa = new DataSet();
                        if (Convert.ToInt32(Paciente.Tables[0].Rows[0]["IdConvenio"].ToString()) <= 3)
                        {
                            Empresa = Conexion.SelectEmpresaActiva();
                        }
                        else
                        {
                            Empresa = Conexion.SelectEmpresaConvenio(Paciente.Tables[0].Rows[0]["IdConvenio"].ToString());
                        }
                        // Logo = XImage.FromFile(string.Format(@"" + Ruta + @"\Logos\{0}", Empresa.Tables[0].Rows[0]["Ruta"].ToString()));
                        //gfx.DrawImage(Logo, 5, 1);
                        gfx.DrawString(Empresa.Tables[0].Rows[0]["Nombre"].ToString(), Titulo, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(15, 30, 250, 48);
                        tf.Alignment = XParagraphAlignment.Left;
                        string Formato = string.Format("RIF: {0}\nDireccion:{1} TLF:{2} Correo:{3}", Empresa.Tables[0].Rows[0]["RIF"].ToString(), Empresa.Tables[0].Rows[0]["Direccion"].ToString(), Empresa.Tables[0].Rows[0]["Telefono"].ToString(), Empresa.Tables[0].Rows[0]["Correo"].ToString());
                        tf.DrawString(Formato, Direccion, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        XColor color = new XColor { R = 105, G = 105, B = 105 };
                        XPen pen = new XPen(color);
                        XPoint point = new XPoint(5, 70);
                        XSize size = new XSize(580, 15);
                        XSize Elipsesize = new XSize(5, 5);
                        XRect rect = new XRect(point, size);
                        DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                        Margen = new XRect(5, PosicionP, 60, 15);
                        rect = new XRect(point, size);
                        nacimiento = DateTime.Parse(Paciente.Tables[0].Rows[0]["Fecha"].ToString());
                        gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                        point = new XPoint(15, 70);
                        rect = new XRect(point, size);
                        gfx.DrawString(string.Format("Paciente:{0} {1}       C.I: {2}        Edad: {3}", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString(), Paciente.Tables[0].Rows[0]["Cedula"].ToString(), Conexion.Fecha(nacimiento)), fontRegular, XBrushes.Black, rect, XStringFormats.CenterLeft);
                        Margen = new XRect(5, PosicionP, 75, 15);
                        gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
                        gfx.DrawString("CoproAnalisis", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        PosicionP = 110;
                        point = new XPoint(500, 20);
                        size = new XSize(80, 15);
                        rect = new XRect(point, size);
                        FechaImp = Convert.ToDateTime(dsPrint.Tables[0].Rows[0]["FechaImp"].ToString());
                        gfx.DrawString(string.Format("{0}", FechaImp.ToString("dd/MM/yyyy")), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
                        gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                        point = new XPoint(500, 40);
                        size = new XSize(80, 15);
                        rect = new XRect(point, size);
                        gfx.DrawString(IdOrden.ToString(), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
                        gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                        Margen = new XRect(5, 110, 150, 15);
                        gfx.DrawString("Características Químicas", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(298, 110, 150, 15);
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
                                gfx.DrawLine(pen, 5, Posicion = Posicion + MargenAncho * 7, 292, Posicion);
                            }
                            else if (i == 4)
                            {
                                gfx.DrawLine(pen, 5, Posicion = Posicion + MargenAncho * 6, 292, Posicion);
                            }
                            else
                            {
                                gfx.DrawLine(pen, 5, Posicion = Posicion + MargenAncho, 292, Posicion);
                            }

                        }
                        Posicion = 130;
                        Margen = new XRect(10, Posicion, 95, 15);
                        gfx.DrawString("Análisis", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Color", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Consistencia", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Aspecto", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Moco", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Reacción", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Sangre", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Restos Alimenticios", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Observaciones", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        string Observaciones = Print1.Tables[0].Rows[0]["COMENTARIO"].ToString();
                        Margen = new XRect(10, Posicion = Posicion + 15, 280, 90);
                        tf.Alignment = XParagraphAlignment.Left;
                        tf.DrawString(Observaciones, fontRegular, XBrushes.Black, Margen, XStringFormats.TopLeft);

                        Posicion = 130;
                        Margen = new XRect(170, Posicion, 95, 15);
                        gfx.DrawString("Resultado", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(170, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Color"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(170, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Consistencia"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(170, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Aspecto"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(170, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Moco"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(170, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Reaccion"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(170, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Sangre"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(170, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["RestosAlimenticios"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);

                        Posicion = 130;
                        Margen = new XRect(430, Posicion, 95, 15);
                        gfx.DrawString("Resultado", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(430, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Leucocitos"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(430, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Hematies"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(430, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Amilorrea"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(430, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Creatorrea"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(430, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Polisacaridos"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(430, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Gotas"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(430, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Levaduras"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(430, Posicion = Posicion + 15, 95, 15);
                     
                        Posicion = 130;
                        Margen = new XRect(500, Posicion, 95, 15);
                        gfx.DrawString("Referencia", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(500, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("X CPO", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(500, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("X CPO", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);

                        Posicion = 130;
                        Margen = new XRect(303, Posicion, 95, 15);
                        gfx.DrawString("Análisis", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(303, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Leucocitos", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(303, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Hematíes", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(303, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Amilorrea", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(303, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Creatorrea", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(303, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Polisacaridos", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(303, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Gotas", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(303, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Levaduras", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(303, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Examen Parasitológico", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        string Cristales = Print1.Tables[0].Rows[0]["Parasitos"].ToString();
                        if (Cristales == "")
                        {
                            Cristales = "No se observaron parásitos ni sus formas evolutivas";
                        }
                        XFont BioAnalistaFont = new XFont(facename, 8, XFontStyle.Regular);
                        tf.Alignment = XParagraphAlignment.Left;
                        Margen = new XRect(303, Posicion = Posicion + 15, 255, 60);
                        tf.DrawString(Cristales, ObservacionesHeces, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        tf.DrawString(Cristales, ObservacionesHeces, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(300, Posicion = Posicion + 30, 95, 45);
                        gfx.DrawString("Analista", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        DataSet Bioanalista = new DataSet();
                        Bioanalista = Conexion.Bioanalista(IdOrden, 12);
                        Margen = new XRect(297, Posicion = Posicion + 35, 150, 60);
                        string Bioanalista1 = string.Format("LCD. {0} CB:{1} MPPS:{2}", Bioanalista.Tables[0].Rows[0]["NombreUsuario"].ToString(), Bioanalista.Tables[0].Rows[0]["CB"].ToString(), Bioanalista.Tables[0].Rows[0]["MPPS"].ToString());
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString(Bioanalista1, BioAnalistaFont, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        XImage Firma = XImage.FromFile(string.Format(@"Firma\{1}", Ruta, Bioanalista.Tables[0].Rows[0]["FIRMA"].ToString()));
                        gfx.DrawImage(Firma, 500, 310);
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
                        if (r["EstadoDeResultado"].ToString() != "1" && Metodo != "Correo" && Metodo != "Convenio")
                        {
                            Conexion.ActualizarOrden("EstadoDeResultado = 3", Convert.ToInt32(IdOrden), Convert.ToInt32(r["IdAnalisis"].ToString()));
                        }
                        if (r != dsPrint.Tables[0].Rows[dsPrint.Tables[0].Rows.Count - 1])
                        {
                            int index = dsPrint.Tables[0].Rows.IndexOf(r) + 1;
                            if (dsPrint.Tables[0].Rows[index]["EstadoDeResultado"].ToString() != "" && dsPrint.Tables[0].Rows[index]["EstadoDeResultado"].ToString() != "1")
                            {
                                page = document.AddPage();
                                gfx = XGraphics.FromPdfPage(page);
                                tf = new XTextFormatter(gfx);
                                Posicion = 110;
                            }
                            else
                            {
                                return document;
                            }
                        }

                    }
                    else if (r["IdAnalisis"].ToString() == "257" && r["EstadoDeResultado"].ToString() != "" && r["EstadoDeResultado"].ToString() != " " && r["EstadoDeResultado"].ToString() != x)
                    {
                        DataSet Print1 = new DataSet();
                        Print1 = Conexion.SelectHecesDirecta(Convert.ToInt32(IdOrden.ToString()));
                        if (Metodo == "Correo")
                        {
                            page.Height = 400;
                        }
                        fontRegular = new XFont(facename, 10, XFontStyle.Regular);
                        fontRegular2 = new XFont(facename, 9, XFontStyle.Regular);
                        PosicionP = 90;
                        Margen = new XRect(15, 12, 145, 14);
                        tf.Alignment = XParagraphAlignment.Center;
                        Empresa = new DataSet();
                        if (Convert.ToInt32(Paciente.Tables[0].Rows[0]["IdConvenio"].ToString()) > 3 && Convert.ToInt32(Paciente.Tables[0].Rows[0]["IdConvenio"].ToString()) < 11)
                        {
                            Empresa = Conexion.SelectEmpresaConvenio(Paciente.Tables[0].Rows[0]["IdConvenio"].ToString());

                        }
                        else
                        {
                            Empresa = Conexion.SelectEmpresaActiva();
                        }
                        // Logo = XImage.FromFile(string.Format(@"" + Ruta + @"\Logos\{0}", Empresa.Tables[0].Rows[0]["Ruta"].ToString()));
                        //gfx.DrawImage(Logo, 5, 1);
                        gfx.DrawString(Empresa.Tables[0].Rows[0]["Nombre"].ToString(), Titulo, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(15, 30, 250, 48);
                        tf.Alignment = XParagraphAlignment.Left;
                        string Formato = string.Format("RIF: {0}\nDireccion:{1} TLF:{2} Correo:{3}", Empresa.Tables[0].Rows[0]["RIF"].ToString(), Empresa.Tables[0].Rows[0]["Direccion"].ToString(), Empresa.Tables[0].Rows[0]["Telefono"].ToString(), Empresa.Tables[0].Rows[0]["Correo"].ToString());
                        tf.DrawString(Formato, Direccion, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        XColor color = new XColor { R = 105, G = 105, B = 105 };
                        XPen pen = new XPen(color);
                        XPoint point = new XPoint(5, 70);
                        XSize size = new XSize(580, 15);
                        XSize Elipsesize = new XSize(5, 5);
                        XRect rect = new XRect(point, size);
                        DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                        Margen = new XRect(5, PosicionP, 60, 15);
                        rect = new XRect(point, size);
                        nacimiento = DateTime.Parse(Paciente.Tables[0].Rows[0]["Fecha"].ToString());
                        gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                        point = new XPoint(15, 70);
                        rect = new XRect(point, size);
                        gfx.DrawString(string.Format("Paciente:{0} {1}       C.I: {2}        Edad: {3}", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString(), Paciente.Tables[0].Rows[0]["Cedula"].ToString(), Conexion.Fecha(nacimiento)), fontRegular, XBrushes.Black, rect, XStringFormats.CenterLeft);
                        Margen = new XRect(5, PosicionP, 75, 15);
                        gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
                        gfx.DrawString("Heces Directa", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        PosicionP = 110;
                        point = new XPoint(500, 20);
                        size = new XSize(80, 15);
                        rect = new XRect(point, size);
                        FechaImp = Convert.ToDateTime(dsPrint.Tables[0].Rows[0]["FechaImp"].ToString());
                        gfx.DrawString(string.Format("{0}", FechaImp.ToString("dd/MM/yyyy")), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
                        gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                        point = new XPoint(500, 40);
                        size = new XSize(80, 15);
                        rect = new XRect(point, size);
                        gfx.DrawString(IdOrden.ToString(), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
                        gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                        Margen = new XRect(5, 110, 150, 15);
                        gfx.DrawString("Examen Microscópico", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(298, 110, 150, 15);
                        gfx.DrawString("Examen Parasitológico", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
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
                                gfx.DrawLine(pen, 5, Posicion = Posicion + MargenAncho * 2, 292, Posicion);
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
                        Margen = new XRect(10, Posicion, 95, 15);
                        gfx.DrawString("Análisis", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Leucocitos", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Hematíes", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(10, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("Observaciones", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        string Observaciones = Print1.Tables[0].Rows[0]["COMENTARIO"].ToString();
                        Margen = new XRect(10, Posicion = Posicion + 15, 280, 90);
                        tf.Alignment = XParagraphAlignment.Left;
                        tf.DrawString(Observaciones, fontRegular, XBrushes.Black, Margen, XStringFormats.TopLeft);

                        Posicion = 130;
                        Margen = new XRect(90, Posicion, 95, 15);
                        gfx.DrawString("Resultado", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(90, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Leucocitos"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(90, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString(Print1.Tables[0].Rows[0]["Hematies"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);

                        Posicion = 130;
                        Margen = new XRect(190, Posicion, 95, 15);
                        gfx.DrawString("Referencia", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(190, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("X CPO", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                        Margen = new XRect(190, Posicion = Posicion + 15, 95, 15);
                        gfx.DrawString("X CPO", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);

                        string Cristales = Print1.Tables[0].Rows[0]["Parasitos"].ToString();
                        if (Cristales == "")
                        {
                            Cristales = "No se observaron parásitos ni sus formas evolutivas";
                        }

                        tf.Alignment = XParagraphAlignment.Left;
                        Margen = new XRect(303, 135, 255, 60);
                        tf.DrawString(Cristales, fontRegular2, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        Margen = new XRect(10, 235, 95, 45);
                        gfx.DrawString("Responsable", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        DataSet Bioanalista = new DataSet();
                        Bioanalista = Conexion.Bioanalista(IdOrden, 257);
                        Margen = new XRect(10, 280, 150, 60);
                        string Bioanalista1 = string.Format("LCD. {0} CB:{1} MPPS:{2}", Bioanalista.Tables[0].Rows[0]["NombreUsuario"].ToString(), Bioanalista.Tables[0].Rows[0]["CB"].ToString(), Bioanalista.Tables[0].Rows[0]["MPPS"].ToString());
                        tf.Alignment = XParagraphAlignment.Center;
                        tf.DrawString(Bioanalista1, fontRegular, XBrushes.Black, Margen, XStringFormats.TopLeft);
                        XImage Firma = XImage.FromFile(string.Format(@"Firma\{1}", Ruta, Bioanalista.Tables[0].Rows[0]["FIRMA"].ToString()));
                        gfx.DrawImage(Firma, 200, 280);
                        MargenAncho = 15;
                        Posicion = 130;

                        if (r["EstadoDeResultado"].ToString() != "1" && Metodo != "Correo" && Metodo != "Convenio")
                        {
                            Conexion.ActualizarOrden("EstadoDeResultado = 3", Convert.ToInt32(IdOrden), Convert.ToInt32(r["IdAnalisis"].ToString()));
                        }
                        if (r != dsPrint.Tables[0].Rows[dsPrint.Tables[0].Rows.Count - 1])
                        {
                            int index = dsPrint.Tables[0].Rows.IndexOf(r) + 1;
                            if (dsPrint.Tables[0].Rows[index]["EstadoDeResultado"].ToString() != "" && dsPrint.Tables[0].Rows[index]["EstadoDeResultado"].ToString() != "1")
                            {
                                page = document.AddPage();
                                gfx = XGraphics.FromPdfPage(page);
                                tf = new XTextFormatter(gfx);
                                Posicion = 110;
                            }
                            else
                            {
                                return document;
                            }
                        }

                    }
                    else if ((r["IdAnalisis"].ToString() == "203" || r["IdAnalisis"].ToString() == "49") && r["EstadoDeResultado"].ToString() != "" && r["EstadoDeResultado"].ToString() != " " && r["EstadoDeResultado"].ToString() != x)
                    {
                        fontRegular = new XFont(facename, 11, XFontStyle.Regular);
                        PosicionP = 90;
                        Margen = new XRect(15, 12, 145, 14);
                        XColor color = new XColor { R = 105, G = 105, B = 105 };
                        XPen pen = new XPen(color);
                        XPoint point = new XPoint(5, 70);
                        XSize size = new XSize(580, 15);
                        XSize Elipsesize = new XSize(5, 5);
                        XRect rect = new XRect(point, size);
                        DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                        string Formato;
                        //Hematologia

                        if (r["IdAnalisis"].ToString() == "203")
                        {

                            if (Metodo == "Correo")
                            {
                                page.Height = 400;
                            }
                            tf.Alignment = XParagraphAlignment.Center;

                            //gfx.DrawImage(Logo, 5, 10);
                            gfx.DrawString(Empresa.Tables[0].Rows[0]["Nombre"].ToString(), Titulo, XBrushes.Black, Margen, XStringFormats.TopLeft);
                            Margen = new XRect(15, 30, 250, 48);
                            tf.Alignment = XParagraphAlignment.Left;


                            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                            point = new XPoint(500, 20);
                            size = new XSize(80, 15);
                            rect = new XRect(point, size);
                            FechaImp = Convert.ToDateTime(dsPrint.Tables[0].Rows[0]["FechaImp"].ToString());
                            gfx.DrawString(string.Format("{0}", FechaImp.ToString("dd/MM/yyyy")), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
                            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                            point = new XPoint(500, 40);
                            size = new XSize(80, 15);
                            rect = new XRect(point, size);
                            gfx.DrawString(IdOrden.ToString(), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
                            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                            point = new XPoint(5, 110);
                            size = new XSize(580, 225);
                            rect = new XRect(point, size);
                            gfx.DrawRoundedRectangle(pen, rect, Elipsesize); ;
                            MargenAncho = 15;
                            Posicion = 110;
                            for (int i = 1; i < 6; i++)
                            {
                                if (i == 2)
                                {
                                    gfx.DrawLine(pen, 5, Posicion = Posicion + MargenAncho * 4, 585, Posicion);
                                }
                                else if (i == 4)
                                {
                                    gfx.DrawLine(pen, 5, Posicion = Posicion + MargenAncho * 8, 585, Posicion);
                                }
                                else
                                {
                                    gfx.DrawLine(pen, 5, Posicion = Posicion + MargenAncho, 585, Posicion);
                                }

                            }

                            PosicionP = 70;
                            Margen = new XRect(15, PosicionP, 145, 14);

                            nacimiento = DateTime.Parse(dsPrint.Tables[0].Rows[0]["Fecha"].ToString());
                            gfx.DrawString(string.Format("Paciente:{0} {1}       C.I: {2}        Edad: {3}", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString(), Paciente.Tables[0].Rows[0]["Cedula"].ToString(), Conexion.Fecha(nacimiento)), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                            PosicionP = 90;
                            Margen = new XRect(5, PosicionP, 145, 14);
                            gfx.DrawRoundedRectangle(pen, Margen, Elipsesize); ;
                            gfx.DrawString("Hematologia Especial", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            PosicionP = 110;
                            Margen = new XRect(5, PosicionP, 145, 14);
                            //ANALISIS
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
                            gfx.DrawString("Neutrófilos%", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                            Margen = new XRect(10, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("Linfocitos%", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                            Margen = new XRect(10, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("Monocitos%", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                            Margen = new XRect(10, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("Eosinofilos%", fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
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

                            PosicionP = 110;
                            double YPalabras = 180;
                            //Resultados
                            DataSet Print = new DataSet();
                            Print = Conexion.HematologiaEspecial(IdOrden, 203);
                            Margen = new XRect(YPalabras, PosicionP, 145, 14);
                            gfx.DrawString("Resultados", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString(string.Format("{0:0.0}", Convert.ToDouble(Print.Tables[0].Rows[0]["Leucocitos"].ToString())), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString(string.Format("{0:0.0}", Convert.ToDouble(Print.Tables[0].Rows[0]["Hematies"].ToString())), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString(string.Format("{0:0.0}", Convert.ToDouble(Print.Tables[0].Rows[0]["Hemoglobina"].ToString())), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString(string.Format("{0:0.0}", Convert.ToDouble(Print.Tables[0].Rows[0]["Hematocritos"].ToString())), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("Resultados", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            if (Print.Tables[0].Rows[0]["Neutrofilos"].ToString() != "")
                            {
                                gfx.DrawString(string.Format("{0:0.0}%", Convert.ToDouble(Print.Tables[0].Rows[0]["Neutrofilos"].ToString())), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            }
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            if (Print.Tables[0].Rows[0]["Linfocitos"].ToString() != "")
                            {
                                gfx.DrawString(string.Format("{0:0.0}%", Convert.ToDouble(Print.Tables[0].Rows[0]["Linfocitos"].ToString())), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            }
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            if (Print.Tables[0].Rows[0]["MONOCITOS"].ToString() != "")
                            {
                                gfx.DrawString(string.Format("{0:0.0}%", Convert.ToDouble(Print.Tables[0].Rows[0]["MONOCITOS"].ToString())), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            }
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            if (Print.Tables[0].Rows[0]["EOSINOFILOS"].ToString() != "")
                            {
                                gfx.DrawString(string.Format("{0:0.0}%", Convert.ToDouble(Print.Tables[0].Rows[0]["EOSINOFILOS"].ToString())), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            }
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            if (Print.Tables[0].Rows[0]["Neutrofilos2"].ToString() != "")
                            {
                                gfx.DrawString(string.Format("{0:0.0}  10^3/ul", Convert.ToDouble(Print.Tables[0].Rows[0]["Neutrofilos2"].ToString())), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            }
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            if (Print.Tables[0].Rows[0]["Linfocitos2"].ToString() != "")
                            {
                                gfx.DrawString(string.Format("{0:0.0}  10^3/ul", Convert.ToDouble(Print.Tables[0].Rows[0]["Linfocitos2"].ToString())), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            }
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            if (Print.Tables[0].Rows[0]["MONOCITOS2"].ToString() != "")
                            {
                                gfx.DrawString(string.Format("{0:0.0}  10^3/ul", Convert.ToDouble(Print.Tables[0].Rows[0]["MONOCITOS2"].ToString())), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            }

                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            if (Print.Tables[0].Rows[0]["EOSINOFILOS2"].ToString() != "")
                            {
                                gfx.DrawString(string.Format("{0:0.0}  10^3/ul", Convert.ToDouble(Print.Tables[0].Rows[0]["EOSINOFILOS2"].ToString())), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            }
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            if (Print.Tables[0].Rows[0]["plaquetas"].ToString() != "")
                            {
                                gfx.DrawString(string.Format("{0:0.0}  10^3/ul", Convert.ToDouble(Print.Tables[0].Rows[0]["plaquetas"].ToString())), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            }

                            PosicionP = 110;
                            YPalabras = 320;
                            //Unidades
                            Margen = new XRect(YPalabras, PosicionP, 135, 14);
                            gfx.DrawString("Unidades", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("10^6/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("gr/dL", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("%", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("Unidades", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("%", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("%", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("%", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("%", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);

                            PosicionP = 110;
                            YPalabras = 460;
                            //Valores Normales
                            Margen = new XRect(YPalabras, PosicionP, 135, 14);
                            gfx.DrawString("Valores Normales", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("4.0 – 10.0 10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("3.5 – 5.5 10^6/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("11.0 – 16.0 gr/dL", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("35.0 – 54.0 %", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("Valores Normales", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("40.0% – 70.0 %", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("18.0% – 48.0 %", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("3.0% 0 – 12.0%", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("1.0% 0 – 7.0 %", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("2.0 – 7.0 10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("1.1 – 2.9 10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("0.12 – 1.2 10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("0.02 – 0.50  10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            Margen = new XRect(YPalabras, PosicionP = PosicionP + 15, 135, 14);
                            gfx.DrawString("150 – 450 10^3/ul", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            PosicionP = 335;
                            Margen = new XRect(5, PosicionP, 320, 60);
                            gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
                            Margen = new XRect(10, PosicionP + 5, 320, 60);
                            fontRegular2 = new XFont(facename, 10, XFontStyle.Regular);
                            rect = new XRect(10, PosicionP + 15, 300, 40);
                            tf.Alignment = XParagraphAlignment.Left;
                            Margen = new XRect(325, PosicionP, 260, 60);
                            gfx.DrawRoundedRectangle(pen, Margen, Elipsesize);
                            Margen = new XRect(330, PosicionP + 10, 50, 60);
                            DataSet Bioanalista = new DataSet();
                            Bioanalista = Conexion.Bioanalista(IdOrden, 203);
                            string text = string.Format("LCD. {0} CB:{1} MPPS:{2}", Bioanalista.Tables[0].Rows[0]["NombreUsuario"].ToString(), Bioanalista.Tables[0].Rows[0]["CB"].ToString(), Bioanalista.Tables[0].Rows[0]["MPPS"].ToString());
                            rect = new XRect(330, PosicionP + 5, 150, 40);
                            tf.Alignment = XParagraphAlignment.Center;
                            tf.DrawString(text, fontRegular, XBrushes.Black, rect, XStringFormats.TopLeft);
                            XImage Firma = XImage.FromFile(string.Format(@"Firma\{1}", Ruta, Bioanalista.Tables[0].Rows[0]["FIRMA"].ToString()));
                            gfx.DrawImage(Firma, 500, PosicionP + 5);
                            //point = new XPoint(Convert.ToDouble(textBox3.Text), Convert.ToDouble(textBox4.Text));
                            //size = new XSize(Convert.ToDouble(textBox1.Text), Convert.ToDouble(textBox2.Text));
                            // rect = new XRect(point, size);
                            //gfx.DrawRectangle(pen, rect);

                            //gfx.DrawImage(Logo, 5, 10);

                            page = document.AddPage();
                            gfx = XGraphics.FromPdfPage(page);
                            tf = new XTextFormatter(gfx);
                            Posicion = 110;
                            Margen = new XRect(15, 12, 145, 14);
                            gfx.DrawString(Empresa.Tables[0].Rows[0]["Nombre"].ToString(), Titulo, XBrushes.Black, Margen, XStringFormats.TopLeft);
                            Margen = new XRect(15, 30, 250, 48);
                            tf.Alignment = XParagraphAlignment.Left;
                            Formato = string.Format("RIF: {0}\nDireccion:{1} TLF:{2} Correo:{3}", Empresa.Tables[0].Rows[0]["RIF"].ToString(), Empresa.Tables[0].Rows[0]["Direccion"].ToString(), Empresa.Tables[0].Rows[0]["Telefono"].ToString(), Empresa.Tables[0].Rows[0]["Correo"].ToString());
                            tf.DrawString(Formato, Direccion, XBrushes.Black, Margen, XStringFormats.TopLeft);

                            color = new XColor { R = 105, G = 105, B = 105 };
                            pen = new XPen(color);
                            point = new XPoint(5, 70);
                            size = new XSize(580, 15);
                            Elipsesize = new XSize(5, 5);
                            rect = new XRect(point, size);
                            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                            point = new XPoint(500, 20);
                            size = new XSize(80, 15);
                            rect = new XRect(point, size);
                            FechaImp = Convert.ToDateTime(dsPrint.Tables[0].Rows[0]["FechaImp"].ToString());
                            gfx.DrawString(string.Format("{0}", FechaImp.ToString("dd/MM/yyyy")), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
                            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                            point = new XPoint(500, 40);
                            size = new XSize(80, 15);
                            rect = new XRect(point, size);
                            gfx.DrawString(IdOrden.ToString(), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
                            //gfx.DrawString(Print.Tables[0].Rows[0]["frotis"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                            point = new XPoint(5, 110);
                            size = new XSize(580, 225);
                            rect = new XRect(point, size);
                            tf.Alignment = XParagraphAlignment.Left;
                            string text1 = Print.Tables[0].Rows[0]["frotis"].ToString();
                            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                            point = new XPoint(10, 110);
                            size = new XSize(580, 225);
                            rect = new XRect(point, size);
                            tf.DrawString(text1, fontRegular2, XBrushes.Black, rect, XStringFormats.TopLeft);

                            MargenAncho = 15;
                            Posicion = 110;
                            PosicionP = 70;
                            Margen = new XRect(10, PosicionP, 145, 14);
                            nacimiento = DateTime.Parse(dsPrint.Tables[0].Rows[0]["Fecha"].ToString());
                            gfx.DrawString(string.Format("Paciente:{0} {1}       C.I: {2}        Edad: {3}", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString(), Paciente.Tables[0].Rows[0]["Cedula"].ToString(), Conexion.Fecha(nacimiento)), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                            PosicionP = 90;

                            Margen = new XRect(5, PosicionP, 145, 14);
                            gfx.DrawString("Descripcion de Frotis", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            gfx.DrawRoundedRectangle(pen, Margen, Elipsesize); ;



                            if (r["EstadoDeResultado"].ToString() != "1" && Metodo != "Correo" && Metodo != "Convenio")
                            {
                                Conexion.ActualizarOrden("EstadoDeResultado = 3", Convert.ToInt32(IdOrden), Convert.ToInt32(r["IdAnalisis"].ToString()));
                            }
                            int index = dsPrint.Tables[0].Rows.IndexOf(r);
                            if (dsPrint.Tables[0].Rows[index]["EstadoDeResultado"].ToString() != "" && dsPrint.Tables[0].Rows[index]["EstadoDeResultado"].ToString() != "1" && r != dsPrint.Tables[0].Rows[dsPrint.Tables[0].Rows.Count - 1])
                            {
                                page = document.AddPage();
                                gfx = XGraphics.FromPdfPage(page);
                                tf = new XTextFormatter(gfx);
                                Posicion = 110;
                            }
                            else
                            {
                                return document;
                            }
                        }
                        else
                        {

                            Margen = new XRect(15, 12, 145, 14);
                            gfx.DrawString(Empresa.Tables[0].Rows[0]["Nombre"].ToString(), Titulo, XBrushes.Black, Margen, XStringFormats.TopLeft);
                            Margen = new XRect(15, 30, 250, 48);
                            tf.Alignment = XParagraphAlignment.Left;
                            Formato = string.Format("RIF: {0}\nDireccion:{1} TLF:{2} Correo:{3}", Empresa.Tables[0].Rows[0]["RIF"].ToString(), Empresa.Tables[0].Rows[0]["Direccion"].ToString(), Empresa.Tables[0].Rows[0]["Telefono"].ToString(), Empresa.Tables[0].Rows[0]["Correo"].ToString());
                            tf.DrawString(Formato, Direccion, XBrushes.Black, Margen, XStringFormats.TopLeft);

                            color = new XColor { R = 105, G = 105, B = 105 };
                            pen = new XPen(color);
                            point = new XPoint(5, 70);
                            size = new XSize(580, 15);
                            Elipsesize = new XSize(5, 5);
                            rect = new XRect(point, size);
                            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                            point = new XPoint(500, 20);
                            size = new XSize(80, 15);
                            rect = new XRect(point, size);
                            FechaImp = Convert.ToDateTime(dsPrint.Tables[0].Rows[0]["FechaImp"].ToString());
                            gfx.DrawString(string.Format("{0}", FechaImp.ToString("dd/MM/yyyy")), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
                            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                            point = new XPoint(500, 40);
                            size = new XSize(80, 15);
                            rect = new XRect(point, size);
                            gfx.DrawString(IdOrden.ToString(), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
                            //gfx.DrawString(Print.Tables[0].Rows[0]["frotis"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                            point = new XPoint(5, 110);
                            size = new XSize(580, 225);
                            rect = new XRect(point, size);
                            tf.Alignment = XParagraphAlignment.Left;
                            string text1 = r["Comentario"].ToString();
                            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                            point = new XPoint(10, 110);
                            size = new XSize(580, 225);
                            rect = new XRect(point, size);
                            tf.DrawString(text1, fontRegular2, XBrushes.Black, rect, XStringFormats.TopLeft);

                            MargenAncho = 15;
                            Posicion = 110;
                            PosicionP = 70;
                            Margen = new XRect(10, PosicionP, 145, 14);
                            nacimiento = DateTime.Parse(dsPrint.Tables[0].Rows[0]["Fecha"].ToString());
                            gfx.DrawString(string.Format("Paciente:{0} {1}       C.I: {2}        Edad: {3}", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString(), Paciente.Tables[0].Rows[0]["Cedula"].ToString(), Conexion.Fecha(nacimiento)), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                            PosicionP = 90;

                            Margen = new XRect(5, PosicionP, 145, 14);
                            gfx.DrawString("Descripcion de Frotis", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            gfx.DrawRoundedRectangle(pen, Margen, Elipsesize); ;



                            if (r["EstadoDeResultado"].ToString() != "1" && Metodo != "Correo" && Metodo != "Convenio")
                            {
                                Conexion.ActualizarOrden("EstadoDeResultado = 3", Convert.ToInt32(IdOrden), Convert.ToInt32(r["IdAnalisis"].ToString()));
                            }
                            int index = dsPrint.Tables[0].Rows.IndexOf(r);
                            if (dsPrint.Tables[0].Rows[index]["EstadoDeResultado"].ToString() != "" && dsPrint.Tables[0].Rows[index]["EstadoDeResultado"].ToString() != "1" && r != dsPrint.Tables[0].Rows[dsPrint.Tables[0].Rows.Count - 1])
                            {
                                page = document.AddPage();
                                gfx = XGraphics.FromPdfPage(page);
                                tf = new XTextFormatter(gfx);
                                Posicion = 110;
                            }
                            else
                            {
                                return document;
                            }
                        }



                    }
                    else if (r["IdAnalisis"].ToString() != "203" && r["IdAnalisis"].ToString() != "49" && r["IdAnalisis"].ToString() != "12" && r["IdAnalisis"].ToString() != "42" && r["IdAnalisis"].ToString() != "55" && r["EstadoDeResultado"].ToString() != "" && r["EstadoDeResultado"].ToString() != " " && r["EstadoDeResultado"].ToString() != x)
                    {
                        int IdAnalisisPrueba = Convert.ToInt32(r["IdAnalisis"].ToString());
                        bioencontrado = false;
                        XColor color = new XColor { R = 105, G = 105, B = 105 };
                        XPen pen;
                        XPoint point = new XPoint(5, 70);
                        XSize size;
                        XSize Elipsesize = new XSize(5, 5);
                        XRect rect;
                        pen = new XPen(color);
                        if (!primerapagina)
                        {
                            if (Metodo == "Correo")
                            {
                                page.Height = 400;
                            }
                            primerapagina = true;
                            PosicionP = 90;
                            Margen = new XRect(15, 12, 145, 14);
                            //Logo = XImage.FromFile(string.Format(@"" + Ruta + @"\Logos\{0}", Empresa.Tables[0].Rows[0]["Ruta"].ToString()));
                            //gfx.DrawImage(Logo, 5, 10);

                            gfx.DrawString(Empresa.Tables[0].Rows[0]["Nombre"].ToString(), Titulo, XBrushes.Black, Margen, XStringFormats.TopLeft);
                            Margen = new XRect(15, 30, 250, 48);
                            tf.Alignment = XParagraphAlignment.Left;
                            string Formato = string.Format("RIF: {0}\nDireccion:{1} TLF:{2} Correo:{3}", Empresa.Tables[0].Rows[0]["RIF"].ToString(), Empresa.Tables[0].Rows[0]["Direccion"].ToString(), Empresa.Tables[0].Rows[0]["Telefono"].ToString(), Empresa.Tables[0].Rows[0]["Correo"].ToString());
                            tf.DrawString(Formato, Direccion, XBrushes.Black, Margen, XStringFormats.TopLeft);

                            pen = new XPen(color);
                            point = new XPoint(5, 70);
                            size = new XSize(580, 15);
                            Elipsesize = new XSize(5, 5);
                            rect = new XRect(point, size);
                            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                            point = new XPoint(500, 20);
                            size = new XSize(80, 15);
                            rect = new XRect(point, size);
                            FechaImp = Convert.ToDateTime(dsPrint.Tables[0].Rows[0]["FechaImp"].ToString());
                            gfx.DrawString(string.Format("{0}", FechaImp.ToString("dd/MM/yyyy")), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
                            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                            point = new XPoint(500, 40);
                            size = new XSize(80, 15);
                            rect = new XRect(point, size);
                            gfx.DrawString(IdOrden.ToString(), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
                            gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                            point = new XPoint(5, 110);
                            size = new XSize(580, 225);
                            rect = new XRect(point, size);
                            gfx.DrawRoundedRectangle(pen, rect, Elipsesize); ;
                            PosicionP = 70;
                            Margen = new XRect(15, PosicionP, 145, 14);
                            DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                            nacimiento = DateTime.Parse(dsPrint.Tables[0].Rows[0]["Fecha"].ToString());
                            gfx.DrawString(string.Format("Paciente:{0} {1}       C.I: {2}        Edad: {3}", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString(), Paciente.Tables[0].Rows[0]["Cedula"].ToString(), Conexion.Fecha(nacimiento)), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                            PosicionP = 95;
                            Margen = new XRect(10, PosicionP, 145, 14);
                            gfx.DrawString("Analisis", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            PosicionP = 95;
                            double YPalabras = 220;
                            Margen = new XRect(YPalabras, PosicionP, 145, 14);
                            gfx.DrawString("Resultados", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            YPalabras = 360;
                            Margen = new XRect(YPalabras, PosicionP, 135, 14);
                            gfx.DrawString("Valores Normales", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                            YPalabras = 480;
                            MargenAncho = 15;

                        }
                        DataSet Bioanalista = new DataSet();
                        XFont fontRegular3;
                        XFont fontRegular4;
                        XFont fontRegular5;
                        fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
                        XSolidBrush blueBrush = new XSolidBrush(XColors.LightGray);
                        fontRegular = new XFont(facename, 11, XFontStyle.Regular);
                        FechaImp = new DateTime();
                        //Hematologia
                        fontRegular = new XFont(facename, 11, XFontStyle.Regular);
                        tf.Alignment = XParagraphAlignment.Center;


                        string text;

                        if (r["TipoAnalisis"].ToString() == "15" || r["TipoAnalisis"].ToString() == "14" || r["EstadoDeResultado"].ToString() != x)
                        {
                            if (r["EstadoDeResultado"].ToString() != "1" && Metodo != "Correo" && Metodo != "Convenio")
                            {
                                Conexion.ActualizarOrden("EstadoDeResultado = 3", Convert.ToInt32(IdOrden), Convert.ToInt32(r["IdAnalisis"].ToString()));
                            }
                            if (r["MultiplesValores"].ToString() == "")
                            {
                                if (r["ValorMenor"].ToString() != "" && r["ValorMayor"].ToString() != "")
                                {
                                    text = Convert.ToDouble(r["ValorMenor"].ToString()).ToString("0.#0") + " - " + Convert.ToDouble(r["ValorMayor"].ToString()).ToString("0.#0") + " " + r["UNIDAD"].ToString();
                                }
                                else
                                {
                                    text = "";
                                }
                            }
                            else
                            {
                                text = r["MultiplesValores"].ToString();
                            }

                            string Analisis = r["NombreAnalisis"].ToString();
                            int AnalisisCount = Analisis.Length;
                            string Observaciones = r["Comentario"].ToString();
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

                            if (r["Titulo"].ToString() == "1")
                            {
                                double.TryParse(r["Lineas"].ToString(), out total);
                            }
                            if (ObservacionesWidth < 1)
                            {
                                ObservacionesWidth = 1;
                            }

                            XImage Firma;
                            y = 380;
                            fontRegular4 = new XFont(facename, 8, XFontStyle.Regular);
                            if (Posicion + MargenAncho * total > 320)
                            {
                                if (r == dsPrint.Tables[0].Rows[dsPrint.Tables[0].Rows.Count - 1])
                                {
                                    foreach (int i in IdAnalisisBio)
                                    {
                                        Bioanalista = new DataSet();
                                        Bioanalista = Conexion.Bioanalista(IdOrden, i);
                                        Margen = new XRect(y, 345, 100, 60);
                                        string Bioanalista1 = string.Format("LCD. {0} CB:{1} MPPS:{2}", Bioanalista.Tables[0].Rows[0]["NombreUsuario"].ToString(), Bioanalista.Tables[0].Rows[0]["CB"].ToString(), Bioanalista.Tables[0].Rows[0]["MPPS"].ToString());
                                        tf.Alignment = XParagraphAlignment.Center;
                                        tf.DrawString(Bioanalista1, fontRegular4, XBrushes.Black, Margen, XStringFormats.TopLeft);
                                        Firma = XImage.FromFile(string.Format(@"Firma\{1}", Ruta, Bioanalista.Tables[0].Rows[0]["FIRMA"].ToString()));
                                        gfx.DrawImage(Firma, y + 105, 345);
                                        y = y - 160;
                                    }
                                    Bio.Clear();
                                    IdAnalisisBio.Clear();
                                    y = 380;
                                    primerapagina = false;
                                    page = document.AddPage();
                                    gfx = XGraphics.FromPdfPage(page);
                                    tf = new XTextFormatter(gfx);
                                    Posicion = 110;
                                    if (!primerapagina)
                                    {
                                        PosicionP = 90;
                                        Margen = new XRect(5, 1, 145, 14);
                                        // Logo = XImage.FromFile(string.Format(@"" + Ruta + @"\Logos\{0}", Empresa.Tables[0].Rows[0]["Ruta"].ToString()));
                                        // gfx.DrawImage(Logo, 5, 10);

                                        gfx.DrawString(Empresa.Tables[0].Rows[0]["Nombre"].ToString(), Titulo, XBrushes.Black, Margen, XStringFormats.TopLeft);
                                        Margen = new XRect(10, 30, 250, 48);
                                        tf.Alignment = XParagraphAlignment.Left;
                                        string Formato = string.Format("RIF: {0}\nDireccion:{1} TLF:{2} Correo:{3}", Empresa.Tables[0].Rows[0]["RIF"].ToString(), Empresa.Tables[0].Rows[0]["Direccion"].ToString(), Empresa.Tables[0].Rows[0]["Telefono"].ToString(), Empresa.Tables[0].Rows[0]["Correo"].ToString());
                                        tf.DrawString(Formato, Direccion, XBrushes.Black, Margen, XStringFormats.TopLeft);

                                        pen = new XPen(color);
                                        point = new XPoint(5, 70);
                                        size = new XSize(580, 15);
                                        Elipsesize = new XSize(5, 5);
                                        rect = new XRect(point, size);
                                        gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                                        point = new XPoint(500, 20);
                                        size = new XSize(80, 15);
                                        rect = new XRect(point, size);
                                        FechaImp = Convert.ToDateTime(dsPrint.Tables[0].Rows[0]["FechaImp"].ToString());
                                        gfx.DrawString(string.Format("{0}", FechaImp.ToString("dd/MM/yyyy")), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
                                        gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                                        point = new XPoint(500, 40);
                                        size = new XSize(80, 15);
                                        rect = new XRect(point, size);
                                        gfx.DrawString(IdOrden.ToString(), fontRegular, XBrushes.Black, rect, XStringFormats.Center);
                                        gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                                        point = new XPoint(5, 110);
                                        size = new XSize(580, 225);
                                        rect = new XRect(point, size);
                                        gfx.DrawRoundedRectangle(pen, rect, Elipsesize); ;
                                        PosicionP = 70;
                                        Margen = new XRect(15, PosicionP, 145, 14);
                                        DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                                        nacimiento = DateTime.Parse(dsPrint.Tables[0].Rows[0]["Fecha"].ToString());
                                        gfx.DrawString(string.Format("Paciente:{0} {1}       C.I: {2}        Edad: {3}", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString(), Paciente.Tables[0].Rows[0]["Cedula"].ToString(), Conexion.Fecha(nacimiento)), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                                        PosicionP = 95;
                                        Margen = new XRect(10, PosicionP, 145, 14);
                                        gfx.DrawString("Analisis", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                                        PosicionP = 95;
                                        double YPalabras = 220;
                                        Margen = new XRect(YPalabras, PosicionP, 145, 14);
                                        gfx.DrawString("Resultados", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                                        YPalabras = 360;
                                        Margen = new XRect(YPalabras, PosicionP, 135, 14);
                                        gfx.DrawString("Valores Normales", fontRegular, XBrushes.Black, Margen, XStringFormats.Center);
                                        YPalabras = 480;
                                        MargenAncho = 15;
                                    }

                                    foreach (int i in IdAnalisisBio)
                                    {
                                        Bioanalista = new DataSet();
                                        Bioanalista = Conexion.Bioanalista(IdOrden, i);
                                        Margen = new XRect(y, 345, 100, 60);
                                        string Bioanalista1 = string.Format("LCD. {0} CB:{1} MPPS:{2}", Bioanalista.Tables[0].Rows[0]["NombreUsuario"].ToString(), Bioanalista.Tables[0].Rows[0]["CB"].ToString(), Bioanalista.Tables[0].Rows[0]["MPPS"].ToString());
                                        tf.Alignment = XParagraphAlignment.Center;
                                        tf.DrawString(Bioanalista1, fontRegular4, XBrushes.Black, Margen, XStringFormats.TopLeft);
                                        Firma = XImage.FromFile(string.Format(@"Firma\{1}", Ruta, Bioanalista.Tables[0].Rows[0]["FIRMA"].ToString()));
                                        gfx.DrawImage(Firma, y + 105, 345);
                                        y = y - 160;
                                    }

                                    Bioanalista = Conexion.Bioanalista(IdOrden, Convert.ToInt32(r["IdAnalisis"].ToString()));
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
                                        IdAnalisisBio.Add(Convert.ToInt32(r["IdAnalisis"].ToString()));
                                        Bio.Add(Convert.ToInt32(Bioanalista.Tables[0].Rows[0]["IdUsuario"].ToString()));
                                    }
                                }
                                else
                                {

                                    foreach (int i in IdAnalisisBio)
                                    {
                                        Bioanalista = new DataSet();
                                        Bioanalista = Conexion.Bioanalista(IdOrden, i);
                                        Margen = new XRect(y, 345, 100, 60);
                                        string Bioanalista1 = string.Format("LCD. {0} CB:{1} MPPS:{2}", Bioanalista.Tables[0].Rows[0]["NombreUsuario"].ToString(), Bioanalista.Tables[0].Rows[0]["CB"].ToString(), Bioanalista.Tables[0].Rows[0]["MPPS"].ToString());
                                        tf.Alignment = XParagraphAlignment.Center;
                                        tf.DrawString(Bioanalista1, fontRegular4, XBrushes.Black, Margen, XStringFormats.TopLeft);
                                        Firma = XImage.FromFile(string.Format(@"Firma\{1}", Ruta, Bioanalista.Tables[0].Rows[0]["FIRMA"].ToString()));
                                        gfx.DrawImage(Firma, y + 105, 345);
                                        y = y - 160;
                                    }
                                    y = 360;
                                    primerapagina = false;
                                    page = document.AddPage();

                                    if (Metodo == "Correo")
                                    {
                                        page.Height = 400;
                                    }
                                    gfx = XGraphics.FromPdfPage(page);
                                    tf = new XTextFormatter(gfx);
                                    Posicion = 110;
                                    Bio.Clear();
                                    IdAnalisisBio.Clear();
                                    Bioanalista = Conexion.Bioanalista(IdOrden, Convert.ToInt32(r["IdAnalisis"].ToString()));
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
                                        IdAnalisisBio.Add(Convert.ToInt32(r["IdAnalisis"].ToString()));
                                        Bio.Add(Convert.ToInt32(Bioanalista.Tables[0].Rows[0]["IdUsuario"].ToString()));
                                    }
                                }
                            }
                            else
                            {

                                Bioanalista = Conexion.Bioanalista(IdOrden, Convert.ToInt32(r["IdAnalisis"].ToString()));
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
                                    IdAnalisisBio.Add(Convert.ToInt32(r["IdAnalisis"].ToString()));
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

                            if (r == dsPrint.Tables[0].Rows[dsPrint.Tables[0].Rows.Count - 1])
                            {
                                foreach (int i in IdAnalisisBio)
                                {
                                    Bioanalista = new DataSet();
                                    Bioanalista = Conexion.Bioanalista(IdOrden, i);
                                    Margen = new XRect(y, 345, 100, 60);
                                    string Bioanalista1 = string.Format("LCD. {0} CB:{1} MPPS:{2}", Bioanalista.Tables[0].Rows[0]["NombreUsuario"].ToString(), Bioanalista.Tables[0].Rows[0]["CB"].ToString(), Bioanalista.Tables[0].Rows[0]["MPPS"].ToString());
                                    tf.Alignment = XParagraphAlignment.Center;
                                    tf.DrawString(Bioanalista1, fontRegular4, XBrushes.Black, Margen, XStringFormats.TopLeft);
                                    Firma = XImage.FromFile(string.Format(@"Firma\{1}", Ruta, Bioanalista.Tables[0].Rows[0]["FIRMA"].ToString()));
                                    gfx.DrawImage(Firma, y + 105, 345);
                                    y = y - 160;
                                }
                                Bio.Clear();
                                IdAnalisisBio.Clear();
                            }
                            double MargenCentrado = Measure.Height * total;
                            double PosicionCentrada = Posicion + 5;


                            //Analisis

                            if (r["Titulo"].ToString() == "1")
                            {
                                fontRegular3 = new XFont(facename, 10, XFontStyle.Regular);
                                rect = new XRect(30, PosicionCentrada, 225, MargenCentrado);
                                gfx.DrawString(Analisis, fontRegular3, XBrushes.Black, rect, XStringFormats.CenterLeft);
                            }
                            else
                            {
                                if (r["IdAnalisis"].ToString() == "27")
                                {
                                    if (Observaciones == "" || Observaciones == " ")
                                    {
                                        if (MargenCentrado > 15)
                                        {
                                            PosicionCentrada = PosicionCentrada + (MargenCentrado / 2 - Measure.Height / 2);
                                        }
                                    }
                                    rect = new XRect(10, Posicion + 5, 230, Measure.Height * total);
                                    tf.Alignment = XParagraphAlignment.Left;
                                    gfx.DrawRectangle(pen, rect);
                                    rect = new XRect(12, PosicionCentrada, 225, MargenCentrado);
                                    tf.DrawString(Analisis, fontRegular3, XBrushes.Black, rect, XStringFormats.TopLeft);
                                    rect = new XRect(242, Posicion + 5, 100 + 233, Measure.Height * total);
                                    tf.Alignment = XParagraphAlignment.Left;
                                    gfx.DrawRectangle(pen, rect);
                                    rect = new XRect(285, PosicionCentrada, 190, MargenCentrado);
                                    tf.DrawString(r["ValorResultado"].ToString() + " " + r["Unidad"].ToString(), fontRegular5, XBrushes.Black, rect, XStringFormats.TopLeft);
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
                                    tf.DrawString(r["ValorResultado"].ToString() + " " + r["Unidad"].ToString(), fontRegular3, XBrushes.Black, rect, XStringFormats.TopLeft);
                                    rect = new XRect(345, Posicion + 5, 230, Measure.Height * total);
                                    tf.Alignment = XParagraphAlignment.Justify;
                                    gfx.DrawRectangle(pen, rect);
                                    rect = new XRect(350, Posicion + 5, 220, Measure.Height * total);
                                    tf.DrawString(text, fontRegular5, XBrushes.Black, rect, XStringFormats.TopLeft);
                                    rect = new XRect(347, Posicion + 5, 180, Measure.Height * total);
                                }

                            }


                            //Respuesta



                            //Unidad
                            //rect = new XRect(515, Posicion+5, 60, Measure.Height * total);
                            //tf.Alignment = XParagraphAlignment.Center;
                            //gfx.DrawRectangle(pen, rect);
                            //rect = new XRect(517, Posicion+5, 60, Measure.Height * total);
                            //tf.DrawString(r["Unidad"].ToString(), fontRegular2, XBrushes.Black, rect, XStringFormats.TopLeft);

                            // Valores de Referencia


                            if (Observaciones != "" && Observaciones != " ")
                            {
                                if (total > 1)
                                {
                                    ObservacionesWidth = ObservacionesString.Width / 230;
                                    rect = new XRect(10, Posicion = Posicion + 17, 300, MargenAncho * total - 15);
                                    tf.Alignment = XParagraphAlignment.Left;
                                    rect = new XRect(12, Posicion, 230, MargenAncho * total - 15);
                                    tf.DrawString(string.Format("Observaciones: {0}", Observaciones), fontRegular2, XBrushes.Black, rect, XStringFormats.TopLeft);
                                    Posicion = Posicion + Measure.Height * total;
                                }
                                else
                                {
                                    rect = new XRect(10, Posicion = Posicion + MargenAncho + 3, 565, MargenAncho * ObservacionesWidth);
                                    tf.Alignment = XParagraphAlignment.Left;
                                    gfx.DrawRectangle(pen, blueBrush, rect);
                                    rect = new XRect(12, Posicion + 2, 550, MargenAncho * ObservacionesWidth);
                                    tf.DrawString(string.Format("Observaciones: {0}", Observaciones), fontRegular2, XBrushes.Black, rect, XStringFormats.TopLeft);

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

                            if (r["FinalTitulo"].ToString() == "1")
                            {
                                fontRegular3 = new XFont(facename, 10, XFontStyle.Regular);
                                rect = new XRect(12, PosicionCentrada, 225, MargenCentrado);
                                Posicion = Posicion + MargenAncho * total;
                            }

                        }


                    }

                }
            }
            catch (Exception ex)
            {
                Conexion.CrearEvento(ex.ToString());
            }
            return document;
        }
        private static void CorreoConvenio(PdfSharp.Pdf.PdfDocument document, int IdOrden, int userId)
        {

            string path = @"C:\TemporalesPDF\";
            VerificarRuta(path);
            string filename;
            DataSet Empresa = new DataSet();
            DataSet ds1 = new DataSet();
            DataSet Paciente = Conexion.PacienteAImprimir(IdOrden);
            filename = string.Format(path + "{0} {1} {2}.pdf", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString(), DateTime.Now.ToString("ddMMyyyyhhmmss"));
            document.Save(filename);
            Empresa = Conexion.CorreoEmpresa();
            ds1 = Conexion.SELECTConvenioEmail(Paciente.Tables[0].Rows[0]["IdConvenio"].ToString());
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(Empresa.Tables[0].Rows[0]["Correo"].ToString());
            SmtpClient smtp = new SmtpClient();
            smtp.Port = Convert.ToInt32(Empresa.Tables[0].Rows[0]["Puerto"].ToString());
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(Empresa.Tables[0].Rows[0]["Correo"].ToString(), Empresa.Tables[0].Rows[0]["Clave"].ToString(), "");
            smtp.Host = "smtp.gmail.com";

            //recipient
            mail.To.Add(new MailAddress(ds1.Tables[0].Rows[0]["CorreoSede"].ToString()));
            mail.Attachments.Add(new Attachment(filename));
            mail.IsBodyHtml = true;
            string st = "Examenes de Laboratorio";
            mail.Body = st;
            string UserState = "";
            smtp.Send(mail);
            string EnviadoPorCorreo = Conexion.EnviadoPorCorreo(IdOrden, userId);


        }
        private static void VerificarRuta(string ruta)
        {
            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }
        }
        private static void CorreoPaciente(PdfSharp.Pdf.PdfDocument document, int IdOrden, int userId)
        {
            try
            {
                string path = @"C:\TemporalesPDF\";
                VerificarRuta(path);


                string filename;
                DataSet Empresa = new DataSet();
                DataSet ds1 = new DataSet();

                DataSet Paciente = Conexion.PacienteAImprimir(Convert.ToInt32(IdOrden));
                Empresa = Conexion.CorreoEmpresa();
                filename = string.Format(path + "{0} {1} {2}.pdf", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString(), DateTime.Now.ToString("ddMMyyyyhhmmss"));
                document.Save(filename);
                ds1 = Conexion.SELECTPersonaEmail(IdOrden.ToString());
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(Empresa.Tables[0].Rows[0]["Correo"].ToString());
                SmtpClient smtp = new SmtpClient();
                smtp.Port = Convert.ToInt32(Empresa.Tables[0].Rows[0]["Puerto"].ToString());
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(Empresa.Tables[0].Rows[0]["Correo"].ToString(), Empresa.Tables[0].Rows[0]["Clave"].ToString(), "");
                smtp.Host = "smtp.gmail.com";
                string Correo = string.Format("{0}{1}", ds1.Tables[0].Rows[0]["Correo"].ToString(), ds1.Tables[0].Rows[0]["TipoCorreo"].ToString());
                //recipient
                mail.To.Add(new MailAddress(Correo));
                mail.Attachments.Add(new Attachment(filename));
                mail.IsBodyHtml = true;
                string st = "Examenes de Laboratorio";
                mail.Body = st;
                string UserState = "";
                smtp.Send(mail);
                string EnviadoPorCorreo = Conexion.EnviadoPorCorreo(Convert.ToInt32(IdOrden), userId);
            }
            catch (Exception ex)
            {
                Conexion.CrearEvento(ex.ToString());
            }


        }

        public static void SendEmail(int IdOrden, int IdAnalisis, string Metodo, int userId)
        {
            PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
            document = Documento(IdOrden, IdAnalisis, Metodo);
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
                Conexion.CrearEvento(e.ToString());
            }

        }

        public static void SendWhatsapp(int IdOrden, int IdAnalisis, string Metodo, int userId, string telefono)
        {
            PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
            document = Documento(IdOrden, IdAnalisis, Metodo);
            string path = @"C:\Whatsapp\";
            VerificarRuta(path);
            string filename;
            DataSet Paciente = Conexion.PacienteAImprimir(IdOrden);
            filename = string.Format(path + "{0} {1}.pdf", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString());
            document.Save(filename);
            DataSet Empresa = new DataSet();
            Empresa = Conexion.SelectEmpresaActiva();
            string NombrePaciente = string.Format("{0} {1}.pdf", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString());
            if (telefono != "" && telefono != " " && telefono != null)
            {
                ProcessStartInfo SendWhatsapp = new ProcessStartInfo(string.Format("https://web.whatsapp.com/send/?phone={0}&text=Saludos%20cordiales%20{1}%20{2}%20de%20parte%20de%20{3}%20Próximamente%20se%20les%20enviara%20sus%20resultados%20de%20laboratorio,%20si%20desea%20comunicarse%20con%20nosotros%20por%20favor%20llame%20a%20los%20siguientes%20números%20Tlf:%20{4}%20y%20Tlf:%20{5}.", telefono, Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString(), Empresa.Tables[0].Rows[0]["Nombre"].ToString(), Empresa.Tables[0].Rows[0]["Telefono"].ToString(), Empresa.Tables[0].Rows[0]["Telefono2"].ToString()));
                Process.Start(SendWhatsapp);
            }
        }
        public static void SendWhatsapp(int IdOrden, int IdAnalisis, string Metodo, int userId)
        {
            PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
            document = Documento(IdOrden, IdAnalisis, Metodo);
            string path = @"C:\Whatsapp\";
            VerificarRuta(path);
            string filename;
            DataSet Paciente = Conexion.PacienteAImprimir(IdOrden);
            filename = string.Format(path + "{0} {1}.pdf", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString());
            document.Save(filename);
            DataSet Empresa = new DataSet();
            Empresa = Conexion.SelectEmpresaActiva();
            string NombrePaciente = string.Format("{0} {1}.pdf", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString());
            string Telefono = "+58" + Paciente.Tables[0].Rows[0]["CodigoCelular"].ToString() + Paciente.Tables[0].Rows[0]["Celular"].ToString();
            if (Telefono != "" && Telefono != " " && Telefono != null)
            {
                ProcessStartInfo SendWhatsapp = new ProcessStartInfo(string.Format("https://web.whatsapp.com/send/?phone={0}&text=Saludos%20cordiales%20{1}%20{2}%20de%20parte%20de%20{3}%20Próximamente%20se%20les%20enviara%20sus%20resultados%20de%20laboratorio,%20si%20desea%20comunicarse%20con%20nosotros%20por%20favor%20llame%20a%20los%20siguientes%20números%20Tlf:%20{4}%20y%20Tlf:%20{5}.", Telefono, Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString(), Empresa.Tables[0].Rows[0]["Nombre"].ToString(), Empresa.Tables[0].Rows[0]["Telefono"].ToString(), Empresa.Tables[0].Rows[0]["Telefono2"].ToString()));
                Process.Start(SendWhatsapp);
            }
        }
        public static void SendWhatsapp(int IdOrden, int userId)
        {

            DataSet Paciente = Conexion.PacienteAImprimir(IdOrden);
            DataSet Empresa = new DataSet();
            Empresa = Conexion.SelectEmpresaActiva();
            string NombrePaciente = string.Format("{0} {1}.pdf", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString());
            string Telefono = "+58" + Paciente.Tables[0].Rows[0]["CodigoCelular"].ToString() + Paciente.Tables[0].Rows[0]["Celular"].ToString();
            if (Telefono != "" && Telefono != " " && Telefono != null)
            {
                ProcessStartInfo SendWhatsapp = new ProcessStartInfo(string.Format("https://web.whatsapp.com/send?phone={0}&text=Saludos%20cordiales%20{1}%20{2}%20de%20parte%20de%20{3}%20Próximamente%20se%20les%20enviara%20sus%20resultados%20de%20laboratorio,%20si%20desea%20comunicarse%20con%20nosotros%20por%20favor%20llame%20a%20los%20siguientes%20números%20Tlf:%20{4}%20y%20Tlf:%20{5}.", Telefono, Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString(), Empresa.Tables[0].Rows[0]["Nombre"].ToString(), Empresa.Tables[0].Rows[0]["Telefono"].ToString(), Empresa.Tables[0].Rows[0]["Telefono2"].ToString()));
                Process.Start(SendWhatsapp);
            }
        }
    }
}
