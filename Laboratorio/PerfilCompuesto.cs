using System;
using Conexiones;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;
using Conexiones.DbConnect;
using Conexiones.Dto;
using FontAwesome.Sharp;

namespace Laboratorio
{
    public partial class PerfilCompuesto : Form
    {
        List<Servidores> Server = new List<Servidores>();
        List<Task> Tareas = new List<Task>();
        List<Perfil> perfiles = new List<Perfil>();
        Perfil perfil = new Perfil();
        Servidores servidores = new Servidores();
        List<AnalisisLaboratorio> ListadeAnalisis = new List<AnalisisLaboratorio>();
        public PerfilCompuesto()
        {
            InitializeComponent();
            Server = servidores.DatosDeServidores(Server);
        }

        private async Task<string> ConexionAlServer(string cmd)
        {
            string results = await Conexion.AsyncTestConnection(cmd);
            if (results == "Conectado")
            {
                insertarDatos();
            }
            return results;
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
                        insertarDatos();
                    }
                }
            }
            this.Close();

        }
        private bool ValidarDatosDePerfil()
        {
            if (TNombrePerfil.Text.Length < 0)
            {
                MessageBox.Show("Por favor, escriba un Nombre para el perfil");
                return false;
            }
            if (TPrecioDolar.Text.Length < 0)
            {
                MessageBox.Show("Por favor, escriba un Precio para el perfil");
                return false;
            }
            return true;
        }
        private void DatosDePerfil()
        {
            if (!ValidarDatosDePerfil())
            {
                return;
            }
            perfil.NombrePerfil = TNombrePerfil.Text;
            double.TryParse(TPrecioDolar.Text, out double PrecioDolar);
            perfil.PrecioDolar = PrecioDolar;
            perfil.Precio = PrecioBs.Text.Replace(",", ".");
            if (checkActivo.Checked)
            {
                perfil.Activo = 1;
            }
            else 
            {
                perfil.Activo = 0;
            }
          
        }
        private void insertarDatos()
        {
            DatosDePerfil();

            if (dataGridView2.Rows.Count > 0)
            {
                foreach (DataGridViewRow r in dataGridView2.Rows)
                {
                    AnalisisLaboratorio analisis = new AnalisisLaboratorio();
                    analisis.IdAnalisis = Convert.ToInt32(r.Cells["IdAnalisis"].Value);
                    analisis.idOrganizador = Convert.ToInt32(r.Cells["idOrganizador"].Value);
                    ListadeAnalisis.Add(analisis);
                }
            }
            int respuesta = Conexion.InsertarPerfilCompuesto(perfil, ListadeAnalisis);
            if (respuesta > 0)
            {
                MessageBox.Show("Agregado Satisfactoriamente");
            }
            else
            {
                MessageBox.Show("Ha ocurrido un Error");
            }

        }

        private void PerfilCompuesto_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = perfiles = Conexion.selectListaDePerfiles().ToList();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            try
            {
                int index = dataGridView1.CurrentCell.RowIndex;
                int IdPerfil = 0;
                int.TryParse(dataGridView1.Rows[index].Cells["IdPerfil"].Value.ToString(), out IdPerfil);
                if (index < 0)
                {
                    MessageBox.Show("Debe seleccionar Analisis la lista de la izquierda");
                    return;
                }

                var perfilseleccionado = new Perfil();
                perfilseleccionado = perfiles.Where(p=> p.IdPerfil == IdPerfil).First();
                perfilseleccionado.analisisLaboratorios = Conexion.selectAnalisisAgrupadosPorPerfil(perfilseleccionado.IdPerfil);
                bool encontrado = false;
                if (perfilseleccionado.analisisLaboratorios.Count > 0)
                {
                    foreach (var analisis in perfilseleccionado.analisisLaboratorios)
                    {

                        foreach (DataGridViewRow re in dataGridView2.Rows)
                        {
                            if (analisis.IdAnalisis.ToString() == re.Cells["IdAnalisis"].Value.ToString())
                            {
                                encontrado = true;
                            }

                        }

                        if (!encontrado)
                        {
                            dataGridView2.Rows.Add(analisis.IdAnalisis, analisis.NombreAnalisis, analisis.idOrganizador);
                        }
                        else
                        {
                            MessageBox.Show("Analisis ya agregado");
                        }
                    }
                }

                if (dataGridView2.Rows.Count > 0)
                {
                    dataGridView2.Sort(dataGridView2.Columns["idOrganizador"], ListSortDirection.Ascending);
                }
            }
            catch (Exception ex)
            {
                Conexion.CrearEvento(ex.ToString());
            }
        }

        private void TPrecioDolar_TextChanged(object sender, EventArgs e)
        {
            double tasa = 0;
            double PrecioDolar = 0;
            double Calculo = 0;
            DataSet DataTasa = new DataSet();
            DataTasa = Conexion.SELECTTasaDia();
            if (DataTasa.Tables.Count > 0)
            {
                if (DataTasa.Tables[0].Rows.Count > 0)
                {
                    double.TryParse(DataTasa.Tables[0].Rows[0]["Dolar"].ToString().Replace(".", ","), out tasa);
                    double.TryParse(TPrecioDolar.Text, out PrecioDolar);

                    Calculo = tasa * PrecioDolar;
                    PrecioBs.Text = Calculo.ToString();

                }
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = new List<Perfiles>();
            dataGridView1.DataSource = perfiles.Where(x => x.NombrePerfil.Contains(textBox1.Text) || x.IdPerfil.Equals(textBox1.Text)).ToList();
        }

        private void TPrecioDolar_KeyPress(object sender, KeyPressEventArgs e)
        {
          if(e.KeyChar == '\b')
            {
                e.Handled = false;
            }
            else if (e.KeyChar >= 48 && e.KeyChar <= 57/*Admite los numeros del 0 al 9*/)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

      
        private void iconButton4_Click(object sender, EventArgs e)
        {
            
       
        }
    }
}
