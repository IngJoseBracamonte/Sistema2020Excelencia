using Conexiones.DbConnect;
using DocumentFormat.OpenXml.Spreadsheet;
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
    public partial class ReporteDePagos : Form
    {
        public ReporteDePagos()
        {
            InitializeComponent();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (advancedDataGridView1.Rows.Count != 0)
                {

                    int C = 1;
                    int R = 2;
                    SLDocument sl = new SLDocument();

                    SLStyle style = sl.CreateStyle();
                    // each indent is 3 spaces, so this is 15 spaces total
                    style.Alignment.Vertical = VerticalAlignmentValues.Center;
                    style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                    style.Font.FontSize = 12;
                    style.FormatCode = "#,##_);[Red](#,##)";
                    style.Font.Bold = true;

                    SLStyle style2 = sl.CreateStyle();
                    style2.Alignment.ReadingOrder = SLAlignmentReadingOrderValues.RightToLeft;
                    style2.FormatCode = "#,##0.00_);[Red](#,##0.00)";
                    style2.Font.FontSize = 12;

                    SLStyle style3 = sl.CreateStyle();
                    style3.Alignment.ReadingOrder = SLAlignmentReadingOrderValues.LeftToRight;
                    style3.Font.FontSize = 12;


                    SLStyle style4 = sl.CreateStyle();
                    style4.Alignment.ReadingOrder = SLAlignmentReadingOrderValues.LeftToRight;
                    style4.SetVerticalAlignment(VerticalAlignmentValues.Center);
                    style4.Font.FontSize = 12;

                    SLStyle style5 = sl.CreateStyle();
                    // each indent is 3 spaces, so this is 15 spaces total
                    style5.Alignment.Vertical = VerticalAlignmentValues.Center;
                    style5.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                    style5.Font.FontSize = 12;
                    style5.FormatCode = "#,##0.00_);[Red](#,##0.00)";


                    try
                    {
                        foreach (DataGridViewColumn colum in advancedDataGridView1.Columns)
                        {
                            sl.SetCellValue(1, C, colum.HeaderText.ToString());
                            sl.SetCellStyle(1, C, style);
                            C++;
                        }
                        sl.SetCellValue(1, C = 10, "TipodePago");
                        sl.SetCellStyle(1, C, style);
                        sl.SetCellValue(1, C = 11, "NombreBanco");
                        sl.SetCellStyle(1, C, style);
                        sl.SetCellValue(1, C = 12, "Cantidad");
                        sl.SetCellStyle(1, C, style);
                        sl.SetCellValue(1, C = 13, "Cantidad En BsS");
                        sl.SetCellStyle(1, C, style);
                        sl.SetCellValue(1, C = 14, "Serial");
                        sl.SetCellStyle(1, C, style);
                        sl.SetCellValue(1, C = 15, "Descripcion");
                        sl.SetCellStyle(1, C, style);
                        sl.SetCellValue(1, C = 16, "Fecha");
                        sl.SetCellStyle(1, C, style);
                        C = 1;

                        bool primeravez = true;
                        try
                        {
                            foreach (DataGridViewRow row in advancedDataGridView1.Rows)
                            {

                                sl.SetCellStyle(R, 1, style4);
                                sl.SetCellValue(R, 1, row.Cells[0].Value.ToString());
                                //
                                sl.SetCellStyle(R, 2, style4);
                                sl.SetCellValue(R, 2, row.Cells[1].Value.ToString());
                                //
                                sl.SetCellStyle(R, 3, style3);
                                sl.SetCellValue(R, 3, row.Cells[2].Value.ToString());

                                //
                                sl.SetCellStyle(R, 4, style2);
                                sl.SetCellValue(R, 4,row.Cells[3].Value.ToString());

                                //
                                sl.SetCellStyle(R, 5, style2);
                                sl.SetCellValue(R, 5, Convert.ToDouble(row.Cells[4].Value.ToString().Replace(".", ",")));

                                //
                                sl.SetCellStyle(R, 6, style2);
                                sl.SetCellValue(R, 6, Convert.ToDouble(row.Cells[5].Value.ToString().Replace(".", ",")));
                                //
                                sl.SetCellStyle(R, 7, style2);
                                sl.SetCellValue(R, 7, Convert.ToDouble(row.Cells[6].Value.ToString().Replace(".", ",")));
                                //
                                sl.SetCellStyle(R, 8, style5);
                                sl.SetCellValue(R, 8, Convert.ToDouble(row.Cells[7].Value.ToString().Replace(".", ",")));
                                //
                                sl.SetCellStyle(R, 9, style5);
                                sl.SetCellValue(R, 9, Convert.ToDouble(row.Cells[8].Value.ToString().Replace(".", ",")));
                                //
                                //
                                string fecha1, fecha2;
                                fecha1 = dateTimePicker1.Value.ToString("yyyy/MM/dd");
                                fecha2 = dateTimePicker2.Value.ToString("yyyy/MM/dd");

                                DataSet ds5 = new DataSet();
                                ds5 = Conexion.SeleccionarPagos(row.Cells[1].Value.ToString(), fecha1, fecha2);
                                if (ds5.Tables.Count != 0)
                                {
                                    if (ds5.Tables[0].Rows.Count == 0)
                                    {
                                        R++;
                                        primeravez = false;

                                    }
                                    else
                                    {
                                        foreach (DataRow r in ds5.Tables[0].Rows)
                                        {
                                            if (!primeravez)
                                            {

                                                sl.SetCellStyle(R, 1, style4);
                                                sl.SetCellValue(R, 1, row.Cells[0].Value.ToString());
                                                //
                                                sl.SetCellStyle(R, 2, style4);
                                                sl.SetCellValue(R, 2, row.Cells[1].Value.ToString());
                                                //
                                                sl.SetCellStyle(R, 3, style3);
                                                sl.SetCellValue(R, 3, row.Cells[2].Value.ToString());

                                                //
                                                sl.SetCellStyle(R, 4, style2);
                                                sl.SetCellValue(R, 4, row.Cells[3].Value.ToString().Replace(".", ","));

                                                //
                                                sl.SetCellStyle(R, 5, style2);
                                                sl.SetCellValue(R, 5, Convert.ToDouble(row.Cells[4].Value.ToString().Replace(".", ",")));

                                                //
                                                sl.SetCellStyle(R, 6, style2);
                                                sl.SetCellValue(R, 6, Convert.ToDouble(row.Cells[5].Value.ToString().Replace(".", ",")));
                                                //
                                                sl.SetCellStyle(R, 7, style2);
                                                sl.SetCellValue(R, 7, Convert.ToDouble(row.Cells[6].Value.ToString().Replace(".", ",")));
                                                //
                                                sl.SetCellStyle(R, 8, style5);
                                                sl.SetCellValue(R, 8, Convert.ToDouble(row.Cells[7].Value.ToString().Replace(".", ",")));
                                                //
                                                sl.SetCellStyle(R, 9, style5);
                                                sl.SetCellValue(R, 9, Convert.ToDouble(row.Cells[8].Value.ToString().Replace(".", ",")));
                                            }

                                            sl.SetCellStyle(R, 10, style2);
                                            sl.SetCellValue(R, 10, r["tipodepago"].ToString());
                                            //
                                            sl.SetCellStyle(R, 11, style4);
                                            sl.SetCellValue(R, 11, r["NombreBanco"].ToString());
                                            //
                                            sl.SetCellStyle(R, 12, style2);
                                            sl.SetCellValue(R, 12, Convert.ToDouble(r["Cantidad"].ToString().Replace(".", ",")));
                                            //
                                            sl.SetCellStyle(R, 13, style2);
                                            sl.SetCellValue(R, 13, Convert.ToDouble(r["ValorResultado"].ToString().Replace(".", ",")));

                                            //
                                            sl.SetCellStyle(R, 14, style2);
                                            sl.SetCellValue(R, 14, r["Serial"].ToString());

                                            //
                                            if (r["Clasificacion"].ToString() == "1")
                                            {
                                                sl.SetCellStyle(R, 15, style2);
                                                sl.SetCellValue(R, 15, "Ingreso");
                                            }
                                            else
                                            {
                                                sl.SetCellStyle(R, 15, style2);
                                                sl.SetCellValue(R, 15, "Vuelto");
                                            }

                                            sl.SetCellStyle(R, 16, style2);
                                            sl.SetCellValue(R, 16, r["Fecha"].ToString());
                                            R++;
                                            primeravez = false;
                                        }
                                    }
                            
                                    primeravez = true;
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            ex.ToString();
                        }
                        saveFileDialog1.Filter = "Excel|*.xlsx";
                        sl.SetColumnWidth(1, 5);
                        sl.SetColumnWidth(2, 15);
                        sl.SetColumnWidth(3, 25);
                        sl.SetColumnWidth(4, 45);
                        sl.SetColumnWidth(5, 20);
                        sl.SetColumnWidth(6, 20);
                        sl.SetColumnWidth(7, 20);
                        sl.SetColumnWidth(8, 15);
                        sl.SetColumnWidth(8, 15);
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            sl.SaveAs(saveFileDialog1.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
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

        private void iconButton1_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            DataSet ds3 = new DataSet();
            DataSet ds4 = new DataSet();
            string cmd, cmd2;
            cmd = dateTimePicker1.Value.ToString("yyyy/MM/dd");
            cmd2 = dateTimePicker2.Value.ToString("yyyy/MM/dd");
            ds.Clear();
            ds = Conexion.SELECTordenesPacientesCobro(cmd, cmd2);
            ds4.Tables.Add("Tabla1");
            ds4.Tables[0].Columns.Add("#");
            ds4.Tables[0].Columns.Add("IdOrden");
            ds4.Tables[0].Columns.Add("Cedula");
            ds4.Tables[0].Columns.Add("Nombre");
            ds4.Tables[0].Columns.Add("PrecioF");
            ds4.Tables[0].Columns.Add("Entradas");
            ds4.Tables[0].Columns.Add("Salidas");
            ds4.Tables[0].Columns.Add("Pagos");
            ds4.Tables[0].Columns.Add("Total");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                ds3 = Conexion.SELECTPagosordenesPacientes(r["IdOrden"].ToString(), cmd, cmd2);
                string IdORden = r["IdOrden"].ToString();
                if (ds3.Tables.Count != 0)
                {
                    if (ds3.Tables[0].Rows.Count != 0)
                    {
                        ds4.Tables[0].Rows.Add(r["#"].ToString(), r["IdOrden"].ToString(), r["Cedula"].ToString(), r["Nombre"].ToString(), r["Facturado"].ToString(), ds3.Tables[0].Rows[0]["Entradas"].ToString(), ds3.Tables[0].Rows[0]["Salidas"].ToString(), ds3.Tables[0].Rows[0]["Pago"].ToString(), ds3.Tables[0].Rows[0]["Total"].ToString());
                    }
                    else
                    {
                        ds4.Tables[0].Rows.Add(r["#"].ToString(), r["IdOrden"].ToString(), r["Cedula"].ToString(), r["Nombre"].ToString(), r["Facturado"].ToString(), "0", "0", "0", "-" + r["Facturado"].ToString());
                    }
                }

            }

            if (ds4.Tables.Count != 0)
            {
                advancedDataGridView1.DataSource = ds4.Tables[0];
            }
            else
            {
                ds.Clear();
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
