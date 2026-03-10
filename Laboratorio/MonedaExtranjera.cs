using Conexiones.DbConnect;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laboratorio
{
    public partial class MonedaExtranjera : Form
    {
        public MonedaExtranjera()
        {
            InitializeComponent();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count != 0)
                {

                    int C = 1;
                    int R = 2;
                    SLDocument sl = new SLDocument();
                    SLStyle style = new SLStyle();
                    style.Font.FontSize = 12;
                    style.Font.Bold = true;


                    foreach (DataGridViewColumn colum in dataGridView1.Columns)
                    {
                        sl.SetCellValue(1, C, colum.HeaderText.ToString());
                        if (dataGridView1.Columns.Count != C)
                        {
                            C++;
                        }
                    }
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        for (int count = 1; count <= C; count++)
                        {
                            sl.SetCellValue(R, count , row.Cells[count -1].Value.ToString());
                        }
                        R++;
                    }
                    saveFileDialog1.Filter = "Excel|*.xlsx";
                   
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        sl.SaveAs(saveFileDialog1.FileName);
                    }
                }
                else
                {
                    MessageBox.Show("No hay datos para mostrar en la tabla");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Selecciono el nombre de un archivo que posiblemente este en uso, por favor cierre el archivo o cambie el nombre");
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveFileDialog2_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            string cmd, cmd2;
            cmd = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            cmd2 = dateTimePicker2.Value.ToString("yyyy-MM-dd");
            ds.Clear();
            ds = Conexion.MonedaExtranjera(cmd, cmd2);
            if (ds.Tables.Count != 0)
            {
                dataGridView1.DataSource = ds.Tables[0];
            }
            else
            {
                ds.Clear();
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

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                MessageBox.Show("Debe escoger una fecha menor o igual a la Inicial");
                dateTimePicker2.Value = dateTimePicker1.Value;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = "";
            DataSet ds = new DataSet();
            string cmd;
            ds.Clear();
            ds = Conexion.SerialDeBillete(textBox1.Text);
            if (ds.Tables.Count != 0)
            {
                dataGridView1.DataSource = ds.Tables[0];
            }
            else
            {
                ds.Clear();
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
