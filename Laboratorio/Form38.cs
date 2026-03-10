using System.ServiceProcess;
using System;
using System.Configuration;
using System.IO;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Forms;
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class Form38 : Form
    {
        int IdOrden;
        int IdAnalisis;
        public Form38(int idOrden, int idAnalisis)
        {
            InitializeComponent();
            IdOrden = idOrden;
            IdAnalisis = idAnalisis;
        }
        private void Form38_Load(object sender, EventArgs e)
        {
            DataSet temp = new DataSet();
            DataSet Bioanalista = new DataSet();
            label17.Text = IdOrden.ToString();
            temp = Conexion.SelectTemp(IdOrden,IdAnalisis);
            label18.Text = temp.Tables[0].Rows[0]["NumeroDia"].ToString();
            Nombre.Text = temp.Tables[0].Rows[0]["Nombre"].ToString() + " " + temp.Tables[0].Rows[0]["Apellidos"].ToString();
            Sexo.Text = temp.Tables[0].Rows[0]["Sexo"].ToString();
            label20.Text = temp.Tables[0].Rows[0]["Nombre1"].ToString();
            label4.Text = temp.Tables[0].Rows[0]["NombreUsuario"].ToString();
            label10.Text = Convert.ToDateTime(temp.Tables[0].Rows[0]["Fecha"].ToString()).ToString("dd/MM/yyyy");
            label9.Text = temp.Tables[0].Rows[0]["HoraIngreso"].ToString();
            if (temp.Tables[0].Rows[0]["HoraValidacion"].ToString() != "" && temp.Tables[0].Rows[0]["HoraValidacion"].ToString() != " ")
            {
                label11.Text = Convert.ToDateTime(temp.Tables[0].Rows[0]["HoraValidacion"].ToString()).ToString("dd/MM/yyyy hh:mm:ss");
            }

            try
            {
                label13.Text = Convert.ToInt32(temp.Tables[0].Rows[0]["PrecioF"].ToString().Replace(".", ",")).ToString("#,0.00");

            }
            catch
            {
                label13.Text = temp.Tables[0].Rows[0]["PrecioF"].ToString();
            }
            Bioanalista = Conexion.Bioanalista(IdOrden, IdAnalisis);
            label5.Text = Bioanalista.Tables[0].Rows[0]["NombreUsuario"].ToString();

        }
    }
       
}
