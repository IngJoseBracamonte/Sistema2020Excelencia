using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Conexiones.DbConnect;
using Conexiones.Impresion;
using System.Security.Cryptography;

namespace Laboratorio
{
    public partial class Form2 : Form
    {
        DataSet dsPrint = new DataSet();
        int CobroID,OrdenID, AnalisisID,filaSeleccionada = 0,IdUser;
        DateTime date;
        System.Windows.Forms.Timer ti;
        System.Windows.Forms.Timer ti2;
        string impresion;
        DateTime FechaDesde;
        DateTime FechaHasta;
        string NotificacionListaDePreciostr;
        HttpClient client = new HttpClient();
        ServiceController sc = new ServiceController("Sistema2020", ConfigurationManager.ConnectionStrings["ESPECIALES"].ConnectionString);

        string TipoServer;
        string ruta = ConfigurationManager.ConnectionStrings["Probando"].ConnectionString;
        public Form2(int idUser)
        {
            IdUser = idUser;
            ti = new System.Windows.Forms.Timer();
            ti.Interval = 60000;
            ti.Tick += new EventHandler(ActualizarSecuencia);
            InitializeComponent();
            ti.Enabled = true;

        }
        private void CrearEvento(string cmd)
        {
            Conexion.CrearEvento(cmd);
        }

        private void VerificarServicio()
        {
        }
        private void ActualizarSecuencia(object sender,EventArgs e)
        {
            NotificacionListaDePrecio();
            DataSet JobSec = new DataSet();
            JobSec = Conexion.SecuenciaDeTrabajo();
            try
            {
                if (JobSec.Tables.Count != 0)
                {

                    if (dataGridView1.Rows.Count != JobSec.Tables[0].Rows.Count)
                    {
                        dataGridView1.DataSource = JobSec.Tables[0];
                    }
                }
                DataGridViewColumn column = dataGridView1.Columns[0];
                column.Visible = false;
                DataGridViewColumn column1 = dataGridView1.Columns[1];
                column1.Width = 25;
                DataGridViewColumn column2 = dataGridView1.Columns[2];
                column2.Width = 80;
                DataGridViewColumn column3 = dataGridView1.Columns[3];
                column3.Width = 70;
                DataGridViewColumn column4 = dataGridView1.Columns[4];
                column4.Width = 80;
                DataGridViewColumn column5 = dataGridView1.Columns[5];
                column5.Width = 120;
                DataGridViewColumn column6 = dataGridView1.Columns[6];
                column6.Width = 140;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
            }
        }
        public void Seleccionar(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView3.Rows.Clear();
            DataSet ds = new DataSet();
            if(e.RowIndex < 0)
            {
                return;
            }
            if (dataGridView1.Rows.Count != 0)
            {
                filaSeleccionada = e.RowIndex;
                ds = Conexion.SELECTAnalisisPrueba(dataGridView1.Rows[e.RowIndex].Cells["IdOrden"].Value.ToString());
                label9.Text = dataGridView1.Rows[e.RowIndex].Cells["IdOrden"].Value.ToString();
                label19.Text = dataGridView1.Rows[e.RowIndex].Cells["NumeroDia"].Value.ToString();
                DataSet ds1 = new DataSet();
                ds1 = Conexion.SELECTPersonaOrden(dataGridView1.Rows[e.RowIndex].Cells["IdOrden"].Value.ToString());
                label12.Text = ds1.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds1.Tables[0].Rows[0]["Apellidos"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds1.Tables[0].Rows[0]["fecha"].ToString());
                DateTime Hoy = DateTime.Now;
                string edad = Conexion.Fecha(nacimiento);
                label10.Text = edad;
                label11.Text = ds1.Tables[0].Rows[0]["sexo"].ToString();
                DataSet ds4 = Conexion.SelectCantidadCorreo(label9.Text);
                string enviados;
                if (ds4.Tables.Count != 0)
                {
                    if (ds4.Tables[0].Rows.Count != 0)
                    {
                        enviados = ds4.Tables[0].Rows[0]["Enviado"].ToString();
                    }
                    else
                    {
                        enviados = "0";
                    }
                }
                else
                {
                    enviados = "0";
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dataGridView3.Rows.Add(dr["IdOrden"].ToString(), dr["IdAnalisis"].ToString(), dr["NombreAnalisis"].ToString(), dr["ValorResultado"].ToString(), dr["Unidad"].ToString(), dr["TipoAnalisis"].ToString(), dr["IdOrganizador"].ToString(), dr["EstadoDeResultado"].ToString(), enviados);
                }
            }

        }
        private void Form2_Load(object sender, EventArgs e)
        {
              DataSet ds = new DataSet();
                string cmd = IdUser.ToString();
                DataSet Cargo = new DataSet();
                Cargo = Conexion.Privilegios(IdUser);
                ds = Conexion.DatosUsuario(cmd);
                if (ds.Tables.Count != 0)
                {
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        label7.Text = "Bienvenido " + ds.Tables[0].Rows[0][0].ToString() + " esta en la sede: " + ds.Tables[0].Rows[0][1].ToString();
                    }
                }

                DataSet JobSec = new DataSet();
                JobSec = Conexion.SecuenciaDeTrabajo();
                try
                {
                    if (JobSec.Tables.Count != 0)
                    {
                        dataGridView1.DataSource = JobSec.Tables[0];
                    }
                    DataGridViewColumn column = dataGridView1.Columns[0];
                    column.Visible = false;
                    DataGridViewColumn column1 = dataGridView1.Columns[1];
                    column1.Width = 25;
                    DataGridViewColumn column2 = dataGridView1.Columns[2];
                    column2.Width = 80;
                    DataGridViewColumn column3 = dataGridView1.Columns[3];
                    column3.Width = 67;
                    DataGridViewColumn column4 = dataGridView1.Columns[4];
                    column4.Width = 80;
                    DataGridViewColumn column5 = dataGridView1.Columns[5];
                    column5.Width = 130;
                    DataGridViewColumn column6 = dataGridView1.Columns[6];
                    column6.Width = 140;
                }
                catch (Exception ex)
                {
                    CrearEvento(ex.ToString());
                }
                NotificacionListaDePrecio();
                Conexion.FlushHost();

           
        }

