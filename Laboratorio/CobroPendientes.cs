using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;
using Conexiones.DbConnect;
using Conexiones.Modelos;

namespace Laboratorio
{
    public partial class CobroPendientes : Form
    {
        List<Bancos> bancosIngresoSeleccionado = new List<Bancos>();
        List<Bancos> bancosEgresoSeleccionado = new List<Bancos>();
        int IdBancoIngreso = 0, IdBancoEgreso = 0;


        List<Pagos> tipoPago = new List<Pagos>();
        public static int IdOrden;
        int monedaI = 0, monedaF = 0;
        decimal Dolar = 0, Euros = 0, Pesos = 0, DDolarT = 0, DPesosT = 0, DEurosT = 0, DBolivares = 0, DEuro = 0, BolivaresS;

        private void iconButton5_Click(object sender, EventArgs e)
        {
            DataSet ds4 = new DataSet();
            ds4 = Conexion.SeleccionarPagos(IdOrden.ToString(), DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"));
            if (ds4.Tables.Count != 0)
            {
                if (ds4.Tables[0].Rows.Count != 0)
                {
                    string mensaje = "¿Desea imprimir el Comprobante de Pago?";
                    string titulo = "Alarma";
                    MessageBoxButtons button = MessageBoxButtons.YesNo;
                    DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                    if (dialog == DialogResult.Yes)
                    {
                        Form formatoDePendientes = new FormatoDePendientes();
                        formatoDePendientes.Show();
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("No ha agregado datos el dia de hoy por favor verificar los datos ingresados");
                }
            }
            else
            {
                MessageBox.Show("No ha agregado datos el dia de hoy por favor verificar los datos ingresados");
            }


        }

        private void iconButton2_Click(object sender, EventArgs e)
        {

            //if (BancosIngreso.Visible == true)
            //{
            //    if (BancosIngreso.SelectedIndex < 0)
            //    {
            //        MessageBox.Show("Debe Seleccionar un banco");
            //        return;
            //    }
            //}
            if (comboBox1.SelectedIndex != (-1))
            {

                if (textBox2.Text != "")
                {
                    if (comboBox1.Text == "Ordenes")
                    {
                        Conexion.InsertarPagos(IdOrden.ToString(), comboBox1.Text, textBox2.Text, Regex.Replace(label32.Text, Cambio, ""), textBox1.Text, monedaI.ToString(), "1", tasadia, IdBancoIngreso);
                        Conexion.InsertarPagos(textBox1.Text, comboBox1.Text, textBox2.Text, Regex.Replace(label32.Text, Cambio, ""), IdOrden.ToString(), monedaI.ToString(), "2", tasadia, IdBancoIngreso);
                        dataGridView1.Rows.Add(textBox2.Text, comboBox1.Text, Regex.Replace(label32.Text, Cambio, ""), textBox1.Text, monedaI);
                        Cobrar();
                    }
                    else
                    {
                        Conexion.InsertarPagos(IdOrden.ToString(), comboBox1.Text, textBox2.Text, Regex.Replace(label32.Text, Cambio, ""), textBox1.Text, monedaI.ToString(), "1", tasadia, IdBancoIngreso);
                        dataGridView1.Rows.Add(textBox2.Text, comboBox1.Text, Regex.Replace(label32.Text, Cambio, ""), textBox1.Text, monedaI);
                        Cobrar();
                    }

                }
                else
                {
                    MessageBox.Show("Por favor ingrese una cantidad valida");
                }
            }
            else
            {
                MessageBox.Show("Por Favor Seleccione un Metodo de Pago");
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            string FechaOrden;
            DataSet FechaPago = new DataSet();

                if (dataGridView1.SelectedRows.Count != 0)
                {
                FechaPago = Conexion.SelecFechaPago(IdOrden.ToString(), dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["Cantidad"].Value.ToString(), dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["Moneda"].Value.ToString());

                if (Convert.ToDateTime(FechaPago.Tables[0].Rows[0]["Fecha"]).ToString("dd-MM-yyyy") == DateTime.Now.ToString("dd-MM-yyyy"))
                {
                    if (dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["Tipo"].Value.ToString() == "Ordenes")
                    {
                        string mensaje = "Estas Eliminando un Cruce de Ordenes, Esta Seguro de Realizar esta Accion?";
                        string titulo = "Alarma";
                        DataSet ds1 = new DataSet();
                        ds1 = Conexion.Privilegios(IdUser);
                        if (ds1.Tables[0].Rows[0]["Privilegios"].ToString() != "")
                        {
                            FechaOrden = Conexion.BuscarFechaOrden(IdOrden.ToString(), dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["Serial"].Value.ToString());

                        }
                        else
                        {
                            MessageBox.Show("No tienes privilegios para realizar esta accion");
                        }
                        MessageBoxButtons button = MessageBoxButtons.YesNo;
                        DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                        if (dialog == DialogResult.Yes)
                        {
                            try
                            {
                                int Ingresada = Conexion.BurcarOrdenCruzada(dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["Serial"].Value.ToString(), IdOrden.ToString());
                                if (Ingresada == 1)
                                {
                                    Conexion.BorrarPago(IdOrden.ToString(), dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["Serial"].Value.ToString());
                                }
                                else
                                {
                                    Conexion.BorrarPagoSeleccionado(IdOrden.ToString(), dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["Cantidad"].Value.ToString(), dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["Moneda"].Value.ToString(), dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["Serial"].Value.ToString());
                                }
                                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                            }
                            catch 
                            {
                                MessageBox.Show("Ha Ocurrido Un Error");
                            }
                        }
                    }
                    else
                    {
                        Conexion.BorrarPagoSeleccionado(IdOrden.ToString(), dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["Cantidad"].Value.ToString(), dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["Moneda"].Value.ToString(), dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells["Serial"].Value.ToString());
                        dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                    }
                }
                else
                {
                    MessageBox.Show("No tiene privilegios para borrar un pago realizado dias anteriores. Por favor comuniquese con administracion o realice el cambio desde el libro de venta");
                }
            }
           
            Cobrar();
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex != (-1))
            {
                //if (BancosEgreso.Visible == true)
                //{
                //    if (BancosEgreso.SelectedIndex < 0)
                //    {
                //        MessageBox.Show("Debe Seleccionar un banco");
                //        return;
                //    }
                //}
                if (textBox3.Text != "")
                {
                    if (comboBox2.Text == "Ordenes")
                    {
                        Conexion.InsertarPagos(textBox4.Text, comboBox2.Text, textBox3.Text, Regex.Replace(label33.Text, Cambio, ""), IdOrden.ToString(), monedaF.ToString(), "1", tasadia,IdBancoEgreso);
                        Conexion.InsertarPagos(IdOrden.ToString(), comboBox2.Text, textBox3.Text, Regex.Replace(label33.Text, Cambio, ""), textBox4.Text, monedaF.ToString(), "2", tasadia, IdBancoEgreso);
                        dataGridView2.Rows.Add(textBox3.Text, comboBox2.Text, Regex.Replace(label33.Text, Cambio, ""), textBox4.Text, monedaF);
                        Cobrar();
                    }
                    else
                    {
                        Conexion.InsertarPagos(IdOrden.ToString(), comboBox2.Text, textBox3.Text, Regex.Replace(label33.Text, Cambio, ""), textBox4.Text, monedaF.ToString(), "2", tasadia, IdBancoEgreso);
                        dataGridView2.Rows.Add(textBox3.Text, comboBox2.Text, Regex.Replace(label33.Text, Cambio, ""), textBox4.Text, monedaF);
                        Cobrar();
                    }
                }
                else
                {
                    MessageBox.Show("Por favor ingrese una cantidad valida");
                }
            }
            else
            {
                MessageBox.Show("Por Favor Seleccione un Metodo de Pago");
            }

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
           
        if (dataGridView2.SelectedRows.Count != 0)
        {
                DataSet FechaPago = new DataSet();
                FechaPago = Conexion.SelecFechaPago(IdOrden.ToString(), dataGridView2.Rows[dataGridView2.SelectedRows[0].Index].Cells["CantidadD"].Value.ToString(), dataGridView2.Rows[dataGridView2.SelectedRows[0].Index].Cells["MonedaD"].Value.ToString());
                if (Convert.ToDateTime(FechaPago.Tables[0].Rows[0]["Fecha"]).ToString("dd-MM-yyyy") == DateTime.Now.ToString("dd-MM-yyyy"))
                {
                    if (dataGridView2.Rows[dataGridView2.SelectedRows[0].Index].Cells["TipoD"].Value.ToString() == "Ordenes")
                    {
                        string mensaje = "Estas Eliminando un Cruce de Ordenes, Esta Seguro de Realizar esta Accion?";
                        string titulo = "Alarma";
                        MessageBoxButtons button = MessageBoxButtons.YesNo;
                        DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                        if (dialog == DialogResult.Yes)
                        {
                            try
                            {
                                int Ingresada = Conexion.BurcarOrdenCruzada(dataGridView2.Rows[dataGridView2.SelectedRows[0].Index].Cells["SerialD"].Value.ToString(), IdOrden.ToString());
                                if (Ingresada == 1)
                                {
                                    Conexion.BorrarPago(IdOrden.ToString(), dataGridView2.Rows[dataGridView2.SelectedRows[0].Index].Cells["SerialD"].Value.ToString());
                                }
                                else
                                {
                                    Conexion.BorrarPagoSeleccionado(IdOrden.ToString(), dataGridView2.Rows[dataGridView2.SelectedRows[0].Index].Cells["CantidadD"].Value.ToString(), dataGridView2.Rows[dataGridView2.SelectedRows[0].Index].Cells["MonedaD"].Value.ToString(), dataGridView2.Rows[dataGridView2.SelectedRows[0].Index].Cells["SerialD"].Value.ToString());
                                }

                                dataGridView2.Rows.RemoveAt(dataGridView2.SelectedRows[0].Index);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Ha Ocurrido Un Error");
                            }
                        }
                    }
                    else
                    {
                        Conexion.BorrarPagoSeleccionado(IdOrden.ToString(), dataGridView2.Rows[dataGridView2.SelectedRows[0].Index].Cells["CantidadD"].Value.ToString(), dataGridView2.Rows[dataGridView2.SelectedRows[0].Index].Cells["MonedaD"].Value.ToString(), dataGridView2.Rows[dataGridView2.SelectedRows[0].Index].Cells["SerialD"].Value.ToString());
                        dataGridView2.Rows.RemoveAt(dataGridView2.SelectedRows[0].Index);

                    }
                }
                else
                {
                    MessageBox.Show("No tiene privilegios para borrar un pago realizado dias anteriores. Por favor comuniquese con administracion o realice el cambio desde el libro de venta");
                }
        }
          

    Cobrar();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            label32.Text = "0,00";
            if (comboBox1.SelectedIndex != (-1))
            {
                var pago = tipoPago.FirstOrDefault(x => x.NombreTIpoPago == comboBox1.Text);
                if (pago.bancos.Count() > 0)
                {
                    BancosIngreso.Visible = true;
                    BancosIngreso.Items.Clear();
                    bancosIngresoSeleccionado = pago.bancos;
                    foreach (var bancos in pago.bancos)
                    {
                        BancosIngreso.Items.Add(bancos.NombreBanco);
                    }
                }
                else
                {
                    BancosIngreso.Visible = false;
                    BancosIngreso.Items.Clear();

                }
                monedaI = Convert.ToInt32(pago.Moneda);

            }
            if (comboBox1.Text == "Ordenes")
            {
                iconButton7.Visible = true;
                iconButton2.Enabled = false;
            }
            else
            {
                iconButton7.Visible = false;
                iconButton2.Enabled = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (monedaI != 0)
            {
                SeleccionDeMonedaIngreso(monedaI);
            }
            else
            {
                MessageBox.Show("Por Favor Selecicone un Metodo de Pago");
                textBox2.Clear();
            }

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox3.Clear();
            textBox4.Clear();
            label33.Text = "0,00";
            if (comboBox2.SelectedIndex != (-1))
            {
                var pago = tipoPago.FirstOrDefault(x => x.NombreTIpoPago == comboBox2.Text);
                if (pago.bancos.Count() > 0)
                {
                    BancosEgreso.Visible = true;
                    LBancoEgreso.Visible = true;
                    bancosEgresoSeleccionado = pago.bancos;
                    BancosEgreso.Items.Clear();
                    foreach (var bancos in pago.bancos)
                    {
                        BancosEgreso.Items.Add(bancos.NombreBanco);
                    }
                }
                else
                {
                    BancosEgreso.Visible = false;
                    BancosEgreso.Items.Clear();

                }

                monedaF = Convert.ToInt32(pago.Moneda);
            }
            if (comboBox2.Text == "Ordenes")
            {
                iconButton8.Visible = true;
                iconButton4.Enabled = false;
            }
            else
            {
                iconButton8.Visible = false;
                iconButton4.Enabled = true;
            }

        }

        private string MonedaIngreso;
        private string MonedaEgreso;

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (monedaF != 0)
            {
                SeleccionDeMonedaEngreso(monedaF);
            }
            else
            {
                MessageBox.Show("Por Favor Selecicone un Metodo de Pago");
                textBox3.Clear();
            }
        }

        private void iconButton6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
            {
                // Allow Digits and BackSpace char
            }
            else if (e.KeyChar == '.' && !((TextBox)sender).Text.Contains(','))
            {
                e.KeyChar = ',';
            }
            else if (e.KeyChar == ',' && !((TextBox)sender).Text.Contains(','))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
            {
                // Allow Digits and BackSpace char
            }
            else if (e.KeyChar == '.' && !((TextBox)sender).Text.Contains(','))
            {
                e.KeyChar = ',';
            }
            else if (e.KeyChar == ',' && !((TextBox)sender).Text.Contains(','))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void iconButton7_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox1.Text != " " && textBox1.Text != IdOrden.ToString())
            {
                string Cruzada = Cruzando(textBox1.Text);
                if (Cruzada != "0")
                {
                    textBox2.Text = Cruzada;
                    Conexion.InsertarPagos(IdOrden.ToString(), comboBox1.Text, textBox2.Text, Regex.Replace(label32.Text, Cambio, ""), textBox1.Text, monedaI.ToString(), "1", tasadia,IdBancoIngreso);
                    Conexion.InsertarPagos(textBox1.Text, comboBox1.Text, textBox2.Text, Regex.Replace(label32.Text, Cambio, ""), IdOrden.ToString(), monedaI.ToString(), "2", tasadia, IdBancoIngreso);
                    dataGridView1.Rows.Add(textBox2.Text, comboBox1.Text, Regex.Replace(label32.Text, Cambio, ""), textBox1.Text, monedaI);
                    Cobrar();
                }
            }
            else
            {
                MessageBox.Show("El campo Serial no puede estar vacio, o estas cruzando una Orden con ella misma");
            }
        }

