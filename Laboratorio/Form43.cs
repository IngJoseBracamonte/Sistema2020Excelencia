using System;
using System.Data;
using System.Windows.Forms;
using Conexiones.DbConnect;
using SpreadsheetLight;

namespace Laboratorio
{
    public partial class Form43 : Form
    {
        public Form43()
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

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                MessageBox.Show("Debe escoger una fecha menor o igual a la Inicial");
                dateTimePicker2.Value = dateTimePicker1.Value;
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            DataSet ds3 = new DataSet();
            string cmd, cmd2;
            cmd = dateTimePicker1.Value.ToString("yyyy/MM/dd");
            cmd2 = dateTimePicker2.Value.ToString("yyyy/MM/dd");
            ds.Clear();
            ds = Conexion.TotalFacturadoPoPerfil(cmd, cmd2);
            if (ds.Tables.Count != 0)
            {
                dataGridView1.DataSource = ds.Tables[0];
            }
            else
            {
                ds.Clear();
            }
            ds3 = Conexion.SELECTTotalFacturadoFecha(cmd, cmd2);
            if (ds3.Tables[0].Rows.Count != 0)
            {
                if (ds3.Tables[0].Rows[0]["SumaDePrecioF"].ToString() != "" && ds3.Tables[0].Rows[0]["SumaDePrecioF"].ToString() != " ")
                {
                    textBox1.Text = ds3.Tables[0].Rows[0]["SumaDePrecioF"].ToString();
                    textBox3.Text = ds3.Tables[0].Rows[0]["Dolares"].ToString() + "$";
                }
            }
            else
            {
                textBox1.Text = "0";
                textBox3.Text = "0";
            }
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
                        C++;
                    }
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        for (int i = 1; i < C; i++)
                        {
                            sl.SetCellValue(R, i, row.Cells[i - 1].Value.ToString());
                        }
                        R++;
                    }
                    sl.SetColumnWidth(1, 11);
                    sl.SetColumnWidth(2, 9);
                    sl.SetColumnWidth(3, 6);
                    sl.SetColumnWidth(4, 12);
                    sl.SetColumnWidth(5, 40);
                    sl.SetColumnWidth(6, 20);
                    sl.SetColumnWidth(7, 20);
                    sl.SetColumnWidth(20, 20);
                    saveFileDialog1.Filter = "Excel|*.xlsx";
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        sl.SaveAs(saveFileDialog1.FileName);
                        saveFileDialog1.FileName = "";
                    }
                }
                else
                {
                    MessageBox.Show("No hay datos para mostrar en la tabla");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form43_Load(object sender, EventArgs e)
        {

        }
    }
}
