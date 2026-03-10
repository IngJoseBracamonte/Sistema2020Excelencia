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
    public partial class Frotis : Form
    {
        bool activo = false;
        Double ValorMenor = 0;
        Double ValorMayor = 0;
        int IdUser,IdOrden,IdAnalisis;
        public Frotis(int idUser, int idOrden, int idAnalisis)
        {
            IdUser = idUser;
            IdOrden = idOrden;
            IdAnalisis = idAnalisis;
            InitializeComponent();
        }

        private void Frotis_Load(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = Conexion.SELECTAnalisisFinal1(IdOrden, IdAnalisis);
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int edad = Hoy - nacimiento.Year;
                Edad.Text = Conexion.Fecha(nacimiento);
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();
                Analisis.Text = ds.Tables[0].Rows[0]["NombreAnalisis"].ToString();
                textBox2.Text = ds.Tables[0].Rows[0]["Comentario"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                activo = false;
            }
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
                    string MS = Conexion.InsertarFinal(" ", textBox2.Text, IdUser, IdOrden, IdAnalisis);
                    MessageBox.Show(MS);
                    this.Close();
                }
            }
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
                    string MS = Conexion.InsertarSinValidar(" ", textBox2.Text, IdUser, IdOrden, IdAnalisis);
                    MessageBox.Show(MS);
                    this.Close();
                }
            }
        }
    }
}
