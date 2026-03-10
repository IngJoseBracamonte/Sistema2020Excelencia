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
    public partial class Form15 : Form
    {
        int IDOrden = 0;
        int IdAnalisis = 0;
        List<string> Ordenes = new List<string>();
        int PoscionActual;
        private int IdUser;

        public Form15(int idUser, int idOrden, int idAnalisis)
        {
            IdUser = idUser;
            IDOrden = idOrden;
            IdAnalisis = idAnalisis;
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form15_Load(object sender, EventArgs e)
        {
            DataSet Posiciones = new DataSet();
            DataSet Fecha = new DataSet();
            Fecha = Conexion.FechaDeOrden(IDOrden);
            Posiciones = Conexion.CantidadesDeExamenes(IdAnalisis, Convert.ToDateTime(Fecha.Tables[0].Rows[0]["Fecha"].ToString()).ToString("yyyy-MM-dd"));
            if (Posiciones.Tables.Count != 0)
            {
                if (Posiciones.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow r in Posiciones.Tables[0].Rows)
                    {
                        Ordenes.Add(r["IdOrden"].ToString());
                    }
                }
            }
            PoscionActual = Convert.ToInt32(Ordenes.IndexOf(IDOrden.ToString()));
            if (Ordenes.Count != 0 && PoscionActual > (-1))
            {
                if (Ordenes[0] == Ordenes[PoscionActual])
                {
                    Izquierda.Visible = false;
                }
                if (Ordenes[Ordenes.Count - 1] == Ordenes[PoscionActual])
                {
                    Derecha.Visible = false;
                }
            }
            else
            {
                Izquierda.Visible = false;
                Derecha.Visible = false;
            }
            AcceptButton = iconButton2;
            
            DataSet ds = new DataSet();
            ds = Conexion.SelectPersonaOrden(IDOrden);
            try
            {
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int edad = Hoy - nacimiento.Year;
                Edad.Text = Conexion.Fecha(nacimiento);
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();

            }
            catch
            {

            }
            DataSet ds1 = new DataSet();
            ds1 = Conexion.SelectPT(IDOrden);
            try
            {
                foreach (DataRow r in ds1.Tables[0].Rows)
                {
                    if (r["IdAnalisis"].ToString() == "121")
                    {
                        textBox1.Text = r["ValorResultado"].ToString();
                    }
                    else if (r["IdAnalisis"].ToString() == "122")
                    {
                        textBox2.Text = r["ValorResultado"].ToString();
                    }
                    else if (r["IdAnalisis"].ToString() == "123")
                    {
                        textBox3.Text = r["ValorResultado"].ToString();
                    }
                    else if (r["IdAnalisis"].ToString() == "124")
                    {
                        textBox4.Text = r["ValorResultado"].ToString();
                    }
                    else if (r["IdAnalisis"].ToString() == "125")
                    {
                        textBox6.Text = r["ValorResultado"].ToString();
                    }
                    else if (r["IdAnalisis"].ToString() == "126")
                    {
                        textBox5.Text = r["ValorResultado"].ToString();
                    }
                }
            }
            catch
            {

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Double FormulaRazon = 0;
            Double protombina = 0;
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                FormulaRazon = Convert.ToDouble(textBox1.Text) / Convert.ToDouble(textBox2.Text);
                protombina = (Convert.ToDouble(textBox1.Text) / Convert.ToDouble(textBox2.Text))*100;

            }
            textBox4.Text = FormulaRazon.ToString("0.##");
            textBox5.Text = protombina.ToString("0.##");

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            Double FormulaRazon2 = 0;
            if (textBox3.Text != "" && textBox4.Text != "")
            {
                FormulaRazon2 = Math.Pow(Convert.ToDouble(textBox4.Text), Convert.ToDouble(textBox3.Text));
            }
            textBox6.Text = FormulaRazon2.ToString("0.##");
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Double FormulaRazon = 0;
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                FormulaRazon = Convert.ToDouble(textBox1.Text) / Convert.ToDouble(textBox2.Text);
            }
            textBox4.Text = FormulaRazon.ToString("0.##");
            Double FormulaRazon2 = 0;
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                FormulaRazon2 = (Convert.ToDouble(textBox2.Text) / Convert.ToDouble(textBox1.Text))*100;
            }
            textBox5.Text = FormulaRazon2.ToString("0.##");
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Double FormulaRazon2 = 0;
            if (textBox3.Text != "" && textBox4.Text != "")
            {
                FormulaRazon2 = Math.Pow(Convert.ToDouble(textBox4.Text), Convert.ToDouble(textBox3.Text));
            }
            textBox6.Text = FormulaRazon2.ToString("0.##");
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
                        string con = Conexion.InsertarFinal("", "", IdUser, IDOrden, 37);
                        con = Conexion.InsertarFinal(textBox1.Text," " ,IdUser, IDOrden, 121);
                        string con1 = Conexion.InsertarFinal(textBox2.Text, " ", IdUser, IDOrden, 122);
                        string con2 = Conexion.InsertarFinal(textBox3.Text, " ", IdUser, IDOrden, 123);
                        string con3 = Conexion.InsertarFinal(textBox4.Text, " ", IdUser, IDOrden, 124);
                        string con4 = Conexion.InsertarFinal(textBox6.Text, " ", IdUser, IDOrden, 125);
                        string con5 = Conexion.InsertarFinal(textBox5.Text, " ", IdUser, IDOrden, 126);
                    }
                    finally
                    {
                        MessageBox.Show("agregado satisfactoriamente");
                    }
                }
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
                        string con = Conexion.InsertarSinValidar("", "", IdUser, IDOrden, 37);
                        con = Conexion.InsertarSinValidar(textBox1.Text, " ", IdUser, IDOrden, 121);
                        string con1 = Conexion.InsertarSinValidar(textBox2.Text, " ", IdUser, IDOrden, 122);
                        string con2 = Conexion.InsertarSinValidar(textBox3.Text, " ", IdUser, IDOrden, 123);
                        string con3 = Conexion.InsertarSinValidar(textBox4.Text, " ", IdUser, IDOrden, 124);
                        string con4 = Conexion.InsertarSinValidar(textBox6.Text, " ", IdUser, IDOrden, 125);
                        string con5 = Conexion.InsertarSinValidar(textBox5.Text, " ", IdUser, IDOrden, 126);
                    }
                    finally
                    {
                        MessageBox.Show("agregado satisfactoriamente");
                    }
                }
            }
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            Moverizquierda();
            CargarDatosPaciente();

         }

        private void CargarDatosPaciente()
        {

            DataSet ds = new DataSet();
            ds = Conexion.SelectPersonaOrden(IDOrden);
            try
            {
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int edad = Hoy - nacimiento.Year;
                Edad.Text = Conexion.Fecha(nacimiento);
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();

            }
            catch
            {

            }
            DataSet ds1 = new DataSet();
            ds1 = Conexion.SelectPT(IDOrden);
            try
            {
                foreach (DataRow r in ds1.Tables[0].Rows)
                {
                    if (r["IdAnalisis"].ToString() == "121")
                    {
                        textBox1.Text = r["ValorResultado"].ToString();
                    }
                    else if (r["IdAnalisis"].ToString() == "122")
                    {
                        textBox2.Text = r["ValorResultado"].ToString();
                    }
                    else if (r["IdAnalisis"].ToString() == "123")
                    {
                        textBox3.Text = r["ValorResultado"].ToString();
                    }
                    else if (r["IdAnalisis"].ToString() == "124")
                    {
                        textBox4.Text = r["ValorResultado"].ToString();
                    }
                    else if (r["IdAnalisis"].ToString() == "125")
                    {
                        textBox6.Text = r["ValorResultado"].ToString();
                    }
                    else if (r["IdAnalisis"].ToString() == "126")
                    {
                        textBox5.Text = r["ValorResultado"].ToString();
                    }
                }
            }
            catch
            {

            }
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            MoverDerecha();
            CargarDatosPaciente();
         }


        private void Moverizquierda()
        {
            if (PoscionActual > 0)
            {
                PoscionActual = PoscionActual - 1;
                IDOrden = Convert.ToInt32(Ordenes[PoscionActual].ToString());
                if (Ordenes.Count != 0)
                {
                    if (Ordenes[0] == Ordenes[PoscionActual])
                    {
                        Izquierda.Visible = false;
                    }
                    else
                    {
                        Izquierda.Visible = true;
                    }
                    if (Ordenes[Ordenes.Count - 1] == Ordenes[PoscionActual])
                    {
                        Derecha.Visible = false;
                    }
                    else
                    {
                        Derecha.Visible = true;
                    }
                }
                else
                {
                    Izquierda.Visible = false;
                    Derecha.Visible = false;
                }
                AcceptButton = iconButton2;
            }
        }
        private void MoverDerecha()
        {
            if (PoscionActual < Ordenes.Count - 1)
            {
                PoscionActual = PoscionActual + 1;
                IDOrden = Convert.ToInt32(Ordenes[PoscionActual].ToString());
                if (Ordenes.Count != 0)
                {
                    if (Ordenes[0] == Ordenes[PoscionActual])
                    {
                        Izquierda.Visible = false;
                    }
                    else
                    {
                        Derecha.Visible = true;
                    }
                    if (Ordenes[Ordenes.Count - 1] == Ordenes[PoscionActual])
                    {
                        Derecha.Visible = false;
                    }
                    else
                    {
                        Izquierda.Visible = true;
                    }
                }
                else
                {
                    Derecha.Visible = false;
                    Izquierda.Visible = false;
                }

            }
        }
    }
}
