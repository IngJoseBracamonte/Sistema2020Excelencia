using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using SpreadsheetLight;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Spreadsheet;
using Conexiones.DbConnect;
using System.Security.Cryptography;

namespace Laboratorio
{
    public partial class Form23 : Form
    {
        DataSet Empresa = new DataSet();
        int IdUser;
        public Form23(int idUser)
        {
            IdUser = idUser;
            InitializeComponent();
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
            ds = Conexion.SELECTordenesPacientes(cmd, cmd2);
            ds4.Tables.Add("Tabla1");
            ds4.Tables[0].Columns.Add("#");
            ds4.Tables[0].Columns.Add("IdOrden");
            ds4.Tables[0].Columns.Add("Cedula");
            ds4.Tables[0].Columns.Add("Nombre");
            ds4.Tables[0].Columns.Add("Facturado");
            ds4.Tables[0].Columns.Add("Entradas");
            ds4.Tables[0].Columns.Add("Salidas");
            ds4.Tables[0].Columns.Add("Pagos");
            ds4.Tables[0].Columns.Add("Total");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                ds3 = Conexion.SELECTPagosordenesPacientes(r["IdOrden"].ToString(),cmd,cmd2);
                string IdORden = r["IdOrden"].ToString();
                if (ds3.Tables.Count != 0)
                {
                    if (ds3.Tables[0].Rows.Count != 0)
                    {
                        ds4.Tables[0].Rows.Add(r["#"].ToString(), r["IdOrden"].ToString(), r["Cedula"].ToString(), r["Nombre"].ToString(), r["Facturado"].ToString(), ds3.Tables[0].Rows[0]["Entradas"].ToString(), ds3.Tables[0].Rows[0]["Salidas"].ToString(), ds3.Tables[0].Rows[0]["Pago"].ToString(), ds3.Tables[0].Rows[0]["Total"].ToString());
                    }
                    else
                    {
                        ds4.Tables[0].Rows.Add(r["#"].ToString(), r["IdOrden"].ToString(), r["Cedula"].ToString(), r["Nombre"].ToString(), r["Facturado"].ToString(), "0", "0", "0","-" + r["Facturado"].ToString());
                    }
                }
                
            }
            
            if (ds4.Tables.Count != 0)
            {
                dataGridView1.DataSource = ds4.Tables[0];
            }
            else
            {
                ds.Clear();
            }

            DataSet ds1 = new DataSet();
            ds1 = Conexion.SELECTTotalFacturadoFecha(cmd, cmd2);
            if (ds1.Tables[0].Rows.Count != 0)
            {
                if (ds1.Tables[0].Rows[0]["SumaDePrecioF"].ToString() != "")
                {
                    if (ds1.Tables[0].Rows[0]["SumaDePrecioF"].ToString() != " " && ds1.Tables[0].Rows[0]["SumaDePrecioF"].ToString() != "")
                    {
                        Nombre.Text = ds1.Tables[0].Rows[0]["SumaDePrecioF"].ToString();
                    }
                    else
                    {
                        Nombre.Text = "0";
                    }
                    if (ds1.Tables[0].Rows[0]["DOLARES"].ToString() != " " && ds1.Tables[0].Rows[0]["DOLARES"].ToString() != "")
                    {
                        textBox3.Text = ds1.Tables[0].Rows[0]["DOLARES"].ToString();
                    }
                    else
                    {
                        textBox3.Text = "0";
                    }
                }
            }
            else
            {
                Nombre.Text = "0";
                textBox3.Text = "0";
            }

            DataSet ds2 = new DataSet();
            ds2 = Conexion.SELECTTotalPagoFecha(cmd, cmd2);
            if (ds2.Tables[0].Rows[0]["PAGO"].ToString() != " " && ds2.Tables[0].Rows[0]["PAGO"].ToString() != "")
            {
                textBox1.Text = ds2.Tables[0].Rows[0]["PAGO"].ToString();
            }
            else
            {
                textBox1.Text = "0";
            }
            double PrecioF, Total, Calculo;
            string patron = @"[^\w]";
            Regex regex = new Regex(patron);
            string Text1, Text2;
            Text1 = Nombre.Text;
            Text2 = textBox1.Text;
            if (Text1 == "" || Text1 == "0")
            {
                PrecioF = 0;
            }
            else 
            {
                PrecioF = Convert.ToDouble(Text1);
            }
            if (Text1 == "" || Text1 == "0")
            {
                Total = 0;
            }
            else
            {
                Total = Convert.ToDouble(Text2);
            }
            Calculo = Math.Round(Total - PrecioF,2);
            textBox2.Text = Calculo.ToString();
        }



