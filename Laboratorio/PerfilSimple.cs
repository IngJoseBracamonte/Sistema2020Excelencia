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
    public partial class PerfilSimple : Form
    {
        int Seccion = 0;
        Perfil PERFIL = new Perfil();
        AnalisisLaboratorio analisis = new AnalisisLaboratorio();
        mayoromenorreferencial Valores = new mayoromenorreferencial();
        List<Servidores> Server = new List<Servidores>();
        Servidores servidores = new Servidores();
        List<Task> Tareas = new List<Task>();
        public PerfilSimple()
        {
            InitializeComponent();
            Server = servidores.DatosDeServidores(Server);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
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
                    double.TryParse(DataTasa.Tables[0].Rows[0]["Dolar"].ToString().Replace(".",","), out tasa);
                    double.TryParse(TPrecioDolar.Text,out PrecioDolar);

                    Calculo = tasa * PrecioDolar;
                    PrecioBs.Text = Calculo.ToString();
                    
                }
            }
        }






        //CENTRALIZADOS
        private void iconButton1_Click(object sender, EventArgs e)
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
                        Task.WhenAll(Tareas);
                    }
                    else
                    {
                        BtnGuardar();
                    }
                }
            }
            this.Close();
        }

        private void SeccionCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SeccionCombo.SelectedIndex > (-1))
            {
                TEtiquetas.Enabled = true;
            }
            else
            {
                TEtiquetas.Enabled = false;
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
        private async Task<string> ConexionAlServer(string cmd)
        {
            string results = await Conexion.AsyncTestConnection(cmd);
            if (results == "Conectado")
            {
                BtnGuardar();
            }
            return results;
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

        private void iconButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PerfilSimple_Load(object sender, EventArgs e)
        {

        }

        private void TPrecioDolar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
            {
                // Allow Digits and BackSpace char
            }
            else if (e.KeyChar == '.' && !((TextBox)sender).Text.Contains(','))
            {
                e.KeyChar = ',';
            }
            else if (e.KeyChar == ',' && !((TextBox)sender).Text.Contains(','))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Tdesde_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
            {
                // Allow Digits and BackSpace char
            }
            else if (e.KeyChar == '.' && !((TextBox)sender).Text.Contains(','))
            {
                e.KeyChar = ',';
            }
            else if (e.KeyChar == ',' && !((TextBox)sender).Text.Contains(','))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Thasta_TextChanged(object sender, EventArgs e)
        {

        }

        private void Thasta_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
            {
                // Allow Digits and BackSpace char
            }
            else if (e.KeyChar == '.' && !((TextBox)sender).Text.Contains(','))
            {
                e.KeyChar = ',';
            }
            else if (e.KeyChar == ',' && !((TextBox)sender).Text.Contains(','))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void TNombrePerfil_TextChanged(object sender, EventArgs e)
        {
          
        }
        private void BtnGuardar()
        {
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

            PERFIL.NombrePerfil = TNombrePerfil.Text;
            PERFIL.Precio = PrecioBs.Text.Replace(",", ".");
            PERFIL.PrecioDolar = Convert.ToDouble(TPrecioDolar.Text);
            if (checkActivo.Checked)
            {
                PERFIL.Activo = 1;
            }
            analisis.NombreAnalisis = TNombrePerfil.Text;
            if (cuantitativoscheck.Checked)
            {
                analisis.TipoAnalisis = 5;
            }
            if (cualitativoscheck.Checked)
            {
                analisis.TipoAnalisis = 18;
            }
            analisis.IdSeccion = Seccion;
            analisis.Etiqueta = TEtiquetas.Text;
            if (Visible.Checked)
            {
                analisis.Visible = true;
            }
            if (especiales.Checked)
            {
                analisis.Especiales = 1;
            }
            Valores.Unidad = TUnidad.Text;
            if (string.IsNullOrEmpty(Tdesde.Text))
            {
                Valores.ValorMenor = "0";

            }
            else
            {
                Valores.ValorMenor = Tdesde.Text;
            }
            if (string.IsNullOrEmpty(Thasta.Text))
            {
                Valores.ValorMayor = "0";

            }
            else
            {
                Valores.ValorMayor = Thasta.Text;
            }
            Valores.MultiplesValores = TValores.Text;
            Valores.lineas = TValores.Lines.Count();
            int id = Conexion.InsertarPerfilSimple(PERFIL, analisis, Valores);
            if (id > 0)
            {
                MessageBox.Show("Agregado Satisfactoriamente");
            }
            else
            {
                MessageBox.Show("Ha ocurrido un error");
            }
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void PrecioBs_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
