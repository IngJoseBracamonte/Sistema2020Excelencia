using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class Form31 : Form
    {
        bool Seleccionado;
        string Activo;
        int idConvenio;
        public Form31()
        {
            InitializeComponent();
        }

        private void Form31_Load(object sender, EventArgs e)
        {
            Actualizar();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (Tdescuento.Value > 100)
            {
                MessageBox.Show("El descuento no puede ser mayor al 100%");
                Tdescuento.Value = 100;
            } 
            else if (Tdescuento.Value < 0)
            {
                MessageBox.Show("El descuento no puede ser menor a 0%");
                Tdescuento.Value = 0;
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            if (Seleccionado == true)
            {
                string mensaje = "¿Desea Actualizar este Convenio?";
                string titulo = "Alarma";
                MessageBoxButtons button = MessageBoxButtons.YesNo;
                DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                    if (dialog == DialogResult.Yes)
                    {
                        if (checkBox1.Checked == true)
                        {
                            Activo = "1";
                        }
                        else
                        {
                            Activo = "0";
                        }

                        string cmd = string.Format("Convenios.Nombre = '{0}', Convenios.Telefono = '{1}', Convenios.Correo = '{2', Convenios.Descuento = '{3}', Convenios.Activos = {4}", Tnombre.Text, Ttelefono.Text, Tcorreo.Text, Tdescuento.Value, Activo);
                        Conexion.ActualizarConvenio(idConvenio, cmd);
                        Actualizar();
                    }
               
                }
            else
            {
                string mensaje = "¿Desea Guardar este Convenio?";
                string titulo = "Alarma";
                MessageBoxButtons button = MessageBoxButtons.YesNo;
                DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    if (checkBox1.Checked == true)
                    {
                        Activo = "1";
                    }
                    else
                    {
                        Activo = "0";
                    }

                    string cmd = string.Format("'{0}','{1}','{2}','{3}','{4}'", Tnombre.Text, Tdescuento.Value, Ttelefono.Text, Tcorreo.Text, Activo);
                    string MS = Conexion.InsertarConvenio(cmd);
                    Actualizar();
                    MessageBox.Show(MS);
                }
            }
        }
          

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        private void Actualizar()
        {
            DataSet ds = new DataSet();
            ds = Conexion.conveniosUser();
            if (ds.Tables.Count != 0)
            {
                dataGridView1.DataSource = ds.Tables[0];
                DataGridViewColumn column = dataGridView1.Columns[0];
                column.Width = 30;
                DataGridViewColumn column1 = dataGridView1.Columns[1];
                column1.Width = 280;
                DataGridViewColumn column2 = dataGridView1.Columns[2];
                column2.Width = 50;
  

        }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataSet ds = new DataSet();
            idConvenio = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["IdConvenio"].Value);
            ds = Conexion.SelectConvenio(idConvenio.ToString());
            Tnombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString();
            Tdescuento.Value = Convert.ToInt32(ds.Tables[0].Rows[0]["Descuento"].ToString());
            Ttelefono.Text = ds.Tables[0].Rows[0]["Telefono"].ToString();
            Tcorreo.Text = ds.Tables[0].Rows[0]["Correo"].ToString();
            if (ds.Tables[0].Rows[0]["Activos"].ToString() == "1")
            {
                checkBox1.Checked = true;
            }
            else 
            {
                checkBox1.Checked = false;
            }
            Seleccionado = true;

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            Seleccionado = false;
            Tnombre.Text ="";
            Tdescuento.Value = 0;
            Ttelefono.Text = "";
            Tcorreo.Text = "";
        }
    }
  
}
