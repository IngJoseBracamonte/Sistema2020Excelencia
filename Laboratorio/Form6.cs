using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class Form6 : Form
    {
        Double CreatininaVM, VoluOrinaVM, Creatinina24hVM, CreaOrinaVM, PesoVM, AlturaVM, SuperficieVM, DepCreatininaVM, DepCreatinina24VM, VoluMinutoVM;
        Double CreatininaVm, VoluOrinaVm, Creatinina24hVm, CreaOrinaVm, PesoVm, AlturaVm, SuperficieVm, DepCreatininaVm, DepCreatinina24Vm, VoluMinutoVm;
        private int IdUser;
        private int IdOrden;
        private int IdAnalisis;

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (textBox6.Text != "")
            {
                Double DepCreatinina = Convert.ToDouble(textBox6.Text);
                if (DepCreatininaVM < DepCreatinina)
                {
                    textBox6.ForeColor = Color.Red;
                }
                else if (DepCreatininaVm > DepCreatinina)
                {
                    textBox6.ForeColor = Color.Blue;
                }
                else
                {
                    textBox6.ForeColor = Color.Black;
                }
                if (textBox4.Text != "") 
                { 
                double calculo = (Convert.ToDouble(textBox6.Text) * 1.73 )/ Convert.ToDouble(textBox4.Text);
                    textBox9.Text = Math.Round(calculo, 2).ToString();
                }
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (textBox5.Text != "")
            {
                Double CreaOrina = Convert.ToDouble(textBox5.Text);
                if (CreaOrinaVM < CreaOrina)
                {
                    textBox5.ForeColor = Color.Red;
                }
                else if (CreaOrinaVm > CreaOrina)
                {
                    textBox5.ForeColor = Color.Blue;
                }
                else 
                {
                    textBox5.ForeColor = Color.Black;
                }
            }
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBox2.Text != "" && textBox3.Text != "") 
            { 
            double calculo = Math.Pow(Convert.ToDouble(textBox2.Text), 0.425) * Math.Pow(Convert.ToDouble(textBox3.Text), 0.725)* 0.007184;
                textBox4.Text = Math.Round(calculo, 2).ToString();
            }
           

        }

        private void textBox3_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBox2.Text != "" && textBox3.Text != "")
            {
                double calculo = Math.Pow(Convert.ToDouble(textBox2.Text), 0.425) * Math.Pow(Convert.ToDouble(textBox3.Text), 0.725) * 0.007184;
                textBox4.Text = Math.Round(calculo, 2).ToString();
            }

        }

        private void textBox8_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBox5.Text != "" && textBox8.Text != "" && textBox10.Text != "")
            {
                double calculo = Convert.ToDouble(textBox5.Text)/Convert.ToDouble(textBox8.Text)*Convert.ToDouble(textBox10.Text);
                textBox6.Text = Math.Round(calculo, 2).ToString();
            }
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBox1.Text != "")
            {
                double calculo = Convert.ToDouble(textBox1.Text) / 1440;
                textBox10.Text = Math.Round(calculo, 2).ToString();
            }
            if (textBox5.Text != "" && textBox1.Text != "")
            {
                double calculo = Convert.ToDouble(textBox1.Text) * Convert.ToDouble(textBox5.Text) / 100000;
                textBox7.Text = Math.Round(calculo,2).ToString();
            }

        }

        private void textBox5_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBox5.Text != "" && textBox8.Text != "" && textBox10.Text != "")
            {
                double calculo = Convert.ToDouble(textBox5.Text) / Convert.ToDouble(textBox8.Text) * Convert.ToDouble(textBox10.Text);
                textBox6.Text =  Math.Round(calculo,2).ToString();
            }
            if (textBox5.Text != "" && textBox1.Text != "")
            {
                double calculo = Convert.ToDouble(textBox1.Text) * Convert.ToDouble(textBox5.Text) / 100000;
                textBox7.Text =  Math.Round(calculo,2).ToString();
            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            DataSet ds2 = new DataSet();
            ds2 = Conexion.PrivilegiosCargar(IdUser.ToString());
            string mensaje = "Al momento de Guardar estos Valores se tomara los datos como validados ¿Desea Validar?";
            string titulo = "Alarma";
            MessageBoxButtons button = MessageBoxButtons.YesNo;
            if (ds2.Tables[0].Rows[0]["Validar"].ToString() == "1")
            {
                DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    //(ValorResultado,Unidad,HoraValidacion,EstadoDeResultado,Comentario)
                    string MS = Conexion.InsertarSinValidar("", "", IdUser, IdOrden, 159);
                    Conexion.InsertarSinValidar(textBox1.Text, "", IdUser, IdOrden, 133);
                    Conexion.InsertarSinValidar(textBox2.Text, "", IdUser, IdOrden, 160);
                    Conexion.InsertarSinValidar(textBox3.Text, "", IdUser, IdOrden, 161);
                    Conexion.InsertarSinValidar(textBox4.Text, "", IdUser, IdOrden, 162);
                    Conexion.InsertarSinValidar(textBox5.Text, "", IdUser, IdOrden, 157);
                    Conexion.InsertarSinValidar(textBox6.Text, "", IdUser, IdOrden, 163);
                    Conexion.InsertarSinValidar(textBox7.Text, "", IdUser, IdOrden, 156);
                    Conexion.InsertarSinValidar(textBox8.Text, "", IdUser, IdOrden, 13);
                    Conexion.InsertarSinValidar(textBox9.Text, "", IdUser, IdOrden, 164);
                    Conexion.InsertarSinValidar(textBox10.Text, "", IdUser, IdOrden, 165);
                    MessageBox.Show(MS);
                    this.Close();
                }
            }

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (textBox7.Text != "")
            {
                Double Creatinina24h = Convert.ToDouble(textBox7.Text);
                if (Creatinina24hVM < Creatinina24h)
                {
                    textBox7.ForeColor = Color.Red;
                }
                else if (Creatinina24hVm > Creatinina24h)
                {
                    textBox7.ForeColor = Color.Blue;
                }
                else
                {
                    textBox7.ForeColor = Color.Black;
                }
            }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            if (textBox8.Text != "")
            {
                Double Creatinina = Convert.ToDouble(textBox8.Text);
                if (CreatininaVM < Creatinina)
                {
                    textBox8.ForeColor = Color.Red;
                }
                else if (CreatininaVm > Creatinina)
                {
                    textBox8.ForeColor = Color.Blue;
                }
                else
                {
                   textBox8.ForeColor = Color.Black;
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            { 
            Double VolOrina = Convert.ToDouble(textBox1.Text);
                if (VoluOrinaVM < VolOrina)
                {
                    textBox1.ForeColor = Color.Red;
                }
                else if (VoluOrinaVm > VolOrina)
                {
                    textBox1.ForeColor = Color.Blue;
                }
                else
                {
                    textBox1.ForeColor = Color.Black;
                }
            }
        }

        public Form6(int idUser, int idOrden, int idAnalisis)
        {
            IdUser = idUser;
            IdOrden = idOrden;
            IdAnalisis = idAnalisis;
            InitializeComponent();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            DataSet ds2 = new DataSet();
            ds2 = Conexion.PrivilegiosCargar(IdUser.ToString());
            string mensaje = "Al momento de Guardar estos Valores se tomara los datos como validados ¿Desea Validar?";
            string titulo = "Alarma";
            MessageBoxButtons button = MessageBoxButtons.YesNo;
            if (ds2.Tables[0].Rows[0]["Validar"].ToString() == "1")
            {
                DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    //(ValorResultado,Unidad,HoraValidacion,EstadoDeResultado,Comentario)
                    string MS = Conexion.InsertarFinal("","", IdUser, IdOrden, 159);
                    Conexion.InsertarFinal(textBox1.Text, "", IdUser, IdOrden, 133);
                    Conexion.InsertarFinal(textBox2.Text, "", IdUser, IdOrden, 160);
                    Conexion.InsertarFinal(textBox3.Text, "", IdUser, IdOrden, 161);
                    Conexion.InsertarFinal(textBox4.Text, "", IdUser, IdOrden, 162);
                    Conexion.InsertarFinal(textBox5.Text, "", IdUser, IdOrden, 157);
                    Conexion.InsertarFinal(textBox6.Text, "", IdUser, IdOrden, 163);
                    Conexion.InsertarFinal(textBox7.Text, "", IdUser, IdOrden, 156);
                    Conexion.InsertarFinal(textBox8.Text, "", IdUser, IdOrden, 13);
                    Conexion.InsertarFinal(textBox9.Text, "", IdUser, IdOrden, 164);                
                    Conexion.InsertarFinal(textBox10.Text, "", IdUser, IdOrden, 165);
                    MessageBox.Show(MS);
                    this.Close();
                }
            }
           
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds = Conexion.SELECTAnalisisFinal3(IdOrden);
            try
            {
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int edad = Hoy - nacimiento.Year;
                Edad.Text = Conexion.Fecha(nacimiento);
                Analisis.Text = ds.Tables[0].Rows[0]["NombreAnalisis"].ToString();
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    switch (Convert.ToInt32(r["IdAnalisis"].ToString()))
                    {
                        case 13:
           
                            CREATININAREF.Text = string.Format("{0} {1} - {2}", r["Unidad"].ToString(), r["ValorMenor"].ToString(), r["ValorMayor"].ToString());
                            CreatininaVM = Convert.ToDouble(r["ValorMayor"].ToString());
                            CreatininaVm = Convert.ToDouble(r["ValorMenor"].ToString());
                            textBox8.Text = r["ValorResultado"].ToString();
                            break;
                        case 133:
                           
                            VolOrinaREF.Text = string.Format("{0} {1} - {2}", r["Unidad"].ToString(), r["ValorMenor"].ToString(), r["ValorMayor"].ToString());
                            VoluOrinaVM = Convert.ToDouble(r["ValorMayor"].ToString());
                            VoluOrinaVm = Convert.ToDouble(r["ValorMenor"].ToString());
                            textBox1.Text = r["ValorResultado"].ToString();
                            break;
                        case 156:
                        
                            CREAORINA24REF.Text = string.Format("{0} - {1}", r["ValorMenor"].ToString(), r["ValorMayor"].ToString());
                            Creatinina24hVM = Convert.ToDouble(r["ValorMayor"].ToString());
                            Creatinina24hVm = Convert.ToDouble(r["ValorMenor"].ToString());
                            textBox7.Text = r["ValorResultado"].ToString();
                            break;
                        case 157:
                            
                            CREAORINAREF.Text = string.Format("{0} {1} - {2}", r["Unidad"].ToString(), r["ValorMenor"].ToString(), r["ValorMayor"].ToString());
                            CreaOrinaVM = Convert.ToDouble(r["ValorMayor"].ToString());
                            CreaOrinaVm = Convert.ToDouble(r["ValorMenor"].ToString());
                            textBox5.Text = r["ValorResultado"].ToString();
                            break;
                        case 160:
                           
                            PESOREF.Text = string.Format("{0} {1} - {2}", r["Unidad"].ToString(), r["ValorMenor"].ToString(), r["ValorMayor"].ToString());
                            if (r["ValorMayor"].ToString() != "")
                            {
                                PesoVM = Convert.ToDouble(r["ValorMayor"].ToString());
                            }
                            if (r["ValorMenor"].ToString() != "")
                            {
                                PesoVm = Convert.ToDouble(r["ValorMenor"].ToString()); DepCreatinina24Vm = Convert.ToDouble(r["ValorMenor"].ToString());
                            }
                            textBox2.Text = r["ValorResultado"].ToString();
                            break;
                        case 161:
                           
                            ALTURAREF.Text = string.Format("{0} {1} - {2}", r["Unidad"].ToString(), r["ValorMenor"].ToString(), r["ValorMayor"].ToString());
                            if (r["ValorMayor"].ToString() != "")
                            {
                                AlturaVM = Convert.ToDouble(r["ValorMayor"].ToString());
                            }
                            if (r["ValorMenor"].ToString() != "")
                            {
                                AlturaVm = Convert.ToDouble(r["ValorMenor"].ToString());
                            }
                            textBox3.Text = r["ValorResultado"].ToString();
                            break;
                        case 162:
                          
                            SUPERFICIEREF.Text = string.Format("{0} {1} - {2}", r["Unidad"].ToString(), r["ValorMenor"].ToString(), r["ValorMayor"].ToString());
                            if (r["ValorMayor"].ToString() != "")
                            {
                                SuperficieVM = Convert.ToDouble(r["ValorMayor"].ToString());
                            }
                            if (r["ValorMenor"].ToString() != "")
                            {
                                SuperficieVm = Convert.ToDouble(r["ValorMenor"].ToString());
                            }
                            textBox4.Text = r["ValorResultado"].ToString();
                            break;
                        case 163:
                           
                            DEPCREAREF.Text = string.Format("{0} {1} - {2}", r["Unidad"].ToString(), r["ValorMenor"].ToString(), r["ValorMayor"].ToString());
                            DepCreatininaVM = Convert.ToDouble(r["ValorMayor"].ToString());
                            DepCreatininaVm = Convert.ToDouble(r["ValorMenor"].ToString());
                            textBox6.Text = r["ValorResultado"].ToString();
                            break;
                        case 164:
                           
                            DCREAORINA24.Text = string.Format("{0} {1} - {2}", r["Unidad"].ToString(), r["ValorMenor"].ToString(), r["ValorMayor"].ToString());
                            if (r["ValorMayor"].ToString() != "")
                            {
                                DepCreatinina24VM = Convert.ToDouble(r["ValorMayor"].ToString());
                            }
                            if (r["ValorMenor"].ToString() != "")
                            {
                                DepCreatinina24Vm = Convert.ToDouble(r["ValorMenor"].ToString());
                            }
                            textBox9.Text = r["ValorResultado"].ToString();
                            break;
                        case 165:
                            
                            VOLMINUTOREF.Text = string.Format("{0} {1} - {2}", r["Unidad"].ToString(), r["ValorMenor"].ToString(), r["ValorMayor"].ToString());
                            if (r["ValorMayor"].ToString() != "")
                            {
                                VoluMinutoVM = Convert.ToDouble(r["ValorMayor"].ToString());
                            }
                            if (r["ValorMenor"].ToString() != "")
                            {
                                VoluMinutoVm = Convert.ToDouble(r["ValorMenor"].ToString());
                            }
                            textBox10.Text = r["ValorResultado"].ToString();
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox9_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox3.Text != "")
            {
                
            }

        }
    }
}
