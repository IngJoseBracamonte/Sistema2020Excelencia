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
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class HojadeTrabajo : Form
    {

        bool error = false;
        DataSet dsPrint = new DataSet();
        PdfiumViewer.PdfViewer pdf = new PdfViewer();
        PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
        PdfiumViewer.PdfDocument document1;
        string path = @"C:\TemporalesPDF\";
        public static string filename;
        DataSet Paciente = new DataSet();
        bool impresion;
        bool primerapagina;
        DateTime FechaDesde;
        DateTime FechaHasta;
        public HojadeTrabajo(DateTime fechaDesde,DateTime fechaHasta)
        {
            FechaDesde = fechaDesde;
            FechaHasta = fechaHasta;
            InitializeComponent();
        }

        private void HojadeTrabajo_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
            }
            try
            {
                ImpresionDeHoja(document,FechaDesde,FechaHasta);
                filename = string.Format("HojaDeTrabajo{0}.pdf", DateTime.Now.ToString("ddMMyyyyhhmmss"));
                document.Save(filename);
                document1 = PdfiumViewer.PdfDocument.Load(filename);
                pdfViewer1.Renderer.Load(document1);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void ImpresionDeHoja(PdfSharp.Pdf.PdfDocument document,DateTime FechaDesde,DateTime FechaHasta)
        {
           
            DataSet Analisis = new DataSet();
            int PosicionX = 10;

            const string facename = "Arial Rounded MT";
            XFont fontRegular = new XFont(facename, 11, XFontStyle.Regular);
            XFont fontRegular2 = new XFont(facename, 11, XFontStyle.Regular);
            Double PosicionP = 90;
            XBrush brushes;
            XSize Elipsesize = new XSize(5, 5);
            XRect Margen = new XRect(5, 0, 145, 14);
            PdfSharp.Pdf.PdfPage page;
            XGraphics gfx;
            XTextFormatter tf;
            DataSet Empresa = new DataSet();
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            fontRegular = new XFont(facename, 11, XFontStyle.Regular);
            PosicionP = 90;
            Margen = new XRect(5, 15, 145, 14);
            page = document.AddPage();
            page.Orientation = PdfSharp.PageOrientation.Portrait;
            gfx = XGraphics.FromPdfPage(page);

            tf = new XTextFormatter(gfx);
            tf.Alignment = XParagraphAlignment.Left;
            int MargenAncho = 0;
            int Posicion = 20;
            PosicionP = Posicion;
            Empresa = Conexion.SelectEmpresaActiva();
            int Conteo = 0;
            DataSet Examenes = new DataSet();
            var ToDate = FechaHasta.Date;
            try 
            {
                for (var Day = FechaDesde.Date; Day.Date <= ToDate.Date; Day = Day.AddDays(1))
                {
                    Examenes = Conexion.ListadeTrabajo(Day.ToString("yyyy/MM/dd"),FechaHasta.ToString("yyyy/MM/dd"));
                    if (Examenes.Tables.Count > 0)
                    {
                        if (Examenes.Tables[0].Rows.Count > 0)
                        {

                            gfx.DrawString("Lista de Trabajo - Fecha: " + Day.ToString("dd/MM/yyyy hh:mm:ss"), fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                            XPoint point = new XPoint(5, 70);
                            XRect rect;
                            PosicionP = PosicionP + 20;
                            foreach (DataRow r in Examenes.Tables[0].Rows)
                            {
                                if (PosicionX < 430)
                                {
                                    Analisis = Conexion.AnalisisListadeTrabajo(r["IdAnalisis"].ToString(), Day.ToString("yyyy/MM/dd"));
                                    Conteo = Analisis.Tables[0].Rows.Count;
                                    if (MargenAncho <= Conteo * 13)
                                    {
                                        MargenAncho = (1 + Conteo) * 13;
                                    }
                                    rect = new XRect(PosicionX, PosicionP, 140, (1 + Conteo) * 13);
                                    string text1 = " - " + r["Etiquetas"].ToString();
                                    foreach (DataRow row in Analisis.Tables[0].Rows)
                                    {
                                        text1 += string.Format("\n {0}", row["NumeroDia"].ToString());
                                    }
                                    tf.DrawString(text1, fontRegular2, XBrushes.Black, rect, XStringFormats.TopLeft);
                                    gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                                    PosicionX = PosicionX + 145;
                                    if (PosicionP > 600 && (r != Examenes.Tables[0].Rows[dsPrint.Tables[0].Rows.Count - 1]))
                                    {
                                        page = document.AddPage();
                                        gfx = XGraphics.FromPdfPage(page);
                                        tf = new XTextFormatter(gfx);
                                        PosicionX = 10;
                                        PosicionP = 20;
                                        MargenAncho = 0;
                                    }
                                }
                                else
                                {
                                    PosicionX = 10;
                                    PosicionP = PosicionP + MargenAncho + 5;
                                    rect = new XRect(PosicionX, PosicionP, 140, 50);
                                    tf.Alignment = XParagraphAlignment.Left;
                                    MargenAncho = 0;
                                    Analisis = Conexion.AnalisisListadeTrabajo(r["IdAnalisis"].ToString(), Day.ToString("yyyy/MM/dd"));
                                    Conteo = Analisis.Tables[0].Rows.Count;
                                    if (MargenAncho < Conteo * 13)
                                    {
                                        MargenAncho = (1 + Conteo) * 13;
                                    }
                                    rect = new XRect(PosicionX, PosicionP, 140, (1 + Conteo) * 13);
                                    string text1 = " - " + r["Etiquetas"].ToString();
                                    foreach (DataRow row in Analisis.Tables[0].Rows)
                                    {
                                        text1 += string.Format("\n {0}", row["NumeroDia"].ToString());
                                    }
                                    tf.DrawString(text1, fontRegular2, XBrushes.Black, rect, XStringFormats.TopLeft);
                                    gfx.DrawRoundedRectangle(pen, rect, Elipsesize);
                                    PosicionX = PosicionX + 145;
                                    if (PosicionP > 600 && (r != Examenes.Tables[0].Rows[Examenes.Tables[0].Rows.Count - 1]))
                                    {
                                        page = document.AddPage();
                                        gfx = XGraphics.FromPdfPage(page);
                                        tf = new XTextFormatter(gfx);
                                        PosicionX = 10;
                                        PosicionP = 20;
                                        MargenAncho = 0;
                                    }
                                }
                            }
                            if (ToDate.Date != Day.Date)
                            {
                                page = document.AddPage();
                                gfx = XGraphics.FromPdfPage(page);
                                tf = new XTextFormatter(gfx);
                                PosicionX = 10;
                                PosicionP = 20;
                                MargenAncho = 0;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
     



        }

        private void pdfViewer1_Load(object sender, EventArgs e)
        {

        }

        private void imprimirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDialog dialogPrint = new PrintDialog();
            dialogPrint.AllowPrintToFile = true;
            dialogPrint.AllowSomePages = true;
            if (dialogPrint.ShowDialog() == DialogResult.OK)
            {
                using (var document = PdfDocument.Load(filename))
                {
                    using (var pd = document.CreatePrintDocument())
                    {
                        pd.DocumentName = string.Format("Hoja De Trabajo {0}", DateTime.Now.ToString("ddMMyyyyhhmmss"));
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
            }
        }

        private void HojadeTrabajo_FormClosing(object sender, FormClosingEventArgs e)
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
