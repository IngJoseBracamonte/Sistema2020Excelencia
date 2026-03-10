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
    public partial class Pendientes : Form
    {
        int IdUser;
        public Pendientes(int idUser)
        {
            IdUser = idUser;
            InitializeComponent();
        }

        private void Pendientes_Load(object sender, EventArgs e)
        {
            DataSet Ordenes = new DataSet();
            Ordenes = Conexion.ordenesPendientes();
            if (Ordenes.Tables.Count != 0)
            {
                if (Ordenes.Tables[0].Rows.Count != 0)
                {


                    dataGridView1.DataSource = Ordenes.Tables[0];
                    DataGridViewColumn column = dataGridView1.Columns[1];
                    column.Width = 80;
                    DataGridViewColumn column1 = dataGridView1.Columns[0];
                    column1.Width = 30;
                    DataGridViewColumn column2 = dataGridView1.Columns[3];
                    column2.Width = 300;
                }
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            int CobroID = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["IdOrden"].Value);
            Form Cobro = new CobroPendientes(CobroID,IdUser);
            Cobro.Show();
            Cobro.FormClosing += Cobro_FormClosing;
        }

        private void Cobro_FormClosing(object sender, FormClosingEventArgs e)
        {
            DataSet Ordenes = new DataSet();
            Ordenes = Conexion.ordenesPendientes();
            if (Ordenes.Tables.Count != 0)
            {
                if (Ordenes.Tables[0].Rows.Count != 0)
                {


                    dataGridView1.DataSource = Ordenes.Tables[0];
                    DataGridViewColumn column = dataGridView1.Columns[1];
                    column.Width = 50;
                    DataGridViewColumn column1 = dataGridView1.Columns[0];
                    column1.Width = 50;
                    DataGridViewColumn column2 = dataGridView1.Columns[3];
                    column2.Width = 300;
                }
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
