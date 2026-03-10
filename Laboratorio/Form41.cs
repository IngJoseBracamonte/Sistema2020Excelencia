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
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class Form41 : Form
    {
        PdfiumViewer.PdfViewer pdf = new PdfViewer();
        PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
        PdfiumViewer.PdfDocument document1;
        public static string filename;
        string IdOrden;
        DataSet Paciente = new DataSet();
        string path = @"Referidos\";
        public Form41()
        {
            InitializeComponent();
        }

        private void Form41_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
            }
            try
            {
                Hematologia(document);
                filename = string.Format(path+"{0}.pdf", DateTime.Now.ToString("ddMMyyyyhhmmss"));
                document.Save(filename);
                document1 = PdfiumViewer.PdfDocument.Load(filename);
                pdfViewer1.Renderer.Load(document1);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Hematologia(PdfSharp.Pdf.PdfDocument document)
        {
            const string facename = "Arial Rounded MT";
            XFont fontRegular = new XFont(facename, 11, XFontStyle.Regular);
            XFont fontRegular2 = new XFont(facename, 15, XFontStyle.Regular);
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
            tf.Alignment = XParagraphAlignment.Center;
            int MargenAncho = 15;
            PosicionP = 0;
            Empresa = Conexion.SelectEmpresaActiva();
            gfx.DrawString(Empresa.Tables[0].Rows[0]["Nombre"].ToString() + " - Fecha: "+ DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
            Margen = new XRect(5, PosicionP = PosicionP + 20, 580, 15);
            foreach (DataRow r in Form24.data.Tables[0].Rows)
            {
              
                 if (IdOrden != r["IdOrden"].ToString())
                {
                    int numberOfRecords = Form24.data.Tables[0].AsEnumerable().Where(x => x["IdOrden"].ToString() == r["IdOrden"].ToString()).ToList().Count;
                    if ( PosicionP+numberOfRecords * MargenAncho > 360)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        tf = new XTextFormatter(gfx);
                        PosicionP = 0;
                        Margen = new XRect(5, 15, 145, 14);
                        gfx.DrawString(Empresa.Tables[0].Rows[0]["Nombre"].ToString() + " - Fecha: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), fontRegular2, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                        Margen = new XRect(5, PosicionP = PosicionP + 20, 580, 15);
                    }
                    gfx.DrawLine(pen, 5, PosicionP= PosicionP + 15, 580, PosicionP);
                    IdOrden = r["IdOrden"].ToString();
                    Paciente = Conexion.PacienteAImprimir(Convert.ToInt32(r["IdOrden"].ToString()));
                    Margen = new XRect(10, PosicionP = PosicionP + 5, 145, 14);
                    gfx.DrawString(string.Format("Paciente #{4} {0} {1},      C.I: {2},       Fecha de Nacimiento: {3}", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString(), Paciente.Tables[0].Rows[0]["Cedula"].ToString(), Paciente.Tables[0].Rows[0]["Fecha"].ToString(), r["NumeroDia"].ToString()), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                    Margen = new XRect(10, PosicionP = PosicionP + 15, 135, 14);
                    gfx.DrawString("- " + r["NombreAnalisis"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);
                }
                else 
                {
                    Margen = new XRect(10, PosicionP = PosicionP + 15, 135, 14);
                    gfx.DrawString("- " + r["NombreAnalisis"].ToString(), fontRegular, XBrushes.Black, Margen, XStringFormats.CenterLeft);

                }
                Conexion.ActualizarPorEnviar(r["IdOrden"].ToString(),r["IdAnalisis"].ToString());
             
            }
            gfx.DrawLine(pen, 5, PosicionP = PosicionP + 15, 580, PosicionP);
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
                        dialogPrint.PrinterSettings.MinimumPage = 1;
                        dialogPrint.PrinterSettings.MaximumPage = document.PageCount;
                        dialogPrint.PrinterSettings.FromPage = 1;
                        dialogPrint.PrinterSettings.ToPage = document.PageCount;
                        pd.DocumentName = string.Format("Referidos {0}", DateTime.Now.ToString("ddMMyyyyhhmmss"));
                        pd.PrinterSettings.FromPage = dialogPrint.PrinterSettings.FromPage;
                        pd.PrinterSettings.ToPage = dialogPrint.PrinterSettings.ToPage;
                        pd.PrinterSettings.PrinterName = dialogPrint.PrinterSettings.PrinterName;
                        pd.Print();
                        document1.Dispose();
                    }
                }
            }
        }
    }
}
