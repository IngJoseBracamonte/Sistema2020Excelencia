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
    public partial class Form34 : Form
    {
        public Form34()
        {
            InitializeComponent();
        }

        private void Form34_Load(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds = Conexion.AnalisisTemporal(Form3.Sesion.ToString());
            dataGridView2.DataSource = ds.Tables[0];
            DataGridViewColumn column1 = dataGridView2.Columns[0];
            column1.Width = 350;
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
