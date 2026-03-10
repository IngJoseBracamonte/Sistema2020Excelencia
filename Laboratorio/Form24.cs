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
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;
using SpreadsheetLight.Charts;

namespace Laboratorio
{
    public partial class Form24 : Form
    {
        public static DataSet data = new DataSet();
        public Form24()
        {
            InitializeComponent();

            if (data.Tables.Count == 0)
            {
                data.Tables.Add("Referido");
                data.Tables[0].Columns.Add("IdAnalisis");
                data.Tables[0].Columns.Add("IdOrden");
                data.Tables[0].Columns.Add("NombreAnalisis");
                data.Tables[0].Columns.Add("NumeroDia");
            }
 

        }

        private void Form24_Load(object sender, EventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            DataSet data2 = new DataSet();
            string cmd, cmd2;
            cmd = dateTimePicker1.Value.ToString("yyyy/MM/dd");
            cmd2 = dateTimePicker2.Value.ToString("yyyy/MM/dd");
            data2 = Conexion.SELECTReferidos(cmd, cmd2);
            if (data2.Tables.Count != 0)
            {
               dataGridView1.DataSource = data2.Tables[0]; 
            }

        }

        private void iconButton2_Click(object sender, EventArgs e)
        {

           
            foreach (DataGridViewRow r in dataGridView1.Rows)
            {
                if (r.Cells["PorEnviar"].Value != null)
                {
                   if (r.Cells["PorEnviar"].Value.ToString() == "True")
                    {
                        data.Tables[0].Rows.Add(r.Cells["IdAnalisis"].Value.ToString(), r.Cells["IdOrden"].Value.ToString(), r.Cells["NombreAnalisis"].Value.ToString(), r.Cells["NumeroDia"].Value.ToString());
                    }
                }
            }
            Form form41 = new Form41();
            form41.ShowDialog();
            data.Tables[0].Clear();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                MessageBox.Show("Debe escoger una fecha menor o igual a la Inicial");
                dateTimePicker2.Value = dateTimePicker1.Value;
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker2.Value < dateTimePicker1.Value)
            {
                MessageBox.Show("Debe escoger una fecha mayor o igual a la Inicial");
                dateTimePicker1.Value = dateTimePicker2.Value;
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > (-1))
            {
                if (dataGridView1.Rows[e.RowIndex].Cells["PorEnviar"].Value == null)
                {
                    dataGridView1.Rows[e.RowIndex].Cells["PorEnviar"].Value = true;
                }
                else
                {
                    string variable = dataGridView1.Rows[e.RowIndex].Cells["PorEnviar"].Value.ToString();
                    if (variable == "False")
                    {
                        dataGridView1.Rows[e.RowIndex].Cells["PorEnviar"].Value = true;
                    }
                    else if (variable == "True")
                    {
                        dataGridView1.Rows[e.RowIndex].Cells["PorEnviar"].Value = false;
                    }
                }
            }
        }

        private void seleccionarTodosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells["PorEnviar"].Value = true; 
            }
        }

        private void quitarSeleccionadosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells["PorEnviar"].Value = false;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

