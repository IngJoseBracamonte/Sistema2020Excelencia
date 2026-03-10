using System;
using System.Data;
using System.Windows.Forms;
using Conexiones.DbConnect;
using SpreadsheetLight;

namespace Laboratorio
{
    public partial class Anulaciones : Form
    {
        public Anulaciones()
        {
            InitializeComponent();
        }

        private void Anulaciones_Load(object sender, EventArgs e)
        {
     
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();

            string cmd, cmd2;
            cmd = dateTimePicker1.Value.ToString("yyyy/MM/dd");
            cmd2 = dateTimePicker2.Value.ToString("yyyy/MM/dd");
            ds.Clear();
            ds = Conexion.SELECTordenesPacientesAnulados(cmd, cmd2);
            if (ds.Tables.Count != 0)
            {
                ListaDeAnulaciones.DataSource = ds.Tables[0];
            }
            else
            {
                ds.Clear();
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (ListaDeAnulaciones.Rows.Count != 0)
                {

                    int C = 1;
                    int R = 2;
                    SLDocument sl = new SLDocument();
                    SLStyle style = new SLStyle();
                    style.Font.FontSize = 12;
                    style.Font.Bold = true;


                    foreach (DataGridViewColumn colum in ListaDeAnulaciones.Columns)
                    {
                        sl.SetCellValue(1, C, colum.HeaderText.ToString());
                        C++;
                    }
                    foreach (DataGridViewRow row in ListaDeAnulaciones.Rows)
                    {
                        sl.SetCellValue(R, 1, row.Cells[0].Value.ToString());
                        sl.SetCellValue(R, 2, row.Cells[1].Value.ToString());
                        sl.SetCellValue(R, 3, row.Cells[2].Value.ToString());
                        sl.SetCellValue(R, 4, row.Cells[3].Value.ToString());
                        sl.SetCellValue(R, 5, row.Cells[4].Value.ToString());
                        sl.SetCellValue(R, 6, row.Cells[5].Value.ToString());
                        sl.SetCellValue(R, 7, row.Cells[6].Value.ToString());
                        sl.SetCellValue(R, 8, row.Cells[7].Value.ToString());
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
                MessageBox.Show("Selecciono el nombre de un archivo que posiblemente este en uso, por favor cierre el archivo o cambie el nombre");
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void desvalidarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int IdOrden = 0;
            int.TryParse(ListaDeAnulaciones.Rows[ListaDeAnulaciones.CurrentCell.RowIndex].Cells["IdOrden"].Value.ToString(), out IdOrden);
            if (IdOrden > 0)
            {
                Conexion.ActualizarEstadoDeOrden(IdOrden);
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
