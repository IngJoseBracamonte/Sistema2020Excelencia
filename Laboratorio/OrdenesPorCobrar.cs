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
    public partial class OrdenesPorCobrar : Form
    {
        private int IdUser;

        public OrdenesPorCobrar(int idUser)
        {
            IdUser = idUser;
            InitializeComponent();
        }

        private void OrdenesPorCobrar_Load(object sender, EventArgs e)
        {
            DataSet Ordenes = new DataSet();
            Ordenes = Conexion.ordenesPorCobrar();
            if (Ordenes.Tables.Count != 0)
            {
                if (Ordenes.Tables[0].Rows.Count != 0)
                {
                    dataGridView1.DataSource = Ordenes.Tables[0];
                    DataGridViewColumn column= dataGridView1.Columns[1];
                    column.Width = 50;
                    DataGridViewColumn column1 = dataGridView1.Columns[0];
                    column1.Width = 50;
                    DataGridViewColumn column2 = dataGridView1.Columns[2];
                    column2.Width = 300;
                }
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
           
                int CobroID = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["IdOrden"].Value);
                Form Cobro = new Cobro(CobroID,IdUser);
                Cobro.Show();
                Cobro.FormClosing += Cobro_FormClosing;
        }

        private void Cobro_FormClosing(object sender, FormClosingEventArgs e)
        {
            DataSet Ordenes = new DataSet();
            Ordenes = Conexion.ordenesPorCobrar();
            if (Ordenes.Tables.Count != 0)
            {
                if (Ordenes.Tables[0].Rows.Count != 0)
                {
                    dataGridView1.DataSource = Ordenes.Tables[0];
                    DataGridViewColumn column = dataGridView1.Columns[1];
                    column.Width = 50;
                    DataGridViewColumn column1 = dataGridView1.Columns[0];
                    column1.Width = 50;
                    DataGridViewColumn column2 = dataGridView1.Columns[2];
                    column2.Width = 300;
                }
            }
            this.Show();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void OrdenesPorCobrar_Shown(object sender, EventArgs e)
        {
          
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OrdenesPorCobrar_Enter(object sender, EventArgs e)
        {
            DataSet Ordenes = new DataSet();
            Ordenes = Conexion.ordenesPorCobrar();
            if (Ordenes.Tables.Count != 0)
            {
                if (Ordenes.Tables[0].Rows.Count != 0)
                {
                    dataGridView1.DataSource = Ordenes.Tables[0];
                    DataGridViewColumn column = dataGridView1.Columns[1];
                    column.Width = 50;
                    DataGridViewColumn column1 = dataGridView1.Columns[0];
                    column1.Width = 50;
                    DataGridViewColumn column2 = dataGridView1.Columns[2];
                    column2.Width = 300;
                }
            }
        }
    }
}
