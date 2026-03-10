using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FontAwesome.Sharp;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class Form3 : Form
    {
        private Double PFinal;
        private bool Analisisagregado = false;
        int IdPaciente = 0;
        private string cmd1;
        private bool IDAnalisisBox, Activador, Cambio = false;
        private bool TextAnalisisBox = false;
        public DataTable dt = new DataTable();
        public static int Sesion;
        private int SesionLocal;
        private Double IntPrecioF;
        string DatosAntiguosPaciente;
        int IdUser;
        bool NoEncontrado, tipolab,agregado;
        public Form3(int idUser)
        {
            IdUser = idUser;
            InitializeComponent();
            label26.Visible = false;
            label28.Visible = false;

        }


        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            string cmd;
            cmd = CedulaBox.Text;
            DataSet ds = new DataSet();
            ds = Conexion.SELECTPersona(cmd);
            DataSet ds1 = new DataSet();
            ds1 = Conexion.PrivilegiosCargar(IdUser.ToString());
            Nombresbox.Enabled = true;
            ApellidoBox.Enabled = true;
            CelularBox.Enabled = true;
            TelefonoCasaBox.Enabled = true;
            SexoBox.Enabled = true;
            CodigoCelBox.Enabled = true;
            CodigoTelefonoBox.Enabled = true;
            FechaPicker.Enabled = true;
            CorreoBox.Enabled = true;
            TipoCorreoBox.Enabled = true;

            if (ds.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("Paciente no encontrado");
                iconButton9.IconChar = IconChar.Plus;
                iconButton9.Visible = true;
                NoEncontrado = true;
                this.AcceptButton = iconButton9;
            }
            else
            {
                if (ds1.Tables[0].Rows[0]["ModificarOrden"].ToString() != "1") 
                { 
                Nombresbox.Enabled = false;
                ApellidoBox.Enabled = false;
                }
                DatosAntiguosPaciente = string.Format("{0} {1} {2} {3}", ds.Tables[0].Rows[0]["Nombre"].ToString(), ds.Tables[0].Rows[0]["Apellidos"].ToString(),ds.Tables[0].Rows[0]["Sexo"].ToString(), ds.Tables[0].Rows[0]["Fecha"].ToString());
                CedulaBox.Enabled = false;
                iconButton9.Visible = false;
                IdPaciente = Convert.ToInt32(ds.Tables[0].Rows[0]["IdPersona"].ToString());
                Nombresbox.Text = ds.Tables[0].Rows[0]["Nombre"].ToString();
                ApellidoBox.Text = ds.Tables[0].Rows[0]["Apellidos"].ToString();
                CelularBox.Text = ds.Tables[0].Rows[0]["Celular"].ToString();
                TelefonoCasaBox.Text = ds.Tables[0].Rows[0]["Telefono"].ToString();
                SexoBox.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                CodigoCelBox.Text = ds.Tables[0].Rows[0]["CodigoCelular"].ToString();
                CodigoTelefonoBox.Text = ds.Tables[0].Rows[0]["CodigoTelefono"].ToString();
                FechaPicker.Text = ds.Tables[0].Rows[0]["Fecha"].ToString();
                CorreoBox.Text = ds.Tables[0].Rows[0]["Correo"].ToString();
                TipoCorreoBox.Text = ds.Tables[0].Rows[0]["TipoCorreo"].ToString();
                iconButton4.Enabled = true;
                iconButton4.Visible = true;
                iconButton3.Visible = false;
                this.AcceptButton = iconButton4;
                string cmd3 = string.Format("id={0}",IdPaciente);
                Conexion.ActualizarIDTemporal(cmd3, SesionLocal.ToString());
                Activador = true;
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            ApellidoBox.Text.ToUpper();
            if (Activador)
            {
                Cambio = true;
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
         
            tipolab = false;
            Sesion = Conexion.SesionTemporal();
            SesionLocal = Sesion;
            this.ActiveControl = CedulaBox;
            string cmd;
            DataSet Ds = new DataSet();
            cmd = IdUser.ToString();
            Ds = Conexion.convenios(cmd);
            for (int i = 0; i < Ds.Tables[0].Rows.Count; ++i)
            {
                NConvenioBox.Items.Add(Ds.Tables[0].Rows[i]["Nombre"]);
                comboBox1.Items.Add(Ds.Tables[0].Rows[i]["idConvenio"]);
                comboBox2.Items.Add(Ds.Tables[0].Rows[i]["Descuento"]);
            }
            this.AcceptButton = iconButton3;
             
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            Conexion.ActualizarPrueba(IdPaciente.ToString(), SesionLocal.ToString());
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            try 
            {
                if (Conexion.TestConnection() == "Conectado")
                {
                    if (Permisos.Tables[0].Rows[0]["ImprimirFactura"].ToString() == "1")
                    {
                       string cmd = Conexion.InsertarFactura(DateTime.Now.ToString("yyyy/MM/dd"), IdPaciente.ToString(), DateTime.Now.ToString("h:mm:ss tt"), IdUser.ToString(), IntPrecioF.ToString(), SesionLocal.ToString(), comboBox1.Text);
                        MessageBox.Show("Agregado Satisfactoriamente!");
                        IntPrecioF = 0;
                        int IdOrden = Convert.ToInt32(cmd);
                        Form Cobro = new Cobro(IdOrden,IdUser);
                        Cobro.Show();
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Ha Ocurrido Un Error Por Favor Intente De Nuevo");
                }
             
            }
            catch (Exception ex)
            {
                Conexion.CrearEvento(ex.ToString());
            }
        }

        private void Form3_Leave(object sender, EventArgs e)
        {

        }

        private void IdConveniobox_TextChanged(object sender, EventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void IdConveniobox_KeyUp(object sender, KeyEventArgs e)
        {


        }

        private void NConvenioBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = NConvenioBox.SelectedIndex;
            comboBox1.SelectedIndex = index;

        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            string cmd;
            cmd = string.Format("Nombre ='{0}',Apellidos ='{1}',Celular ='{2}',Telefono ='{3}',Sexo ='{4}',Correo ='{5}',Fecha = '{6}', TipoCorreo ='{7}',CodigoCelular = '{8}', CodigoTelefono = '{9}'", Nombresbox.Text, ApellidoBox.Text, CelularBox.Text, TelefonoCasaBox.Text, SexoBox.Text.ToUpper(), CorreoBox.Text, FechaPicker.Text, TipoCorreoBox.Text, CodigoCelBox.Text, CodigoTelefonoBox.Text);
            agregado = true;
            string MS = Conexion.Actualizardatospersonales(cmd, CedulaBox.Text);
            
            
            if (Cambio)
            {
                string Accion = string.Format("Se cambiaron los datos del paciente C.I:{0} {1} por {2}", CedulaBox.Text, DatosAntiguosPaciente, cmd);
                Conexion.InsertarReporte(Accion, IdUser.ToString());
            }
            

            MessageBox.Show("Paciente Actualizado");
            CedulaBox.Enabled = false;

            Nombresbox.Enabled = false;
            ApellidoBox.Enabled = false;
            CelularBox.Enabled = false;
            TelefonoCasaBox.Enabled = false;
            SexoBox.Enabled = false;
            CodigoCelBox.Enabled = false;
            CodigoTelefonoBox.Enabled = false;
            FechaPicker.Enabled = false;
            CorreoBox.Enabled = false;
            TipoCorreoBox.Enabled = false;
            iconButton3.Visible = false;
            iconButton4.Visible = false;
            iconButton5.Enabled = true;
            this.AcceptButton = iconButton5;
            if (Analisisagregado == true)
            {
                iconButton2.Enabled = true;
            }

        }

        private void iconButton5_Click(object sender, EventArgs e)
        {

            if (NConvenioBox.Text == "" && comboBox1.Text == "")
            {
                int index = 0;
                if (comboBox1.Items.Count != 0) {
                    comboBox1.SelectedIndex = index;
                    comboBox2.SelectedIndex = index;
                    NConvenioBox.SelectedIndex = index;
                    comboBox1.Enabled = false;
                    NConvenioBox.Enabled = false;
                    iconButton5.Enabled = false;
                    IdAnalisisBox.Enabled = true;
                    NombreAnalisisBox.Enabled = true;
                    this.AcceptButton = iconButton6;
                }
            }
            else if (NConvenioBox.Text == "" && comboBox1.Text != "") {
                if (comboBox1.Items.Count != 0)
                {
                    int index = comboBox1.SelectedIndex;
                    comboBox1.SelectedIndex = index;
                    NConvenioBox.SelectedIndex = index;
                    comboBox2.SelectedIndex = index;
                    comboBox1.Enabled = false;
                    NConvenioBox.Enabled = false;
                    iconButton5.Enabled = false;
                    IdAnalisisBox.Enabled = true;
                    NombreAnalisisBox.Enabled = true;
                    this.AcceptButton = iconButton6;
                }
            }
            else if (NConvenioBox.Text != "")
            {
                if (comboBox1.Items.Count != 0)
                {
                    comboBox1.Enabled = false;
                    NConvenioBox.Enabled = false;
                    iconButton5.Enabled = false;
                    IdAnalisisBox.Enabled = true;
                    NombreAnalisisBox.Enabled = true;
                    int index = NConvenioBox.SelectedIndex;
                    comboBox1.SelectedIndex = index;
                    if (NConvenioBox.Text != "Descuento gerencia")
                    {
                        comboBox2.SelectedIndex = index;
                    }
                    comboBox2.Enabled=false;
                    this.AcceptButton = iconButton6;
                }
            }
            IdAnalisisBox.Focus();
        }

        private void IdConveniobox_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void IdAnalisisBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void iconButton6_Click(object sender, EventArgs e)
        {

        }

        private void IdAnalisisBox_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void IdAnalisisBox_DoubleClick(object sender, EventArgs e)
        {

        }

        private void IdAnalisisBox_Click(object sender, EventArgs e)
        {
        }

        private void iconButton6_Click_1(object sender, EventArgs e)
        {

            agregaranalisis();
        }

        private void IdAnalisisBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Escape)
            {
                NombreAnalisisBox.Enabled = true;
                iconButton6.Enabled = true;
            }
            else if (e.KeyChar == '\b')
            {
                e.Handled = false;
            }
            else if (e.KeyChar >= 48 && e.KeyChar <= 57/*Admite los numeros del 0 al 9*/)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

        }

        private void iconButton9_Click(object sender, EventArgs e)
        {

        }

        private void NombreAnalisisBox_Click(object sender, EventArgs e)
        {
            IdAnalisisBox.Enabled = false;
            iconButton6.Enabled = false;
        }


        private void NombreAnalisisBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Escape)
            {
                IdAnalisisBox.Enabled = true;
                iconButton6.Enabled = true;
            }
            DataSet DS1 = new DataSet();
            string cmd = NombreAnalisisBox.Text;
            DS1 = Conexion.SELECTAnalisisText(cmd);
            dataGridView1.DataSource = DS1.Tables[0];
            DataGridViewColumn column2 = dataGridView1.Columns[0];
            column2.Width = 50;
            DataGridViewColumn column = dataGridView1.Columns[1];
            column.Width = 403;
        }

        private void iconButton7_Click(object sender, EventArgs e)
        {
            bool encontrado = false;
            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();
            if (dataGridView1.Rows.Count != 0)
            {
               
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    cmd1 = row.Cells["IdPerfil"].Value.ToString();
                }
                ds = Conexion.SELECTAnalisis(cmd1);

                if (dataGridView2.Rows.Count != 0)
                {
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (Convert.ToString(row.Cells["IdPerfil"].Value) == ds.Tables[0].Rows[0]["IdPerfil"].ToString())
                        {
                            encontrado = true;
                        }
                    }
                    

                    if (encontrado != true)
                    {

                        if (Conexion.TestConnection() == "Conectado")
                        {
                            string Calculado = Math.Round(Convert.ToDouble(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Precio"].Value.ToString()) - Convert.ToDouble(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Precio"].Value.ToString()) * (Convert.ToDouble(comboBox2.SelectedItem) / 100),2).ToString();
                            string respuesta = Conexion.InsertarPrueba(IdPaciente.ToString(), dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["IDPerfil"].Value.ToString(), Calculado, SesionLocal.ToString()).Result;
                            ActualizarDatagrid();

                            if (respuesta != "")
                            {
                                MessageBox.Show(respuesta);
                            }
                        }
                        else 
                        {
                            MessageBox.Show("Ha Ocurrido Un error Por favor Intente de nuevo");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Analisis ya agregado");
                    }
                }
                else
                {
                    if (Conexion.TestConnection() == "Conectado")
                    {

                        string Calculado = Math.Round(Convert.ToDouble(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Precio"].Value.ToString()) - Convert.ToDouble(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Precio"].Value.ToString()) * (Convert.ToDouble(comboBox2.Text) / 100),2).ToString();
                        string respuesta = Conexion.InsertarPrueba(IdPaciente.ToString(), dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["IDPerfil"].Value.ToString(), Calculado, SesionLocal.ToString()).Result;
                        ActualizarDatagrid();
                        if (respuesta != "")
                        {
                            MessageBox.Show("Ha ocurrido un error al Insertar el analisis por favor Ingrese Nuevamente");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ha Ocurrido Un error Por favor Intente de nuevo");
                    }
                }
                IdAnalisisBox.Clear();
                if (agregado == true) {
                    iconButton2.Enabled = true;
                }
                Analisisagregado = true;
                double total = 0;
                ActualizarDatagrid();
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    total += Convert.ToDouble(row.Cells[2].Value);
                }
                SubtotalNum.Text = total.ToString("#,##0.00");
                Double Descuento = 0;
                DescuentoText.Text = comboBox2.Text + " %";
                Double PFinal = total;
                ds1 = Conexion.SELECTTasaDia();
                Double Dolar = PFinal / Convert.ToDouble(ds1.Tables[0].Rows[0]["Dolar"].ToString().Replace(".",","));
                label9.Text = Dolar.ToString("C", new CultureInfo("en-US"));
                Pfinal.Text = PFinal.ToString("N", new CultureInfo("es-Ve"));
                IntPrecioF = PFinal;
                if (ds1.Tables[0].Rows[0]["Pesos"].ToString() != "")
                {
                   
                    label26.Visible = true;
                    label28.Visible = true;
                    double Pesos;
                    Pesos = Convert.ToDouble(ds1.Tables[0].Rows[0]["Pesos"].ToString()) * Dolar;
                    label26.Text = Pesos.ToString("N", new CultureInfo("es-CO"));
                }
            }
            else
            {
                MessageBox.Show("Por Favor Seleccione un Analisis");
            }
            this.AcceptButton = iconButton6;
            if (IDAnalisisBox)
            {
                this.IdAnalisisBox.Focus();
            }
            if (TextAnalisisBox)
            {
                NombreAnalisisBox.Clear();
                this.NombreAnalisisBox.Focus();
                this.AcceptButton = iconButton7;
            }
            ds.Tables.Clear();
            dataGridView1.DataSource = ds;
        }
        private void ActualizarDatagrid()
        {

            DataSet ds3 = new DataSet();
            ds3 = Conexion.SELECTAnalisisAFacturar(SesionLocal.ToString());
            dataGridView2.DataSource = ds3.Tables[0];
            DataGridViewColumn column2 = dataGridView2.Columns[0];
            column2.Width = 50;
            DataGridViewColumn column = dataGridView2.Columns[1];
            column.Width = 403;

        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Delete))
            {
                DataSet ds1 = new DataSet();
                if (dataGridView2.Rows.Count == 0)
                {
                    iconButton2.Enabled = false;
                    PFinal = 0;
                    Pfinal.Text = PFinal.ToString("#,##0.00");
                    label9.Text = "0";
                }
                else
                {
                    if (Conexion.TestConnection() == "Conectado")
                    {
                        Conexion.BorrarTemporal(SesionLocal.ToString(), dataGridView2.Rows[dataGridView2.CurrentCell.RowIndex].Cells["ID"].Value.ToString());
                    dataGridView2.Rows.RemoveAt(dataGridView2.CurrentRow.Index);
                    }
                    if (dataGridView2.Rows.Count == 0)
                    {
                        iconButton2.Enabled = false;
                        double total = 0;
                        SubtotalNum.Text = total.ToString("#,##0.00");
                        PFinal = 0;
                        Pfinal.Text = PFinal.ToString("#,##0.00");
                        label9.Text = "0";
                    }
                    else
                    {
                        double total = 0;
                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            total += Convert.ToDouble(row.Cells[2].Value);
                        }
                        SubtotalNum.Text = total.ToString("#,##0.00");
                        Double Descuento = 0;
                        if ((Convert.ToDouble(comboBox2.SelectedItem)) != 0)
                        {
                            Descuento = total * (Convert.ToDouble(comboBox2.SelectedItem) / 100);
                        }
                        DescuentoText.Text  = comboBox2.Text;
                        PFinal = total;
                        ds1 = Conexion.SELECTTasaDia();
                        Double Dolar = PFinal / Convert.ToDouble(ds1.Tables[0].Rows[0]["Dolar"].ToString());
                        label9.Text = Dolar.ToString("#,##0.00$");
                        if (ds1.Tables[0].Rows[0]["Pesos"].ToString() != "")
                        {
                            double Pesos;
                            label26.Visible = true;
                            label28.Visible = true;
                            Pesos = Convert.ToDouble(ds1.Tables[0].Rows[0]["Pesos"].ToString()) * Dolar;
                            label26.Text = Pesos.ToString("#,##0.00");
                        }

                        Pfinal.Text = PFinal.ToString("#,##0.00");
                    }
                    Pfinal.Text = PFinal.ToString("#,##0.00");
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void agregaranalisis()
        {
            if (IdAnalisisBox.Text == "" && NombreAnalisisBox.Text == "")
            {
                MessageBox.Show("Ingrese un ID o un Nombre de Analisis");
            }
            else if (IdAnalisisBox.Text != "" && NombreAnalisisBox.Text == "")
            {
                dataGridView1.Rows.Clear();
                DataSet ds = new DataSet();
                string cmd = IdAnalisisBox.Text;
                ds = Conexion.SELECTAnalisis(cmd);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("Analisis No Encontrado");
                    IdAnalisisBox.Clear();
                }
                else
                {
                    dataGridView1.DataSource = ds.Tables[0];
                    DataGridViewColumn column2 = dataGridView1.Columns[0];
                    column2.Width = 50;
                    DataGridViewColumn column = dataGridView1.Columns[1];
                    column.Width = 403;
                    IdAnalisisBox.Clear();
                }
            }
            else if (IdAnalisisBox.Text == "" && NombreAnalisisBox.Text != "")
            {

            }
        }

        private void iconButton8_Click(object sender, EventArgs e)
        {

            DataSet ds1 = new DataSet();
            if (dataGridView2.Rows.Count == 0)
            {
                iconButton2.Enabled = false;
                PFinal = 0;
                Pfinal.Text = PFinal.ToString("#,##0.00");
                label9.Text = "0";
            }
            else
            {
                    Conexion.BorrarAFacturar(SesionLocal.ToString(), dataGridView2.Rows[dataGridView2.CurrentCell.RowIndex].Cells["IdPerfil"].Value.ToString());
                    ActualizarDatagrid();
                if (dataGridView2.Rows.Count == 0)
                {
                    iconButton2.Enabled = false;
                    double total = 0;
                    SubtotalNum.Text = total.ToString("#,##0.00");
                    PFinal = 0;
                    Pfinal.Text = PFinal.ToString("#,##0.00");
                    label9.Text = "0";
                    label26.Text = "0";
                }
                else
                {
                    ActualizarDatagrid();
                    double total = 0;
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        total += Convert.ToDouble(row.Cells[2].Value.ToString().Replace(".",","));
                    }
                    SubtotalNum.Text = total.ToString();
                    Double Descuento = 0;
                    if ((Convert.ToDouble(comboBox2.SelectedItem)) != 0)
                    {
                        Descuento = total * (Convert.ToDouble(comboBox2.SelectedItem) / 100);
                    }
                    PFinal = total;
                    ds1 = Conexion.SELECTTasaDia();
                    Double Dolar = PFinal / Convert.ToDouble(ds1.Tables[0].Rows[0]["Dolar"].ToString().Replace(".", ","));
                    label9.Text = Dolar.ToString("#,##0.00$");
                    Pfinal.Text = PFinal.ToString("#,##0.00");
                    double Pesos;
                        label26.Visible = true;
                        label28.Visible = true;
                        Pesos = Convert.ToDouble(ds1.Tables[0].Rows[0]["Pesos"].ToString()) * Dolar;
                        label26.Text = Pesos.ToString("#,##0.00");
            
                }
               
                Pfinal.Text = PFinal.ToString("#,##0.00");
                IntPrecioF = PFinal;
            }
        
        }

        private void IdAnalisisBox_MouseClick(object sender, MouseEventArgs e)
        {

            this.AcceptButton = iconButton6;
        }

        private void IdAnalisisBox_Click_1(object sender, EventArgs e)
        {
            this.AcceptButton = iconButton6;
        }

        private void NConvenioBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            int index = NConvenioBox.SelectedIndex;
           
            if (NConvenioBox.Text == "Descuento gerencia")
            {
                comboBox2.Enabled = true;
                comboBox1.SelectedIndex = index;
            }
            else 
            {
                comboBox2.Enabled = false;
                comboBox1.SelectedIndex = index;
                comboBox2.SelectedIndex = index;
            }
        }

        private void CedulaBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b')
            {
                e.Handled = false;
            }
            else if (char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }

            else if (e.KeyChar >= 48 && e.KeyChar <= 57/*Admite los numeros del 0 al 9*/)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Nombresbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == '\b')
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)Keys.Space)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void ApellidoBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == '\b')
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)Keys.Space)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void SexoBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == '\b')
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
            SexoBox.Text = SexoBox.Text.ToUpper();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = comboBox1.SelectedIndex;
            NConvenioBox.SelectedIndex = index;
            comboBox2.SelectedIndex = index;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void CedulaBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void CelularBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b')
            {
                e.Handled = false;
            }
            else if (e.KeyChar >= 48 && e.KeyChar <= 57/*Admite los numeros del 0 al 9*/)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void CodigoTelefonoBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b')
            {
                e.Handled = false;
            }
            else if (e.KeyChar >= 48 && e.KeyChar <= 57/*Admite los numeros del 0 al 9*/)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void TelefonoCasaBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b')
            {
                e.Handled = false;
            }
            else if (e.KeyChar >= 48 && e.KeyChar <= 57/*Admite los numeros del 0 al 9*/)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void iconButton9_Click_1(object sender, EventArgs e)
        {
            if (CedulaBox.Text != "" && Nombresbox.Text != "" && ApellidoBox.Text != "")
            {
                string cmd;
                DataSet ds = new DataSet();
                cmd = CedulaBox.Text;
                ds = Conexion.SELECTPersona(cmd);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    string cmd2;
                    cmd2 = string.Format("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'", CedulaBox.Text, Nombresbox.Text, ApellidoBox.Text, CelularBox.Text, TelefonoCasaBox.Text, SexoBox.Text.ToUpper(), CorreoBox.Text, FechaPicker.Text, TipoCorreoBox.Text, CodigoCelBox.Text);

                    int MS = Conexion.Insertar(cmd2);

                    if (MS == 0)
                    {
                        MessageBox.Show("Ha Ocurrido un Error Agregando al Paciente");
                    }
                    else
                    {
                        MessageBox.Show("Agregado Satisfactoriamente");
                        IdPaciente = MS;
                        iconButton4.Enabled = true;
                        iconButton4.Visible = true;
                        iconButton3.Visible = false;
                        this.AcceptButton = iconButton4;
                        iconButton9.Visible = false;
                        string cmd3 = string.Format("id={0}", IdPaciente);
                        Conexion.ActualizarIDTemporal(cmd3, SesionLocal.ToString());
                    }

                }
            }
            else
            {
                MessageBox.Show("Los Campos Cedula,Nombres y/o Apellidos estan vacios");
            }
        
        }

        private void NombreAnalisisBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void iconButton6_Click_2(object sender, EventArgs e)
        {
            if (IdAnalisisBox.Text != "") {
                string cmd = IdAnalisisBox.Text;
                DataSet Ds = new DataSet();
                Ds = Conexion.SELECTAnalisis(cmd);
                dataGridView1.DataSource = Ds.Tables[0];
                DataGridViewColumn column2 = dataGridView1.Columns[0];
                column2.Width = 50;
                DataGridViewColumn column = dataGridView1.Columns[1];
                column.Width = 403;
            }
            else
            {
                MessageBox.Show("Por favor Ingrese el ID del Analisis a Buscar");
            }
            AcceptButton = iconButton7;
        }

        private void NombreAnalisisBox_Click_1(object sender, EventArgs e)
        {
            this.AcceptButton = iconButton7;
        }

        private void Nombresbox_TextChanged(object sender, EventArgs e)
        {
            Nombresbox.Text.ToUpper();
            if (Activador)
            {
                Cambio = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void CedulaBox_Click(object sender, EventArgs e)
        {
            AcceptButton = iconButton3;
        }

        private void dataGridView2_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void dataGridView2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DataSet ds1 = new DataSet();
                if (dataGridView2.Rows.Count == 0)
                {
                    iconButton2.Enabled = false;
                    PFinal = 0;
                    Pfinal.Text = PFinal.ToString("#,##0.00");
                    label9.Text = "0";
                }
                else
                {
                    dataGridView2.Rows.RemoveAt(dataGridView2.CurrentRow.Index);

                    if (dataGridView2.Rows.Count == 0)
                    {
                        iconButton2.Enabled = false;
                        double total = 0;
                        SubtotalNum.Text = total.ToString("#,##0.00");
                        PFinal = 0;
                        Pfinal.Text = PFinal.ToString("#,##0.00");
                        label9.Text = "0";
                    }
                    else
                    {
                        double total = 0;
                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            total += Convert.ToDouble(row.Cells[2].Value);
                        }
                        SubtotalNum.Text = total.ToString("#,##0.00");
                        Double Descuento = 0;
                        if ((Convert.ToDouble(comboBox2.SelectedItem)) != 0)
                        {
                            Descuento = total * (Convert.ToDouble(comboBox2.SelectedItem) / 100);
                        }
                       DescuentoText.Text = comboBox2.Text + " %";;
                        PFinal = total - Descuento;
                        ds1 = Conexion.SELECTTasaDia();
                        Double Dolar = PFinal / Convert.ToDouble(ds1.Tables[0].Rows[0]["Dolar"].ToString());
                        label9.Text = Dolar.ToString("#,##0.00$");
                        Pfinal.Text = PFinal.ToString("#,##0.00");
                    }
                    Pfinal.Text = PFinal.ToString("#,##0.00");
                }
            }
        }


        private void Form3_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void IdAnalisisBox_Enter(object sender, EventArgs e)
        {
            IDAnalisisBox = true;
            TextAnalisisBox = false;
        }

        private void NombreAnalisisBox_Enter(object sender, EventArgs e)
        {
            TextAnalisisBox = true;
            AcceptButton = iconButton7;
            IDAnalisisBox = false;
        }

        private void iconButton10_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count != 0)
                {
                    Sesion = SesionLocal;
                    Form form34 = new Form34();
                    form34.ShowDialog();
                }
            else 
                {
                MessageBox.Show("No hay Analisis Para Mostrar");
                }
        }

        private void SexoBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Activador)
            {
                Cambio = true;
            }
        }

        private void FechaPicker_ValueChanged(object sender, EventArgs e)
        {
            if (Activador)
            {
                Cambio = true;
            }
        }

        private void IdAnalisisBox_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
    