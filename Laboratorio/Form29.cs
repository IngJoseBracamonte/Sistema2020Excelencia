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
    public partial class Form29 : Form
    {
        int IdUser;
        public Form29(int idUser)
        {
            IdUser = idUser;
            InitializeComponent();
        }

        private void Form29_Load(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds = Conexion.usuarios();
            if (ds.Tables.Count != 0)
            {
                dataGridView1.DataSource = ds.Tables[0];
                DataGridViewColumn column = dataGridView1.Columns[0];
                column.Width = 30;
                DataGridViewColumn column1 = dataGridView1.Columns[1];
                column1.Width = 250;
            }
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
         
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["ModificarUsuario"].ToString() == "1")
            {
                int IdUsuario = 0;
                int.TryParse(dataGridView1.Rows[e.RowIndex].Cells["IdUsuario"].Value.ToString(),out IdUsuario);
                Form form30 = new Usuario(IdUser,IdUsuario);
                form30.ShowDialog();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["AgregarUsuario"].ToString() == "1")
            {
                Form form32 = new Form32();
                form32.Show();
            }
            else
            {
                MessageBox.Show("No tiene permisos para realizar esta accion.");
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            Form form33 = new Form33(IdUser);
            form33.Show();
        }
    }
}
