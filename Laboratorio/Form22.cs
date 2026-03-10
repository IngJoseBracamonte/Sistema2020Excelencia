using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class Form22 : Form
    {
        public static DataSet data = new DataSet();
        public Form22()
        {
        InitializeComponent();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker2.Value < dateTimePicker1.Value) 
            {
                MessageBox.Show("Debe escoger una fecha mayor o igual a la Inicial");
                dateTimePicker1.Value = dateTimePicker2.Value;
            }

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();

            string cmd, cmd2;
            cmd = dateTimePicker1.Value.ToString("yyyy/MM/dd");
            cmd2 = dateTimePicker2.Value.ToString("yyyy/MM/dd");
            ds.Clear();
            ds = Conexion.SELECTordenes(cmd, cmd2);

            if (ds.Tables.Count != 0)
            {

               
                dataGridView1.DataSource = ds.Tables[0];
      
            }
            else
            {
                ds.Clear();
            }
        
            DataSet ds2 = new DataSet();
            ds2 = Conexion.SELECTTotalAnaliis(cmd, cmd2);
            if (ds2.Tables.Count != 0)
            {
                label7.Text = ds2.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                ds2.Clear();
            }

            DataSet ds3 = new DataSet();
            ds3 = Conexion.SELECTPorReportar(cmd, cmd2);
            if (ds3.Tables.Count != 0)
            {
                label4.Text = ds3.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                ds3.Clear();
            }

            DataSet ds4 = new DataSet();
            ds4 = Conexion.SELECTReportados(cmd, cmd2);
            if (ds4.Tables.Count != 0)
            {
                label6.Text = ds4.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                ds4.Clear();
            }


        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                MessageBox.Show("Debe escoger una fecha menor o igual a la Inicial");
                dateTimePicker2.Value = dateTimePicker1.Value;
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            this.Close();
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
            Form analisis = new Analisis();
            analisis.ShowDialog();
            data.Tables[0].Clear();
        }

        private void Form22_Load(object sender, EventArgs e)
        {
            if (data.Tables.Count == 0)
            {
                data.Tables.Add("Referido");
                data.Tables[0].Columns.Add("IdAnalisis");
                data.Tables[0].Columns.Add("IdOrden");
                data.Tables[0].Columns.Add("NombreAnalisis");
                data.Tables[0].Columns.Add("NumeroDia");
            }

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

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

        private void iconButton2_Click_1(object sender, EventArgs e)
        {

        }
    }

    internal class AdressBokPerson
    {
    }
}
