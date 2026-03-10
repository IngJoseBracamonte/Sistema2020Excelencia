using System;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfiumViewer;
using PdfDocument = PdfiumViewer.PdfDocument;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Script.Serialization;
using Conexiones.DbConnect;
using Conexiones.Impresion;

namespace Laboratorio
{
    public partial class Impresion : Form
    {
        HttpClient httpClient = new HttpClient();
        public class Enviados
        {
            public string NumeroPersona { get; set; }
            public string Mensaje { get; set; }
            public string Archivo { get; set; }
            
            public string Sede { get; set; }
        }
        bool error = false;
        DataSet dsPrint = new DataSet();
        PdfiumViewer.PdfViewer pdf = new PdfViewer();
        PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
        PdfiumViewer.PdfDocument document1;
        const string Curva = "Bookman Old Style";
        const string Curva2 = "Bookman Old Style";
        public static string filename;
        DataSet Paciente = new DataSet();
        bool impresion;
        string path = @"C:\TemporalesPDF\";
        bool primerapagina,cerrar;
        string Metodo;
        int IdOrden, IdAnalisis,IdUser;
        public Impresion(string metodo,int idOrden, int idAnalisis,int idUser)
        {
   
            InitializeComponent();
            IdUser = idUser;
            Metodo = metodo;
            IdOrden = idOrden;
            IdAnalisis = idAnalisis;

        }
        private void Cargar(int IdOrden)
        {
            DataSet Empresa = new DataSet();
            Empresa = Conexion.CorreoEmpresa();
            Paciente = Conexion.PacienteAImprimir(IdOrden);
            document = Impresiones.Documento(IdOrden, IdAnalisis, Metodo);
            filename = string.Format(path + "{0} {1} {2}.pdf", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString(), DateTime.Now.ToString("ddMMyyyyhhmmss"));
            document.Save(filename);
        }

        public bool IsFileReady(string filename)
        {
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private void Impresion_Load(object sender, EventArgs e)
        {
            PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
            Cargar(IdOrden);
            bool list = IsFileReady(filename);
            document1 = PdfiumViewer.PdfDocument.Load(filename);
            pdfViewer1.Renderer.Load(document1);
            pdfViewer1.Renderer.Zoom = 2.1;

        }

        private void imprimirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDialog dialogPrint = new PrintDialog();
            dialogPrint.AllowPrintToFile = true;
            dialogPrint.AllowSomePages = true;
            if (dialogPrint.ShowDialog() == DialogResult.OK)
            {
               var Send= Task.Run(() => 
                {
                    using (var document = PdfDocument.Load(filename))
                    {
                        using (var pd = document.CreatePrintDocument())
                        {
                            pd.DocumentName = string.Format("{0} {1} {2}.pdf", Paciente.Tables[0].Rows[0]["Nombre"].ToString(), Paciente.Tables[0].Rows[0]["Apellidos"].ToString(), DateTime.Now.ToString("ddMMyyyyhhmmss"));
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
                            
                        }
                    }
                });
                Send.Wait();
                this.Close();
            }
        }
        private void RunAsync(string Numero, string NombreArchivo, string DireccionArchivo, string NombreLab)
        {
    
            Enviados product = new Enviados();
            product.NumeroPersona = "58" + Numero;
            product.Mensaje = NombreArchivo;
            product.Archivo = DireccionArchivo;
            product.Sede = NombreLab;


            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://192.168.0.114:3000/Mensaje");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            string json = new JavaScriptSerializer().Serialize(new
            {
                phone = product.NumeroPersona,
                Name = product.Mensaje,
                file = @product.Archivo,
                sede = product.Sede
            });
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            try
            {
                var MensajeInicial = httpClient.PostAsync("http://192.168.0.114:3000/Mensaje", content).Result;

            }
            catch (Exception ex)
            {
               
            }
           
        }
        private void RunAsync2(string Numero, string NombreArchivo, string DireccionArchivo, string NombreLab)
        {
      
            Enviados product = new Enviados();
            product.NumeroPersona = "58" + Numero;
            product.Mensaje = NombreArchivo;
            product.Archivo = DireccionArchivo;
            product.Sede = NombreLab;


            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://192.168.0.114:3000/MensajeSaludo");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            string json = new JavaScriptSerializer().Serialize(new
            {
                phone = product.NumeroPersona,
                Name = product.Mensaje,
                file = @product.Archivo,
                sede = product.Sede
            });
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            try
            {
                var MensajeInicial = httpClient.PostAsync("http://192.168.0.114:3000/MensajeSaludo", content).Result;

            }
            catch (Exception ex)
            {

            }

        }
        private void Impresion_FormClosed(object sender, FormClosedEventArgs e)
        {
  
            
        }

        private void pdfViewer1_Load(object sender, EventArgs e)
        {

        }
        private void CantidadDeImpresiones(int From,int to)
        {

            
        }
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }
        private void PrintButton_Click(object sender, EventArgs e)
        {

        
        }

        private void Impresion_FormClosing(object sender, FormClosingEventArgs e)
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

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}
