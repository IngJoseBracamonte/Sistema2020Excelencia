using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Text;
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class Form20 : Form
    {
        List<Servidores> Server = new List<Servidores>();
        List<Task> Tareas = new List<Task>();
        HttpClient httpClient = new HttpClient();
        private string Title { get; set; }
        private string Url { get; set; }
        private string siteUrl = "http://www.bcv.org.ve";
        public string[] QueryTerms { get; } = { "USD" };
        public Form20()
        {
            InitializeComponent();
            DatosDeServidores();
            DataSet Empresa = new DataSet();
            Empresa = Conexion.SelectEmpresaActiva();
            if (Empresa.Tables.Count != 0)
            {
                if (Empresa.Tables[0].Rows.Count != 0)
                {
                    if (Empresa.Tables[0].Rows[0]["IdEmpresa"].ToString() == "5")
                    {
                        ScrapeWebsite();
                    }
                }
            }
           
        }
        internal async void ScrapeWebsite()
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage request = await httpClient.GetAsync(siteUrl);
            cancellationToken.Token.ThrowIfCancellationRequested();

            Stream response = await request.Content.ReadAsStreamAsync();
            cancellationToken.Token.ThrowIfCancellationRequested();

            HtmlParser parser = new HtmlParser();
            IHtmlDocument document = parser.ParseDocument(response);

            //Add connection between initial scrape, and parsing of results
            GetScrapeResults(document);
        }

        private void GetScrapeResults(IHtmlDocument document)
        {
            IEnumerable<IElement> articleLink = null;

            foreach (var term in QueryTerms)
            {
                articleLink = document.All.Where(x => x.ClassName == "col-sm-12 col-xs-12 " && x.ParentElement.InnerHtml.Contains(term)).Skip(1);
                //Overwriting articleLink above means we have to print it's result for all QueryTerms
                //Appending to a pre-declared IEnumerable (like a List), could mean taking this out of the main loop.
                if (articleLink.Any())
                {
                    PrintResults(articleLink);
                }
            }
        }

        public void PrintResults(IEnumerable<IElement> articleLink)
        {
            //Every element needs to be cleaned and displayed
            foreach (var element in articleLink)
            {
                CleanUpResults(element);
                textBox4.Text = element.TextContent.Replace("USD", "").Trim();
            }
            double USD = Convert.ToDouble(textBox4.Text);
            Double Mate = Math.Round(USD, 2);
            textBox1.Text = Mate.ToString().Replace(",", "."); 

        }

        private void CleanUpResults(IElement result)
        {
            string htmlResult = result.InnerHtml.ReplaceFirst(" <img src = \"/sites/default/files/dollar-04_2.png\" class=\"icono_bss_blanco1\"> ", " ");
            htmlResult = htmlResult.ReplaceFirst("<span> USD</span>	 </div>", "USD");
            htmlResult = htmlResult.ReplaceFirst("< div class=\"col - sm - 6 col - xs - 6 centrado\"><strong> ", "");
            htmlResult = htmlResult.ReplaceFirst("</strong> </div>", "");

        }
        public class Servidores
        {
            public string iPServer { get; set; }
            public int idServer { get; set; }
            public string estado { get; set; }
        }
        private void SeleccionarServer(int Server)
        {
       
            if (Server == 0)
            {
               Conexion.Connection(ConfigurationManager.ConnectionStrings["RIVANA"].ConnectionString, "Rivana");
            } //RIVANA
            else if (Server == 1) //ArcosParada
            {
                Conexion.Connection(ConfigurationManager.ConnectionStrings["ARCOS PARADA"].ConnectionString, "ARCOS PARADA");
               
            }//ArcosParada
            else if (Server == 2)
            {

                Conexion.Connection(ConfigurationManager.ConnectionStrings["HOSPITALAB"].ConnectionString, "HOSPITALAB");
              
            }//Hospitalab
            else if (Server == 3)
            {
                Conexion.Connection(ConfigurationManager.ConnectionStrings["NARDO"].ConnectionString, "NARDO");
            }//Nardo
            else if (Server == 4)
            {

                Conexion.Connection(ConfigurationManager.ConnectionStrings["ARO"].ConnectionString, "ARO");
              

            }//Aro
            else if (Server == 5)
            {

                Conexion.Connection(ConfigurationManager.ConnectionStrings["ESPECIALES"].ConnectionString, "ESPECIALES");
                
            }
            else if (Server == 6)
            {

                Conexion.Connection(ConfigurationManager.ConnectionStrings["ARCOS PARADA 2"].ConnectionString, "ESPECIALES");

            }

        }
        private void DatosDeServidores()
        {
            Server.Add(new Servidores() { idServer = 0, iPServer = "RIVANA" });
            Server.Add(new Servidores() { idServer = 1, iPServer = "ARCOS PARADA" });
            Server.Add(new Servidores() { idServer = 2, iPServer = "HOSPITALAB" });
            Server.Add(new Servidores() { idServer = 3, iPServer = "NARDO" });
            Server.Add(new Servidores() { idServer = 4, iPServer = "ARO" });
            Server.Add(new Servidores() { idServer = 5, iPServer = "ESPECIALES" });
            Server.Add(new Servidores() { idServer = 9, iPServer = "ARCOS PARADA 2" });
        }
        private async Task<string> ConexionAlServer(string cmd)
        {
            string results = await Conexion.AsyncTestConnection(cmd);
            if (results == "Conectado")
            {
                Conexion.ActualizarPrecios(textBox1.Text, textBox2.Text, textBox3.Text);
            }
            return results;
        }
        private void Form20_Load(object sender, EventArgs e)
        {
                AcceptButton = iconButton2;
                DataSet ds = new DataSet();
                ds = Conexion.SELECTTasaDia();
                if (ds.Tables.Count != 0)
                {
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        textBox1.Text = ds.Tables[0].Rows[0]["Dolar"].ToString();
                        textBox2.Text = ds.Tables[0].Rows[0]["Pesos"].ToString();
                        textBox3.Text = ds.Tables[0].Rows[0]["Euros"].ToString();
                    }
                }
                else
                {
                    textBox1.Text = ds.Tables[0].Rows[0]["Dolar"].ToString();
                    textBox2.Text = ds.Tables[0].Rows[0]["Pesos"].ToString();
                    textBox3.Text = ds.Tables[0].Rows[0]["Euros"].ToString();
                }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {

                DataSet Empresa = new DataSet();
                Empresa = Conexion.SelectEmpresaActiva();
                if (Empresa.Tables.Count != 0)
                {
                    if (Empresa.Tables[0].Rows.Count != 0)
                    {
                        if (Empresa.Tables[0].Rows[0]["IdEmpresa"].ToString() == "5")
                        {
                            foreach (var items in Server)
                            {
                                Tareas.Add(ConexionAlServer(items.iPServer));
                            }
                            Task t = Task.WhenAll(Tareas);
                        }
                        else
                        {
                            Conexion.ActualizarPrecios(textBox1.Text, textBox2.Text, textBox3.Text);
                        }
                    }
                }
                this.Close();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
            {
                // Allow Digits and BackSpace char
            }
            else if (e.KeyChar == '.' && !((TextBox)sender).Text.Contains('.'))
            {
                //Allows only one Dot Char
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
            {
                // Allow Digits and BackSpace char
            }
            else if (e.KeyChar == '.' && !((TextBox)sender).Text.Contains('.'))
            {
                //Allows only one Dot Char
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
            {
                // Allow Digits and BackSpace char
            }
            else if (e.KeyChar == '.' && !((TextBox)sender).Text.Contains(','))
            {
                //Allows only one Dot Char
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
