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
    public partial class Form37 : Form
    {
        public Form37()
        {
            InitializeComponent();
        }

        private void Form37_Load(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds = Conexion.ListaPrecios();
            dataGridView2.DataSource = ds.Tables[0];
            DataGridViewColumn column1 = dataGridView2.Columns[0];
            column1.Width = 40;
            column1.ReadOnly = true;
            DataGridViewColumn column2 = dataGridView2.Columns[1];
            column2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            column2.ReadOnly = true;
            DataGridViewColumn column3 = dataGridView2.Columns[2];
            column3.Width = 80;
            column3.ReadOnly = false;
            DataGridViewColumn column4 = dataGridView2.Columns[3];
            column4.Width = 70;
            column4.ReadOnly = false;
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {

        }
    }
}
