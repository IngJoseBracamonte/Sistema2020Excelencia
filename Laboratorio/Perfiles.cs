using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Conexiones;
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class Perfiles : Form
    {
        List<Perfil> perfiles = new List<Perfil>();
        public Perfiles()
        {
            InitializeComponent();
        }

        private void CargarPerfiles()
        {
           
            perfiles = Conexion.selectListaDePerfiles();
            dataGridView1.DataSource = "";
            dataGridView1.DataSource = perfiles;
        }
        private void Perfiles_Load(object sender, EventArgs e)
        {
            CargarPerfiles();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.CurrentCell.RowIndex;
            Form AgregarPerfil = new AgregarPerfil(perfiles.ElementAt(index).IdPerfil);
            AgregarPerfil.ShowDialog();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
