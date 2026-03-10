using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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
    public partial class Form26 : Form
    {
        private bool cargar;
        public Form26()
        {
            InitializeComponent();
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

        private void iconButton1_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();
            DataSet ds2 = new DataSet();
            string cmd, cmd2;
            cmd = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            cmd2 = dateTimePicker2.Value.ToString("yyyy-MM-dd");
            ds.Clear();
            ds = Conexion.BioanalistasAnalisis(cmd, cmd2);
            if (ds.Tables.Count != 0)
            {
                dataGridView1.DataSource = ds.Tables[0];
            }
            else
            {
                ds.Clear();
            }
            ds1 = Conexion.BioanalistasAnalisisContador(cmd, cmd2);
            if (ds1.Tables.Count != 0)
            {
                dataGridView2.DataSource = ds1.Tables[0];
               
            }
            else
            {
                ds.Clear();
            }
            DataGridViewColumn column = dataGridView1.Columns[0];
            column.Width = 100;
            DataGridViewColumn column1 = dataGridView1.Columns[1];
            column1.Width = 100;
            DataGridViewColumn column2 = dataGridView1.Columns[2];
            column1.Width = 100;
            DataGridViewColumn column3 = dataGridView2.Columns[0];
            column.Width = 100;
            DataGridViewColumn column4 = dataGridView2.Columns[1];
            column1.Width = 100;
            label5.Text = dataGridView1.Rows.Count.ToString();
            ds2 = Conexion.BioanalistasAnalisisHoraAgrupado(cmd, cmd2);
            if (ds2.Tables.Count != 0)
            {
                dataGridView3.DataSource = ds2.Tables[0];
            }
            else
            {
                ds2.Clear();
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            try
            {
                CultureInfo ci = new CultureInfo("Es-Es");
                DataSet ds1 = new DataSet();
                DataSet Empresa = new DataSet();
                string cmd, cmd2;
                int C = 3;
                int R = 2;

                SLDocument sl = new SLDocument();
                SLStyle style = new SLStyle();
                style.Font.FontSize = 12;
                style.Font.Bold = true;
                cmd = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                cmd2 = dateTimePicker2.Value.ToString("yyyy-MM-dd");
                Empresa = Conexion.SelectEmpresaActiva();
                if (Empresa.Tables.Count > 0)
                {
                    if (Empresa.Tables[0].Rows.Count > 0)
                    {
                        if (Empresa.Tables[0].Rows[0]["IdEmpresa"].ToString() == "5")
                        {
                            ds1 = Conexion.BioanalistasAnalisisHoraAgrupadosPorPersonaConEspeciales(cmd, cmd2);
                        }
                        else
                        {
                            ds1 = Conexion.BioanalistasAnalisisHoraAgrupadosPorPersona(cmd, cmd2);
                        }

                    }

                }
              
                foreach (DataRow r in ds1.Tables[0].Rows)
                {
                    DataSet ds2 = new DataSet();
                    ds2 = Conexion.BioanalistasAnalisisConteoPorNombre(cmd, cmd2, r["Usuario"].ToString());
                    if (dataGridView2.Rows.Count != 0)
                    {

                        DateTime ToDate = new DateTime();
                        ToDate = dateTimePicker2.Value;
                        DateTime FromDate = new DateTime();
                        FromDate = dateTimePicker1.Value;
                        sl.SetCellValue(4, R - 1, "HORA");
                        int Time = 7;
                        for (int i = 5; i < 21; i++)
                        {
                            sl.SetCellValue(i, R - 1, Time);
                            Time++;
                        }
                        sl.SetCellValue(C - 1, R, "N Pruebas");
                        sl.SetCellValue(C - 1, R + 1, r["Usuario"].ToString());
                        sl.MergeWorksheetCells(C - 1, R + 1, C - 1, R + 2);
                        for (var Day = FromDate.Date; Day.Date <= ToDate.Date; Day = Day.AddDays(1))
                        {

                            sl.SetCellValue(C, R, Day.ToString("dd-MM"));
                            C++;
                            sl.SetCellValue(C, R, ci.DateTimeFormat.GetDayName(Day.DayOfWeek).ToString());
                            DataSet ds3 = new DataSet();
                            ds3 = Conexion.BioanalistasAnalisisConteoPorNombre(Day.ToString("yyyy-MM-dd"), Day.ToString("yyyy-MM-dd"), r["Usuario"].ToString());
                            foreach (DataRow row in ds3.Tables[0].Rows)
                            {
                                switch (Convert.ToInt32(row["Hora"].ToString()))
                                {
                                    case 7:
                                        sl.SetCellValue(5, R, Convert.ToInt32(row["Cantidad"].ToString()));
                                        break;
                                    case 8:
                                        sl.SetCellValue(6, R, Convert.ToInt32(row["Cantidad"].ToString()));
                                        break;
                                    case 9:
                                        sl.SetCellValue(7, R, Convert.ToInt32(row["Cantidad"].ToString()));
                                        break;
                                    case 10:
                                        sl.SetCellValue(8, R, Convert.ToInt32(row["Cantidad"].ToString()));
                                        break;
                                    case 11:
                                        sl.SetCellValue(9, R, Convert.ToInt32(row["Cantidad"].ToString()));
                                        break;
                                    case 12:
                                        sl.SetCellValue(10, R, Convert.ToInt32(row["Cantidad"].ToString()));
                                        break;
                                    case 13:
                                        sl.SetCellValue(11, R, Convert.ToInt32(row["Cantidad"].ToString()));
                                        break;
                                    case 14:
                                        sl.SetCellValue(12, R, Convert.ToInt32(row["Cantidad"].ToString()));
                                        break;
                                    case 15:
                                        sl.SetCellValue(13, R, Convert.ToInt32(row["Cantidad"].ToString()));
                                        break;
                                    case 16:
                                        sl.SetCellValue(14, R, Convert.ToInt32(row["Cantidad"].ToString()));
                                        break;
                                    case 17:
                                        sl.SetCellValue(15, R, Convert.ToInt32(row["Cantidad"].ToString()));
                                        break;
                                    case 18:
                                        sl.SetCellValue(16, R, Convert.ToInt32(row["Cantidad"].ToString()));
                                        break;
                                    case 19:
                                        sl.SetCellValue(17, R, Convert.ToInt32(row["Cantidad"].ToString()));
                                        break;
                                    case 20:
                                        sl.SetCellValue(18, R, Convert.ToInt32(row["Cantidad"].ToString()));
                                        break;
                                    case 21:
                                        sl.SetCellValue(19, R, Convert.ToInt32(row["Cantidad"].ToString()));
                                        break;
                                    case 22:
                                        sl.SetCellValue(20, R, Convert.ToInt32(row["Cantidad"].ToString()));
                                        break;
                                    case 23:
                                        sl.SetCellValue(21, R, Convert.ToInt32(row["Cantidad"].ToString()));
                                        break;
                                }

                            }
                            C = 3;
                            R++;

                        }
                        C = 3;
                        R = R + 2;
                    }


                }
                saveFileDialog1.Filter = "Excel|*.xlsx";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    sl.SaveAs(saveFileDialog1.FileName);
                    saveFileDialog1.Dispose();
                }
            }
            catch 
            {
                MessageBox.Show("Selecciono el nombre de un archivo que posiblemente este en uso, por favor cierre el archivo o cambie el nombre");
            }
        }

        private void Form26_Load(object sender, EventArgs e)
        {

           
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void dateTimePicker4_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
   
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
        private void ConvertirHora()
        {
            if (dateTimePicker3.Value > dateTimePicker4.Value)
            {
                MessageBox.Show("La Fecha Inicial debe ser mayor a la Fecha Final");
                dateTimePicker3.Value = dateTimePicker4.Value;
            }
            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();
            string cmd, cmd2;
            cmd = dateTimePicker1.Value.ToString("yyyy/MM/dd");
            cmd2 = dateTimePicker2.Value.ToString("yyyy/MM/dd");
            ds.Clear();
            ds = Conexion.BioanalistasAnalisisHora(cmd, cmd2,dateTimePicker3.Value.ToString("HH:mm:ss"), dateTimePicker4.Value.ToString("HH:mm:ss"));
            ds1 = Conexion.BioanalistasAnalisisContadorHora(cmd, cmd2, dateTimePicker3.Value.ToString("HH:mm:ss"), dateTimePicker4.Value.ToString("HH:mm:ss"));
            dataGridView1.DataSource = ds.Tables[0];
            dataGridView2.DataSource = ds1.Tables[0];
            label5.Text = dataGridView1.Rows.Count.ToString();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
         
        }

        private void dateTimePicker4_ValueChanged_1(object sender, EventArgs e)
        {
            ConvertirHora();
        }

        private void dateTimePicker3_ValueChanged_1(object sender, EventArgs e)
        {
            ConvertirHora();
        }

        private void copiarToolStripMenuItem_Click(object sender, EventArgs e)
        {

            CopyToClipboard();
        }
        private void CopyToClipboard()
        {
            //Copy to clipboard
            DataObject dataObj = dataGridView3.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DataObject dataObj = dataGridView2.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        private void exportarAExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
               
         }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void exportarAExcelToolStripMenuItem1_Click(object sender, EventArgs e)
        {
          
        }
    }
}