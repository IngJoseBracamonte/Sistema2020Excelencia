using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Conexiones.DbConnect;
using SpreadsheetLight;

namespace Laboratorio
{
    public partial class Form25 : Form
    {

        private int cc3, cc5, cc10,tubos;
        private string OrdenAnterior;
        bool Ultima,especiales = false;
        public Form25()
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
                        sl.SetCellValue(R, 1, row.Cells[0].Value.ToString());
                        sl.SetCellValue(R, 2, row.Cells[1].Value.ToString());
                        R++;
                    }
                    sl.SetCellValue(R, 3, "TOTAL ANALISIS=");
                    sl.SetCellStyle(R, 3, style);
                    sl.SetCellValue(R, 4, Nombre.Text);
                    sl.SetCellStyle(R, 4, style);
                    R++;
                    sl.SetCellValue(R, 3, "TOTAL PACIENTES=");
                    sl.SetCellStyle(R, 3, style);
                    sl.SetCellValue(R, 4, Pacientes.Text);
                    sl.SetCellStyle(R, 4, style);
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
                MessageBox.Show(ex.ToString() + " Selecciono el nombre de un archivo que posiblemente este en uso, por favor cierre el archivo o cambie el nombre");
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            int Suma = 0;
            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();
            DataSet ds2 = new DataSet();
            DataSet ds3 = new DataSet();
            DataSet ds4 = new DataSet();
            DataSet ds5 = new DataSet();
            DataSet ds6 = new DataSet();
            DataSet ds7 = new DataSet();
            DataSet Empresa = new DataSet();
            int ConveniosSumados = 0;

            string cmd, cmd2;
            cmd = dateTimePicker1.Value.ToString("yyyy/MM/dd");
            cmd2 = dateTimePicker2.Value.ToString("yyyy/MM/dd");
            ds.Clear();
            ds6 = Conexion.CantidadDeHojasFecha(cmd, cmd2);
            if (ds6.Tables.Count == 0 || ds6.Tables[0].Rows.Count == 0)
            {
                textBox2.Text = "0";
            }
            else 
            {
                if (ds6.Tables[0].Rows[0]["Total"].ToString() == "")
                {
                    textBox2.Text = "0";
                }
                else 
                {
                    textBox2.Text = ds6.Tables[0].Rows[0]["Total"].ToString();
                }
                    
            }

