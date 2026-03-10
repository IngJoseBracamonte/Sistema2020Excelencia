using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class Form9 : Form
    {
        List<string> Ordenes = new List<string>();
        int PoscionActual, IdOrden, IdAnalisis, IdUser;

        bool activo = false;
        Double ValorMenor = 0;
        Double ValorMayor = 0;
        public Form9(int idUser,int idOrden,int idAnalisis)
        {
            IdUser=idUser;
            IdOrden =idOrden;
            IdAnalisis = idAnalisis;
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form9_Load(object sender, EventArgs e)
        {

         
            DataSet Posiciones = new DataSet();
            DataSet Fecha = new DataSet();
            Fecha = Conexion.FechaDeOrden(IdOrden);
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
            PoscionActual = Convert.ToInt32(Ordenes.IndexOf(IdOrden.ToString()));
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

            try
            {
                DataSet ds = new DataSet();
                ds = Conexion.SELECTAnalisisFinal1(IdOrden, IdAnalisis);
                textBox1.Text = ds.Tables[0].Rows[0]["ValorResultado"].ToString();
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int edad = Hoy - nacimiento.Year;
                Edad.Text = Conexion.Fecha(nacimiento);
                label1.Visible = false;
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();
                if (ds.Tables[0].Rows[0]["ValorMenor"].ToString() != "") 
                {
                ValorMenor = Convert.ToDouble(ds.Tables[0].Rows[0]["ValorMenor"].ToString());
                ValorMayor = Convert.ToDouble(ds.Tables[0].Rows[0]["ValorMayor"].ToString());
                label1.Visible = true;
                label1.Text = ValorMenor.ToString() + " - " + ValorMayor.ToString();
                }
                Analisis.Text = ds.Tables[0].Rows[0]["NombreAnalisis"].ToString();
                textBox2.Text = ds.Tables[0].Rows[0]["Comentario"].ToString();
               
                label3.Text = ds.Tables[0].Rows[0]["Unidad"].ToString();
                textBox1.ForeColor = Color.FromArgb(0, 0, 0);
                if (ds.Tables[0].Rows[0]["ValorMenor"].ToString() != "" && ds.Tables[0].Rows[0]["ValorMayor"].ToString() != "")
                { 
                activo = true;
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.ToString());
                activo = false;
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Double respuesta = 0; 
        if(activo == true) {
            if (textBox1.Text != "")
            {
            respuesta = Convert.ToDouble(textBox1.Text);
            }

            if (respuesta < ValorMenor)
            {
                textBox1.ForeColor = Color.FromArgb(0, 110, 242);

            }
            else if (respuesta > ValorMayor)
            {
                textBox1.ForeColor = Color.FromArgb(150, 40, 130);
            }
            else textBox1.ForeColor = Color.FromArgb(0, 0, 0);
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

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
 
            //(ValorResultado,Unidad,HoraValidacion,EstadoDeResultado,Comentario)
            DataSet ds2 = new DataSet();
            ds2 = Conexion.PrivilegiosCargar(IdUser.ToString());
            string mensaje = "Al momento de Guardar estos Valores se tomara los datos como validados ¿Desea Validar?";
            string titulo = "Alarma";
            MessageBoxButtons button = MessageBoxButtons.YesNo;
            if (ds2.Tables[0].Rows[0]["Validar"].ToString() == "1")
            {
                DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    string MS = Conexion.InsertarFinal(textBox1.Text, textBox2.Text, IdUser, IdOrden, IdAnalisis);
                    MessageBox.Show(MS);
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void Moverizquierda()
        {
            if (PoscionActual > 0)
            {
                PoscionActual = PoscionActual - 1;
                IdOrden = Convert.ToInt32(Ordenes[PoscionActual].ToString());
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
            }
        }
        private void MoverDerecha()
        {
            //IdOrden
            //Izquierda
            //Derecha
            if (PoscionActual < Ordenes.Count - 1)
            {
                PoscionActual = PoscionActual + 1;
                IdOrden = Convert.ToInt32(Ordenes[PoscionActual].ToString());
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
        private void iconButton5_Click(object sender, EventArgs e)
        {
            MoverDerecha();
            cargarDatosPaciente();
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            //(ValorResultado,Unidad,HoraValidacion,EstadoDeResultado,Comentario)
            DataSet ds2 = new DataSet();
            ds2 = Conexion.PrivilegiosCargar(IdUser.ToString());
            string mensaje = "Al momento de Guardar estos Valores se tomara los datos como validados ¿Desea Validar?";
            string titulo = "Alarma";
            MessageBoxButtons button = MessageBoxButtons.YesNo;
            if (ds2.Tables[0].Rows[0]["Validar"].ToString() == "1")
            {
                DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    string MS = Conexion.InsertarSinValidar(textBox1.Text, textBox2.Text, IdUser, IdOrden, IdAnalisis);
                    MessageBox.Show(MS);
                }
            }
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            Moverizquierda();
            cargarDatosPaciente();
        }
        private void cargarDatosPaciente()
        {
            try
            {
                DataSet ds = new DataSet();
                ds = Conexion.SELECTAnalisisFinal1(IdOrden, IdAnalisis);
                textBox1.Text = ds.Tables[0].Rows[0]["ValorResultado"].ToString();
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int edad = Hoy - nacimiento.Year;
                Edad.Text = Conexion.Fecha(nacimiento);
                label1.Visible = false;
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();
                if (ds.Tables[0].Rows[0]["ValorMenor"].ToString() != "")
                {
                    ValorMenor = Convert.ToDouble(ds.Tables[0].Rows[0]["ValorMenor"].ToString());
                    ValorMayor = Convert.ToDouble(ds.Tables[0].Rows[0]["ValorMayor"].ToString());
                    label1.Visible = true;
                    label1.Text = ValorMenor.ToString() + " - " + ValorMayor.ToString();
                }
                Analisis.Text = ds.Tables[0].Rows[0]["NombreAnalisis"].ToString();
                textBox2.Text = ds.Tables[0].Rows[0]["Comentario"].ToString();

                label3.Text = ds.Tables[0].Rows[0]["Unidad"].ToString();
                textBox1.ForeColor = Color.FromArgb(0, 0, 0);
                if (ds.Tables[0].Rows[0]["ValorMenor"].ToString() != "" && ds.Tables[0].Rows[0]["ValorMayor"].ToString() != "")
                {
                    activo = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                activo = false;
            }
        }
    }
 }

