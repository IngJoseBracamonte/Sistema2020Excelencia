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
    public partial class ListaDePrecios : Form
    {
        DataSet dsPrint = new DataSet();
        PdfiumViewer.PdfViewer pdf = new PdfViewer();
        PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
        PdfiumViewer.PdfDocument document1;
        public static string filename;
        string path = @"C:\TemporalesPDF\";
        const string facename = "Arial Rounded MT";
        XFont fontRegular = new XFont(facename, 14, XFontStyle.Regular);
        XFont fontRegular2 = new XFont(facename, 9, XFontStyle.Regular);
        Double PosicionP = 20;
        Double Posicion = 110;
        XBrush brushes;
        Double y = 0;
        DateTime FechaImp = new DateTime();
        int MargenAncho = 15;
  
        public ListaDePrecios()
        {
            InitializeComponent();
        }
        private void ListaDePrecios_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
            }
            try
            {
                HojadeImpresion(document);
                filename = string.Format(path +"ListaDePrecios{0}.pdf", DateTime.Now.ToString("ddMMyyyyhhmmss"));
                document.Save(filename);
                document1 = PdfiumViewer.PdfDocument.Load(filename);
                pdfViewer1.Renderer.Load(document1);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void HojadeImpresion(PdfSharp.Pdf.PdfDocument document)
        {
            XRect Margen = new XRect(10, 10, 145, 14);
            PdfSharp.Pdf.PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            string cmd = "Lista de Precios - Fecha: " + DateTime.Now.ToString("dd/MM/yyyy");
            gfx.DrawString(cmd, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);

            XSize sizeH = PageSizeConverter.ToSize(PdfSharp.PageSize.A4);
            PosicionP = PosicionP + 20;
            DataSet ListadePrecios = new DataSet();
            ListadePrecios = Conexion.ListaPreciosAImprimir();
            Margen = new XRect(30, PosicionP, 145, 14);
            gfx.DrawString(" ID ", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(80, PosicionP, 145, 14);
            gfx.DrawString("Nombre de Analisis ", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(330, PosicionP, 145, 14);
            gfx.DrawString("Precio Bs ", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(450, PosicionP, 145, 14);
            gfx.DrawString("Precio Bs", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            PosicionP = PosicionP + 20;
            foreach (DataRow r in ListadePrecios.Tables[0].Rows)
            {
                Margen = new XRect(30, PosicionP, 145, 14);
                gfx.DrawString(r["IdPerfil"].ToString(), fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                Margen = new XRect(80, PosicionP , 145, 14);
                gfx.DrawString(r["NombrePerfil"].ToString(), fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                Margen = new XRect(330, PosicionP, 145, 14);
                gfx.DrawString(Convert.ToDouble(r["Precio"].ToString()).ToString("N", new CultureInfo("es-VE")), fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                Margen = new XRect(450, PosicionP, 145, 14);
                gfx.DrawString(Convert.ToDouble(r["Bs"].ToString()).ToString("N", new CultureInfo("es-VE")), fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                PosicionP = PosicionP + 15;
                if (PosicionP > sizeH.Height - 100)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    PosicionP = 40;
                    Margen = new XRect(10, 10, 145, 14);
                    gfx.DrawString(cmd, fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                    ListadePrecios = Conexion.ListaPreciosAImprimir();
                    Margen = new XRect(30, PosicionP, 145, 14);
                    gfx.DrawString(" ID ", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                    Margen = new XRect(80, PosicionP, 145, 14);
                    gfx.DrawString("Nombre de Analisis ", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                    Margen = new XRect(330, PosicionP, 145, 14);
                    gfx.DrawString("Precio Bs ", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                    Margen = new XRect(450, PosicionP, 145, 14);
                    gfx.DrawString("Precio Bs", fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                    PosicionP = PosicionP + 20;
                }
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
                        pd.DocumentName = string.Format("Lista De precios {0}", DateTime.Now.ToString("dd/MM/yyyy"));
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

        private void ListaDePrecios_FormClosing(object sender, FormClosingEventArgs e)
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