        private string Cruzando(string cmd)
        {
            DataSet Cruzando = new DataSet();
            Cruzando = Conexion.ordenesPorcruzar(cmd);
            if (Cruzando.Tables.Count != 0)
            {
                if (Cruzando.Tables[0].Rows.Count != 0)
                {
                    double Convertido = Math.Abs(Convert.ToDouble(Cruzando.Tables[0].Rows[0]["Total"].ToString()));
                    if (Convertido != 0)
                    {
                        string mensaje = string.Format("La Orden {0}, es del paciente {1} que tiene una diferencia de {2}, es la Orden que deseas Cruzar?", cmd, Cruzando.Tables[0].Rows[0]["Nombre"].ToString(), Convertido.ToString("N"));
                        string titulo = "Alarma";
                        MessageBoxButtons button = MessageBoxButtons.YesNo;
                        DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                        if (dialog == DialogResult.Yes)
                        {
                            return Convertido.ToString();
                        }
                        else
                        {
                            return "0";
                        }
                    }
                    else
                    {
                        MessageBox.Show("Esta Orden no tiene saldo pendiente");
                        return "0";
                    }
                }
                else
                {
                    return "0";
                }
            }
            return "0";
        }
        private void iconButton8_Click(object sender, EventArgs e)
        {

            if (textBox4.Text != "" && textBox4.Text != " " && textBox4.Text != IdOrden.ToString())
            {
                string Cruzada = Cruzando(textBox4.Text);
                if (Cruzada != "0")
                {
                    textBox3.Text = Cruzada;
                    iconButton8.Enabled = true;
                    Conexion.InsertarPagos(textBox4.Text, comboBox2.Text, textBox3.Text, Regex.Replace(label33.Text, Cambio, ""), IdOrden.ToString(), monedaF.ToString(), "1", tasadia,IdBancoEgreso);
                    Conexion.InsertarPagos(IdOrden.ToString(), comboBox2.Text, textBox3.Text, Regex.Replace(label33.Text, Cambio, ""), textBox4.Text, monedaF.ToString(), "2", tasadia, IdBancoEgreso);
                    dataGridView2.Rows.Add(textBox3.Text, comboBox2.Text, Regex.Replace(label33.Text, Cambio, ""), textBox4.Text, monedaF);
                    Cobrar();
                }
            }
            else
            {
                MessageBox.Show("El campo Serial no puede estar vacio, o estas cruzando una Orden con ella misma");
            }
        }

