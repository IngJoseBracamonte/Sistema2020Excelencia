using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MigraDoc.Rendering.Forms;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfiumViewer;
using System.Drawing.Printing;
using PdfDocument = PdfiumViewer.PdfDocument;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using PdfSharp;
using System.Globalization;
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class FormatoDePago : Form
    {
        DataSet dsPrint = new DataSet();
        PdfiumViewer.PdfViewer pdf = new PdfViewer();
        PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
        PdfiumViewer.PdfDocument document1;
        string path = @"C:\TemporalesPDF\";
        public static string filename;
        const string facename = "Arial Rounded MT";
        XFont fontRegular = new XFont(facename, 10, XFontStyle.Underline);
        XFont fontRegular2 = new XFont(facename,10, XFontStyle.Regular);
        XFont fontRegular3 = new XFont(facename, 14, XFontStyle.Bold);
        XFont fontRegular4 = new XFont(facename, 10, XFontStyle.Bold);
        Double PosicionP = 15;
        Double Posicion = 110;
        XBrush brushes;
        Double y = 0;
        DateTime FechaImp = new DateTime();
        int MargenAncho = 15;
        private int IdOrden;
        public FormatoDePago(int CobroIdOrden)
        {
            IdOrden = CobroIdOrden;
            InitializeComponent();
        }

        private void FormatoDePago_Load(object sender, EventArgs e)
        {
         
            
            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
            }
            try
            {
               
                filename = string.Format(path+"FormatoDePago{0}.pdf", DateTime.Now.ToString("ddMMyyyyhhmmss"));
                var t =Task.Run(() => 
                {
                    HojadeImpresion(document).Wait();
                    document.Save(filename);
                    document1 = PdfiumViewer.PdfDocument.Load(filename);
                    pdfViewer1.Renderer.Load(document1);
                   
                });
                t.Wait();
                pdfViewer1.Renderer.Zoom = 2.1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private async Task HojadeImpresion(PdfSharp.Pdf.PdfDocument document)
        {
            string cmd, cmd2, cmd3, cmd4, cmd5, cmd6, cmd7, cmd8;
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen;
            XSize size,length;
            XPen lineRed = new XPen(XColors.Black, 1);
            XRect rect;
            pen = new XPen(color);
            XSize Elipsesize = new XSize(5, 5);
            PdfSharp.Pdf.PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XPoint point;
            double contador;
            point = new XPoint(3, 3);
            size = new XSize(589, 394);
            rect = new XRect(point, size);
            gfx.DrawRectangle(pen, rect);
            DataSet Persona = new DataSet();
            DataSet Tasa = new DataSet();
            
          
            Persona = Conexion.PersonaCobro(IdOrden.ToString());
            DataSet ds4 = new DataSet();
            if (Persona.Tables[0].Rows[0]["IdTasa"].ToString() != "" && Persona.Tables[0].Rows[0]["IdTasa"].ToString() != " ")
            {
                Tasa = Conexion.SELECTTasaPorId(Persona.Tables[0].Rows[0]["IdTasa"].ToString());
            }
            ds4 = Conexion.SeleccionarPagos2(IdOrden.ToString());
            XRect Margen = new XRect(220, 15, 145, 14);
            string Titulo = "COMPROBANTE DE PAGO";
            gfx.DrawString(Titulo, fontRegular3, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            PosicionP = PosicionP + 15;
            Margen = new XRect(10, PosicionP, 145, 14);
            gfx.DrawString("Numero de Orden: ", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            cmd = "Numero de Orden: ";
            length = gfx.MeasureString(cmd, fontRegular2);
            contador = 10 + length.Width;
            Margen = new XRect(contador, PosicionP, 145, 14);
            gfx.DrawString(IdOrden.ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            PosicionP = PosicionP + 15;
            Margen = new XRect(10, PosicionP, 145, 14);
            cmd = "Fecha: ";
            cmd2 = DateTime.Now.ToString("dd/MM/yyyy");
            cmd3 = "Nombre del Paciente: ";
            cmd4 = Persona.Tables[0].Rows[0]["Nombre"].ToString() + " " + Persona.Tables[0].Rows[0]["Apellidos"].ToString();
            cmd5 = "C.I: ";
            cmd6 = Persona.Tables[0].Rows[0]["Cedula"].ToString();
            cmd7 = "#";
            cmd8 = Persona.Tables[0].Rows[0]["NumeroDia"].ToString();
            
           
            gfx.DrawString(cmd, fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);

            length = gfx.MeasureString(cmd, fontRegular2);
            contador = 10 + length.Width;
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


            length = gfx.MeasureString(cmd6, fontRegular2);
            contador = contador + length.Width + 5;
            Margen = new XRect(contador, PosicionP, 145, 14);
            gfx.DrawString(cmd7, fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);

            length = gfx.MeasureString(cmd7, fontRegular2);
            contador = contador + length.Width + 5;
            Margen = new XRect(contador, PosicionP, 145, 14);
            gfx.DrawString(cmd8, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);

            PosicionP = PosicionP + 15;
            DataSet ListadePrecios = new DataSet();
            ListadePrecios = Conexion.ListaPreciosAImprimir();
            Margen = new XRect(10, PosicionP, 145, 14);
            Double BolivaresT = Convert.ToDouble(Persona.Tables[0].Rows[0]["PrecioF"].ToString().Replace(".",","));
            Double BolivaresF = Convert.ToDouble(Tasa.Tables[0].Rows[0]["Dolar"].ToString().Replace(".", ","));
            string TotalT = string.Format("Bs.S. {0}", BolivaresT.ToString("N", new CultureInfo("es-VE")));
            string TotalF = string.Format("Bs.S. {0}", BolivaresF.ToString("N", new CultureInfo("es-VE")));
            Double Dolares = BolivaresT / BolivaresF;
            cmd = "Monto: ";
            length = gfx.MeasureString(cmd, fontRegular2);
            contador = 10;
            Margen = new XRect(contador, PosicionP, 145, 14);
            gfx.DrawString(cmd, fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);


            length = gfx.MeasureString(cmd, fontRegular2);
            contador = contador + length.Width+5;
            Margen = new XRect(contador, PosicionP, 145, 14);
            gfx.DrawString(TotalT, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);

            cmd = "Tasa: ";
            length = gfx.MeasureString(TotalT, fontRegular2);
            contador = contador + length.Width +5 ;
            Margen = new XRect(contador, PosicionP, 145, 14);
            gfx.DrawString(cmd, fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);

            length = gfx.MeasureString(cmd, fontRegular2);
            contador = contador + length.Width;
            Margen = new XRect(contador, PosicionP, 145, 14);
            gfx.DrawString(TotalF, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);

            cmd = "Dolares: ";
            length = gfx.MeasureString(TotalF, fontRegular2);
            contador = contador + length.Width + 5;
            Margen = new XRect(contador, PosicionP, 145, 14);
            gfx.DrawString(cmd, fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);

            length = gfx.MeasureString(cmd, fontRegular2);
            contador = contador + length.Width;
            Margen = new XRect(contador, PosicionP, 145, 14);
            gfx.DrawString(Dolares.ToString("C2", new CultureInfo("en-US")), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);

            PosicionP = PosicionP + 15;
            Margen = new XRect(10, PosicionP, 145, 14);
            gfx.DrawString("Tipo de Pago ", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(110, PosicionP, 145, 14);
            gfx.DrawString("Cantidad ", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(250, PosicionP, 145, 14);
            gfx.DrawString("Cantidad en Bolivares", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(450, PosicionP, 145, 14);
            gfx.DrawString("Serial", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);

            PosicionP = PosicionP + 15;
            Margen = new XRect(10, PosicionP, 145, 14);
            gfx.DrawString("Ingreso", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            PosicionP = PosicionP + 15;
           
            bool parte2 = true;
            foreach (DataRow r in ds4.Tables[0].Rows)
            {
                if (r["Clasificacion"].ToString() == "1")
                {
                    Margen = new XRect(30, PosicionP, 145, 14);
                    gfx.DrawString(r["TipoDePago"].ToString(), fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                    Margen = new XRect(130, PosicionP, 145, 14);

                    if (Convert.ToInt32(r["Moneda"].ToString()) > 2)
                    {
                        gfx.DrawString(r["Cantidad"].ToString(), fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);

                    }
                    Margen = new XRect(250, PosicionP, 145, 14);
                    Double BolivaresS = Convert.ToDouble(r["ValorResultado"].ToString());
                    string TotalS = string.Format("Bs.S. {0}", BolivaresS.ToString("N", new CultureInfo("es-VE")));
                    gfx.DrawString(TotalS, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                    Margen = new XRect(450, PosicionP, 145, 14);
                    gfx.DrawString(r["Serial"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                    PosicionP = PosicionP + 15;
                }
                else
                {
                    if (parte2)
                    {
                        PosicionP = PosicionP + 15;
                        Margen = new XRect(10, PosicionP, 145, 14);
                        gfx.DrawString("Vuelto", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        parte2 = false;
                    }
                    PosicionP = PosicionP + 15;
                    Margen = new XRect(30, PosicionP, 145, 14);
                    gfx.DrawString(r["TipoDePago"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                    Margen = new XRect(130, PosicionP, 145, 14);

                    if (Convert.ToInt32(r["Moneda"].ToString()) > 2)
                    {
                        gfx.DrawString(r["Cantidad"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);

                    }
                    Margen = new XRect(250, PosicionP, 145, 14);
                    Double BolivaresS = Convert.ToDouble(r["ValorResultado"].ToString());
                    string TotalS = string.Format("Bs.S. {0}", BolivaresS.ToString("N", new CultureInfo("es-VE")));
                    gfx.DrawString(TotalS, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                    Margen = new XRect(450, PosicionP, 145, 14);
                    gfx.DrawString(r["Serial"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                }
            }
            PosicionP = PosicionP + 50;
            gfx.DrawLine(lineRed, 589 - 589/4, PosicionP, 580, PosicionP);
            Margen = new XRect((589 - 589 / 4) - 30, PosicionP - 10, 140, 14);
            gfx.DrawString("Firma", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);

        }

        private void imprimirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDialog dialogPrint = new PrintDialog();
            dialogPrint.AllowPrintToFile = true;
            dialogPrint.AllowSomePages = true;
            if (dialogPrint.ShowDialog() == DialogResult.OK)
            {
                Task.Run(() =>
                {
                    using (var document = PdfDocument.Load(filename))
                    {
                        using (var pd = document.CreatePrintDocument())
                        {
                            pd.DocumentName = string.Format(path + "FormatoDePago{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                            pd.PrinterSettings.FromPage = dialogPrint.PrinterSettings.FromPage;
                            pd.PrinterSettings.ToPage = dialogPrint.PrinterSettings.ToPage;
                            pd.PrinterSettings.PrinterName = dialogPrint.PrinterSettings.PrinterName;
                            pd.Print();
                            Conexion.CantidaddeHojas(document.PageCount);
                            document.Dispose();
                            document1.Dispose();
                            try
                            {
                                File.Delete(filename);
                            }
                            catch
                            {

                            }
                            this.Close();
                        }
                    }
                });
                    
            }
        }

        private void pdfViewer1_Load(object sender, EventArgs e)
        {

        }

        private void FormatoDePago_FormClosing(object sender, FormClosingEventArgs e)
        {
            document.Dispose();
            document1.Dispose();
            try
            {
                File.Delete(filename);
            }
            catch
            {

            }
        }
    }
}