        private void Form23_Load(object sender, EventArgs e)
        {

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            // Este es el original
            Empresa = Conexion.SelectEmpresaActiva();
            try
            {
                if (dataGridView1.Rows.Count != 0)
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
                        foreach (DataGridViewColumn colum in dataGridView1.Columns)
                        {
                            sl.SetCellValue(1, C, colum.HeaderText.ToString());
                            sl.SetCellStyle(1, C, style);
                            C++;
                        }
                        C = 1;
                        try
                        {
                            foreach (DataGridViewRow row in dataGridView1.Rows)
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
                                sl.SetCellValue(R, 5, row.Cells[4].Value.ToString());

                                //
                                sl.SetCellStyle(R, 6, style2);
                                sl.SetCellValue(R, 6, Convert.ToDouble(row.Cells[5].Value.ToString()));
                                //
                                sl.SetCellStyle(R, 7, style2);
                                sl.SetCellValue(R, 7, Convert.ToDouble(row.Cells[6].Value.ToString()));
                                //
                                sl.SetCellStyle(R, 8, style5);
                                sl.SetCellValue(R, 8, Convert.ToDouble(row.Cells[7].Value.ToString()));
                                //
                                sl.SetCellStyle(R, 9, style5);
                                sl.SetCellValue(R, 9, Convert.ToDouble(row.Cells[8].Value.ToString()));
                                //
                                //
                                string fecha1, fecha2;
                                fecha1 = dateTimePicker1.Value.ToString("yyyy/MM/dd");
                                fecha2 = dateTimePicker2.Value.ToString("yyyy/MM/dd");

                                DataSet ds5 = new DataSet();
                                ds5 = Conexion.SeleccionarPagos(row.Cells[1].Value.ToString(), fecha1, fecha2);
                                if (ds5.Tables.Count != 0)
                                {
                                    R++;
                                    sl.SetCellValue(R, 2, "Tipo de Pago");
                                    sl.SetCellStyle(R, 2, style);
                                    sl.SetCellValue(R, 3, "Cantidad");
                                    sl.SetCellStyle(R, 3, style);
                                    sl.SetCellValue(R, 4, "Cantidad En BsS");
                                    sl.SetCellStyle(R, 4, style);
                                    sl.SetCellValue(R, 5, "Serial");
                                    sl.SetCellStyle(R, 5, style);
                                    sl.SetCellValue(R, 6, "Clasificacion");
                                    sl.SetCellStyle(R, 6, style);
                                    sl.SetCellValue(R, 7, "Fecha");
                                    sl.SetCellStyle(R, 7, style);
                                    C = 1;
                               
                                    foreach (DataRow r in ds5.Tables[0].Rows)
                                    {
                                        R++;
                                        sl.SetCellStyle(R, 2, style2);
                                        sl.SetCellValue(R, 2, $"{r["tipodepago"].ToString()} {r["NombreBanco"].ToString()}");
                                        //
                                        sl.SetCellStyle(R, 3, style2);
                                        sl.SetCellValue(R, 3, Convert.ToDouble(r["Cantidad"].ToString()));
                                        //
                                        sl.SetCellStyle(R, 4, style2);
                                        sl.SetCellValue(R, 4, Convert.ToDouble(r["ValorResultado"].ToString()));

                                        //
                                        sl.SetCellStyle(R, 5, style2);
                                        sl.SetCellValue(R, 5, r["Serial"].ToString());

                                        //
                                        if (r["Clasificacion"].ToString() == "1")
                                        {
                                            sl.SetCellStyle(R, 6, style2);
                                            sl.SetCellValue(R, 6, "Ingreso");
                                        }
                                        else
                                        {
                                            sl.SetCellStyle(R, 6, style2);
                                            sl.SetCellValue(R, 6, "Devuelto");
                                        }
                                        
                                        sl.SetCellStyle(R, 7, style2);
                                        sl.SetCellValue(R, 7, r["Fecha"].ToString());
                                    }
                                    R++;
                                }
                               
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.ToString();
                        }
                        string patron = @"[^\w]";
                        Regex regex = new Regex(patron);
                        sl.SetCellValue(R, 3, "TOTAL FACTURADO");
                        sl.SetCellStyle(R, 3, style);
                        string Convertido = textBox3.Text;
                        sl.SetCellValue(R, 5, Convert.ToDouble(textBox3.Text));
                        sl.SetCellStyle(R, 5, style);
                        sl.SetCellValue(R, 4, Nombre.Text);
                        sl.SetCellStyle(R, 4, style);
                        R++;
                        sl.SetCellValue(R, 3, "TOTAL COBRADO");
                        sl.SetCellStyle(R, 3, style);
                        string Convertido1 = textBox1.Text;
                        sl.SetCellValue(R, 4, Convert.ToDouble(Convertido1));
                        sl.SetCellStyle(R, 4, style);

                        R++;
                        sl.SetCellValue(R, 3, "DIFERENCIA");
                        sl.SetCellStyle(R, 3, style);
                        string Convertido2 = textBox2.Text;
                        sl.SetCellValue(R, 4, Convert.ToDouble(Convertido2));
                        sl.SetCellStyle(R, 4, style);
                        R++;
                        sl.SetCellValue(R, 3, "SEDE");
                        sl.SetCellStyle(R, 3, style);
                        sl.SetCellValue(R, 4, Empresa.Tables[0].Rows[0]["Nombre"].ToString());
                        sl.SetCellStyle(R, 4, style);
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            int CobroID = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["IdOrden"].Value);
            Form Cobro = new Cobro(CobroID,IdUser);
            Cobro.Show();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
           
    
        }
    }
}