        private void BancosIngreso_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BancosIngreso.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione un Banco");
                return;
            }
            IdBancoIngreso = bancosIngresoSeleccionado.ElementAt(BancosIngreso.SelectedIndex).IdBancos;
        }

        private void BancosEgreso_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BancosEgreso.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione un Banco");
                return;
            }
            IdBancoEgreso = bancosEgresoSeleccionado.ElementAt(BancosEgreso.SelectedIndex).IdBancos;
        }

        string Busqueda = @"Bs.F.";
        string Cambio = @"Bs.S.";
        string tasadia;
        int IdUser;
        public CobroPendientes(int idOrden,int idUser)
        {
            IdOrden = idOrden;
            IdUser = idUser;
            InitializeComponent();
        }

        private class Pagos
        {
            private string idTipopago;
            private string moneda;
            private string nombreTIpoPago;
            public List<Bancos> bancos { get; set; } = new List<Bancos>();
            public Pagos(string idTipopago, string nombreTIpoPago, string moneda)
            {
                this.idTipopago = idTipopago;
                this.nombreTIpoPago = nombreTIpoPago;
                this.moneda = moneda;
            }

            public string IdTipoPago
            {
                get { return idTipopago; }
                set { idTipopago = value; }
            }

            public string NombreTIpoPago
            {
                get { return nombreTIpoPago; }
                set { nombreTIpoPago = value; }
            }
            public string Moneda
            {
                get { return moneda; }
                set { moneda = value; }
            }

        }
        private void Cobrar()
        {
            decimal DolarC = 0, PesosC = 0, BolivaresC = 0, EurosC = 0, DolarD = 0, PesosD = 0, BolivaresD = 0, EurosD = 0, BDolarC, BPesosC, BEurosC, BDolarD, BPesosD, BEurosD, TotalBolivares, SubTotalBolivares, SubTotalBolivaresC, SubTotalBolivaresD;
            if (dataGridView1.Rows.Count != 0)
            {
                foreach (DataGridViewRow r in dataGridView1.Rows)
                {
                    if (r.Cells["Moneda"].Value.ToString() != "" && r.Cells["Moneda"].Value.ToString() != null)
                    {
                        if (Convert.ToInt32(r.Cells["Moneda"].Value.ToString()) == 1 || Convert.ToInt32(r.Cells["Moneda"].Value.ToString()) == 2)
                        {
                            BolivaresC = BolivaresC + Convert.ToDecimal(r.Cells["Bolivares"].Value.ToString());
                        }
                        else if (Convert.ToInt32(r.Cells["Moneda"].Value.ToString()) == 3)
                        {
                            DolarC = DolarC + Math.Round(Convert.ToDecimal(r.Cells["Cantidad"].Value.ToString()), 2);
                        }
                        else if (Convert.ToInt32(r.Cells["Moneda"].Value.ToString()) == 4)
                        {
                            PesosC = PesosC + Math.Round(Convert.ToDecimal(r.Cells["Cantidad"].Value.ToString()), 2);
                        }
                        else if (Convert.ToInt32(r.Cells["Moneda"].Value.ToString()) == 5)
                        {
                            EurosC = EurosC + Math.Round(Convert.ToDecimal(r.Cells["Cantidad"].Value.ToString()), 2);
                        }
                    }
                }
            }
            if (dataGridView2.Rows.Count != 0)
            {
                foreach (DataGridViewRow r in dataGridView2.Rows)
                {
                    if (r.Cells["MonedaD"].Value.ToString() != "" && r.Cells["MonedaD"].Value.ToString() != null)
                    {
                        if (Convert.ToInt32(r.Cells["MonedaD"].Value.ToString()) == 1 || Convert.ToInt32(r.Cells["MonedaD"].Value.ToString()) == 2)
                        {
                            BolivaresD = BolivaresD + Convert.ToDecimal(r.Cells["BolivaresD"].Value.ToString());
                        }
                        else if (Convert.ToInt32(r.Cells["MonedaD"].Value.ToString()) == 3)
                        {
                            DolarD = DolarD + Math.Round(Convert.ToDecimal(r.Cells["CantidadD"].Value.ToString()), 2);
                        }
                        else if (Convert.ToInt32(r.Cells["MonedaD"].Value.ToString()) == 4)
                        {
                            PesosD = PesosD + Math.Round(Convert.ToDecimal(r.Cells["CantidadD"].Value.ToString()), 2);
                        }
                        else if (Convert.ToInt32(r.Cells["MonedaD"].Value.ToString()) == 5)
                        {
                            EurosD = EurosD + Math.Round(Convert.ToDecimal(r.Cells["CantidadD"].Value.ToString()), 2);
                        }
                    }
                }
            }
            BDolarC = Math.Round((DolarC * Dolar), 2);
            BPesosC = Math.Round((PesosC / Pesos * Dolar), 2);
            BEurosC = Math.Round((EurosC * Euros), 2);
            BDolarD = Math.Round((DolarD * Dolar), 2);
            BPesosD = Math.Round((PesosD / Pesos * Dolar), 2);
            BEurosD = Math.Round((EurosD * Euros), 2);
            SubTotalBolivaresC = BolivaresC + BDolarC + BPesosC + BEurosC;
            SubTotalBolivaresD = BolivaresD + BDolarD + BPesosD + BEurosD;
            SubTotalBolivares = SubTotalBolivaresC - SubTotalBolivaresD;
            label34.Text = string.Format("Bs.S. {0}", SubTotalBolivaresC.ToString("N", new CultureInfo("es-VE")));
            label36.Text = string.Format("Bs.S. {0}", SubTotalBolivaresD.ToString("N", new CultureInfo("es-VE")));
            TotalBolivares = 0 - DBolivares + SubTotalBolivares;
            TotalF.Text = string.Format("Bs.S. {0}", TotalBolivares.ToString("N", new CultureInfo("es-VE")));
            DolarF.Text = string.Format("$ {0}", Math.Round(TotalBolivares / Dolar, 2).ToString());
            PesosF.Text = Math.Round(TotalBolivares / Dolar * Pesos, 2).ToString("C", new CultureInfo("es-CO"));
            EurosF.Text = Math.Round(TotalBolivares / Euros, 2).ToString("C", new CultureInfo("fr-FR"));
            comboBox1.SelectedIndex = -1;
            textBox2.Clear();
            textBox1.Clear();
            comboBox2.SelectedIndex = -1;
            textBox3.Clear();
            textBox4.Clear();
            monedaI = 0;
            monedaF = 0;
            BancosIngreso.Items.Clear();
            BancosIngreso.Visible = false;
            BancosEgreso.Items.Clear();
            BancosEgreso.Visible = false;
        }
        private void SeleccionDeMonedaIngreso(int moneda)
        {
            if (textBox2.Text != "")
            {
                switch (moneda)
                {
                    case 1:

                        label32.Text = string.Format("Bs.S. {0}", Convert.ToDouble(textBox2.Text).ToString("N", new CultureInfo("es-VE")));
                        break;
                    case 2:
                        label32.Text = string.Format("Bs.S. {0}", Convert.ToDouble(textBox2.Text).ToString("N", new CultureInfo("es-VE")));
                        textBox1.Enabled = true;
                        break;
                    case 3:

                        decimal dolaresBolivares;
                        if (textBox2.Text != "")
                        {
                            dolaresBolivares = Convert.ToDecimal(textBox2.Text) * Dolar;
                            label32.Text = string.Format("Bs.S. {0}", dolaresBolivares.ToString("N", new CultureInfo("es-VE")));
                            textBox1.Enabled = true;
                        }

                        break;
                    case 4:
                        decimal Apesos;
                        Apesos = (Convert.ToDecimal(textBox2.Text) / Pesos) * Dolar;
                        label32.Text = string.Format("Bs.S. {0}", Apesos.ToString("N", new CultureInfo("es-VE")));
                        break;
                    case 5:
                        decimal AEuros;
                        AEuros = Convert.ToDecimal(textBox2.Text) * Euros;
                        label32.Text = string.Format("Bs.S. {0}", AEuros.ToString("N", new CultureInfo("es-VE")));
                        break;
                }
            }
        }
        private void SeleccionDeMonedaEngreso(int moneda)
        {
            if (textBox3.Text != "")
            {
                switch (moneda)
                {
                    case 1:

                        label33.Text = string.Format("Bs.S. {0}", Convert.ToDouble(textBox3.Text).ToString("N", new CultureInfo("es-VE")));
                        break;
                    case 2:
                        label33.Text = string.Format("Bs.S. {0}", Convert.ToDouble(textBox3.Text).ToString("N", new CultureInfo("es-VE")));

                        break;
                    case 3:
                        decimal dolaresBolivares;
                        if (textBox3.Text != "")
                        {
                            dolaresBolivares = Convert.ToDecimal(textBox3.Text) * Dolar;
                            label33.Text = string.Format("Bs.S. {0}", dolaresBolivares.ToString("N", new CultureInfo("es-VE")));
                            textBox1.Enabled = true;
                        }

                        break;
                    case 4:
                        decimal Apesos;
                        Apesos = (Convert.ToDecimal(textBox3.Text) / Pesos) * Dolar;
                        label33.Text = string.Format("Bs.S. {0}", Apesos.ToString("N", new CultureInfo("es-VE")));
                        break;
                    case 5:
                        decimal AEuros;
                        AEuros = Convert.ToDecimal(textBox3.Text) * Euros;
                        label33.Text = string.Format("Bs.S. {0}", AEuros.ToString("N", new CultureInfo("es-VE")));
                        break;
                }
            }
        }

        private void CobroPendientes_Load(object sender, EventArgs e)
        {
            DataSet ds4 = new DataSet();
            ds4 = Conexion.SeleccionarPagos2(IdOrden.ToString());
            DataSet ds = new DataSet();
            DataSet ds2 = new DataSet();
            DataSet ds3 = new DataSet();
            ds = Conexion.SELECTPersonaOrden(IdOrden.ToString());
            Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
            Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
            DateTime nacimiento = new DateTime(); //Fecha de nacimiento
            nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
            int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
            int edad = Hoy - nacimiento.Year;
            Edad.Text = Conexion.Fecha(nacimiento);
            NPaciente.Text = "#" + ds.Tables[0].Rows[0]["NumeroDia"].ToString();
            BolivaresS = Convert.ToDecimal(ds.Tables[0].Rows[0]["PrecioF"].ToString().Replace(".", ",")) * (-1);
            TotalF.Text = string.Format("Bs.S. {0}", BolivaresS.ToString("N", new CultureInfo("es-VE")));
            DBolivares = Convert.ToDecimal(ds.Tables[0].Rows[0]["PrecioF"].ToString().Replace(".", ","));
            Pfinal.Text = Regex.Replace(Convert.ToDouble(ds.Tables[0].Rows[0]["PrecioF"].ToString().Replace(".", ",")).ToString("N", new CultureInfo("es-VE")), Busqueda, Cambio);

            ds3 = Conexion.TiposdePago();
            foreach (DataRow r in ds3.Tables[0].Rows)
            {
                Pagos pago = new Pagos(r["idTipodePago"].ToString(), r["descripcion"].ToString(), r["Moneda"].ToString());
                int idPago = int.Parse(pago.IdTipoPago);
                pago.bancos = new Conexion().ObtenerBancosPorTipoDepago(idPago);
                tipoPago.Add(pago);
                comboBox1.Items.Add(r["descripcion"].ToString());
                comboBox2.Items.Add(r["descripcion"].ToString());
            }
            if (ds4.Tables[0].Rows.Count != 0)
            {
                ds2 = Conexion.SELECTTasaPorId(ds.Tables[0].Rows[0]["IdTasa"].ToString());
                label18.Text = (DBolivares / Convert.ToDecimal(ds2.Tables[0].Rows[0]["Dolar"].ToString().Replace(".", ","))).ToString("C", new CultureInfo("en-US"));
                tasadia = ds.Tables[0].Rows[0]["IdTasa"].ToString();
                Dolar = Convert.ToDecimal(ds2.Tables[0].Rows[0]["Dolar"].ToString().Replace(".", ","));
                Pesos = Convert.ToDecimal(ds2.Tables[0].Rows[0]["Pesos"].ToString());
                Euros = Convert.ToDecimal(ds2.Tables[0].Rows[0]["Euros"].ToString());
                label7.Text = ds2.Tables[0].Rows[0]["Dolar"].ToString().Replace(".", ",");
                label24.Text = ds2.Tables[0].Rows[0]["Pesos"].ToString();
                label26.Text = ds2.Tables[0].Rows[0]["Euros"].ToString();

                foreach (DataRow r in ds4.Tables[0].Rows)
                {
                    if (r["Clasificacion"].ToString() == "1")
                    {
                        dataGridView1.Rows.Add(r["Cantidad"].ToString(), r["TipodePago"].ToString(), Convert.ToDouble(r["ValorResultado"].ToString()).ToString("N"), r["Serial"].ToString(), r["Moneda"].ToString(), r["IdPago"].ToString());
                    }
                    else
                    {
                        dataGridView2.Rows.Add(r["Cantidad"].ToString(), r["TipodePago"].ToString(), Convert.ToDouble(r["ValorResultado"].ToString()).ToString("N"), r["Serial"].ToString(), r["Moneda"].ToString(), r["IdPago"].ToString());
                    }
                }
                Cobrar();
            }
            else
            {
                ds2 = Conexion.SELECTTasaPorId(ds.Tables[0].Rows[0]["IdTasa"].ToString());
                tasadia = ds.Tables[0].Rows[0]["IdTasa"].ToString();
                Dolar = Convert.ToDecimal(ds2.Tables[0].Rows[0]["Dolar"].ToString().Replace(".", ",").Replace(".", ","));
                Pesos = Convert.ToDecimal(ds2.Tables[0].Rows[0]["Pesos"].ToString());
                Euros = Convert.ToDecimal(ds2.Tables[0].Rows[0]["Euros"].ToString());
                label7.Text = ds2.Tables[0].Rows[0]["Dolar"].ToString().Replace(".", ",");
                label24.Text = ds2.Tables[0].Rows[0]["Pesos"].ToString();
                label26.Text = ds2.Tables[0].Rows[0]["Euros"].ToString();
                ds2 = Conexion.SELECTTasaPorId(ds.Tables[0].Rows[0]["IdTasa"].ToString());
                label18.Text = (DBolivares / Convert.ToDecimal(ds2.Tables[0].Rows[0]["Dolar"].ToString().Replace(".", ","))).ToString("C", new CultureInfo("en-US"));
                if (BolivaresS != 0)
                {
                    DDolarT = BolivaresS / Dolar;
                    DPesosT = BolivaresS / Dolar * Pesos;
                    DEurosT = BolivaresS / Euros;
                }
                DolarF.Text = string.Format("$ {0}", DDolarT);
                PesosF.Text = DPesosT.ToString("C", new CultureInfo("es-CO"));
                EurosF.Text = DEurosT.ToString("C", new CultureInfo("fr-FR"));
            }

        }
    }
}
