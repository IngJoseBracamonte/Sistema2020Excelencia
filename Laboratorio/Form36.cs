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
using Conexiones.Dto;

namespace Laboratorio
{
    public partial class Form36 : Form
    {
        List<Task> Tareas = new List<Task>();
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
        List<Servidores> Server = new List<Servidores>();
        Servidores servidores = new Servidores();
        AnalisisLaboratorio analisisLaboratorio = new AnalisisLaboratorio();
        public Form36(AnalisisLaboratorio analisisSeleccionado)
        {
            InitializeComponent();
            analisisLaboratorio = analisisSeleccionado;
            Server = servidores.DatosDeServidores(Server);
            cargarDatos();
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

        private void BtnGuardar()
        {
            int Seccion = 0;
            switch (SeccionCombo.SelectedIndex)
            {
                //INDIVIDUAL 
                case 0:
                    Seccion = 0;
                    break;
                //COAGULACION 
                case 1:
                    Seccion = 1;
                    break;
                //QUIMICA
                case 2:
                    Seccion = 2;
                    break;
                //ORINA
                case 3:
                    Seccion = 3;
                    break;
                //HECES
                case 4:
                    Seccion = 4;
                    break;
                //ESPECIALES
                case 5:
                    Seccion = 6;
                    break;
                //CENTRALIZADOS
                case 6:
                    Seccion = 7;
                    break;
                default:
                    break;

            }
            analisisLaboratorio.NombreAnalisis = Nombre.Text;
            if (cuantitativoscheck.Checked)
            {
                analisisLaboratorio.TipoAnalisis = 5;
            }
            if (cualitativoscheck.Checked)
            {
                analisisLaboratorio.TipoAnalisis = 18;
            }
            analisisLaboratorio.IdSeccion = Seccion;
            analisisLaboratorio.Etiqueta = TEtiquetas.Text;
            if (Visible.Checked)
            {
                analisisLaboratorio.Visible = true;
            }
            if (especiales.Checked)
            {
                analisisLaboratorio.Especiales = 1;
            }
            else 
            {
                analisisLaboratorio.Especiales = 0;
            }
            mayoromenorreferencial ValoresRef = new mayoromenorreferencial();
            ValoresRef.Unidad = TUnidad.Text;
            if (string.IsNullOrEmpty(Tdesde.Text))
            {
                ValoresRef.ValorMenor = "0";

            }
            else
            {
                ValoresRef.ValorMenor = Tdesde.Text;
            }
            if (string.IsNullOrEmpty(Thasta.Text))
            {
                ValoresRef.ValorMayor = "0";

            }
            else
            {
                ValoresRef.ValorMayor = Thasta.Text;
            }
            ValoresRef.MultiplesValores = TValores.Text;
            ValoresRef.lineas = TValores.Lines.Count();
            ValoresRef.IdAnalisis = analisisLaboratorio.IdAnalisis;
            analisisLaboratorio.valoresDeReferencia = ValoresRef;
            Conexion.ActualizarAnalisis(analisisLaboratorio);
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

        private void Form36_Load(object sender, EventArgs e)
        {
            
        }
        private void cargarDatos()
        {
            Nombre.Text = analisisLaboratorio.NombreAnalisis;
            TEtiquetas.Text = analisisLaboratorio.Etiqueta;
            if (analisisLaboratorio.Visible)
            {
                Visible.Checked = true;
            }
            else
            {
                Visible.Checked = false;
            }
            if (analisisLaboratorio.Especiales == 1)
            {
                especiales.Checked = true;
            }
            else
            {
                especiales.Checked = false;
            }
            var VReferencia = analisisLaboratorio.valoresDeReferencia;
            TUnidad.Text = VReferencia.Unidad;
            if (string.IsNullOrEmpty(VReferencia.MultiplesValores))
            {
                cuantitativoscheck.Checked = true;
                Tdesde.Text = VReferencia.ValorMenor;
                Thasta.Text = VReferencia.ValorMayor;
            }
            else
            {
                cualitativoscheck.Checked = true;
                TValores.Text = VReferencia.MultiplesValores;
            }

            switch (analisisLaboratorio.TipoAnalisis)
            {
                //INDIVIDUAL 

                case 0:
                    SeccionCombo.SelectedIndex = 0;
                    break;
                //COAGULACION 
                case 1:
                    SeccionCombo.SelectedIndex = 1;
                    break;
                //QUIMICA
                case 2:
                    SeccionCombo.SelectedIndex = 2;
                    break;
                //ORINA
                case 3:
                    SeccionCombo.SelectedIndex = 3;
                    break;
                //HECES
                case 4:
                    SeccionCombo.SelectedIndex = 4;
                    break;
                //ESPECIALES
                case 6:
                    SeccionCombo.SelectedIndex = 5;
                    break;
                //CENTRALIZADOS
                case 7:
                    SeccionCombo.SelectedIndex = 6;
                    break;
                default:
                    break;
            }
        }

        private void cuantitativoscheck_CheckedChanged(object sender, EventArgs e)
        {
            if (cuantitativoscheck.Checked)
            {
                Tdesde.Enabled = true;
                Thasta.Enabled = true;
                cualitativoscheck.Checked = false;
                TValores.Enabled = false;
                TValores.Text = "";
            }
            else
            {
                Tdesde.Enabled = false;
                Thasta.Enabled = false;
                Tdesde.Text = "";
                Thasta.Text = "";
            }
        }

        private void cualitativoscheck_CheckedChanged(object sender, EventArgs e)
        {
            if (cualitativoscheck.Checked)
            {
                Tdesde.Enabled = false;
                Thasta.Enabled = false;
                Tdesde.Text = "";
                Thasta.Text = "";
                cuantitativoscheck.Checked = false;
                TValores.Enabled = true;
            }
            else
            {
                TValores.Enabled = false;
                TValores.Text = "";
            }
        }

        private void SeccionCombo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            
        }
    }
}