        private void NotificacionListaDePrecio()
        {
            DataSet AvisoListaDePrecio = new DataSet();
            AvisoListaDePrecio = Conexion.CambiarListaPrecio();
            if (AvisoListaDePrecio.Tables.Count != 0)
            {
                if (AvisoListaDePrecio.Tables[0].Rows.Count != 0)
                {
                    NotificacionListaDePreciostr = AvisoListaDePrecio.Tables[0].Rows[0][1].ToString();
                    if (AvisoListaDePrecio.Tables[0].Rows[0][0].ToString() == "0")
                    {
                        label21.Visible = true;
                    }
                    else
                    {
                        label21.Visible = false;
                    }
                }
            }
        }
        private async Task NotificacionListaDePrecioASync()
        {

                DataSet AvisoListaDePrecio = new DataSet();
                AvisoListaDePrecio = await Conexion.CambiarListaPrecioAsync();
                NotificacionListaDePreciostr = AvisoListaDePrecio.Tables[0].Rows[0][1].ToString();
                if (AvisoListaDePrecio.Tables[0].Rows[0][0].ToString() == "0")
                {
                    label21.Visible = true;
                }
                else
                {
                    label21.Visible = false;
                }
  
           
        }
        private void iconButton1_Click(object sender, EventArgs e)
        {
            Form Pacientes = new Form3(IdUser);
            Pacientes.Show();

        
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            this.Close();

        }


        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > (-1))
            {
                filaSeleccionada = dataGridView1.CurrentCell.RowIndex;
                Seleccionar(sender, e);
                dataGridView3.ClearSelection();
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > (-1))
            {
                filaSeleccionada = dataGridView1.CurrentCell.RowIndex;
                Seleccionar(sender, e);
                dataGridView3.ClearSelection();
            }
        
        }
        private void ActualizarDG3()
        {
            dataGridView3.Rows.Clear();
            DataSet ds = new DataSet();
            if (dataGridView1.Rows.Count != 0)
            {
                ds = Conexion.SELECTAnalisisPrueba(dataGridView1.Rows[filaSeleccionada].Cells["IdOrden"].Value.ToString());
                label9.Text = dataGridView1.Rows[filaSeleccionada].Cells["IdOrden"].Value.ToString();

                DataSet ds1 = new DataSet();
                ds1 = Conexion.SELECTPersonaOrden(dataGridView1.Rows[filaSeleccionada].Cells["IdOrden"].Value.ToString());
                label12.Text = ds1.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds1.Tables[0].Rows[0]["Apellidos"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds1.Tables[0].Rows[0]["fecha"].ToString());
                DateTime Hoy = DateTime.Now;
                string edad = Conexion.Fecha(nacimiento);
                label10.Text = edad;
                label11.Text = ds1.Tables[0].Rows[0]["sexo"].ToString();
                DataSet ds4 = Conexion.SelectCantidadCorreo(label9.Text);
                string enviados;
                if (ds4.Tables.Count != 0)
                {
                    if (ds4.Tables[0].Rows.Count != 0)
                    {
                        enviados = ds4.Tables[0].Rows[0]["Enviado"].ToString();
                    }
                    else
                    {
                        enviados = "0";
                    }
                }
                else
                {
                    enviados = "0";
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dataGridView3.Rows.Add(dr["IdOrden"].ToString(), dr["IdAnalisis"].ToString(), dr["NombreAnalisis"].ToString(), dr["ValorResultado"].ToString(), dr["Unidad"].ToString(), dr["TipoAnalisis"].ToString(), dr["IdOrganizador"].ToString(), dr["EstadoDeResultado"].ToString(), enviados);
                }
            }

        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form form1 = new Login();
            form1.Visible = true;
            ti.Enabled = false;
        }

        private void imprimirSeccionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells["Orden"].Value.ToString(), out int OrdenID))
            {
                return;
            }
            if (!int.TryParse(dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells["IdAnalisis"].Value.ToString(), out int AnalisisID))
            {
                return;
            }
            impresion = "Seccion";
            Form Impresion1 = new Impresion(impresion,OrdenID, AnalisisID,IdUser);
            DataSet Empresa = new DataSet();
            Empresa = Conexion.SelectEmpresaActiva();
            DataSet ds = new DataSet();
            ds = Conexion.Bioanalista(OrdenID, AnalisisID);
            if (ds.Tables.Count != 0) 
            {
                if (ds.Tables[0].Rows.Count != 0)
                {

                    if ((ds.Tables[0].Rows[0]["IdUsuario"].ToString() != "") || Empresa.Tables[0].Rows[0]["IdEmpresa"].ToString() == "5")
                    {
                        Cruzando(OrdenID, AnalisisID);
                    }
                    else
                    {
                        MessageBox.Show("Analisis No Reportado");
                    }
                }
                else
                {
                    MessageBox.Show("Analisis No Reportado");
                }
            }

        }

        private void iconMenuItem1_Click(object sender, EventArgs e)
        {
            
        impresion = "Total";
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            DataSet Empresa = new DataSet();
            Empresa = Conexion.SelectEmpresaActiva();
            if (Permisos.Tables[0].Rows[0]["ImprimirResultado"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                if (!int.TryParse(dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells["Orden"].Value.ToString(), out int OrdenID))
                {
                    return;
                }
                if (!int.TryParse(dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells["IdAnalisis"].Value.ToString(), out int AnalisisID))
                {
                    return;
                }
                string cmd = dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells["Orden"].Value.ToString();
                int Contador = Conexion.VerificarCorreo(cmd);
                if (Contador != 0 || Empresa.Tables[0].Rows[0]["IdEmpresa"].ToString() == "5")
                {
                    Cruzando(OrdenID, AnalisisID);
                }
                else
                {
                    MessageBox.Show("No Hay Resultados Validados Para Mostrar.");
                }
               ActualizarDG3();
            }
            else
            {
                MessageBox.Show("No tienes privilegios para realizar esta accion");
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (dataGridView3.Rows.Count == 0)
            {
                AnalisisMenu.Enabled = false;
            }
            else
            {
                AnalisisMenu.Enabled = true;
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                ti.Enabled = false;
            }
            else
            {
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox4.Enabled = true;
                checkBox1.Checked = true;
                checkBox1.Checked = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                dateTimePicker1.Text = DateTime.Now.ToString("dd/MM/yyyy");
                dateTimePicker2.Text = DateTime.Now.ToString("dd/MM/yyyy");
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                checkBox2.Checked = false;
                ti.Enabled = true;
                checkBox1.Checked = true;
                ActualizarSecuencia(sender, e);
                borrarDatos();

            }
        }

        private void iconButton4_MouseClick(object sender, MouseEventArgs e)
        {
            ServiciosMenu.Show(Cursor.Position.X, Cursor.Position.Y);
        }

        private void iconMenuItem2_Click(object sender, EventArgs e)
        {
           
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
                DataSet Permisos = new DataSet();
                Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
                if (Permisos.Tables[0].Rows[0]["CambioDePrecios"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
                {
                    Form form20 = new Form20();
                    form20.ShowDialog();
                    NotificacionListaDePrecio();
                }
                else
                {
                    MessageBox.Show("No tiene privilegios para realizar esta accion");
                }

        
        }

        private void referidosToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        private void ordenesToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        private void Almacenados()
        {
            //Conectarse a Usuarios
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["AgregarConvenio"].ToString() == "1")
            {
                Form form29 = new Form29(IdUser);
                form29.ShowDialog();
            }
            else
            {
                MessageBox.Show("No tiene permisos para realizar esta accion");
            }
        }


        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
                string cmd, cmd2;
                if (dateTimePicker2.Value < dateTimePicker1.Value)
                {
                    MessageBox.Show("Debe escoger una fecha menor");
                    dateTimePicker1.Value = dateTimePicker2.Value;

                }
                else
                {
                    cmd = dateTimePicker1.Value.ToString("yyyy/MM/dd");
                    cmd2 = dateTimePicker2.Value.ToString("yyyy/MM/dd");
                    checkBox1.Checked = false;
                    DataSet ds1 = new DataSet();
                    ds1 = Conexion.SecuenciaDeTrabajoPorFecha(cmd, cmd2);
                    try
                    {
                        dataGridView1.DataSource = ds1.Tables[0];
                        DataGridViewColumn column = dataGridView1.Columns[0];
                        column.Visible = false;
                        DataGridViewColumn column1 = dataGridView1.Columns[1];
                        column1.Width = 25;
                        DataGridViewColumn column2 = dataGridView1.Columns[2];
                        column2.Width = 80;
                        DataGridViewColumn column3 = dataGridView1.Columns[3];
                        column3.Width = 70;
                        DataGridViewColumn column4 = dataGridView1.Columns[4];
                        column4.Width = 70;
                        DataGridViewColumn column5 = dataGridView1.Columns[5];
                        column5.Width = 120;
                        DataGridViewColumn column6 = dataGridView1.Columns[6];
                        column6.Width = 140;
                    }
                    catch (Exception ex)
                    {
                        CrearEvento(ex.ToString());
                    }
                }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            string cmd, cmd2;
            if (dateTimePicker2.Value < dateTimePicker1.Value)
            {
                MessageBox.Show("Debe escoger una fecha Mayor");
                dateTimePicker2.Value = dateTimePicker1.Value;

            }
            else
            {
                cmd = dateTimePicker1.Value.ToString("yyyy/MM/dd");
                cmd2 = dateTimePicker2.Value.ToString("yyyy/MM/dd");
                checkBox1.Checked = false;
                DataSet ds1 = new DataSet();
                ds1 = Conexion.SecuenciaDeTrabajoPorFecha(cmd, cmd2);
                try
                {
                    dataGridView1.DataSource = ds1.Tables[0];
                    DataGridViewColumn column = dataGridView1.Columns[0];
                    column.Visible = false;
                    DataGridViewColumn column1 = dataGridView1.Columns[1];
                    column1.Width = 25;
                    DataGridViewColumn column2 = dataGridView1.Columns[2];
                    column2.Width = 80;
                    DataGridViewColumn column3 = dataGridView1.Columns[3];
                    column3.Width = 70;
                    DataGridViewColumn column4 = dataGridView1.Columns[4];
                    column4.Width = 70;
                    DataGridViewColumn column5 = dataGridView1.Columns[5];
                    column5.Width = 120;
                    DataGridViewColumn column6 = dataGridView1.Columns[6];
                    column6.Width = 140;
                }
                catch (Exception ex)
                {
                    CrearEvento(ex.ToString());
                }
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {

            if ((int)e.KeyChar == (int)Keys.Escape)
            {
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox4.Enabled = true;
                checkBox1.Checked = true;
                checkBox1.Checked = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                dateTimePicker1.Text = DateTime.Now.ToString("yyyy/MM/dd");
                dateTimePicker2.Text = DateTime.Now.ToString("yyyy/MM/dd");
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
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

        private void textBox3_MouseClick(object sender, MouseEventArgs e)
        {
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox4.Enabled = false;
            checkBox1.Checked = false;
            dateTimePicker1.Text = DateTime.Now.ToString("yyyy/MM/dd");
            dateTimePicker2.Text = DateTime.Now.ToString("yyyy/MM/dd");
            textBox1.Clear();
            textBox2.Clear();
            textBox4.Clear();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Escape)
            {
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                checkBox1.Checked = true;
                checkBox1.Checked = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                dateTimePicker1.Text = DateTime.Now.ToString("yyyy/MM/dd");
                dateTimePicker2.Text = DateTime.Now.ToString("yyyy/MM/dd");
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
            }
            else if (e.KeyChar == (char)Keys.Back)
            {
                e.Handled = false;
            }
            else if (Char.IsLetter(e.KeyChar))
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

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            checkBox1.Checked = false;
            textBox3.Clear();
            textBox4.Clear();
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b')
            {
                e.Handled = false;
            }
            else if ((int)e.KeyChar == (int)Keys.Escape)
            {
                textBox3.Enabled = true;
                textBox2.Enabled = true;
                textBox1.Enabled = true;
                checkBox1.Checked = true;
                checkBox1.Checked = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                dateTimePicker1.Text = DateTime.Now.ToString("yyyy/MM/dd");
                dateTimePicker2.Text = DateTime.Now.ToString("yyyy/MM/dd");
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
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

        private void textBox4_MouseClick(object sender, MouseEventArgs e)
        {
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            checkBox1.Checked = false;
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
        }

        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            textBox4.Enabled = false;
            textBox3.Enabled = false;
            checkBox1.Checked = false;
            textBox4.Clear();
            textBox3.Clear();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Escape)
            {
                checkBox1.Checked = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                dateTimePicker1.Text = DateTime.Now.ToString("yyyy/MM/dd");
                dateTimePicker2.Text = DateTime.Now.ToString("yyyy/MM/dd");
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
            }
            else if (e.KeyChar == (char)Keys.Back)
            {
                e.Handled = false;
            }
            else if (Char.IsLetter(e.KeyChar))
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

        private void textBox3_KeyUp(object sender, KeyEventArgs e)
        {
                DataSet ds1 = new DataSet();
                string cmd;
                cmd = textBox3.Text;
                if (textBox3.Text != "")
                {
                    ds1 = Conexion.SecuenciaDeTrabajoPorID(cmd);
                    if (ds1.Tables.Count != 0)
                    {
                        dataGridView1.DataSource = ds1.Tables[0];
                        DataGridViewColumn column = dataGridView1.Columns[0];
                        column.Visible = false;
                        DataGridViewColumn column1 = dataGridView1.Columns[1];
                        column1.Width = 25;
                        DataGridViewColumn column2 = dataGridView1.Columns[2];
                        column2.Width = 80;
                        DataGridViewColumn column3 = dataGridView1.Columns[3];
                        column3.Width = 70;
                        DataGridViewColumn column4 = dataGridView1.Columns[4];
                        column4.Width = 70;
                        DataGridViewColumn column5 = dataGridView1.Columns[5];
                        column5.Width = 120;
                        DataGridViewColumn column6 = dataGridView1.Columns[6];
                        column6.Width = 140;
                    }
                }            
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
                DataSet ds1 = new DataSet();
                string cmd, cmd2;
                cmd = textBox1.Text;
                cmd2 = textBox2.Text;
                if (textBox1.Text != "")
                {
                    ds1 = Conexion.SecuenciaDeTrabajoNombreApellido(cmd, cmd2);
                    if (ds1.Tables.Count != 0)
                    {
                        dataGridView1.DataSource = ds1.Tables[0];
                        DataGridViewColumn column = dataGridView1.Columns[0];
                        column.Visible = false;
                        DataGridViewColumn column1 = dataGridView1.Columns[1];
                        column1.Width = 25;
                        DataGridViewColumn column2 = dataGridView1.Columns[2];
                        column2.Width = 80;
                        DataGridViewColumn column3 = dataGridView1.Columns[3];
                        column3.Width = 70;
                        DataGridViewColumn column4 = dataGridView1.Columns[4];
                        column4.Width = 70;
                        DataGridViewColumn column5 = dataGridView1.Columns[5];
                        column5.Width = 120;
                        DataGridViewColumn column6 = dataGridView1.Columns[6];
                        column6.Width = 140;
                    }
                }
            
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
                DataSet ds1 = new DataSet();
                string cmd, cmd2;
                cmd = textBox1.Text;
                cmd2 = textBox2.Text;
                if (textBox2.Text != "")
                {
                    ds1 = Conexion.SecuenciaDeTrabajoNombreApellido(cmd, cmd2);
                    if (ds1.Tables.Count != 0)
                    {
                        dataGridView1.DataSource = ds1.Tables[0];
                        DataGridViewColumn column = dataGridView1.Columns[0];
                        column.Visible = false;
                        DataGridViewColumn column1 = dataGridView1.Columns[1];
                        column1.Width = 25;
                        DataGridViewColumn column2 = dataGridView1.Columns[2];
                        column2.Width = 80;
                        DataGridViewColumn column3 = dataGridView1.Columns[3];
                        column3.Width = 70;
                        DataGridViewColumn column4 = dataGridView1.Columns[4];
                        column4.Width = 70;
                        DataGridViewColumn column5 = dataGridView1.Columns[5];
                        column5.Width = 120;
                        DataGridViewColumn column6 = dataGridView1.Columns[6];
                        column6.Width = 140;
                    }
                }
     

             
        }

        private void textBox4_KeyUp(object sender, KeyEventArgs e)
        {
                DataSet ds1 = new DataSet();
                string cmd;
                cmd = textBox4.Text;
                if (textBox4.Text != "")
                {
                    ds1 = Conexion.SecuenciaDeTrabajoPorCedula(cmd);
                    if (ds1.Tables.Count != 0)
                    {
                        dataGridView1.DataSource = ds1.Tables[0];
                        DataGridViewColumn column = dataGridView1.Columns[0];
                        column.Visible = false;
                        DataGridViewColumn column1 = dataGridView1.Columns[1];
                        column1.Width = 25;
                        DataGridViewColumn column2 = dataGridView1.Columns[2];
                        column2.Width = 80;
                        DataGridViewColumn column3 = dataGridView1.Columns[3];
                        column3.Width = 70;
                        DataGridViewColumn column4 = dataGridView1.Columns[4];
                        column4.Width = 70;
                        DataGridViewColumn column5 = dataGridView1.Columns[5];
                        column5.Width = 120;
                        DataGridViewColumn column6 = dataGridView1.Columns[6];
                        column6.Width = 140;
                    }
                }

              
        }

        private void libroDeVentaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerLibroVenta"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                Form form23 = new Form23(IdUser);
                form23.ShowDialog();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void estadisticasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void reporteBioanalistaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerReporteBioanalista"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                Form form26 = new Form26();
                form26.Show();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void dataGridView3_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (this.dataGridView3.Columns[e.ColumnIndex].Name == "Color")
            {
                if (dataGridView3.Rows[e.RowIndex].Cells["EstadoResultado"].Value.ToString() != "")
                {
                    if (Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells["EstadoResultado"].Value) == 1)
                    {
                        e.CellStyle.BackColor = System.Drawing.Color.Aqua;
                    }
                    if (Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells["EstadoResultado"].Value) == 2)
                    {
                        e.CellStyle.BackColor = System.Drawing.Color.Black;
                    }
                    if (Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells["EstadoResultado"].Value) == 3)
                    {
                        e.CellStyle.BackColor = System.Drawing.Color.DarkOrange;
                    }
                }

            }
            if (this.dataGridView3.Columns[e.ColumnIndex].Name == "EnviadoPorCorreo")
            {
                if (Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells["EstadoDeCorreo"].Value) > 0)
                {
                    e.CellStyle.BackColor = System.Drawing.Color.Green;
                }
                else
                {
                    e.CellStyle.BackColor = System.Drawing.Color.Salmon;
                }

            }

        }

        private void contextMenuStrip3_Opening(object sender, CancelEventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                OrdenesMenu.Enabled = false;
            }
            else
            {
                OrdenesMenu.Enabled = true;
                checkBox1.Checked = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBox2.Checked == true)
            {
                checkBox1.Checked = false;
                DataTable ds1 = new DataTable();
                ds1 = Conexion.SecuenciaPorValidar();
                try
                {
                    dataGridView1.DataSource = ds1;
                    if (filaSeleccionada != 0)
                    {
                        dataGridView1.Rows[filaSeleccionada].Selected = true;
                    }
                    DataGridViewColumn column = dataGridView1.Columns[0];
                    column.Visible = false;
                    DataGridViewColumn column1 = dataGridView1.Columns[1];
                    column1.Width = 25;
                    DataGridViewColumn column2 = dataGridView1.Columns[2];
                    column2.Width = 80;
                    DataGridViewColumn column3 = dataGridView1.Columns[3];
                    column3.Width = 70;
                    DataGridViewColumn column4 = dataGridView1.Columns[4];
                    column4.Width = 70;
                    DataGridViewColumn column5 = dataGridView1.Columns[5];
                    column5.Width = 120;
                    DataGridViewColumn column6 = dataGridView1.Columns[6];
                    column6.Width = 140;
                }
                catch (Exception ex)
                {
                    CrearEvento(ex.ToString());
                }
            }
        }

        private void validarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet ds2 = new DataSet();
            ds2 = Conexion.PrivilegiosCargar(IdUser.ToString());

            string mensaje = "Al momento de Guardar estos Valores se tomara los datos como validados ¿Desea Validar?";
            string titulo = "Alarma";
            if (ds2.Tables[0].Rows[0]["Validar"].ToString() == "1" || ds2.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                MessageBoxButtons button = MessageBoxButtons.YesNo;
                DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    OrdenID = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["IdOrden"].Value.ToString());
                    Conexion.EstadoOrden(OrdenID);
                    ActualizarSecuencia(sender, e);
                }
            }
        }

        private void empresasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet ds1 = new DataSet();
            ds1 = Conexion.Privilegios(IdUser);
            if (ds1.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                Form form27 = new Form27();
                form27.Show();
            }
            else
            {
                MessageBox.Show("No tienes privilegios para realizar esta accion");
            }
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }
        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {

        }

        private void envioToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }
        private void Cruzando(int idOrden,int idAnalisis)
        {
            DataSet Cruzando = new DataSet();
            Cruzando = Conexion.ordenesPorcruzar(idOrden.ToString());
            if (Cruzando.Tables.Count != 0)
            {
                if (Cruzando.Tables[0].Rows.Count != 0)
                {
                    double Convertido = Convert.ToDouble(Cruzando.Tables[0].Rows[0]["Total"].ToString());
                    if (Math.Round(Convertido,2) != 0)
                    {
                        string mensaje = string.Format("La Orden {0}, es del paciente {1} que tiene una diferencia de {2}, Deseas proceder igualmente?", idOrden, Cruzando.Tables[0].Rows[0]["Nombre"].ToString(), Convertido.ToString("N"));
                        string titulo = "Alarma";
                        MessageBoxButtons button = MessageBoxButtons.YesNo;
                        DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                        if (dialog == DialogResult.Yes)
                        {
                            Form Impresion1 = new Impresion(impresion, idOrden, idAnalisis, IdUser);
                            Impresion1.Show();
                            impresion = "";
                        }
                        else
                        {
                          
                        }
                    }
                    else
                    {
                        Form Impresion1 = new Impresion(impresion, idOrden, idAnalisis, IdUser);
                        Impresion1.Show();
                        impresion = "";
                    }
                }
                else
                {
                    MessageBox.Show("Ha ocurrido un error el mensaje no fue enviado.");
                }
            }
            ActualizarDG3();

        }
        private void pacienteToolStripMenuItem_Click(object sender, EventArgs e)
        {
 
                impresion = "Correo";
                string cmd = dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells["Orden"].Value.ToString();
                DataSet Correo = new DataSet();
                Correo = Conexion.SELECTPersonaEmail(cmd);
                if (Correo.Tables[0].Rows[0]["Correo"].ToString() != "")
                {
                    OrdenID = Convert.ToInt32(dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells["Orden"].Value.ToString());
                    AnalisisID = Convert.ToInt32(dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells["IdAnalisis"].Value.ToString());
                Task.Run(() =>
                {
                    Impresiones.SendEmail(OrdenID, AnalisisID, impresion, IdUser);
                });
                    

                }
                else
                {
                    MessageBox.Show("El paciente no tiene correo electronico");
                }
            ActualizarDG3();

        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerReferidos"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                Form form24 = new Form24();
                form24.Show();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void iconButton6_Click(object sender, EventArgs e)
        {
            Form form42 = new Form42();
            form42.Show();
        }

        private void iconButton8_Click(object sender, EventArgs e)
        {
            Form form33 = new Form33(IdUser); ;
            form33.Show();
        }



        private void dateTimePicker1_Enter(object sender, EventArgs e)
        {
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox4.Enabled = true;
            checkBox1.Checked = true;
            checkBox1.Checked = true;
            textBox3.Enabled = true;
            textBox4.Enabled = true;
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }



        private void iconButton9_Click(object sender, EventArgs e)
        {
            Form form22 = new Form22();
            form22.ShowDialog();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
        }

        private void dataGridView3_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

               int OrdenID = Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells["Orden"].Value.ToString());
                int AnalisisID = Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells["IdAnalisis"].Value.ToString());
                int prueba = Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells["TipoAnalisis"].Value);
                if (dataGridView3.Rows[e.RowIndex].Cells["TipoAnalisis"].Value.ToString() != "" || dataGridView3.Rows[e.RowIndex].Cells["TipoAnalisis"].Value.ToString() != " ")
                {
                    switch (Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells["TipoAnalisis"].Value))
                    {
                        case 3:
                            Form frotis = new Frotis(IdUser,OrdenID,AnalisisID);
                            frotis.ShowDialog();
                            break;
                        case 4:
                            Form form8 = new Form8(IdUser, OrdenID, AnalisisID);
                            form8.ShowDialog();
                            break;
                        case 5:
                            Form form9 = new Form9(IdUser, OrdenID, AnalisisID);
                            form9.ShowDialog();
                            break;
                        case 6:
                            Form form4 = new HematologiaForm(IdUser, OrdenID, AnalisisID);
                            form4.ShowDialog();
                            break;
                        case 7:
                            Form OrinaForm = new OrinaForm(IdUser, OrdenID, AnalisisID);
                            OrinaForm.ShowDialog();
                            break;
                        case 8:
                            Form HecesForm = new HecesForm(IdUser, OrdenID, AnalisisID);
                            HecesForm.ShowDialog();
                            break;
                        case 12:
                            Form form14 = new Form14(IdUser, OrdenID, AnalisisID);
                            form14.ShowDialog();
                            break;
                        case 14:
                            Form form13 = new Form13(IdUser, OrdenID, AnalisisID);
                            form13.ShowDialog();
                            break;
                        case 16:
                            Form form15 = new Form15(IdUser, OrdenID, AnalisisID);
                            form15.ShowDialog();
                            break;
                        case 17:
                            Form form16 = new Form16(IdUser, OrdenID, AnalisisID);
                            form16.ShowDialog();
                            break;
                        case 18:
                            Form form17 = new Form17(IdUser, OrdenID, AnalisisID);
                            form17.ShowDialog();
                            break;
                        case 19:
                            Form form19 = new Form19(IdUser, OrdenID, AnalisisID);
                            form19.ShowDialog();
                            break;
                        case 20:
                            Form form28 = new Form28(IdUser, OrdenID, AnalisisID);
                            form28.ShowDialog();
                            break;
                        case 21:
                            Form form6 = new Form6(IdUser, OrdenID, AnalisisID);
                            form6.ShowDialog();
                            break;

                    }
                    ActualizarDG3();
                }

  
        }


        private void iconButton11_Click(object sender, EventArgs e)
        {
            bool activo = true;
            if (activo)
            {
                Conexion.ListaDePrecioImpresa(NotificacionListaDePreciostr);
                NotificacionListaDePrecio();
                FechaDesde = dateTimePicker1.Value;
                FechaHasta = dateTimePicker2.Value;
                Form ListaDePrecios = new ListaDePrecios();
                ListaDePrecios.ShowDialog();
               
            }
            else
            {
                MessageBox.Show("no configurada");
            }
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {

        }

        private void convenioToolStripMenuItem_Click(object sender, EventArgs e)
        {

                impresion = "Convenio";
                string cmd = dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells["Orden"].Value.ToString();
                DataSet ds1 = new DataSet();
                DataSet Paciente = Conexion.PacienteAImprimir(Convert.ToInt32(cmd));
                ds1 = Conexion.SELECTConvenioEmail(Paciente.Tables[0].Rows[0]["IdConvenio"].ToString());
                if (Convert.ToInt32(Paciente.Tables[0].Rows[0]["IdConvenio"].ToString()) <= 3)
                {
                    MessageBox.Show("Los Convenios: Ambulatorio,Descuento de Personal o de Gerencia no contienen Correo Electronico.");
                }
                else
                {
                    if (ds1.Tables[0].Rows[0]["CorreoSede"].ToString() != "")
                    {
                        OrdenID = Convert.ToInt32(dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells["Orden"].Value.ToString());
                        AnalisisID = Convert.ToInt32(dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells["IdAnalisis"].Value.ToString());
                        int Contador = Conexion.VerificarCorreo(cmd);
                        if (Contador != 0)
                        {
                            Task.Run(() =>
                            {
                                Impresiones.SendEmail(OrdenID, AnalisisID, impresion, IdUser);
                            });
                        }
                        else
                        {
                            MessageBox.Show("No Hay Resultados Validados Para Enviar.");
                        }
                        ActualizarDG3();

                    }
                    else
                    {
                        MessageBox.Show("No hay correo configurado para esta sede");
                    }

                }
        }

        private void todoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
     

        private void iconButton12_Click(object sender, EventArgs e)
        {

                FechaDesde = dateTimePicker1.Value;
                FechaHasta = dateTimePicker2.Value;
                Form hojadeTrabajo = new HojadeTrabajo(FechaDesde, FechaHasta);
                hojadeTrabajo.ShowDialog();
           
        }
        private void contextMenuStrip3_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
        }


        private void ValidacionesParaAnular()
        {
            
        }
        private void anularToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
            if (dataGridView1.SelectedRows.Count < 0)
            {
                MessageBox.Show("Por favor, Seleccione una Orden");
                return;
            }
            if (dataGridView1.CurrentCell == null)
            {
                MessageBox.Show("Por favor, Seleccione una Orden");
                return;
            }
            int Index = dataGridView1.CurrentCell.RowIndex;
            OrdenID = Convert.ToInt32(dataGridView1.Rows[Index].Cells["IdOrden"].Value.ToString());
            if (OrdenID == 0)
            {
                MessageBox.Show("Por favor, Seleccione una Orden");
                return;
            }

            DataSet ds2 = new DataSet();
            ds2 = Conexion.PrivilegiosCargar(IdUser.ToString());

            if (ds2.Tables[0].Rows[0]["AnularOrden"].ToString() != "1")
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
                return;
            }


            DataSet ds3 = new DataSet();
            ds3 = Conexion.SELECTPruebasReportadas(OrdenID.ToString());
            if (ds2.Tables[0].Rows[0]["Cargo"].ToString() != "1" && ds2.Tables[0].Rows[0]["Cargo"].ToString() != "2")
            {
                if (ds3.Tables[0].Rows[0]["Conteo"].ToString() != "0")
                {
                    MessageBox.Show("Esta Orden tiene Examenes Reportados, Comuniquese con Sistema");
                    return;
                }
            }
          
            string mensaje = "¿Desea Anular?";
            string titulo = "Alarma";
            MessageBoxButtons button = MessageBoxButtons.YesNo;
            DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
            if (dialog == DialogResult.Yes)
            {
                Conexion.AnularOrden(OrdenID.ToString(), IdUser.ToString());
                ActualizarSecuencia(sender, e);
                borrarDatos();
            }
           
       
        }

        private void borrarDatos()
        {
            label12.Text = "Seleccione un Paciente";
            label11.Text = "ND";
            label9.Text = "00000";
            label10.Text = "0";
            dataGridView3.Rows.Clear();
        }

        private void iconButton10_Click(object sender, EventArgs e)
        {
            ServiceController sc = new ServiceController("Sistema2020", ConfigurationManager.ConnectionStrings["ESPECIALES"].ConnectionString);
            try
            {
                sc.Stop();
                sc.Start();
            }
            catch (Exception ex)
            {
                sc.Start();
            }
            finally
            {
                VerificarServicio();
            }
           
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            Form form7 = new Form7(IdUser);
            form7.ShowDialog();
        }

        private void enviarPorCorreoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void reenviarReferidoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet ds2 = new DataSet();
            ds2 = Conexion.PrivilegiosCargar(IdUser.ToString());
            DataSet ds3 = new DataSet();
            DataSet Empresa = new DataSet();
            ds3 = Conexion.SELECTPruebasReportadas(OrdenID.ToString());
            Empresa = Conexion.SelectEmpresaActiva();
            string mensaje = "Este proceso Reenviara datos que se pueden duplicar en especiales, Esta Seguro de Proceder?";
            string titulo = "Alarma";
            if (ds2.Tables[0].Rows[0]["VerReferidos"].ToString() == "1" || ds2.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                MessageBoxButtons button = MessageBoxButtons.YesNo;
                DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    if (Empresa.Tables[0].Rows[0]["IdEmpresa"].ToString() != "5")
                    {
                        OrdenID = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["IdOrden"].Value.ToString());
                        Conexion.ReenviarReferido(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["IdOrden"].Value.ToString());
                    }
                    else
                    {
                        OrdenID = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["IdOrden"].Value.ToString());
                        Conexion.ReenviarReferidoEspeciales(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["IdOrden"].Value.ToString());
                    }
                  

                }
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void etiquetaParaImprimirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["ImprimirEtiqueta"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                string mensaje = "Al activar esta opción la etiqueta aparecera en la seccion 'Muestra', Esta seguro de realizar esta acción? ";
                string titulo = "Alarma";
                MessageBoxButtons button = MessageBoxButtons.YesNo;
                DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    OrdenID = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["IdOrden"].Value.ToString());
                    Conexion.ReimprimirEtiquetas(OrdenID.ToString());

                }
            }
        }

        private void temporalesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerEstadisticas"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                OrdenID = Convert.ToInt32(dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells["Orden"].Value.ToString());
                AnalisisID = Convert.ToInt32(dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells["IdAnalisis"].Value.ToString());
                Form form38 = new Form38(OrdenID, AnalisisID);
                form38.Show();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
   
            DataSet IdAgrupador = new DataSet();
            string Anterior = "", Siguiente= "";
            bool firsttime = true;
            List<int> Index = new List<int>();
            if (checkBox3.Checked == true)
            {
                if (dataGridView3.Rows.Count != 0)
                {
                    foreach (DataGridViewRow r in dataGridView3.Rows)
                    {
                        if (firsttime)
                        {
                            IdAgrupador = Conexion.SelectAgrupador(r.Cells["IdAnalisis"].Value.ToString());
                            Anterior = IdAgrupador.Tables[0].Rows[0]["IdAgrupador"].ToString();
                            firsttime = false;
                            Siguiente = "";
                        }
                        else
                        {
                            IdAgrupador = Conexion.SelectAgrupador(r.Cells["IdAnalisis"].Value.ToString());
                            Siguiente = IdAgrupador.Tables[0].Rows[0]["IdAgrupador"].ToString();
                            if (Anterior == Siguiente)
                            {
                                Index.Add(Convert.ToInt32(r.Cells["IdAnalisis"].Value));
                            }
                            Anterior = Siguiente;
                        }

                    }
                    foreach (int i in Index)
                    {
                        foreach (DataGridViewRow r in dataGridView3.Rows)
                        {
                            if (r.Cells["IdAnalisis"].Value.ToString() == i.ToString())
                            {
                                dataGridView3.Rows.RemoveAt(r.Index);
                            }
                        }
                    }

                }
            }
            else 
            {
                try
                {
                    dataGridView3.Rows.Clear();
                    DataSet ds = new DataSet();
                    string cmd = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["IdOrden"].Value.ToString();
                    ds = Conexion.SELECTAnalisisPrueba(cmd);
                    DataSet ds1 = new DataSet();
                    ds1 = Conexion.SELECTPersona(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Cedula"].Value.ToString());
                    label12.Text = ds1.Tables[0].Rows[0][2].ToString() + " " + ds1.Tables[0].Rows[0][3].ToString();
                    label19.Text = dataGridView1.Rows[filaSeleccionada].Cells["NumeroDia"].Value.ToString();
                    DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                    nacimiento = DateTime.Parse(ds1.Tables[0].Rows[0][8].ToString());
                    DateTime Hoy = DateTime.Now;
                    string edad = Conexion.Fecha(nacimiento);
                    label10.Text = edad;
                    label11.Text = ds1.Tables[0].Rows[0][6].ToString();
                    OrdenID = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["IdOrden"].Value);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        dataGridView3.Rows.Add(dr["IdOrden"].ToString(), dr["IdAnalisis"].ToString(), dr["NombreAnalisis"].ToString(), dr["ValorResultado"].ToString(), dr["Unidad"].ToString(), dr["TipoAnalisis"].ToString(), dr["IdOrganizador"].ToString(), dr["EstadoDeResultado"].ToString());
                    }
                }
                catch(Exception ex)
                {
                    if (ex.Message == "Referencia a objeto no establecida como instancia de un objeto.")
                    {
                        MessageBox.Show("Por favor seleccione una Orden");
                    }
                }
            }
           
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ((DataTable)dataGridView1.DataSource).DefaultView.RowFilter = string.Format("[Nombre1] LIKE '%{0}%'", comboBox1.Text);
        }

        private void listadoDePacientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void envioDeEspecialesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void analisisToolStripMenuItem_Click(object sender, EventArgs e)
        {

                DataSet Permisos = new DataSet();
                Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
                if (Permisos.Tables[0].Rows[0]["VerOrdenes"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
                {
                    Form form22 = new Form22();
                    form22.Show();
                }
                else
                {
                    MessageBox.Show("No tiene privilegios para realizar esta accion");
                }
 
        }

        private void cobroToolStripMenuItem1_Click(object sender, EventArgs e)
        {
                DataSet Permisos = new DataSet();
                Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerOrdenes"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                Form form22 = new OrdenesPorCobrar(IdUser);
                form22.ShowDialog();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }

       }

          private void Form22_FormClosing(object sender, FormClosingEventArgs e)
        {
            throw new NotImplementedException();
        }


        private void pendientesToolStripMenuItem_Click(object sender, EventArgs e)
        {

                DataSet Permisos = new DataSet();
                Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
                if (Permisos.Tables[0].Rows[0]["VerCierreCaja"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
                {
                    Form pendientes = new Pendientes(IdUser);
                    pendientes.ShowDialog();
                }
                else
                {
                    MessageBox.Show("No tiene privilegios para realizar esta accion");
                }
 
        }

        private void auditarCierreToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        private void iconButton7_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerCierreCaja"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                Form form21 = new Ordenestxt();
                form21.Show();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void cierreToolStripMenuItem_Click(object sender, EventArgs e)
        {

                DataSet Permisos = new DataSet();
                Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
                if (Permisos.Tables[0].Rows[0]["VerLibroVenta"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
                {
                    Form Auditoriacierre = new AuditoriaCierre();
                    Auditoriacierre.Show();
                }
                else
                {
                    MessageBox.Show("No tiene privilegios para realizar esta accion");
                }

           
        }

        private void monedaExtranjeraToolStripMenuItem_Click(object sender, EventArgs e)
        {

                DataSet Permisos = new DataSet();
                Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
                if (Permisos.Tables[0].Rows[0]["VerLibroVenta"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
                {
                    Form monedaExtranjera = new MonedaExtranjera();
                    monedaExtranjera.Show();
                }
                else
                {
                    MessageBox.Show("No tiene privilegios para realizar esta accion");
                }

        }

        private void porPersonaToolStripMenuItem_Click(object sender, EventArgs e)
        {

                DataSet Permisos = new DataSet();
                Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
                if (Permisos.Tables[0].Rows[0]["VerLibroVenta"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
                {
                    Form monedaExtranjera = new TotalFacturadoPorPersona();
                    monedaExtranjera.Show();
                }
                else
                {
                    MessageBox.Show("No tiene privilegios para realizar esta accion");
                }

        }

        private void porAnalisisToolStripMenuItem_Click(object sender, EventArgs e)
        {

                DataSet Permisos = new DataSet();
                Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
                if (Permisos.Tables[0].Rows[0]["VerLibroVenta"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
                {
                    Form monedaExtranjera = new Form43();
                    monedaExtranjera.Show();
                }
                else
                {
                    MessageBox.Show("No tiene privilegios para realizar esta accion");
                }
 
        }

        private void iconButton11_MouseEnter(object sender, EventArgs e)
        {
            ToolTip toolTip1 = new ToolTip();
            toolTip1.SetToolTip(this.ListaDePrecios, "Lista de Precios");
        }

        private void iconButton12_MouseEnter(object sender, EventArgs e)
        {
            ToolTip toolTip1 = new ToolTip();
            toolTip1.SetToolTip(this.HojaDeTrabajo, "Hoja de Trabajo");
        }

        private void iconButton8_MouseEnter(object sender, EventArgs e)
        {
            ToolTip toolTip1 = new ToolTip();
            toolTip1.SetToolTip(this.TeclasHematologia, "Teclas para Hematologia");
        }

        private void iconButton10_MouseEnter(object sender, EventArgs e)
        {
            ToolTip toolTip1 = new ToolTip();
            toolTip1.SetToolTip(this.Servicio, "Reiniciar Servicio");
        }

        private void iconButton13_Click(object sender, EventArgs e)
        {
            VerificarServicio();
        }

        private void iconButton13_MouseEnter(object sender, EventArgs e)
        {

        }

        private void escribirPorWhatsappToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void pruebasToolStripMenuItem_Click(object sender, EventArgs e)
        {

              
        }

        private void anulacionesToolStripMenuItem2_Click(object sender, EventArgs e)
        {

            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerLibroVenta"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                Form form26 = new Anulaciones();
                form26.Show();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void iconButton1_MouseClick(object sender, MouseEventArgs e)
        {
         
        }

        private void ingresoIndividualToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        private void ingresoAgrupadoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void numeroEnSistemaToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string cmd = dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells["Orden"].Value.ToString();
            string Telefono = Conexion.Telefono(cmd);
            if (Telefono != "0")
            {
                impresion = "Guardar";

                DataSet ds1 = new DataSet();
                OrdenID = Convert.ToInt32(dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells["Orden"].Value.ToString());
                AnalisisID = Convert.ToInt32(dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells["IdAnalisis"].Value.ToString());
                int Contador = Conexion.VerificarCorreo(cmd);
                if (Contador != 0)
                {
                        Impresiones.SendWhatsapp(OrdenID, AnalisisID, impresion, IdUser);               
                }
                else
                {
                    MessageBox.Show("No Hay Resultados Validados Para Enviar.");
                }
                ActualizarDG3();
            }
            else
            {
                ActualizarDG3();
                MessageBox.Show("Paciente no tiene telefono celular configurado");
            }

        }

        private void estadisticaPorPersonaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerEstadisticas"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                Form form25 = new EstadisticaPersonascs();
                form25.Show();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void perfilesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void analisisToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void conveniosToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void empresasToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

        }

        private void almacenToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void pacientesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void crearUsuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerEstadisticas"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                Form form32 = new Form32();
                form32.Show();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void modificarUsuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerEstadisticas"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                Form form32 = new Form29(IdUser);
                form32.Show();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void Admin_MouseClick(object sender, MouseEventArgs e)
        {
            contextMenuStrip4.Show(Cursor.Position.X, Cursor.Position.Y);
        }

        private void anulacionesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerLibroVenta"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                Form form32 = new Anulaciones();
                form32.Show();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void crearPerfilSimpleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerLibroVenta"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                Form form32 = new PerfilSimple();
                form32.Show();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void crearPerfilCompuestoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerLibroVenta"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                Form form32 = new PerfilCompuesto();
                form32.Show();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void modificarPerfilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerLibroVenta"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                Form form32 = new ModificarPerfilCompuesto();
                form32.Show();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void insertarConvenioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerLibroVenta"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                Form form32 = new Form31();
                form32.Show();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void crearAnalisisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerEstadisticas"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                Form form32 = new ListaDeAnalisis();
                form32.ShowDialog();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {

        }

        private void estadista2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerEstadisticas"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                Form form25 = new SegundaEstadistica();
                form25.Show();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void agregarBancosToolStripMenuItem_Click(object sender, EventArgs e)
        {


           
        }

        private void bancosPorPagoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void listaDeBancosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerEstadisticas"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                BancosForm bancosFormulario = new BancosForm();
                bancosFormulario.ShowDialog();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void asignarATiposDePagoToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerEstadisticas"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                BancosTipoPago bancosFormulario = new BancosTipoPago();
                bancosFormulario.ShowDialog();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void reporteCobroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerEstadisticas"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                ReporteDePagos reporte = new ReporteDePagos();
                reporte.ShowDialog();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
           
        }

        private void limpiarSeleccionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView3.ClearSelection();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void agregarNumeroToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void estadisticaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["VerEstadisticas"].ToString() == "1" || Permisos.Tables[0].Rows[0]["Cargo"].ToString() == "2")
            {
                Form form25 = new Form25();
                form25.Show();
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }
    }
}

