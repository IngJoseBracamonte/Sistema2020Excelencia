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
    public partial class Form16 : Form
    {
        double PTT1 = 0; 
        double PTT2 = 0;
        double diff = 0;
        int IdOrden = 0;
        int IdAnalisis = 0;
        int IdUser;
        public Form16(int idUser, int idOrden, int idAnalisis)
        {
            IdUser = idUser;
            IdOrden = idOrden;
            IdAnalisis = idAnalisis;
            InitializeComponent();
        }

        private void Form16_Load(object sender, EventArgs e)
        {
            AcceptButton = iconButton2;
            DataSet ds = new DataSet();
            ds = Conexion.SelectPersonaOrden(IdOrden);
            try
            {
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int edad = Hoy - nacimiento.Year;
                Edad.Text = edad.ToString();
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();

            }
            catch
            {

            }
            DataSet ds1 = new DataSet();
            ds1 = Conexion.SelectPTT(IdOrden);
            try
            {
                foreach (DataRow r in ds1.Tables[0].Rows)
                {
                    if (r["IdAnalisis"].ToString() == "127")
                    {
                        textBox1.Text = r["ValorResultado"].ToString();
                    }
                    else if (r["IdAnalisis"].ToString() == "128")
                    {
                        textBox2.Text = r["ValorResultado"].ToString();
                    }
                    else if (r["IdAnalisis"].ToString() == "129")
                    {
                        textBox3.Text = r["ValorResultado"].ToString();
                    }
                }
            }
            catch
            {

            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            string mensaje = "Al momento de Guardar estos Valores se tomara los datos como validados ¿Desea Validar?";
            string titulo = "Alarma";
            MessageBoxButtons button = MessageBoxButtons.YesNo;
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["Validar"].ToString() == "1")
            {
                DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    try
                    {
                        string cmd = Conexion.InsertarFinal(textBox1.Text, "",IdUser, IdOrden, 127);
                        cmd = Conexion.InsertarFinal(textBox2.Text, "",IdUser, IdOrden, 128);
                        cmd = Conexion.InsertarFinal(textBox3.Text, "",IdUser, IdOrden, 129);
                        cmd = Conexion.InsertarFinal("", "",IdUser, IdOrden, 37);
                    }
                    finally
                    {
                        MessageBox.Show("agregado satisfactoriamente");
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                PTT1 = Convert.ToDouble(textBox1.Text);
                if (textBox2.Text != "")
                {
                    PTT2 = Convert.ToDouble(textBox2.Text);
                }
                diff = PTT1 - PTT2;
                textBox3.Text = diff.ToString("#,##");
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                PTT1 = Convert.ToDouble(textBox1.Text);
                if (textBox2.Text != "")
                {
                    PTT2 = Convert.ToDouble(textBox2.Text);
                }
                diff = PTT1 - PTT2;
                textBox3.Text = diff.ToString();
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            string mensaje = "Al momento de Guardar estos Valores se tomara los datos como validados ¿Desea Validar?";
            string titulo = "Alarma";
            MessageBoxButtons button = MessageBoxButtons.YesNo;
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["Validar"].ToString() == "1")
            {
                DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    try
                    {
                        string cmd = Conexion.InsertarSinValidar(textBox1.Text, "",IdUser, IdOrden, 127);
                        cmd = Conexion.InsertarSinValidar(textBox2.Text, "",IdUser, IdOrden, 128);
                        cmd = Conexion.InsertarSinValidar(textBox3.Text, "",IdUser, IdOrden, 129);
                        cmd = Conexion.InsertarSinValidar("", "",IdUser, IdOrden, 37);
                    }
                    finally
                    {
                        MessageBox.Show("agregado satisfactoriamente");
                    }
                }
            }
        }
    }
}
