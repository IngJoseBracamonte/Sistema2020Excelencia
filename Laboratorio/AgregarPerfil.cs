using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Conexiones;
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class AgregarPerfil : Form
    {
        int idPerfil = 0;
        DataSet DataTasa = new DataSet();
        List<Servidores> Server = new List<Servidores>();
        Servidores servidores = new Servidores();
        List<Task> Tareas = new List<Task>();
        public AgregarPerfil(int _IdPerfil)
        {
          
            InitializeComponent();
            idPerfil = _IdPerfil;
            DataTasa = Conexion.SELECTTasaDia();
            Server = servidores.DatosDeServidores(Server);
            if (idPerfil != 0)
            {
                Perfil perfil = new Perfil();
                perfil = Conexion.selectPerfil(_IdPerfil);
                textBox5.Text = perfil.NombrePerfil;
                double precioDolar = Convert.ToDouble(perfil.PrecioDolar.ToString().Replace(".",","));
                PrecioDolar.Text = precioDolar.ToString();
                PrecioBs.Text = perfil.Precio.Replace(",", ".");
                if (perfil.Activo == 1)
                {
                    checkBox2.Checked = true;
                }
                else
                {
                    checkBox2.Checked = false;
                }
            }
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
        private async Task<string> ConexionAlServer(string cmd)
        {
            string results = await Conexion.AsyncTestConnection(cmd);
            if (results == "Conectado")
            {
                BtnGuardar();
            }
            return results;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            double Precio;
            string TPrecio = PrecioDolar.Text.Replace(".", ",");
            bool parse = Double.TryParse(TPrecio, out Precio);
            if (!parse)
            {
                return;
            }
            double TasaDia = Convert.ToDouble(DataTasa.Tables[0].Rows[0]["Dolar"].ToString().Replace(".", ","));
            double Resultado = TasaDia * Precio;
            PrecioBs.Text = Resultado.ToString().Replace(",", ".");
        }

        private void AgregarPerfil_Load(object sender, EventArgs e)
        {
            DataTasa = Conexion.SELECTTasaDia();
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
                        BtnGuardar();
                    }
                }
            }
            this.Close();

        }
        private void BtnGuardar()
        {
            Perfil perfil = new Perfil();
            string nombrePerfil = textBox5.Text.Trim();
            string Precio = PrecioBs.Text.Trim();
            if (!(nombrePerfil.Length > 0))
            {
                MessageBox.Show("Por favor ingrese un nombre del perfil");
                return;
            }
            if (!(Precio.Length > 0))
            {
                MessageBox.Show("Por favor ingrese un precio al perfil");
            }
            perfil.IdPerfil = idPerfil;
            perfil.NombrePerfil = nombrePerfil;
            perfil.Precio = Precio;
            perfil.PrecioDolar = Convert.ToDouble(PrecioDolar.Text.Replace(",", "."));
            if (checkBox2.Checked == true)
            {
                perfil.Activo = 1;
            }
            else
            {
                perfil.Activo = 0;
            }
            int Respuesta = 0;
            Respuesta = Conexion.ActualizarPerfil(perfil);

            if (Respuesta == 1)
            {
                MessageBox.Show("Perfil Agregado Satisfactoriamente");
                this.Close();
            }
            else
            {
                MessageBox.Show("Ha ocurrido un error");
            }
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

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
