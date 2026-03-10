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
    public partial class Form33 : Form
    {
        int IdUser;
        public Form33(int idUser)
        {
            IdUser = idUser;
            InitializeComponent();
        }

        private void Form33_Load(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds = Conexion.TeclasYPrivilegios(IdUser);
            if (ds.Tables.Count != 0) { 
                if (ds.Tables[0].Rows.Count != 0)
            {
                textBox5.Text = ds.Tables[0].Rows[0]["Leucocitos"].ToString();
                textBox6.Text = ds.Tables[0].Rows[0]["Neutrofilos"].ToString();
                textBox7.Text = ds.Tables[0].Rows[0]["Linfocitos"].ToString();
                textBox10.Text = ds.Tables[0].Rows[0]["Monocitos"].ToString();
                textBox9.Text = ds.Tables[0].Rows[0]["Eosinofilos"].ToString();
                textBox8.Text = ds.Tables[0].Rows[0]["Basofilos"].ToString();
                textBox11.Text = ds.Tables[0].Rows[0]["Plaquetas"].ToString();
            }
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            string cmd = string.Format("TeclasPorUsuario.Leucocitos = '{0}', TeclasPorUsuario.Neutrofilos = '{1}', TeclasPorUsuario.Linfocitos = '{2}', TeclasPorUsuario.Monocitos = '{3}', TeclasPorUsuario.Eosinofilos = '{4}', TeclasPorUsuario.Basofilos = '{5}', TeclasPorUsuario.Plaquetas = '{6}'", textBox5.Text, textBox6.Text, textBox7.Text, textBox10.Text, textBox9.Text, textBox8.Text, textBox11.Text);
            string MS = Conexion.ActualizarTeclas(cmd, IdUser);
            MessageBox.Show(MS);
        }
    }
}