            if (comboBox2.Text == "1")
            {
                checkBox1.Checked = false;
                ds5 = Conexion.SecuenciaDeLEAN(cmd, cmd2);
                dataGridView2.DataSource = ds5.Tables[0];
                Empresa = Conexion.SelectEmpresaActiva();
                if (Empresa.Tables[0].Rows[0]["IdEmpresa"].ToString() == "5")
                {
                    ds = Conexion.EstadisticasEspeciales(cmd, cmd2);
                    ds.Tables[0].Columns.Remove(ds.Tables[0].Columns[0]);
                    ds4 = Conexion.EstadisticasPorConvenio(cmd, cmd2);
                    if (ds4.Tables.Count != 0)
                    {
                        if (ds4.Tables[0].Rows.Count != 0)
                        {
                            foreach (DataRow r in ds4.Tables[0].Rows)
                            {
                                if (r["IdConvenio"].ToString() == "5")
                                {
                                    label11.Text = r["Cantidad"].ToString();
                                    try
                                    {
                                        int Rivana;
                                        Rivana = Convert.ToInt32(r["Cantidad"].ToString());
                                        ConveniosSumados = ConveniosSumados + Rivana;
                                    }
                                    catch
                                    {

                                    }
                                }
                                else if (r["IdConvenio"].ToString() == "6")
                                {
                                    label12.Text = r["Cantidad"].ToString();
                                    try
                                    {
                                        int AP;
                                        AP = Convert.ToInt32(r["Cantidad"].ToString());
                                        ConveniosSumados = ConveniosSumados + AP;
                                    }
                                    catch
                                    {

                                    }

                                }
                                else if (r["IdConvenio"].ToString() == "7")
                                {
                                    label17.Text = r["Cantidad"].ToString();
                                    try
                                    {
                                        int ARO;
                                        ARO = Convert.ToInt32(r["Cantidad"].ToString());
                                        ConveniosSumados = ConveniosSumados + ARO;
                                    }
                                    catch
                                    {

                                    }

                                }
                                else if (r["IdConvenio"].ToString() == "8")
                                {
                                    label14.Text = r["Cantidad"].ToString();
                                    try
                                    {
                                        int AB;
                                        AB = Convert.ToInt32(r["Cantidad"].ToString());
                                        ConveniosSumados = ConveniosSumados + AB;
                                    }
                                    catch
                                    {

                                    }
                                }
                                else if (r["IdConvenio"].ToString() == "9")
                                {

                                    label13.Text = r["Cantidad"].ToString();
                                    try
                                    {
                                        int HOL;
                                        HOL = Convert.ToInt32(r["Cantidad"].ToString());
                                        ConveniosSumados = ConveniosSumados + HOL;
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                            label15.Text = ConveniosSumados.ToString();
                        }
                    }
                }
                else
                {
                    ds = Conexion.Estadisticas(cmd, cmd2);
                    ds4 = Conexion.SELECTConteoReferidos(cmd, cmd2);
                    if (ds4.Tables.Count != 0)
                    {
                        if (ds4.Tables[0].Rows.Count != 0)
                        {
                            label5.Visible = true;
                            label15.Visible = true;
                            label15.Text = ds4.Tables[0].Rows[0][0].ToString();
                        }
                    }
                }

                if (ds.Tables.Count != 0)
                {
                    dataGridView1.DataSource = ds.Tables[0];
                }
                else
                {
                    ds.Clear();
                }
                ds1 = Conexion.TotalPersonasFacturadas(cmd, cmd2);
                if (ds1.Tables[0].Rows.Count != 0)
                {
                    if (ds1.Tables[0].Rows[0]["CuentaDeIdOrden"].ToString() != "")
                    {
                        Pacientes.Text = ds1.Tables[0].Rows[0]["CuentaDeIdOrden"].ToString();
                    }
                }
                else
                {
                    Nombre.Text = "0";
                }
                if (dataGridView1.Rows.Count != 0)
                {
                    if (Empresa.Tables[0].Rows[0]["IdEmpresa"].ToString() == "5")
                    {
                        foreach (DataGridViewRow r in dataGridView1.Rows)
                        {
                            Suma += Convert.ToInt32(r.Cells["Cantidad"].Value);
                        }
                        Nombre.Text = Suma.ToString();
                    }
                    else
                    {
                        foreach (DataGridViewRow r in dataGridView1.Rows)
                        {
                            Suma += Convert.ToInt32(r.Cells["CuentaDeIdPerfil"].Value);
                        }
                        Nombre.Text = Suma.ToString();
                    }
                }
                else
                {
                    Nombre.Text = Suma.ToString();
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
            else 
            {
                checkBox1.Checked = false;
                ds5 = Conexion.SecuenciaDeLEANPorSede(cmd, cmd2, comboBox2.Text);
                dataGridView2.DataSource = ds5.Tables[0];
                Empresa = Conexion.SelectEmpresaActiva();
                if (Empresa.Tables[0].Rows[0]["IdEmpresa"].ToString() == "5")
                {
                    ds = Conexion.EstadisticasEspeciales(cmd, cmd2);
                    ds.Tables[0].Columns.Remove(ds.Tables[0].Columns[0]);
                    ds4 = Conexion.EstadisticasPorConvenio(cmd, cmd2);
                    if (ds4.Tables.Count != 0)
                    {
                        if (ds4.Tables[0].Rows.Count != 0)
                        {
                            foreach (DataRow r in ds4.Tables[0].Rows)
                            {
                                if (r["IdConvenio"].ToString() == "5")
                                {
                                    label11.Text = r["Cantidad"].ToString();
                                    try
                                    {
                                        int Rivana;
                                        Rivana = Convert.ToInt32(r["Cantidad"].ToString());
                                        ConveniosSumados = ConveniosSumados + Rivana;
                                    }
                                    catch
                                    {

                                    }
                                }
                                else if (r["IdConvenio"].ToString() == "6")
                                {
                                    label12.Text = r["Cantidad"].ToString();
                                    try
                                    {
                                        int AP;
                                        AP = Convert.ToInt32(r["Cantidad"].ToString());
                                        ConveniosSumados = ConveniosSumados + AP;
                                    }
                                    catch
                                    {

                                    }

                                }
                                else if (r["IdConvenio"].ToString() == "7")
                                {
                                    label17.Text = r["Cantidad"].ToString();
                                    try
                                    {
                                        int ARO;
                                        ARO = Convert.ToInt32(r["Cantidad"].ToString());
                                        ConveniosSumados = ConveniosSumados + ARO;
                                    }
                                    catch
                                    {

                                    }

                                }
                                else if (r["IdConvenio"].ToString() == "8")
                                {
                                    label14.Text = r["Cantidad"].ToString();
                                    try
                                    {
                                        int AB;
                                        AB = Convert.ToInt32(r["Cantidad"].ToString());
                                        ConveniosSumados = ConveniosSumados + AB;
                                    }
                                    catch
                                    {

                                    }
                                }
                                else if (r["IdConvenio"].ToString() == "9")
                                {

                                    label13.Text = r["Cantidad"].ToString();
                                    try
                                    {
                                        int HOL;
                                        HOL = Convert.ToInt32(r["Cantidad"].ToString());
                                        ConveniosSumados = ConveniosSumados + HOL;
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                            label15.Text = ConveniosSumados.ToString();
                        }
                    }
                }
                else
                {
                    ds = Conexion.EstadisticasPorSede(cmd, cmd2,comboBox2.Text);
                    ds4 = Conexion.SELECTConteoReferidos(cmd, cmd2);
                    if (ds4.Tables.Count != 0)
                    {
                        if (ds4.Tables[0].Rows.Count != 0)
                        {
                            label5.Visible = true;
                            label15.Visible = true;
                            label15.Text = ds4.Tables[0].Rows[0][0].ToString();
                        }
                    }
                }

                if (ds.Tables.Count != 0)
                {
                    dataGridView1.DataSource = ds.Tables[0];
                }
                else
                {
                    ds.Clear();
                    dataGridView1.DataSource = "";
                }
                ds1 = Conexion.TotalPersonasFacturadasPorSede(cmd, cmd2, comboBox2.Text);
                if (ds1.Tables[0].Rows.Count != 0)
                {
                    if (ds1.Tables[0].Rows[0]["CuentaDeIdOrden"].ToString() != "")
                    {
                        Pacientes.Text = ds1.Tables[0].Rows[0]["CuentaDeIdOrden"].ToString();
                    }
                }
                else
                {
                    Nombre.Text = "0";
                }
                if (dataGridView1.Rows.Count != 0)
                {
                    if (Empresa.Tables[0].Rows[0]["IdEmpresa"].ToString() == "5")
                    {
                        foreach (DataGridViewRow r in dataGridView1.Rows)
                        {
                            Suma += Convert.ToInt32(r.Cells["Cantidad"].Value);
                        }
                        Nombre.Text = Suma.ToString();
                    }
                    else
                    {
                        foreach (DataGridViewRow r in dataGridView1.Rows)
                        {
                            Suma += Convert.ToInt32(r.Cells["CuentaDeIdPerfil"].Value);
                        }
                        Nombre.Text = Suma.ToString();
                    }
                }
                else
                {
                    Nombre.Text = Suma.ToString();
                }


                ds3 = Conexion.SELECTTotalFacturadoFecha(cmd, cmd2);
                if (ds3.Tables[0].Rows.Count != 0)
                {
                    if (ds3.Tables[0].Rows[0]["SumaDePrecioF"].ToString() != "" && ds3.Tables[0].Rows[0]["SumaDePrecioF"].ToString() != " ")
                    {
                        textBox1.Text = ds3.Tables[0].Rows[0]["SumaDePrecioF"].ToString();
                        textBox3.Text = Convert.ToDouble(ds3.Tables[0].Rows[0]["Dolares"].ToString()).ToString("C", new CultureInfo("en-US"));
                    }
                }
                else
                {
                    textBox1.Text = "0";
                    textBox3.Text = "0";
                }
            }
        }

        private void Nombre_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form25_Load(object sender, EventArgs e)
        {
            DataSet Empresa = new DataSet();
            DataSet Empresa1 = new DataSet();
            DataSet EstadisticasPorConvenio = new DataSet();
            Empresa = Conexion.SelectEmpresaActiva();
            if (Empresa.Tables[0].Rows[0][0].ToString() == "5")
            {

                label5.Visible = true;
                label6.Visible = true;
                label7.Visible = true;
                label8.Visible = true;
                label9.Visible = true;
                label11.Visible = true;
                label12.Visible = true;
                label13.Visible = true;
                label14.Visible = true;
                label15.Visible = true;
                label18.Visible = true;
                label17.Visible = true;
            }
            Empresa1 = Conexion.SelectEmpresaActivaEstadisticas();
            foreach (DataRow r in Empresa1.Tables[0].Rows)
            {
                comboBox1.Items.Add(r["Nombre"]);
                comboBox2.Items.Add(r["IdConvenio"]);
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = comboBox1.SelectedIndex;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }
        private void CalcularJeringas(string Orden,string Seccion)
        {
            if (OrdenAnterior == null)
            {
                OrdenAnterior = Orden;
            }

            if (OrdenAnterior == Orden)
            {
                if (Seccion == "0")
                {
                    cc3 = cc3 + 1;
                }
                else if (Seccion == "1")
                {
                    tubos = tubos + 1;
                }
                else if (Seccion == "2")
                {
                    tubos = tubos + 1;
                }
                else if (Seccion == "6" || Seccion == "7")
                {
                    if (!especiales)
                    {
                        tubos = tubos + 1;
                    }
                    especiales = true;
                }
                else if (Seccion == "21")
                {
                    tubos = tubos + 1;
                } 
            }
            else
            {
                switch (tubos)
                {
                    case 1:
                        cc3 = cc3 + 1;
                        break;
                    case 2:
                        cc5 = cc5 + 1;
                        break;
                    case 3:
                        cc10 = cc10 + 1;
                        break;
                    case 4:
                        cc3 = cc3 + 1;
                        cc10 = cc10 + 1;
                        break;
                    case 5:
                        cc5 = cc5 + 1;
                        cc10 = cc10 + 1;
                        break;
                }
                tubos = 0;
                especiales = false;
                OrdenAnterior = Orden;
                if (Seccion == "0")
                {
                    cc3 = cc3 + 1;
                }
                else if (Seccion == "1")
                {
                    tubos = tubos + 1;
                }
                else if (Seccion == "2")
                {
                    tubos = tubos + 1;
                }
                else if (Seccion == "6" || Seccion == "7")
                {
                    if (!especiales)
                    {
                        tubos = tubos + 1;
                    }
                    especiales = true;
                }
                else if (Seccion == "21")
                {
                    tubos = tubos + 1;
                }
                if (Ultima)
                {
                    switch (tubos)
                    {
                        case 1:
                            cc3 = cc3 + 1;
                            break;
                        case 2:
                            cc5 = cc5 + 1;
                            break;
                        case 3:
                            cc10 = cc10 + 1;
                            break;
                        case 4:
                            cc3 = cc3 + 1;
                            cc10 = cc10 + 1;
                            break;
                        case 5:
                            cc5 = cc5 + 1;
                            cc10 = cc10 + 1;
                            break;
                    }
                }
               
            }

        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                
                string cmd, cmd2;
                DataSet ds5 = new DataSet();
                cmd = dateTimePicker1.Value.ToString("yyyy/MM/dd");
                cmd2 = dateTimePicker2.Value.ToString("yyyy/MM/dd");
                ds5 = Conexion.ConteoSecuenciaDeLEAN(cmd, cmd2);
                dataGridView2.DataSource = ds5.Tables[0];
            }
            else 
            {
                DataSet ds5 = new DataSet();
                string cmd, cmd2;
                cmd = dateTimePicker1.Value.ToString("yyyy/MM/dd");
                cmd2 = dateTimePicker2.Value.ToString("yyyy/MM/dd");
                ds5 = Conexion.SecuenciaDeLEAN(cmd, cmd2);
                dataGridView2.DataSource = ds5.Tables[0];
            }
        }
    }
}
