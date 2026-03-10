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
    public partial class Form17 : Form
    {
        private int IdUser;
        int IdOrden = 0;
        int IdAnalisis = 0;
        List<string> Ordenes = new List<string>();
        int PoscionActual;
        public Form17(int idUser, int idOrden, int idAnalisis)
        {
            IdUser = idUser;
            IdOrden = idOrden;
            IdAnalisis = idAnalisis;
            InitializeComponent();

        }

        private void Form17_Load(object sender, EventArgs e)
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
            AcceptButton = iconButton2;
            try
            {
               
                DataSet ds3 = new DataSet();
                ds3 =Conexion.ListaDeRespuesta(IdAnalisis.ToString());
                if (ds3.Tables[0].Rows.Count != 0) 
                {
                    comboBox1.Items.Clear();
                    foreach (DataRow r in ds3.Tables[0].Rows)
                    {
                        comboBox1.Items.Add(r["Respuesta"].ToString());
                        
                    }
                }
                DataSet ds = new DataSet();
                ds = Conexion.SELECTAnalisisFinal2(IdOrden, IdAnalisis).Result;
                comboBox1.Text = ds.Tables[0].Rows[0]["ValorResultado"].ToString();
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int edad = Hoy - nacimiento.Year;
                Edad.Text = Conexion.Fecha(nacimiento);
                Analisis.Text = ds.Tables[0].Rows[0]["NombreAnalisis"].ToString();
                textBox2.Text = ds.Tables[0].Rows[0]["Comentario"].ToString();
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();
                if (ds.Tables[0].Rows[0]["Unidad"].ToString() != "")
                { 
                Unidad.Text  = ds.Tables[0].Rows[0]["Unidad"].ToString();
                }
                if (ds.Tables[0].Rows[0]["MultiplesValores"].ToString() != "")
                {
                    ValorRef.Text = ds.Tables[0].Rows[0]["MultiplesValores"].ToString();
                }
                comboBox1.ForeColor = Color.FromArgb(0, 0, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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
    
                    //(ValorResultado,Unidad,HoraValidacion,EstadoDeResultado,Comentario)
                    string MS = Conexion.InsertarFinal(comboBox1.Text, textBox2.Text, IdUser, IdOrden, IdAnalisis);
                    MessageBox.Show(MS);
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void NPaciente_Click(object sender, EventArgs e)
        {

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
       
                    //(ValorResultado,Unidad,HoraValidacion,EstadoDeResultado,Comentario)
                    string MS = Conexion.InsertarSinValidar(comboBox1.Text, textBox2.Text, IdUser, IdOrden, IdAnalisis);
                    MessageBox.Show(MS);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '.' && !((ComboBox)sender).Text.Contains(','))
            {
                e.KeyChar = ',';
            }
            else if (e.KeyChar == ',' && !((ComboBox)sender).Text.Contains(','))
            {
                e.Handled = true;
            }
                   }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            MoverIzqueirda();
            CargarDatosPaciente();
        }

        private void MoverIzqueirda()
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
                AcceptButton = iconButton2;


            }
        }

        private void CargarDatosPaciente()
        {
            try
            {
                DataSet ds3 = new DataSet();
                ds3 = Conexion.ListaDeRespuesta(IdAnalisis.ToString());
                if (ds3.Tables[0].Rows.Count != 0)
                {
                    comboBox1.Items.Clear();
                    foreach (DataRow r in ds3.Tables[0].Rows)
                    {
                        comboBox1.Items.Add(r["Respuesta"].ToString());
                    }
                }
                DataSet ds = new DataSet();
                ds = Conexion.SELECTAnalisisFinal2(IdOrden, IdAnalisis).Result;
                comboBox1.Text = ds.Tables[0].Rows[0]["ValorResultado"].ToString();
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int edad = Hoy - nacimiento.Year;
                Edad.Text = Conexion.Fecha(nacimiento);
                Analisis.Text = ds.Tables[0].Rows[0]["NombreAnalisis"].ToString();
                textBox2.Text = ds.Tables[0].Rows[0]["Comentario"].ToString();
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();
                if (ds.Tables[0].Rows[0]["Unidad"].ToString() != "")
                {
                    Unidad.Text = ds.Tables[0].Rows[0]["Unidad"].ToString();
                }
                if (ds.Tables[0].Rows[0]["MultiplesValores"].ToString() != "")
                {
                    ValorRef.Text = ds.Tables[0].Rows[0]["MultiplesValores"].ToString();
                }
                comboBox1.ForeColor = Color.FromArgb(0, 0, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void MoverDerecha()
        {
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
            CargarDatosPaciente();
        }
    }
}
